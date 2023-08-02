using ExitGames.Client.Photon;

public class RoomOptions
{
	public bool isVisible = true;

	public bool isOpen = true;

	public int maxPlayers;

	public bool cleanupCacheOnLeave = PhotonNetwork.autoCleanUpPlayerObjects;

	public Hashtable customRoomProperties;

	public string[] customRoomPropertiesForLobby = new string[0];
}
