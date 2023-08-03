using UnityEngine;

public class BTN_LEADERBOARD : MonoBehaviour
{
	public GameObject mainMenu;

	public GameObject leaderboard;

	private void OnClick()
	{
		NGUITools.SetActive(mainMenu, false);
		NGUITools.SetActive(leaderboard, true);
	}
}
