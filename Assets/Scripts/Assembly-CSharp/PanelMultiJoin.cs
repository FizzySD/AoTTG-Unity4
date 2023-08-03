using System.Collections;
using UnityEngine;

public class PanelMultiJoin : MonoBehaviour
{
	public GameObject[] items;

	private int currentPage = 1;

	private int totalPage = 1;

	private float elapsedTime = 10f;

	private ArrayList filterRoom;

	private string filter = string.Empty;

	private void Start()
	{
		int num = 0;
		for (num = 0; num < 10; num++)
		{
			items[num].SetActive(true);
			items[num].GetComponentInChildren<UILabel>().text = string.Empty;
			items[num].SetActive(false);
		}
	}

	private void showlist()
	{
		if (filter == string.Empty)
		{
			if (PhotonNetwork.GetRoomList().Length > 0)
			{
				totalPage = (PhotonNetwork.GetRoomList().Length - 1) / 10 + 1;
			}
		}
		else
		{
			updateFilterRooms();
			if (filterRoom.Count > 0)
			{
				totalPage = (filterRoom.Count - 1) / 10 + 1;
			}
		}
		if (currentPage < 1)
		{
			currentPage = totalPage;
		}
		if (currentPage > totalPage)
		{
			currentPage = 1;
		}
		showServerList();
	}

	private void Update()
	{
		elapsedTime += Time.deltaTime;
		if (elapsedTime > 3f)
		{
			elapsedTime = 0f;
			showlist();
		}
	}

	private string getServerDataString(RoomInfo room)
	{
		string[] array = room.name.Split("`"[0]);
		return ((!(array[5] == string.Empty)) ? "[PWD]" : string.Empty) + array[0] + "/" + array[1] + "/" + array[2] + "/" + array[4] + " " + room.playerCount + "/" + room.maxPlayers;
	}

	private void showServerList()
	{
		if (PhotonNetwork.GetRoomList().Length == 0)
		{
			return;
		}
		int num = 0;
		if (filter == string.Empty)
		{
			for (num = 0; num < 10; num++)
			{
				int num2 = 10 * (currentPage - 1) + num;
				if (num2 < PhotonNetwork.GetRoomList().Length)
				{
					items[num].SetActive(true);
					items[num].GetComponentInChildren<UILabel>().text = getServerDataString(PhotonNetwork.GetRoomList()[num2]);
					items[num].GetComponentInChildren<BTN_Connect_To_Server_On_List>().roomName = PhotonNetwork.GetRoomList()[num2].name;
				}
				else
				{
					items[num].SetActive(false);
				}
			}
		}
		else
		{
			for (num = 0; num < 10; num++)
			{
				int num3 = 10 * (currentPage - 1) + num;
				if (num3 < filterRoom.Count)
				{
					RoomInfo roomInfo = (RoomInfo)filterRoom[num3];
					items[num].SetActive(true);
					items[num].GetComponentInChildren<UILabel>().text = getServerDataString(roomInfo);
					items[num].GetComponentInChildren<BTN_Connect_To_Server_On_List>().roomName = roomInfo.name;
				}
				else
				{
					items[num].SetActive(false);
				}
			}
		}
		GameObject.Find("LabelServerListPage").GetComponent<UILabel>().text = currentPage + "/" + totalPage;
	}

	public void pageUp()
	{
		currentPage--;
		if (currentPage < 1)
		{
			currentPage = totalPage;
		}
		showServerList();
	}

	public void pageDown()
	{
		currentPage++;
		if (currentPage > totalPage)
		{
			currentPage = 1;
		}
		showServerList();
	}

	private void OnEnable()
	{
		currentPage = 1;
		totalPage = 0;
		refresh();
	}

	private void OnDisable()
	{
	}

	public void refresh()
	{
		showlist();
	}

	public void connectToIndex(int index, string roomName)
	{
		int num = 0;
		for (num = 0; num < 10; num++)
		{
			items[num].SetActive(false);
		}
		num = 10 * (currentPage - 1) + index;
		string[] array = roomName.Split("`"[0]);
		if (array[5] != string.Empty)
		{
			PanelMultiJoinPWD.Password = array[5];
			PanelMultiJoinPWD.roomName = roomName;
			NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().PanelMultiPWD, true);
			NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMultiROOM, false);
		}
		else
		{
			PhotonNetwork.JoinRoom(roomName);
		}
	}

	private void OnFilterSubmit(string content)
	{
		filter = content;
		updateFilterRooms();
		showlist();
	}

	private void updateFilterRooms()
	{
		filterRoom = new ArrayList();
		if (filter == string.Empty)
		{
			return;
		}
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo in roomList)
		{
			if (roomInfo.name.ToUpper().Contains(filter.ToUpper()))
			{
				filterRoom.Add(roomInfo);
			}
		}
	}
}
