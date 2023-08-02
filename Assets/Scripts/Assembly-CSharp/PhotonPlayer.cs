using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPlayer
{
	private int actorID = -1;

	private string nameField = string.Empty;

	public readonly bool isLocal;

	public object TagObject;

	public int ID
	{
		get
		{
			return actorID;
		}
	}

	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			if (!isLocal)
			{
				Debug.LogError("Error: Cannot change the name of a remote player!");
			}
			else
			{
				nameField = value;
			}
		}
	}

	public bool isMasterClient
	{
		get
		{
			return PhotonNetwork.networkingPeer.mMasterClient == this;
		}
	}

	public Hashtable customProperties { get; private set; }

	public Hashtable allProperties
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Merge(customProperties);
			hashtable[byte.MaxValue] = name;
			return hashtable;
		}
	}

	public PhotonPlayer(bool isLocal, int actorID, string name)
	{
		customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		nameField = name;
	}

	protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
	{
		customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		InternalCacheProperties(properties);
	}

	public override bool Equals(object p)
	{
		PhotonPlayer photonPlayer = p as PhotonPlayer;
		return photonPlayer != null && GetHashCode() == photonPlayer.GetHashCode();
	}

	public override int GetHashCode()
	{
		return ID;
	}

	internal void InternalChangeLocalID(int newID)
	{
		if (!isLocal)
		{
			Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
		}
		else
		{
			actorID = newID;
		}
	}

	internal void InternalCacheProperties(Hashtable properties)
	{
		if (properties != null && properties.Count != 0 && !customProperties.Equals(properties))
		{
			if (properties.ContainsKey(byte.MaxValue))
			{
				nameField = (string)properties[byte.MaxValue];
			}
			customProperties.MergeStringKeys(properties);
			customProperties.StripKeysWithNullValues();
		}
	}

	public void SetCustomProperties(Hashtable propertiesToSet)
	{
		if (propertiesToSet != null)
		{
			customProperties.MergeStringKeys(propertiesToSet);
			customProperties.StripKeysWithNullValues();
			Hashtable actorProperties = propertiesToSet.StripToStringKeys();
			if (actorID > 0 && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(actorID, actorProperties, true, 0);
			}
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, this, propertiesToSet);
		}
	}

	public static PhotonPlayer Find(int ID)
	{
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			PhotonPlayer photonPlayer = PhotonNetwork.playerList[i];
			if (photonPlayer.ID == ID)
			{
				return photonPlayer;
			}
		}
		return null;
	}

	public PhotonPlayer Get(int id)
	{
		return Find(id);
	}

	public PhotonPlayer GetNext()
	{
		return GetNextFor(ID);
	}

	public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
	{
		if (currentPlayer == null)
		{
			return null;
		}
		return GetNextFor(currentPlayer.ID);
	}

	public PhotonPlayer GetNextFor(int currentPlayerId)
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.Count < 2)
		{
			return null;
		}
		Dictionary<int, PhotonPlayer> mActors = PhotonNetwork.networkingPeer.mActors;
		int num = int.MaxValue;
		int num2 = currentPlayerId;
		foreach (int key in mActors.Keys)
		{
			if (key < num2)
			{
				num2 = key;
			}
			else if (key > currentPlayerId && key < num)
			{
				num = key;
			}
		}
		return (num == int.MaxValue) ? mActors[num2] : mActors[num];
	}

	public override string ToString()
	{
		if (string.IsNullOrEmpty(name))
		{
			return string.Format("#{0:00}{1}", ID, (!isMasterClient) ? string.Empty : "(master)");
		}
		return string.Format("'{0}'{1}", name, (!isMasterClient) ? string.Empty : "(master)");
	}

	public string ToStringFull()
	{
		return string.Format("#{0:00} '{1}' {2}", ID, name, customProperties.ToStringFull());
	}
}
