using UnityEngine;

public class SettingReciveInput : MonoBehaviour
{
	public int id;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().startListening(id);
		base.transform.Find("Label").gameObject.GetComponent<UILabel>().text = "*wait for input";
	}
}
