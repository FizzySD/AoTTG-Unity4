using UnityEngine;

public class UIMainReferences : MonoBehaviour
{
	public GameObject panelMain;

	public GameObject panelOption;

	public GameObject panelMultiROOM;

	public GameObject PanelMultiJoinPrivate;

	public GameObject PanelMultiWait;

	public GameObject PanelDisconnect;

	public GameObject panelMultiSet;

	public GameObject panelMultiStart;

	public GameObject panelCredits;

	public GameObject panelSingleSet;

	public GameObject PanelMultiPWD;

	public GameObject PanelSnapShot;

	private static bool isGAMEFirstLaunch = true;

	public static string version = "01042015";

	private void Start()
	{
		NGUITools.SetActive(panelMain, true);
		GameObject.Find("VERSION").GetComponent<UILabel>().text = version;
		if (isGAMEFirstLaunch)
		{
			isGAMEFirstLaunch = false;
			GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("InputManagerController"));
			gameObject.name = "InputManagerController";
			Object.DontDestroyOnLoad(gameObject);
		}
	}
}
