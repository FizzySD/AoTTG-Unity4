using UnityEngine;

public class BTN_SIGNOUT : MonoBehaviour
{
	public GameObject loginPanel;

	public GameObject logincomponent;

	private void OnClick()
	{
		NGUITools.SetActive(base.transform.parent.gameObject, false);
		NGUITools.SetActive(loginPanel, true);
		logincomponent.GetComponent<LoginFengKAI>().logout();
	}
}
