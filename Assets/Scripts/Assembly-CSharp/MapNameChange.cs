using UnityEngine;

public class MapNameChange : MonoBehaviour
{
	private void OnSelectionChange()
	{
		LevelInfo info = LevelInfo.getInfo(GetComponent<UIPopupList>().selection);
		if (info != null)
		{
			GameObject.Find("LabelLevelInfo").GetComponent<UILabel>().text = info.desc;
		}
	}
}
