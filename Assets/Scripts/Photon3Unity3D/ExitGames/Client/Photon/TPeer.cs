using System;
using System.Collections.Generic;
using System.IO;

namespace ExitGames.Client.Photon
{
	internal class TPeer : PeerBase
	{
		internal const int TCP_HEADER_BYTES = 7;

		internal const int MSG_HEADER_BYTES = 2;

		internal const int ALL_HEADER_BYTES = 9;

		private List<byte[]> incomingList = new List<byte[]>();

		internal MemoryStream outgoingStream;

		private int lastPingResult;

		private byte[] pingRequest = new byte[5] { 240, 0, 0, 0, 0 };

		internal static readonly byte[] tcpHead = new byte[9] { 251, 0, 0, 0, 0, 0, 0, 243, 2 };

		internal static readonly byte[] messageHeader = tcpHead;

		internal override int QueuedIncomingCommandsCount
		{
			get
			{
				return incomingList.Count;
			}
		}

		internal override int QueuedOutgoingCommandsCount
		{
			get
			{
				return outgoingCommandsInStream;
			}
		}

		internal TPeer()
		{
			PeerBase.peerCount++;
			InitOnce();
			TrafficPackageHeaderSize = 0;
		}

		internal TPeer(IPhotonPeerListener listener)
			: this()
		{
			base.Listener = listener;
		}

		internal override void InitPeerBase()
		{
			base.InitPeerBase();
			incomingList = new List<byte[]>();
		}

		internal override bool Connect(string serverAddress, string appID)
		{
			if (peerConnectionState != 0)
			{
				base.Listener.DebugReturn(DebugLevel.WARNING, "Connect() can't be called if peer is not Disconnected. Not connecting.");
				return false;
			}
			if ((int)debugOut >= 5)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "Connect()");
			}
			base.ServerAddress = serverAddress;
			InitPeerBase();
			outgoingStream = new MemoryStream(PeerBase.outgoingStreamBufferSize);
			if (appID == null)
			{
				appID = "Lite";
			}
			for (int i = 0; i < 32; i++)
			{
				INIT_BYTES[i + 9] = (byte)((i < appID.Length) ? ((byte)appID[i]) : (byte)0);
			}
			rt = new SocketTcp(this);
			if (rt == null)
			{
				base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed, because SocketImplementation or socket was null. Set PhotonPeer.SocketImplementation before Connect().");
				return false;
			}
			if (rt.Connect())
			{
				peerConnectionState = ConnectionStateValue.Connecting;
				EnqueueInit();
				SendOutgoingCommands();
				return true;
			}
			peerConnectionState = ConnectionStateValue.Disconnected;
			return false;
		}

		internal override void Disconnect()
		{
			if (peerConnectionState != 0 && peerConnectionState != ConnectionStateValue.Disconnecting)
			{
				if ((int)debugOut >= 5)
				{
					base.Listener.DebugReturn(DebugLevel.ALL, "TPeer.Disconnect()");
				}
				StopConnection();
			}
		}

		internal override void StopConnection()
		{
			peerConnectionState = ConnectionStateValue.Disconnecting;
			if (rt != null)
			{
				rt.Disconnect();
			}
			lock (incomingList)
			{
				incomingList.Clear();
			}
			peerConnectionState = ConnectionStateValue.Disconnected;
			base.Listener.OnStatusChanged(StatusCode.Disconnect);
		}

		internal override void FetchServerTimestamp()
		{
			if (peerConnectionState != ConnectionStateValue.Connected)
			{
				if ((int)debugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "FetchServerTimestamp() was skipped, as the client is not connected. Current ConnectionState: " + peerConnectionState);
				}
				base.Listener.OnStatusChanged(StatusCode.SendError);
			}
			else
			{
				SendPing();
				serverTimeOffsetIsAvailable = false;
			}
		}

		private void EnqueueInit()
		{
			MemoryStream memoryStream = new MemoryStream(0);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			byte[] array = new byte[7] { 251, 0, 0, 0, 0, 0, 1 };
			int targetOffset = 1;
			Protocol.Serialize(INIT_BYTES.Length + array.Length, array, ref targetOffset);
			binaryWriter.Write(array);
			binaryWriter.Write(INIT_BYTES);
			byte[] array2 = memoryStream.ToArray();
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsOutgoing.TotalPacketCount++;
				TrafficStatsOutgoing.TotalCommandsInPackets++;
				TrafficStatsOutgoing.CountControlCommand(array2.Length);
			}
			EnqueueMessageAsPayload(true, array2, 0);
		}

		internal override bool DispatchIncomingCommands()
		{
			while (true)
			{
				bool flag = true;
				MyAction myAction;
				lock (ActionQueue)
				{
					if (ActionQueue.Count <= 0)
					{
						break;
					}
					myAction = ActionQueue.Dequeue();
					goto IL_0041;
				}
				IL_0041:
				myAction();
			}
			byte[] array;
			lock (incomingList)
			{
				if (incomingList.Count <= 0)
				{
					return false;
				}
				array = incomingList[0];
				incomingList.RemoveAt(0);
			}
			ByteCountCurrentDispatch = array.Length + 3;
			return DeserializeMessageAndCallback(array);
		}

		internal override bool SendOutgoingCommands()
		{
			if (peerConnectionState == ConnectionStateValue.Disconnected)
			{
				return false;
			}
			if (!rt.Connected)
			{
				return false;
			}
			if (peerConnectionState == ConnectionStateValue.Connected && SupportClass.GetTickCount() - lastPingResult > timePingInterval)
			{
				SendPing();
			}
			lock (outgoingStream)
			{
				if (outgoingStream.Position > 0)
				{
					SendData(outgoingStream.ToArray());
					outgoingStream.Position = 0L;
					outgoingStream.SetLength(0L);
					outgoingCommandsInStream = 0;
				}
			}
			return false;
		}

		internal override bool SendAcksOnly()
		{
			if (rt == null || !rt.Connected)
			{
				return false;
			}
			if (peerConnectionState == ConnectionStateValue.Connected && SupportClass.GetTickCount() - lastPingResult > timePingInterval)
			{
				SendPing();
			}
			return false;
		}

		internal override bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, bool sendReliable, byte channelId, bool encrypt, EgMessageType messageType)
		{
			if (peerConnectionState != ConnectionStateValue.Connected)
			{
				if ((int)debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: " + opCode + "! Not connected. PeerState: " + peerConnectionState);
				}
				base.Listener.OnStatusChanged(StatusCode.SendError);
				return false;
			}
			if (channelId >= ChannelCount)
			{
				if ((int)debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: Selected channel (" + channelId + ")>= channelCount (" + ChannelCount + ").");
				}
				base.Listener.OnStatusChanged(StatusCode.SendError);
				return false;
			}
			byte[] opMessage = SerializeOperationToMessage(opCode, parameters, messageType, encrypt);
			return EnqueueMessageAsPayload(sendReliable, opMessage, channelId);
		}

		internal override byte[] SerializeOperationToMessage(byte opc, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt)
		{
			byte[] array;
			lock (SerializeMemStream)
			{
				SerializeMemStream.Position = 0L;
				SerializeMemStream.SetLength(0L);
				if (!encrypt)
				{
					SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
				}
				Protocol.SerializeOperationRequest(SerializeMemStream, opc, parameters, false);
				if (encrypt)
				{
					byte[] data = SerializeMemStream.ToArray();
					data = CryptoProvider.Encrypt(data);
					SerializeMemStream.Position = 0L;
					SerializeMemStream.SetLength(0L);
					SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
					SerializeMemStream.Write(data, 0, data.Length);
				}
				array = SerializeMemStream.ToArray();
			}
			if (messageType != EgMessageType.Operation)
			{
				array[messageHeader.Length - 1] = (byte)messageType;
			}
			if (encrypt)
			{
				array[messageHeader.Length - 1] = (byte)(array[messageHeader.Length - 1] | 0x80u);
			}
			int targetOffset = 1;
			Protocol.Serialize(array.Length, array, ref targetOffset);
			return array;
		}

		internal bool EnqueueMessageAsPayload(bool sendReliable, byte[] opMessage, byte channelId)
		{
			if (opMessage == null)
			{
				return false;
			}
			opMessage[5] = channelId;
			opMessage[6] = (byte)(sendReliable ? 1 : 0);
			lock (outgoingStream)
			{
				outgoingStream.Write(opMessage, 0, opMessage.Length);
				outgoingCommandsInStream++;
				if (outgoingCommandsInStream % warningSize == 0)
				{
					base.Listener.OnStatusChanged(StatusCode.QueueOutgoingReliableWarning);
				}
			}
			ByteCountLastOperation = opMessage.Length;
			if (base.TrafficStatsEnabled)
			{
				if (sendReliable)
				{
					TrafficStatsOutgoing.CountReliableOpCommand(opMessage.Length);
				}
				else
				{
					TrafficStatsOutgoing.CountUnreliableOpCommand(opMessage.Length);
				}
				TrafficStatsGameLevel.CountOperation(opMessage.Length);
			}
			return true;
		}

		internal void SendPing()
		{
			int targetOffset = 1;
			Protocol.Serialize(SupportClass.GetTickCount(), pingRequest, ref targetOffset);
			lastPingResult = SupportClass.GetTickCount();
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsOutgoing.CountControlCommand(pingRequest.Length);
			}
			SendData(pingRequest);
		}

		internal void SendData(byte[] data)
		{
			try
			{
				bytesOut += data.Length;
				if (base.TrafficStatsEnabled)
				{
					TrafficStatsOutgoing.TotalPacketCount++;
					TrafficStatsOutgoing.TotalCommandsInPackets += outgoingCommandsInStream;
				}
				if (base.NetworkSimulationSettings.IsSimulationEnabled)
				{
					SendNetworkSimulated(delegate
					{
						rt.Send(data, data.Length);
					});
				}
				else
				{
					rt.Send(data, data.Length);
				}
			}
			catch (Exception ex)
			{
				if ((int)debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, ex.ToString());
				}
				SupportClass.WriteStackTrace(ex);
			}
		}

		internal override void ReceiveIncomingCommands(byte[] inbuff, int dataLength)
		{
			if (inbuff == null)
			{
				if ((int)debugOut >= 1)
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "checkAndQueueIncomingCommands() inBuff: null");
				}
				return;
			}
			timestampOfLastReceive = SupportClass.GetTickCount();
			bytesIn += inbuff.Length + 7;
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsIncoming.TotalPacketCount++;
				TrafficStatsIncoming.TotalCommandsInPackets++;
			}
			if (inbuff[0] == 243 || inbuff[0] == 244)
			{
				lock (incomingList)
				{
					incomingList.Add(inbuff);
					if (incomingList.Count % warningSize == 0)
					{
						EnqueueStatusCallback(StatusCode.QueueIncomingReliableWarning);
					}
					return;
				}
			}
			if (inbuff[0] == 240)
			{
				TrafficStatsIncoming.CountControlCommand(inbuff.Length);
				ReadPingResult(inbuff);
			}
			else if ((int)debugOut >= 1)
			{
				EnqueueDebugReturn(DebugLevel.ERROR, "receiveIncomingCommands() MagicNumber should be 0xF0, 0xF3 or 0xF4. Is: " + inbuff[0]);
			}
		}

		private void ReadPingResult(byte[] inbuff)
		{
			int value = 0;
			int value2 = 0;
			int offset = 1;
			Protocol.Deserialize(out value, inbuff, ref offset);
			Protocol.Deserialize(out value2, inbuff, ref offset);
			lastRoundTripTime = SupportClass.GetTickCount() - value2;
			if (!serverTimeOffsetIsAvailable)
			{
				roundTripTime = lastRoundTripTime;
			}
			UpdateRoundTripTimeAndVariance(lastRoundTripTime);
			if (!serverTimeOffsetIsAvailable)
			{
				serverTimeOffset = value + (lastRoundTripTime >> 1) - SupportClass.GetTickCount();
				serverTimeOffsetIsAvailable = true;
			}
		}
	}
}
