using System.Collections;
using UnityEngine;

public class LoginFengKAI : MonoBehaviour
{
	private static string playerName = string.Empty;

	private static string playerGUILDName = string.Empty;

	private static string playerPassword = string.Empty;

	public string formText = string.Empty;

	private string CheckUserURL = "http://fenglee.com/game/aog/login_check.php";

	private string RegisterURL = "http://fenglee.com/game/aog/signup_check.php";

	private string ForgetPasswordURL = "http://fenglee.com/game/aog/forget_password.php";

	private string GetInfoURL = "http://fenglee.com/game/aog/require_user_info.php";

	private string ChangePasswordURL = "http://fenglee.com/game/aog/change_password.php";

	private string ChangeGuildURL = "http://fenglee.com/game/aog/change_guild_name.php";

	public GameObject output;

	public GameObject output2;

	public PanelLoginGroupManager loginGroup;

	public GameObject panelLogin;

	public GameObject panelForget;

	public GameObject panelRegister;

	public GameObject panelStatus;

	public GameObject panelChangePassword;

	public GameObject panelChangeGUILDNAME;

	public static PlayerInfoPHOTON player;

	private void Start()
	{
		if (player == null)
		{
			player = new PlayerInfoPHOTON();
			player.initAsGuest();
		}
		if (playerName != string.Empty)
		{
			NGUITools.SetActive(panelLogin, false);
			NGUITools.SetActive(panelStatus, true);
			StartCoroutine(getInfo());
		}
		else
		{
			output.GetComponent<UILabel>().text = "Welcome," + player.name;
		}
	}

	private void Update()
	{
	}

	public void login(string name, string password)
	{
		StartCoroutine(Login(name, password));
	}

	private IEnumerator Login(string name, string password)
	{
		WWWForm form = new WWWForm();
		form.AddField("userid", name);
		form.AddField("password", password);
		form.AddField("version", UIMainReferences.version);
		WWW w = new WWW(CheckUserURL, form);
		yield return w;
		clearCOOKIE();
		if (w.error != null)
		{
			MonoBehaviour.print(w.error);
			yield break;
		}
		output.GetComponent<UILabel>().text = w.text;
		formText = w.text;
		w.Dispose();
		if (formText.Contains("Welcome back") && formText.Contains("(^o^)/~"))
		{
			NGUITools.SetActive(panelLogin, false);
			NGUITools.SetActive(panelStatus, true);
			playerName = name;
			playerPassword = password;
			StartCoroutine(getInfo());
		}
	}

	private IEnumerator getInfo()
	{
		WWWForm form = new WWWForm();
		form.AddField("userid", playerName);
		form.AddField("password", playerPassword);
		WWW w = new WWW(GetInfoURL, form);
		yield return w;
		if (w.error != null)
		{
			MonoBehaviour.print(w.error);
			yield break;
		}
		if (w.text.Contains("Error,please sign in again."))
		{
			NGUITools.SetActive(panelLogin, true);
			NGUITools.SetActive(panelStatus, false);
			output.GetComponent<UILabel>().text = w.text;
			playerName = string.Empty;
			playerPassword = string.Empty;
		}
		else
		{
			string[] result = w.text.Split('|');
			playerGUILDName = result[0];
			output2.GetComponent<UILabel>().text = result[1];
			player.name = playerName;
			player.guildname = playerGUILDName;
		}
		w.Dispose();
	}

	public void signup(string name, string password, string password2, string email)
	{
		StartCoroutine(Register(name, password, password2, email));
	}

	private IEnumerator Register(string name, string password, string password2, string email)
	{
		WWWForm form = new WWWForm();
		form.AddField("userid", name);
		form.AddField("password", password);
		form.AddField("password2", password2);
		form.AddField("email", email);
		WWW w = new WWW(RegisterURL, form);
		yield return w;
		if (w.error != null)
		{
			MonoBehaviour.print(w.error);
		}
		else
		{
			output.GetComponent<UILabel>().text = w.text;
			if (w.text.Contains("Final step,to activate your account, please click the link in the activation email"))
			{
				NGUITools.SetActive(panelRegister, false);
				NGUITools.SetActive(panelLogin, true);
			}
			w.Dispose();
		}
		clearCOOKIE();
	}

	public void cpassword(string oldpassword, string password, string password2)
	{
		if (playerName == string.Empty)
		{
			logout();
			NGUITools.SetActive(panelChangePassword, false);
			NGUITools.SetActive(panelLogin, true);
			output.GetComponent<UILabel>().text = "Please sign in.";
		}
		else
		{
			StartCoroutine(changePassword(oldpassword, password, password2));
		}
	}

	private IEnumerator changePassword(string oldpassword, string password, string password2)
	{
		WWWForm form = new WWWForm();
		form.AddField("userid", playerName);
		form.AddField("old_password", oldpassword);
		form.AddField("password", password);
		form.AddField("password2", password2);
		WWW w = new WWW(ChangePasswordURL, form);
		yield return w;
		if (w.error != null)
		{
			MonoBehaviour.print(w.error);
			yield break;
		}
		output.GetComponent<UILabel>().text = w.text;
		if (w.text.Contains("Thanks, Your password changed successfully"))
		{
			NGUITools.SetActive(panelChangePassword, false);
			NGUITools.SetActive(panelLogin, true);
		}
		w.Dispose();
	}

	public void cGuild(string name)
	{
		if (playerName == string.Empty)
		{
			logout();
			NGUITools.SetActive(panelChangeGUILDNAME, false);
			NGUITools.SetActive(panelLogin, true);
			output.GetComponent<UILabel>().text = "Please sign in.";
		}
		else
		{
			StartCoroutine(changeGuild(name));
		}
	}

	private IEnumerator changeGuild(string name)
	{
		WWWForm form = new WWWForm();
		form.AddField("name", playerName);
		form.AddField("guildname", name);
		WWW w = new WWW(ChangeGuildURL, form);
		yield return w;
		if (w.error != null)
		{
			MonoBehaviour.print(w.error);
			yield break;
		}
		output.GetComponent<UILabel>().text = w.text;
		if (w.text.Contains("Guild name set."))
		{
			NGUITools.SetActive(panelChangeGUILDNAME, false);
			NGUITools.SetActive(panelStatus, true);
			StartCoroutine(getInfo());
		}
		w.Dispose();
	}

	public void resetPassword(string email)
	{
		StartCoroutine(ForgetPassword(email));
	}

	private IEnumerator ForgetPassword(string email)
	{
		WWWForm form = new WWWForm();
		form.AddField("email", email);
		WWW w = new WWW(ForgetPasswordURL, form);
		yield return w;
		if (w.error != null)
		{
			MonoBehaviour.print(w.error);
		}
		else
		{
			output.GetComponent<UILabel>().text = w.text;
			w.Dispose();
			NGUITools.SetActive(panelForget, false);
			NGUITools.SetActive(panelLogin, true);
		}
		clearCOOKIE();
	}

	private void clearCOOKIE()
	{
		playerName = string.Empty;
		playerPassword = string.Empty;
	}

	public void logout()
	{
		clearCOOKIE();
		player = new PlayerInfoPHOTON();
		player.initAsGuest();
		output.GetComponent<UILabel>().text = "Welcome," + player.name;
	}
}
