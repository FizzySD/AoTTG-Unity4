using UnityEngine;

public class BTN_QUICKMATCH : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.GetComponent<UIButton>().isEnabled = false;
	}

	private void OnClick()
	{
	}
}
