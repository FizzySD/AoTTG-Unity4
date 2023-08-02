using UnityEngine;

public class BTN_toSingleSet : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnClick()
	{
		NGUITools.SetActive(base.transform.parent.gameObject, false);
		NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelSingleSet, true);
	}
}
