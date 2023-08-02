using UnityEngine;

public class DaylightChange : MonoBehaviour
{
	private void OnSelectionChange()
	{
		if (GetComponent<UIPopupList>().selection == "DAY")
		{
			IN_GAME_MAIN_CAMERA.dayLight = DayLight.Day;
		}
		if (GetComponent<UIPopupList>().selection == "DAWN")
		{
			IN_GAME_MAIN_CAMERA.dayLight = DayLight.Dawn;
		}
		if (GetComponent<UIPopupList>().selection == "NIGHT")
		{
			IN_GAME_MAIN_CAMERA.dayLight = DayLight.Night;
		}
	}
}
