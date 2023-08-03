using UnityEngine;

public class BTN_Server_EU : MonoBehaviour
{
	private void OnClick()
	{
		PhotonNetwork.Disconnect();
		PhotonNetwork.ConnectToMaster("app-eu.exitgamescloud.com", 5055, FengGameManagerMKII.applicationId, UIMainReferences.version);
	}
}
