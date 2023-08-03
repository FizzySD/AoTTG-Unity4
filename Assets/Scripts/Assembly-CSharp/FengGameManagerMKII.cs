using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class FengGameManagerMKII : Photon.MonoBehaviour
{
	public FengCustomInputs inputManager;

	public static readonly string applicationId = "ff3f48bd-c93d-4944-93f2-596081c5b654";

	public int difficulty;

	private GameObject ui;

	public bool needChooseSide;

	public bool justSuicide;

	private ArrayList chatContent;

	private string myLastHero;

	private string myLastRespawnTag = "playerRespawn";

	public float myRespawnTime;

	private int titanScore;

	private int humanScore;

	public int PVPtitanScore;

	public int PVPhumanScore;

	private int PVPtitanScoreMax = 200;

	private int PVPhumanScoreMax = 200;

	private bool isWinning;

	private bool isLosing;

	private bool isPlayer1Winning;

	private bool isPlayer2Winning;

	private int teamWinner;

	private int[] teamScores;

	private float gameEndCD;

	private float gameEndTotalCDtime = 9f;

	public int wave = 1;

	private int highestwave = 1;

	public static string level = string.Empty;

	public int time = 600;

	private float timeElapse;

	public float roundTime;

	private float timeTotalServer;

	private float maxSpeed;

	private float currentSpeed;

	public static bool LAN;

	private bool startRacing;

	private bool endRacing;

	public GameObject checkpoint;

	private ArrayList racingResult;

	private ArrayList kicklist;

	private bool gameTimesUp;

	private IN_GAME_MAIN_CAMERA mainCamera;

	public bool gameStart;

	private ArrayList heroes;

	private ArrayList eT;

	private ArrayList titans;

	private ArrayList fT;

	private ArrayList cT;

	private ArrayList hooks;

	private string localRacingResult;

	private int single_kills;

	private int single_maxDamage;

	private int single_totalDamage;

	private ArrayList killInfoGO = new ArrayList();

	private void Start()
	{
		base.gameObject.name = "MultiplayerManager";
		HeroCostume.init();
		CharacterMaterials.init();
		Object.DontDestroyOnLoad(base.gameObject);
		heroes = new ArrayList();
		eT = new ArrayList();
		titans = new ArrayList();
		fT = new ArrayList();
		cT = new ArrayList();
		hooks = new ArrayList();
	}

	public void addHero(HERO hero)
	{
		heroes.Add(hero);
	}

	public void removeHero(HERO hero)
	{
		heroes.Remove(hero);
	}

	public void addHook(Bullet h)
	{
		hooks.Add(h);
	}

	public void removeHook(Bullet h)
	{
		hooks.Remove(h);
	}

	public void addET(TITAN_EREN hero)
	{
		eT.Add(hero);
	}

	public void removeET(TITAN_EREN hero)
	{
		eT.Remove(hero);
	}

	public void addTitan(TITAN titan)
	{
		titans.Add(titan);
	}

	public void removeTitan(TITAN titan)
	{
		titans.Remove(titan);
	}

	public void addFT(FEMALE_TITAN titan)
	{
		fT.Add(titan);
	}

	public void removeFT(FEMALE_TITAN titan)
	{
		fT.Remove(titan);
	}

	public void addCT(COLOSSAL_TITAN titan)
	{
		cT.Add(titan);
	}

	public void removeCT(COLOSSAL_TITAN titan)
	{
		cT.Remove(titan);
	}

	public void addCamera(IN_GAME_MAIN_CAMERA c)
	{
		mainCamera = c;
	}

	private void LateUpdate()
	{
		if (!gameStart)
		{
			return;
		}
		foreach (HERO hero in heroes)
		{
			hero.lateUpdate();
		}
		foreach (TITAN_EREN item in eT)
		{
			item.lateUpdate();
		}
		foreach (TITAN titan in titans)
		{
			titan.lateUpdate();
		}
		foreach (FEMALE_TITAN item2 in fT)
		{
			item2.lateUpdate();
		}
		core();
	}

	private void Update()
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && (bool)GameObject.Find("LabelNetworkStatus"))
		{
			GameObject.Find("LabelNetworkStatus").GetComponent<UILabel>().text = PhotonNetwork.connectionStateDetailed.ToString();
			if (PhotonNetwork.connected)
			{
				UILabel component = GameObject.Find("LabelNetworkStatus").GetComponent<UILabel>();
				component.text = component.text + " ping:" + PhotonNetwork.GetPing();
			}
		}
		if (!gameStart)
		{
			return;
		}
		foreach (HERO hero in heroes)
		{
			hero.update();
		}
		foreach (Bullet hook in hooks)
		{
			hook.update();
		}
		if (mainCamera != null)
		{
			mainCamera.snapShotUpdate();
		}
		foreach (TITAN_EREN item in eT)
		{
			item.update();
		}
		foreach (TITAN titan in titans)
		{
			titan.update();
		}
		foreach (FEMALE_TITAN item2 in fT)
		{
			item2.update();
		}
		foreach (COLOSSAL_TITAN item3 in cT)
		{
			item3.update();
		}
		if (mainCamera != null)
		{
			mainCamera.update();
		}
	}

	private void core()
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && needChooseSide)
		{
			if (GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().isInputDown[InputCode.flare1])
			{
				if (NGUITools.GetActive(ui.GetComponent<UIReferArray>().panels[3]))
				{
					Screen.lockCursor = true;
					Screen.showCursor = true;
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], true);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], false);
					GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = false;
					GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = false;
				}
				else
				{
					Screen.lockCursor = false;
					Screen.showCursor = true;
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], false);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], true);
					GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
					GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
				}
			}
			if (GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().isInputDown[15] && !NGUITools.GetActive(ui.GetComponent<UIReferArray>().panels[3]))
			{
				NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], false);
				NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], true);
				NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
				NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], false);
				Screen.showCursor = true;
				Screen.lockCursor = false;
				GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
				GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
				GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().showKeyMap();
				GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().justUPDATEME();
				GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = true;
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER)
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
		{
			string text = string.Empty;
			PhotonPlayer[] playerList = PhotonNetwork.playerList;
			foreach (PhotonPlayer photonPlayer in playerList)
			{
				if (photonPlayer.customProperties[PhotonPlayerProperty.dead] == null)
				{
					continue;
				}
				string text2 = text;
				text = text2 + "[ffffff]#" + photonPlayer.ID + " ";
				if (photonPlayer.isLocal)
				{
					text += "> ";
				}
				if (photonPlayer.isMasterClient)
				{
					text += "M ";
				}
				if ((bool)photonPlayer.customProperties[PhotonPlayerProperty.dead])
				{
					text = text + "[" + ColorSet.color_red + "] *dead* ";
					if ((int)photonPlayer.customProperties[PhotonPlayerProperty.isTitan] == 2)
					{
						text = text + "[" + ColorSet.color_titan_player + "] T ";
					}
					if ((int)photonPlayer.customProperties[PhotonPlayerProperty.isTitan] == 1)
					{
						text = (((int)photonPlayer.customProperties[PhotonPlayerProperty.team] != 2) ? (text + "[" + ColorSet.color_human + "] H ") : (text + "[" + ColorSet.color_human_1 + "] H "));
					}
				}
				else
				{
					if ((int)photonPlayer.customProperties[PhotonPlayerProperty.isTitan] == 2)
					{
						text = text + "[" + ColorSet.color_titan_player + "] T ";
					}
					if ((int)photonPlayer.customProperties[PhotonPlayerProperty.isTitan] == 1)
					{
						text = (((int)photonPlayer.customProperties[PhotonPlayerProperty.team] != 2) ? (text + "[" + ColorSet.color_human + "] H ") : (text + "[" + ColorSet.color_human_1 + "] H "));
					}
				}
				text2 = text;
				text = string.Concat(text2, string.Empty, photonPlayer.customProperties[PhotonPlayerProperty.name], "[ffffff]:", photonPlayer.customProperties[PhotonPlayerProperty.kills], "/", photonPlayer.customProperties[PhotonPlayerProperty.deaths], "/", photonPlayer.customProperties[PhotonPlayerProperty.max_dmg], "/", photonPlayer.customProperties[PhotonPlayerProperty.total_dmg]);
				if ((bool)photonPlayer.customProperties[PhotonPlayerProperty.dead])
				{
					text += "[-]";
				}
				text += "\n";
			}
			ShowHUDInfoTopLeft(text);
			if ((bool)GameObject.Find("MainCamera") && IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.RACING && GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver && !needChooseSide)
			{
				ShowHUDInfoCenter("Press [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.flare1] + "[-] to spectate the next player. \nPress [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.flare2] + "[-] to spectate the previous player.\nPress [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.attack1] + "[-] to enter the spectator mode.\n\n\n\n");
				if (LevelInfo.getInfo(level).respawnMode == RespawnMode.DEATHMATCH)
				{
					myRespawnTime += Time.deltaTime;
					int num = 10;
					if ((int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.isTitan] == 2)
					{
						num = 15;
					}
					ShowHUDInfoCenterADD("Respawn in " + (int)((float)num - myRespawnTime) + "s.");
					if (myRespawnTime > (float)num)
					{
						myRespawnTime = 0f;
						GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
						if ((int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.isTitan] == 2)
						{
							SpawnNonAITitan(myLastHero);
						}
						else
						{
							SpawnPlayer(myLastHero, myLastRespawnTag);
						}
						GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
						ShowHUDInfoCenter(string.Empty);
					}
				}
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
			{
				if (!isLosing)
				{
					currentSpeed = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity.magnitude;
					maxSpeed = Mathf.Max(maxSpeed, currentSpeed);
					ShowHUDInfoTopLeft("Current Speed : " + (int)currentSpeed + "\nMax Speed:" + maxSpeed);
				}
			}
			else
			{
				ShowHUDInfoTopLeft("Kills:" + single_kills + "\nMax Damage:" + single_maxDamage + "\nTotal Damage:" + single_totalDamage);
			}
		}
		if (isLosing && IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.RACING)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
				{
					ShowHUDInfoCenter("Survive " + wave + " Waves!\n Press " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.restart] + " to Restart.\n\n\n");
				}
				else
				{
					ShowHUDInfoCenter("Humanity Fail!\n Press " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.restart] + " to Restart.\n\n\n");
				}
			}
			else
			{
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
				{
					ShowHUDInfoCenter("Survive " + wave + " Waves!\nGame Restart in " + (int)gameEndCD + "s\n\n");
				}
				else
				{
					ShowHUDInfoCenter("Humanity Fail!\nAgain!\nGame Restart in " + (int)gameEndCD + "s\n\n");
				}
				if (gameEndCD <= 0f)
				{
					gameEndCD = 0f;
					if (PhotonNetwork.isMasterClient)
					{
						restartGame();
					}
					ShowHUDInfoCenter(string.Empty);
				}
				else
				{
					gameEndCD -= Time.deltaTime;
				}
			}
		}
		if (isWinning)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
				{
					ShowHUDInfoCenter((float)(int)(timeTotalServer * 10f) * 0.1f - 5f + "s !\n Press " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.restart] + " to Restart.\n\n\n");
				}
				else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
				{
					ShowHUDInfoCenter("Survive All Waves!\n Press " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.restart] + " to Restart.\n\n\n");
				}
				else
				{
					ShowHUDInfoCenter("Humanity Win!\n Press " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.restart] + " to Restart.\n\n\n");
				}
			}
			else
			{
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
				{
					ShowHUDInfoCenter(localRacingResult + "\n\nGame Restart in " + (int)gameEndCD + "s");
				}
				else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
				{
					ShowHUDInfoCenter("Survive All Waves!\nGame Restart in " + (int)gameEndCD + "s\n\n");
				}
				else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
				{
					ShowHUDInfoCenter("Team " + teamWinner + " Win!\nGame Restart in " + (int)gameEndCD + "s\n\n");
				}
				else
				{
					ShowHUDInfoCenter("Humanity Win!\nGame Restart in " + (int)gameEndCD + "s\n\n");
				}
				if (gameEndCD <= 0f)
				{
					gameEndCD = 0f;
					if (PhotonNetwork.isMasterClient)
					{
						restartGame();
					}
					ShowHUDInfoCenter(string.Empty);
				}
				else
				{
					gameEndCD -= Time.deltaTime;
				}
			}
		}
		timeElapse += Time.deltaTime;
		roundTime += Time.deltaTime;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
			{
				if (!isWinning)
				{
					timeTotalServer += Time.deltaTime;
				}
			}
			else if (!isLosing && !isWinning)
			{
				timeTotalServer += Time.deltaTime;
			}
		}
		else
		{
			timeTotalServer += Time.deltaTime;
		}
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				if (!isWinning)
				{
					ShowHUDInfoTopCenter("Time : " + ((float)(int)(timeTotalServer * 10f) * 0.1f - 5f));
				}
				if (timeTotalServer < 5f)
				{
					ShowHUDInfoCenter("RACE START IN " + (int)(5f - timeTotalServer));
				}
				else if (!startRacing)
				{
					ShowHUDInfoCenter(string.Empty);
					startRacing = true;
					endRacing = false;
					GameObject.Find("door").SetActive(false);
				}
			}
			else
			{
				ShowHUDInfoTopCenter("Time : " + ((!(roundTime < 20f)) ? ((float)(int)(roundTime * 10f) * 0.1f - 20f).ToString() : "WAITING"));
				if (roundTime < 20f)
				{
					ShowHUDInfoCenter("RACE START IN " + (int)(20f - roundTime) + ((!(localRacingResult == string.Empty)) ? ("\nLast Round\n" + localRacingResult) : "\n\n"));
				}
				else if (!startRacing)
				{
					ShowHUDInfoCenter(string.Empty);
					startRacing = true;
					endRacing = false;
					GameObject.Find("door").SetActive(false);
				}
			}
			if (GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver && !needChooseSide)
			{
				myRespawnTime += Time.deltaTime;
				if (myRespawnTime > 1.5f)
				{
					myRespawnTime = 0f;
					GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
					if (checkpoint != null)
					{
						SpawnPlayerAt(myLastHero, checkpoint);
					}
					else
					{
						SpawnPlayer(myLastHero, myLastRespawnTag);
					}
					GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
					ShowHUDInfoCenter(string.Empty);
				}
			}
		}
		if (timeElapse > 1f)
		{
			timeElapse -= 1f;
			string text3 = string.Empty;
			if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
			{
				text3 += "Time : ";
				text3 += (int)((float)time - timeTotalServer);
			}
			else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN)
			{
				text3 = "Titan Left: ";
				text3 += GameObject.FindGameObjectsWithTag("titan").Length;
				text3 += "  Time : ";
				text3 = ((IN_GAME_MAIN_CAMERA.gametype != 0) ? (text3 + (int)((float)time - timeTotalServer)) : (text3 + (int)timeTotalServer));
			}
			else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
			{
				text3 = "Titan Left: ";
				text3 += GameObject.FindGameObjectsWithTag("titan").Length;
				text3 += " Wave : ";
				text3 += wave;
			}
			else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT)
			{
				text3 = "Time : ";
				text3 += (int)((float)time - timeTotalServer);
				text3 += "\nDefeat the Colossal Titan.\nPrevent abnormal titan from running to the north gate";
			}
			else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
			{
				string text4 = "| ";
				for (int j = 0; j < PVPcheckPoint.chkPts.Count; j++)
				{
					text4 = text4 + (PVPcheckPoint.chkPts[j] as PVPcheckPoint).getStateString() + " ";
				}
				text4 += "|";
				text3 = PVPtitanScoreMax - PVPtitanScore + "  " + text4 + "  " + (PVPhumanScoreMax - PVPhumanScore) + "\n";
				text3 += "Time : ";
				text3 += (int)((float)time - timeTotalServer);
			}
			ShowHUDInfoTopCenter(text3);
			text3 = string.Empty;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
				{
					text3 = "Time : ";
					text3 += (int)timeTotalServer;
				}
			}
			else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
			{
				text3 = "Humanity " + humanScore + " : Titan " + titanScore + " ";
			}
			else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
			{
				text3 = "Humanity " + humanScore + " : Titan " + titanScore + " ";
			}
			else if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.CAGE_FIGHT)
			{
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
				{
					text3 = "Time : ";
					text3 += (int)((float)time - timeTotalServer);
				}
				else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
				{
					for (int k = 0; k < teamScores.Length; k++)
					{
						string text2 = text3;
						text3 = text2 + ((k == 0) ? string.Empty : " : ") + "Team" + (k + 1) + " " + teamScores[k] + string.Empty;
					}
					text3 += "\nTime : ";
					text3 += (int)((float)time - timeTotalServer);
				}
			}
			ShowHUDInfoTopRight(text3);
			string text5 = ((IN_GAME_MAIN_CAMERA.difficulty < 0) ? "Trainning" : ((IN_GAME_MAIN_CAMERA.difficulty == 0) ? "Normal" : ((IN_GAME_MAIN_CAMERA.difficulty != 1) ? "Abnormal" : "Hard")));
			if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT)
			{
				ShowHUDInfoTopRightMAPNAME((int)roundTime + "s\n" + level + " : " + text5);
			}
			else
			{
				ShowHUDInfoTopRightMAPNAME("\nLeague of penguins " + level + " : " + text5);
			}
			ShowHUDInfoTopRightMAPNAME("\nCamera(" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.camera] + "):" + IN_GAME_MAIN_CAMERA.cameraMode);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && needChooseSide)
			{
				ShowHUDInfoTopCenterADD("\n\nPRESS 1 TO ENTER GAME");
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && killInfoGO.Count > 0 && killInfoGO[0] == null)
		{
			killInfoGO.RemoveAt(0);
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || !PhotonNetwork.isMasterClient || !(timeTotalServer > (float)time))
		{
			return;
		}
		IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
		gameStart = false;
		Screen.lockCursor = false;
		Screen.showCursor = true;
		string text6 = string.Empty;
		string text7 = string.Empty;
		string text8 = string.Empty;
		string text9 = string.Empty;
		string text10 = string.Empty;
		PhotonPlayer[] playerList2 = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer2 in playerList2)
		{
			if (photonPlayer2 != null)
			{
				text6 = string.Concat(text6, photonPlayer2.customProperties[PhotonPlayerProperty.name], "\n");
				text7 = string.Concat(text7, photonPlayer2.customProperties[PhotonPlayerProperty.kills], "\n");
				text8 = string.Concat(text8, photonPlayer2.customProperties[PhotonPlayerProperty.deaths], "\n");
				text9 = string.Concat(text9, photonPlayer2.customProperties[PhotonPlayerProperty.max_dmg], "\n");
				text10 = string.Concat(text10, photonPlayer2.customProperties[PhotonPlayerProperty.total_dmg], "\n");
			}
		}
		string text11;
		if (IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.PVP_AHSS)
		{
			text11 = ((IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.SURVIVE_MODE) ? ("Humanity " + humanScore + " : Titan " + titanScore) : ("Highest Wave : " + highestwave));
		}
		else
		{
			text11 = string.Empty;
			for (int m = 0; m < teamScores.Length; m++)
			{
				text11 += ((m == 0) ? ("Team" + (m + 1) + " " + teamScores[m] + " ") : " : ");
			}
		}
		base.photonView.RPC("showResult", PhotonTargets.AllBuffered, text6, text7, text8, text9, text10, text11);
	}

	public void SpawnPlayer(string id, string tag = "playerRespawn")
	{
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
		{
			SpawnPlayerAt(id, checkpoint);
			return;
		}
		myLastRespawnTag = tag;
		GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
		GameObject pos = array[Random.Range(0, array.Length)];
		SpawnPlayerAt(id, pos);
	}

	public void SpawnPlayerAt(string id, GameObject pos)
	{
		IN_GAME_MAIN_CAMERA component = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>();
		myLastHero = id.ToUpper();
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			if (IN_GAME_MAIN_CAMERA.singleCharacter == "TITAN_EREN")
			{
				component.setMainObject((GameObject)Object.Instantiate(Resources.Load("TITAN_EREN"), pos.transform.position, pos.transform.rotation));
			}
			else
			{
				component.setMainObject((GameObject)Object.Instantiate(Resources.Load("AOTTG_HERO 1"), pos.transform.position, pos.transform.rotation));
				if (IN_GAME_MAIN_CAMERA.singleCharacter == "SET 1" || IN_GAME_MAIN_CAMERA.singleCharacter == "SET 2" || IN_GAME_MAIN_CAMERA.singleCharacter == "SET 3")
				{
					HeroCostume heroCostume = CostumeConeveter.LocalDataToHeroCostume(IN_GAME_MAIN_CAMERA.singleCharacter);
					heroCostume.checkstat();
					CostumeConeveter.HeroCostumeToLocalData(heroCostume, IN_GAME_MAIN_CAMERA.singleCharacter);
					component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
					if (heroCostume != null)
					{
						component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = heroCostume;
						component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = heroCostume.stat;
					}
					else
					{
						heroCostume = HeroCostume.costumeOption[3];
						component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = heroCostume;
						component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.getInfo(heroCostume.name.ToUpper());
					}
					component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
					component.main_object.GetComponent<HERO>().setStat();
					component.main_object.GetComponent<HERO>().setSkillHUDPosition();
				}
				else
				{
					for (int i = 0; i < HeroCostume.costume.Length; i++)
					{
						if (HeroCostume.costume[i].name.ToUpper() == IN_GAME_MAIN_CAMERA.singleCharacter.ToUpper())
						{
							int num = HeroCostume.costume[i].id + CheckBoxCostume.costumeSet - 1;
							if (HeroCostume.costume[num].name != HeroCostume.costume[i].name)
							{
								num = HeroCostume.costume[i].id + 1;
							}
							component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
							component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = HeroCostume.costume[num];
							component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.getInfo(HeroCostume.costume[num].name.ToUpper());
							component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
							component.main_object.GetComponent<HERO>().setStat();
							component.main_object.GetComponent<HERO>().setSkillHUDPosition();
							break;
						}
					}
				}
			}
		}
		else
		{
			component.setMainObject(PhotonNetwork.Instantiate("AOTTG_HERO 1", pos.transform.position, pos.transform.rotation, 0));
			id = id.ToUpper();
			switch (id)
			{
			case "SET 1":
			case "SET 2":
			case "SET 3":
			{
				HeroCostume heroCostume2 = CostumeConeveter.LocalDataToHeroCostume(id);
				heroCostume2.checkstat();
				CostumeConeveter.HeroCostumeToLocalData(heroCostume2, id);
				component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
				if (heroCostume2 != null)
				{
					component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = heroCostume2;
					component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = heroCostume2.stat;
				}
				else
				{
					heroCostume2 = HeroCostume.costumeOption[3];
					component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = heroCostume2;
					component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.getInfo(heroCostume2.name.ToUpper());
				}
				component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
				component.main_object.GetComponent<HERO>().setStat();
				component.main_object.GetComponent<HERO>().setSkillHUDPosition();
				break;
			}
			default:
			{
				for (int j = 0; j < HeroCostume.costume.Length; j++)
				{
					if (HeroCostume.costume[j].name.ToUpper() == id.ToUpper())
					{
						int num2 = HeroCostume.costume[j].id;
						if (id.ToUpper() != "AHSS")
						{
							num2 += CheckBoxCostume.costumeSet - 1;
						}
						if (HeroCostume.costume[num2].name != HeroCostume.costume[j].name)
						{
							num2 = HeroCostume.costume[j].id + 1;
						}
						component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();
						component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = HeroCostume.costume[num2];
						component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.getInfo(HeroCostume.costume[num2].name.ToUpper());
						component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
						component.main_object.GetComponent<HERO>().setStat();
						component.main_object.GetComponent<HERO>().setSkillHUDPosition();
						break;
					}
				}
				break;
			}
			}
			CostumeConeveter.HeroCostumeToPhotonData(component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume, PhotonNetwork.player);
			if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
			{
				component.main_object.transform.position += new Vector3(Random.Range(-20, 20), 2f, Random.Range(-20, 20));
			}
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("dead", false);
			ExitGames.Client.Photon.Hashtable customProperties = hashtable;
			PhotonNetwork.player.SetCustomProperties(customProperties);
			hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.isTitan, 1);
			customProperties = hashtable;
			PhotonNetwork.player.SetCustomProperties(customProperties);
		}
		component.enabled = true;
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setHUDposition();
		GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
		GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
		component.gameOver = false;
		if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
		{
			Screen.lockCursor = true;
		}
		else
		{
			Screen.lockCursor = false;
		}
		Screen.showCursor = false;
		isLosing = false;
		ShowHUDInfoCenter(string.Empty);
	}

	public void NOTSpawnPlayer(string id)
	{
		myLastHero = id.ToUpper();
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("dead", true);
		ExitGames.Client.Photon.Hashtable customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.isTitan, 1);
		customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
		{
			Screen.lockCursor = true;
		}
		else
		{
			Screen.lockCursor = false;
		}
		Screen.showCursor = false;
		ShowHUDInfoCenter("the game has started for 60 seconds.\n please wait for next round.\n Click Right Mouse Key to Enter or Exit the Spectator Mode.");
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
	}

	public void NOTSpawnNonAITitan(string id)
	{
		myLastHero = id.ToUpper();
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("dead", true);
		ExitGames.Client.Photon.Hashtable customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.isTitan, 2);
		customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
		{
			Screen.lockCursor = true;
		}
		else
		{
			Screen.lockCursor = false;
		}
		Screen.showCursor = true;
		ShowHUDInfoCenter("the game has started for 60 seconds.\n please wait for next round.\n Click Right Mouse Key to Enter or Exit the Spectator Mode.");
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
	}

	public void SpawnNonAITitan(string id, string tag = "titanRespawn")
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
		GameObject gameObject = array[Random.Range(0, array.Length)];
		myLastHero = id.ToUpper();
		GameObject gameObject2 = ((IN_GAME_MAIN_CAMERA.gamemode != GAMEMODE.PVP_CAPTURE) ? PhotonNetwork.Instantiate("TITAN_VER3.1", gameObject.transform.position, gameObject.transform.rotation, 0) : PhotonNetwork.Instantiate("TITAN_VER3.1", checkpoint.transform.position + new Vector3(Random.Range(-20, 20), 2f, Random.Range(-20, 20)), checkpoint.transform.rotation, 0));
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObjectASTITAN(gameObject2);
		gameObject2.GetComponent<TITAN>().nonAI = true;
		gameObject2.GetComponent<TITAN>().speed = 30f;
		gameObject2.GetComponent<TITAN_CONTROLLER>().enabled = true;
		if (id == "RANDOM" && Random.Range(0, 100) < 7)
		{
			gameObject2.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER, true);
		}
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
		GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
		GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("dead", false);
		ExitGames.Client.Photon.Hashtable customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.isTitan, 2);
		customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
		{
			Screen.lockCursor = true;
		}
		else
		{
			Screen.lockCursor = false;
		}
		Screen.showCursor = true;
		ShowHUDInfoCenter(string.Empty);
	}

	public void OnConnectedToPhoton()
	{
		UnityEngine.MonoBehaviour.print("OnConnectedToPhoton");
	}

	public void OnLeftRoom()
	{
		UnityEngine.MonoBehaviour.print("OnLeftRoom");
		if (Application.loadedLevel != 0)
		{
			Time.timeScale = 1f;
			if (PhotonNetwork.connected)
			{
				PhotonNetwork.Disconnect();
			}
			IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
			gameStart = false;
			Screen.lockCursor = false;
			Screen.showCursor = true;
			GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
			Object.Destroy(GameObject.Find("MultiplayerManager"));
			Application.LoadLevel("menu");
		}
	}

	public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		UnityEngine.MonoBehaviour.print("OnMasterClientSwitched");
		if (!gameTimesUp && PhotonNetwork.isMasterClient)
		{
			restartGame(true);
		}
	}

	public void OnPhotonCreateRoomFailed()
	{
		UnityEngine.MonoBehaviour.print("OnPhotonCreateRoomFailed");
	}

	public void OnPhotonJoinRoomFailed()
	{
		UnityEngine.MonoBehaviour.print("OnPhotonJoinRoomFailed");
	}

	public void OnCreatedRoom()
	{
		kicklist = new ArrayList();
		racingResult = new ArrayList();
		teamScores = new int[2];
		UnityEngine.MonoBehaviour.print("OnCreatedRoom");
	}

	public void OnJoinedLobby()
	{
		UnityEngine.MonoBehaviour.print("OnJoinedLobby");
		NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMultiStart, false);
		NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMultiROOM, true);
	}

	public void OnLeftLobby()
	{
		UnityEngine.MonoBehaviour.print("OnLeftLobby");
	}

	public void OnDisconnectedFromPhoton()
	{
		UnityEngine.MonoBehaviour.print("OnDisconnectedFromPhoton");
		Screen.lockCursor = false;
		Screen.showCursor = true;
	}

	public void OnConnectionFail(DisconnectCause cause)
	{
		UnityEngine.MonoBehaviour.print("OnConnectionFail : " + cause);
		Screen.lockCursor = false;
		Screen.showCursor = true;
		IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
		gameStart = false;
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[4], true);
		GameObject.Find("LabelDisconnectInfo").GetComponent<UILabel>().text = "OnConnectionFail : " + cause;
	}

	public void OnFailedToConnectToPhoton()
	{
		UnityEngine.MonoBehaviour.print("OnFailedToConnectToPhoton");
	}

	public void OnReceivedRoomListUpdate()
	{
	}

	public void OnJoinedRoom()
	{
		UnityEngine.MonoBehaviour.print("OnJoinedRoom " + PhotonNetwork.room.name + "    >>>>   " + LevelInfo.getInfo(PhotonNetwork.room.name.Split("`"[0])[1]).mapName);
		gameTimesUp = false;
		string[] array = PhotonNetwork.room.name.Split("`"[0]);
		level = array[1];
		if (array[2] == "normal")
		{
			difficulty = 0;
		}
		else if (array[2] == "hard")
		{
			difficulty = 1;
		}
		else if (array[2] == "abnormal")
		{
			difficulty = 2;
		}
		IN_GAME_MAIN_CAMERA.difficulty = difficulty;
		time = int.Parse(array[3]);
		time *= 60;
		if (array[4] == "day")
		{
			IN_GAME_MAIN_CAMERA.dayLight = DayLight.Day;
		}
		else if (array[4] == "dawn")
		{
			IN_GAME_MAIN_CAMERA.dayLight = DayLight.Dawn;
		}
		else if (array[4] == "night")
		{
			IN_GAME_MAIN_CAMERA.dayLight = DayLight.Night;
		}
		IN_GAME_MAIN_CAMERA.gamemode = LevelInfo.getInfo(level).type;
		PhotonNetwork.LoadLevel("The Forest");
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.name, LoginFengKAI.player.name);
		ExitGames.Client.Photon.Hashtable customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.guildName, LoginFengKAI.player.guildname);
		customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.kills, 0);
		customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.max_dmg, 0);
		customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.total_dmg, 0);
		customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.deaths, 0);
		customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.dead, true);
		customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.isTitan, 0);
		customProperties = hashtable;
		PhotonNetwork.player.SetCustomProperties(customProperties);
		humanScore = 0;
		titanScore = 0;
		PVPtitanScore = 0;
		PVPhumanScore = 0;
		wave = 1;
		highestwave = 1;
		localRacingResult = string.Empty;
		needChooseSide = true;
		chatContent = new ArrayList();
		killInfoGO = new ArrayList();
		InRoomChat.messages = new List<string>();
		if (!PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("RequireStatus", PhotonTargets.MasterClient);
		}
	}

	[RPC]
	private void RequireStatus()
	{
		base.photonView.RPC("refreshStatus", PhotonTargets.Others, humanScore, titanScore, wave, highestwave, roundTime, timeTotalServer, startRacing, endRacing);
		base.photonView.RPC("refreshPVPStatus", PhotonTargets.Others, PVPhumanScore, PVPtitanScore);
		base.photonView.RPC("refreshPVPStatus_AHSS", PhotonTargets.Others, teamScores);
	}

	[RPC]
	private void refreshStatus(int score1, int score2, int wav, int highestWav, float time1, float time2, bool startRacin, bool endRacin)
	{
		humanScore = score1;
		titanScore = score2;
		wave = wav;
		highestwave = highestWav;
		roundTime = time1;
		timeTotalServer = time2;
		startRacing = startRacin;
		endRacing = endRacin;
		if (startRacing && (bool)GameObject.Find("door"))
		{
			GameObject.Find("door").SetActive(false);
		}
	}

	[RPC]
	private void refreshPVPStatus(int score1, int score2)
	{
		PVPhumanScore = score1;
		PVPtitanScore = score2;
	}

	[RPC]
	private void refreshPVPStatus_AHSS(int[] score1)
	{
		UnityEngine.MonoBehaviour.print(score1);
		teamScores = score1;
	}

	private void OnLevelWasLoaded(int level)
	{
		if (level == 0 || Application.loadedLevelName == "characterCreation" || Application.loadedLevelName == "SnapShot")
		{
			return;
		}
		ChangeQuality.setCurrentQuality();
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.GetPhotonView() == null || !gameObject.GetPhotonView().owner.isMasterClient)
			{
				Object.Destroy(gameObject);
			}
		}
		isWinning = false;
		gameStart = true;
		ShowHUDInfoCenter(string.Empty);
		GameObject gameObject2 = (GameObject)Object.Instantiate(Resources.Load("MainCamera_mono"), GameObject.Find("cameraDefaultPosition").transform.position, GameObject.Find("cameraDefaultPosition").transform.rotation);
		Object.Destroy(GameObject.Find("cameraDefaultPosition"));
		gameObject2.name = "MainCamera";
		Screen.lockCursor = true;
		Screen.showCursor = true;
		ui = (GameObject)Object.Instantiate(Resources.Load("UI_IN_GAME"));
		ui.name = "UI_IN_GAME";
		ui.SetActive(true);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], true);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], false);
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setHUDposition();
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setDayLight(IN_GAME_MAIN_CAMERA.dayLight);
		LevelInfo info = LevelInfo.getInfo(FengGameManagerMKII.level);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			single_kills = 0;
			single_maxDamage = 0;
			single_totalDamage = 0;
			GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
			GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = true;
			GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = true;
			IN_GAME_MAIN_CAMERA.gamemode = LevelInfo.getInfo(FengGameManagerMKII.level).type;
			SpawnPlayer(IN_GAME_MAIN_CAMERA.singleCharacter.ToUpper());
			if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
			{
				Screen.lockCursor = true;
			}
			else
			{
				Screen.lockCursor = false;
			}
			Screen.showCursor = false;
			int rate = 90;
			if (difficulty == 1)
			{
				rate = 70;
			}
			randomSpawnTitan("titanRespawn", rate, info.enemyNumber);
			return;
		}
		PVPcheckPoint.chkPts = new ArrayList();
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().enabled = false;
		GameObject.Find("MainCamera").GetComponent<CameraShake>().enabled = false;
		IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.MULTIPLAYER;
		if (info.type == GAMEMODE.TROST)
		{
			GameObject.Find("playerRespawn").SetActive(false);
			Object.Destroy(GameObject.Find("playerRespawn"));
			GameObject gameObject3 = GameObject.Find("rock");
			gameObject3.animation["lift"].speed = 0f;
			GameObject.Find("door_fine").SetActiveRecursively(false);
			GameObject.Find("door_broke").SetActiveRecursively(true);
			Object.Destroy(GameObject.Find("ppl"));
		}
		else if (info.type == GAMEMODE.BOSS_FIGHT_CT)
		{
			GameObject.Find("playerRespawnTrost").SetActive(false);
			Object.Destroy(GameObject.Find("playerRespawnTrost"));
		}
		if (needChooseSide)
		{
			ShowHUDInfoTopCenterADD("\n\nPRESS 1 TO ENTER GAME");
		}
		else
		{
			if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.TPS)
			{
				Screen.lockCursor = true;
			}
			else
			{
				Screen.lockCursor = false;
			}
			if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
			{
				if ((int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.isTitan] == 2)
				{
					checkpoint = GameObject.Find("PVPchkPtT");
				}
				else
				{
					checkpoint = GameObject.Find("PVPchkPtH");
				}
			}
			if ((int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.isTitan] == 2)
			{
				SpawnNonAITitan(myLastHero);
			}
			else
			{
				SpawnPlayer(myLastHero, myLastRespawnTag);
			}
		}
		if (info.type == GAMEMODE.BOSS_FIGHT_CT)
		{
			Object.Destroy(GameObject.Find("rock"));
		}
		if (PhotonNetwork.isMasterClient)
		{
			if (info.type == GAMEMODE.TROST)
			{
				if (!isPlayerAllDead())
				{
					GameObject gameObject4 = PhotonNetwork.Instantiate("TITAN_EREN_trost", new Vector3(-200f, 0f, -194f), Quaternion.Euler(0f, 180f, 0f), 0);
					gameObject4.GetComponent<TITAN_EREN>().rockLift = true;
					int rate2 = 90;
					if (difficulty == 1)
					{
						rate2 = 70;
					}
					GameObject[] array3 = GameObject.FindGameObjectsWithTag("titanRespawn");
					GameObject gameObject5 = GameObject.Find("titanRespawnTrost");
					if (gameObject5 != null)
					{
						GameObject[] array4 = array3;
						foreach (GameObject gameObject6 in array4)
						{
							if (gameObject6.transform.parent.gameObject == gameObject5)
							{
								spawnTitan(rate2, gameObject6.transform.position, gameObject6.transform.rotation);
							}
						}
					}
				}
			}
			else if (info.type == GAMEMODE.BOSS_FIGHT_CT)
			{
				if (!isPlayerAllDead())
				{
					PhotonNetwork.Instantiate("COLOSSAL_TITAN", -Vector3.up * 10000f, Quaternion.Euler(0f, 180f, 0f), 0);
				}
			}
			else if (info.type == GAMEMODE.KILL_TITAN || info.type == GAMEMODE.ENDLESS_TITAN || info.type == GAMEMODE.SURVIVE_MODE)
			{
				if (info.name == "Annie" || info.name == "Annie II")
				{
					PhotonNetwork.Instantiate("FEMALE_TITAN", GameObject.Find("titanRespawn").transform.position, GameObject.Find("titanRespawn").transform.rotation, 0);
				}
				else
				{
					int rate3 = 90;
					if (difficulty == 1)
					{
						rate3 = 70;
					}
					randomSpawnTitan("titanRespawn", rate3, info.enemyNumber);
				}
			}
			else if (info.type != GAMEMODE.TROST && info.type == GAMEMODE.PVP_CAPTURE && LevelInfo.getInfo(FengGameManagerMKII.level).mapName == "OutSide")
			{
				GameObject[] array5 = GameObject.FindGameObjectsWithTag("titanRespawn");
				if (array5.Length <= 0)
				{
					return;
				}
				for (int k = 0; k < array5.Length; k++)
				{
					GameObject gameObject7 = spawnTitanRaw(array5[k].transform.position, array5[k].transform.rotation);
					gameObject7.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER, true);
				}
			}
		}
		if (!info.supply)
		{
			Object.Destroy(GameObject.Find("aot_supply"));
		}
		if (!PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("RequireStatus", PhotonTargets.MasterClient);
		}
		if (LevelInfo.getInfo(FengGameManagerMKII.level).lavaMode)
		{
			Object.Instantiate(Resources.Load("levelBottom"), new Vector3(0f, -29.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
			GameObject.Find("aot_supply").transform.position = GameObject.Find("aot_supply_lava_position").transform.position;
			GameObject.Find("aot_supply").transform.rotation = GameObject.Find("aot_supply_lava_position").transform.rotation;
		}
	}

	public void OnPhotonPlayerConnected()
	{
		UnityEngine.MonoBehaviour.print("OnPhotonPlayerConnected");
	}

	public void OnPhotonPlayerDisconnected()
	{
		UnityEngine.MonoBehaviour.print("OnPhotonPlayerDisconnected");
		if (!gameTimesUp)
		{
			oneTitanDown(string.Empty, true);
			someOneIsDead(0);
		}
	}

	public void OnPhotonRandomJoinFailed()
	{
		UnityEngine.MonoBehaviour.print("OnPhotonRandomJoinFailed");
	}

	public void OnConnectedToMaster()
	{
		UnityEngine.MonoBehaviour.print("OnConnectedToMaster");
	}

	public void OnPhotonSerializeView()
	{
		UnityEngine.MonoBehaviour.print("OnPhotonSerializeView");
	}

	public void OnPhotonInstantiate()
	{
		UnityEngine.MonoBehaviour.print("OnPhotonInstantiate");
	}

	public void OnPhotonMaxCccuReached()
	{
		UnityEngine.MonoBehaviour.print("OnPhotonMaxCccuReached");
	}

	public void OnPhotonCustomRoomPropertiesChanged()
	{
		UnityEngine.MonoBehaviour.print("OnPhotonCustomRoomPropertiesChanged");
	}

	public void OnPhotonPlayerPropertiesChanged()
	{
	}

	public void OnUpdatedFriendList()
	{
		UnityEngine.MonoBehaviour.print("OnUpdatedFriendList");
	}

	public void OnCustomAuthenticationFailed()
	{
		UnityEngine.MonoBehaviour.print("OnCustomAuthenticationFailed");
	}

	[RPC]
	private void restartGameByClient()
	{
		restartGame();
	}

	public void restartGame(bool masterclientSwitched = false)
	{
		UnityEngine.MonoBehaviour.print("reset game :" + gameTimesUp);
		if (!gameTimesUp)
		{
			PVPtitanScore = 0;
			PVPhumanScore = 0;
			startRacing = false;
			endRacing = false;
			checkpoint = null;
			timeElapse = 0f;
			roundTime = 0f;
			isWinning = false;
			isLosing = false;
			isPlayer1Winning = false;
			isPlayer2Winning = false;
			wave = 1;
			myRespawnTime = 0f;
			kicklist = new ArrayList();
			killInfoGO = new ArrayList();
			racingResult = new ArrayList();
			ShowHUDInfoCenter(string.Empty);
			PhotonNetwork.DestroyAll();
			base.photonView.RPC("RPCLoadLevel", PhotonTargets.All);
			if (masterclientSwitched)
			{
				sendChatContentInfo("<color=#A8FF24>MasterClient has switched to </color>" + PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
			}
		}
	}

	public void restartGameSingle()
	{
		startRacing = false;
		endRacing = false;
		checkpoint = null;
		single_kills = 0;
		single_maxDamage = 0;
		single_totalDamage = 0;
		timeElapse = 0f;
		roundTime = 0f;
		timeTotalServer = 0f;
		isWinning = false;
		isLosing = false;
		isPlayer1Winning = false;
		isPlayer2Winning = false;
		wave = 1;
		myRespawnTime = 0f;
		ShowHUDInfoCenter(string.Empty);
		Application.LoadLevel("The Forest");
	}

	[RPC]
	private void RPCLoadLevel()
	{
		PhotonNetwork.LoadLevel("The Forest");
	}

	[RPC]
	public void someOneIsDead(int id = -1)
	{
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
		{
			if (id != 0)
			{
				PVPtitanScore += 2;
			}
			checkPVPpts();
			base.photonView.RPC("refreshPVPStatus", PhotonTargets.Others, PVPhumanScore, PVPtitanScore);
		}
		else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
		{
			titanScore++;
		}
		else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.TROST)
		{
			if (isPlayerAllDead())
			{
				gameLose();
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
		{
			if (isPlayerAllDead())
			{
				gameLose();
				teamWinner = 0;
			}
			if (isTeamAllDead(1))
			{
				teamWinner = 2;
				gameWin();
			}
			if (isTeamAllDead(2))
			{
				teamWinner = 1;
				gameWin();
			}
		}
	}

	public void checkPVPpts()
	{
		if (PVPtitanScore >= PVPtitanScoreMax)
		{
			PVPtitanScore = PVPtitanScoreMax;
			gameLose();
		}
		else if (PVPhumanScore >= PVPhumanScoreMax)
		{
			PVPhumanScore = PVPhumanScoreMax;
			gameWin();
		}
	}

	[RPC]
	private void netGameLose(int score)
	{
		isLosing = true;
		titanScore = score;
		gameEndCD = gameEndTotalCDtime;
	}

	public void gameLose()
	{
		if (!isWinning && !isLosing)
		{
			isLosing = true;
			titanScore++;
			gameEndCD = gameEndTotalCDtime;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
			{
				base.photonView.RPC("netGameLose", PhotonTargets.Others, titanScore);
			}
		}
	}

	[RPC]
	private void netGameWin(int score)
	{
		humanScore = score;
		isWinning = true;
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
		{
			teamWinner = score;
			teamScores[teamWinner - 1]++;
			gameEndCD = gameEndTotalCDtime;
		}
		else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
		{
			gameEndCD = 20f;
		}
		else
		{
			gameEndCD = gameEndTotalCDtime;
		}
	}

	public void gameWin()
	{
		if (isLosing || isWinning)
		{
			return;
		}
		isWinning = true;
		humanScore++;
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.RACING)
		{
			gameEndCD = 20f;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
			{
				base.photonView.RPC("netGameWin", PhotonTargets.Others, 0);
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
		{
			gameEndCD = gameEndTotalCDtime;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
			{
				base.photonView.RPC("netGameWin", PhotonTargets.Others, teamWinner);
			}
			teamScores[teamWinner - 1]++;
		}
		else
		{
			gameEndCD = gameEndTotalCDtime;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
			{
				base.photonView.RPC("netGameWin", PhotonTargets.Others, humanScore);
			}
		}
	}

	public void multiplayerRacingFinsih()
	{
		float num = roundTime - 20f;
		if (PhotonNetwork.isMasterClient)
		{
			getRacingResult(LoginFengKAI.player.name, num);
		}
		else
		{
			base.photonView.RPC("getRacingResult", PhotonTargets.MasterClient, LoginFengKAI.player.name, num);
		}
		gameWin();
	}

	[RPC]
	private void getRacingResult(string player, float time)
	{
		RacingResult racingResult = new RacingResult();
		racingResult.name = player;
		racingResult.time = time;
		this.racingResult.Add(racingResult);
		refreshRacingResult();
	}

	private void refreshRacingResult()
	{
		localRacingResult = "Result\n";
		IComparer comparer = new IComparerRacingResult();
		racingResult.Sort(comparer);
		int count = racingResult.Count;
		count = Mathf.Min(count, 6);
		for (int i = 0; i < count; i++)
		{
			string text = localRacingResult;
			localRacingResult = text + "Rank " + (i + 1) + " : ";
			localRacingResult += (racingResult[i] as RacingResult).name;
			localRacingResult = localRacingResult + "   " + (float)(int)((racingResult[i] as RacingResult).time * 100f) * 0.01f + "s";
			localRacingResult += "\n";
		}
		base.photonView.RPC("netRefreshRacingResult", PhotonTargets.All, localRacingResult);
	}

	[RPC]
	private void netRefreshRacingResult(string tmp)
	{
		localRacingResult = tmp;
	}

	public void randomSpawnTitan(string place, int rate, int num, bool punk = false)
	{
		if (num == -1)
		{
			num = 1;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag(place);
		if (array.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			int num2 = Random.Range(0, array.Length);
			GameObject gameObject = array[num2];
			while (array[num2] == null)
			{
				num2 = Random.Range(0, array.Length);
				gameObject = array[num2];
			}
			array[num2] = null;
			spawnTitan(rate, gameObject.transform.position, gameObject.transform.rotation, punk);
		}
	}

	public GameObject randomSpawnOneTitan(string place, int rate)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(place);
		int num = Random.Range(0, array.Length);
		GameObject gameObject = array[num];
		while (array[num] == null)
		{
			num = Random.Range(0, array.Length);
			gameObject = array[num];
		}
		array[num] = null;
		return spawnTitan(rate, gameObject.transform.position, gameObject.transform.rotation);
	}

	private GameObject spawnTitanRaw(Vector3 position, Quaternion rotation)
	{
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			return (GameObject)Object.Instantiate(Resources.Load("TITAN_VER3.1"), position, rotation);
		}
		return PhotonNetwork.Instantiate("TITAN_VER3.1", position, rotation, 0);
	}

	public GameObject spawnTitan(int rate, Vector3 position, Quaternion rotation, bool punk = false)
	{
		GameObject gameObject = spawnTitanRaw(position, rotation);
		if (punk)
		{
			gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_PUNK);
		}
		else if (Random.Range(0, 100) < rate)
		{
			if (IN_GAME_MAIN_CAMERA.difficulty == 2)
			{
				if (Random.Range(0f, 1f) < 0.7f || LevelInfo.getInfo(level).noCrawler)
				{
					gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
				}
				else
				{
					gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
				}
			}
		}
		else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
		{
			if (Random.Range(0f, 1f) < 0.7f || LevelInfo.getInfo(level).noCrawler)
			{
				gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
			}
			else
			{
				gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
			}
		}
		else if (Random.Range(0, 100) < rate)
		{
			if (Random.Range(0f, 1f) < 0.8f || LevelInfo.getInfo(level).noCrawler)
			{
				gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_I);
			}
			else
			{
				gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
			}
		}
		else if (Random.Range(0f, 1f) < 0.8f || LevelInfo.getInfo(level).noCrawler)
		{
			gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
		}
		else
		{
			gameObject.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
		}
		GameObject gameObject2 = ((IN_GAME_MAIN_CAMERA.gametype != 0) ? PhotonNetwork.Instantiate("FX/FXtitanSpawn", gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0) : ((GameObject)Object.Instantiate(Resources.Load("FX/FXtitanSpawn"), gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f))));
		gameObject2.transform.localScale = gameObject.transform.localScale;
		return gameObject;
	}

	[RPC]
	public void titanGetKill(PhotonPlayer player, int Damage, string name)
	{
		Damage = Mathf.Max(10, Damage);
		base.photonView.RPC("netShowDamage", player, Damage);
		base.photonView.RPC("oneTitanDown", PhotonTargets.MasterClient, name, false);
		sendKillInfo(false, (string)player.customProperties[PhotonPlayerProperty.name], true, name, Damage);
		playerKillInfoUpdate(player, Damage);
	}

	public void titanGetKillbyServer(int Damage, string name)
	{
		Damage = Mathf.Max(10, Damage);
		sendKillInfo(false, LoginFengKAI.player.name, true, name, Damage);
		netShowDamage(Damage);
		oneTitanDown(name);
		playerKillInfoUpdate(PhotonNetwork.player, Damage);
	}

	public void playerKillInfoUpdate(PhotonPlayer player, int dmg)
	{
		player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
		{
			PhotonPlayerProperty.kills,
			(int)player.customProperties[PhotonPlayerProperty.kills] + 1
		} });
		player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
		{
			PhotonPlayerProperty.max_dmg,
			Mathf.Max(dmg, (int)player.customProperties[PhotonPlayerProperty.max_dmg])
		} });
		player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
		{
			PhotonPlayerProperty.total_dmg,
			(int)player.customProperties[PhotonPlayerProperty.total_dmg] + dmg
		} });
	}

	public void playerKillInfoSingleUpdate(int dmg)
	{
		single_kills++;
		single_maxDamage = Mathf.Max(dmg, single_maxDamage);
		single_totalDamage += dmg;
	}

	[RPC]
	public void oneTitanDown(string name1 = "", bool onPlayerLeave = false)
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE)
		{
			if (!(name1 == string.Empty))
			{
				switch (name1)
				{
				case "Titan":
					PVPhumanScore++;
					break;
				case "Aberrant":
					PVPhumanScore += 2;
					break;
				case "Jumper":
					PVPhumanScore += 3;
					break;
				case "Crawler":
					PVPhumanScore += 4;
					break;
				case "Female Titan":
					PVPhumanScore += 10;
					break;
				default:
					PVPhumanScore += 3;
					break;
				}
			}
			checkPVPpts();
			base.photonView.RPC("refreshPVPStatus", PhotonTargets.Others, PVPhumanScore, PVPtitanScore);
		}
		else
		{
			if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.CAGE_FIGHT)
			{
				return;
			}
			if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.KILL_TITAN)
			{
				if (checkIsTitanAllDie())
				{
					gameWin();
					GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
				}
			}
			else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
			{
				if (!checkIsTitanAllDie())
				{
					return;
				}
				wave++;
				if (LevelInfo.getInfo(level).respawnMode == RespawnMode.NEWROUND && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
				{
					base.photonView.RPC("respawnHeroInNewRound", PhotonTargets.All);
				}
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
				{
					sendChatContentInfo("<color=#A8FF24>Wave : " + wave + "</color>");
				}
				if (wave > highestwave)
				{
					highestwave = wave;
				}
				if (PhotonNetwork.isMasterClient)
				{
					RequireStatus();
				}
				if (wave > 20)
				{
					gameWin();
					return;
				}
				int rate = 90;
				if (difficulty == 1)
				{
					rate = 70;
				}
				if (!LevelInfo.getInfo(level).punk)
				{
					randomSpawnTitan("titanRespawn", rate, wave + 2);
				}
				else if (wave == 5)
				{
					randomSpawnTitan("titanRespawn", rate, 1, true);
				}
				else if (wave == 10)
				{
					randomSpawnTitan("titanRespawn", rate, 2, true);
				}
				else if (wave == 15)
				{
					randomSpawnTitan("titanRespawn", rate, 3, true);
				}
				else if (wave == 20)
				{
					randomSpawnTitan("titanRespawn", rate, 4, true);
				}
				else
				{
					randomSpawnTitan("titanRespawn", rate, wave + 2);
				}
			}
			else if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN)
			{
				if (!onPlayerLeave)
				{
					humanScore++;
					int rate2 = 90;
					if (difficulty == 1)
					{
						rate2 = 70;
					}
					randomSpawnTitan("titanRespawn", rate2, 1);
				}
			}
			else if (LevelInfo.getInfo(level).enemyNumber != -1)
			{
			}
		}
	}

	[RPC]
	private void respawnHeroInNewRound()
	{
		if (!needChooseSide && GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver)
		{
			SpawnPlayer(myLastHero, myLastRespawnTag);
			GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = false;
			ShowHUDInfoCenter(string.Empty);
		}
	}

	private bool checkIsTitanAllDie()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if ((bool)gameObject.GetComponent<TITAN>() && !gameObject.GetComponent<TITAN>().hasDie)
			{
				return false;
			}
			if ((bool)gameObject.GetComponent<FEMALE_TITAN>())
			{
				return false;
			}
		}
		return true;
	}

	[RPC]
	public void netShowDamage(int speed)
	{
		GameObject.Find("Stylish").GetComponent<StylishComponent>().Style(speed);
		GameObject gameObject = GameObject.Find("LabelScore");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = speed.ToString();
			gameObject.transform.localScale = Vector3.zero;
			speed = (int)((float)speed * 0.1f);
			speed = Mathf.Max(40, speed);
			speed = Mathf.Min(150, speed);
			iTween.Stop(gameObject);
			iTween.ScaleTo(gameObject, iTween.Hash("x", speed, "y", speed, "z", speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f));
			iTween.ScaleTo(gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f));
		}
	}

	[RPC]
	private void showResult(string text0, string text1, string text2, string text3, string text4, string text6, PhotonMessageInfo t)
	{
		if (!gameTimesUp)
		{
			gameTimesUp = true;
			GameObject gameObject = GameObject.Find("UI_IN_GAME");
			NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[0], false);
			NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[1], false);
			NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[2], true);
			NGUITools.SetActive(gameObject.GetComponent<UIReferArray>().panels[3], false);
			GameObject.Find("LabelName").GetComponent<UILabel>().text = text0;
			GameObject.Find("LabelKill").GetComponent<UILabel>().text = text1;
			GameObject.Find("LabelDead").GetComponent<UILabel>().text = text2;
			GameObject.Find("LabelMaxDmg").GetComponent<UILabel>().text = text3;
			GameObject.Find("LabelTotalDmg").GetComponent<UILabel>().text = text4;
			GameObject.Find("LabelResultTitle").GetComponent<UILabel>().text = text6;
			Screen.lockCursor = false;
			Screen.showCursor = true;
			IN_GAME_MAIN_CAMERA.gametype = GAMETYPE.STOP;
			gameStart = false;
		}
	}

	public void sendKillInfo(bool t1, string killer, bool t2, string victim, int dmg = 0)
	{
		base.photonView.RPC("updateKillInfo", PhotonTargets.All, t1, killer, t2, victim, dmg);
	}

	[RPC]
	private void updateKillInfo(bool t1, string killer, bool t2, string victim, int dmg)
	{
		GameObject gameObject = GameObject.Find("UI_IN_GAME");
		GameObject gameObject2 = (GameObject)Object.Instantiate(Resources.Load("UI/KillInfo"));
		for (int i = 0; i < killInfoGO.Count; i++)
		{
			GameObject gameObject3 = (GameObject)killInfoGO[i];
			if (gameObject3 != null)
			{
				gameObject3.GetComponent<KillInfoComponent>().moveOn();
			}
		}
		if (killInfoGO.Count > 4)
		{
			GameObject gameObject3 = (GameObject)killInfoGO[0];
			if (gameObject3 != null)
			{
				gameObject3.GetComponent<KillInfoComponent>().destory();
			}
			killInfoGO.RemoveAt(0);
		}
		gameObject2.transform.parent = gameObject.GetComponent<UIReferArray>().panels[0].transform;
		gameObject2.GetComponent<KillInfoComponent>().show(t1, killer, t2, victim, dmg);
		killInfoGO.Add(gameObject2);
	}

	public void sendChatContentInfo(string content)
	{
		base.photonView.RPC("Chat", PhotonTargets.All, content, string.Empty);
	}

	[RPC]
	private void Chat(string content, string sender)
	{
		if (content.Length > 7 && content.Substring(0, 7) == "/kick #")
		{
			if (PhotonNetwork.isMasterClient)
			{
				kickPlayer(content.Remove(0, 7), sender);
			}
			return;
		}
		if (sender != string.Empty)
		{
			content = sender + ":" + content;
		}
		GameObject.Find("Chatroom").GetComponent<InRoomChat>().addLINE(content);
	}

	private void kickPlayer(string kickPlayer, string kicker)
	{
		bool flag = false;
		for (int i = 0; i < kicklist.Count; i++)
		{
			if (((KickState)kicklist[i]).name == kickPlayer)
			{
				KickState kickState = (KickState)kicklist[i];
				kickState.addKicker(kicker);
				tryKick(kickState);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			KickState kickState = new KickState();
			kickState.init(kickPlayer);
			kickState.addKicker(kicker);
			kicklist.Add(kickState);
			tryKick(kickState);
		}
	}

	private void tryKick(KickState tmp)
	{
		sendChatContentInfo("kicking #" + tmp.name + ", " + tmp.getKickCount() + "/" + (int)((float)PhotonNetwork.playerList.Length * 0.5f) + "vote");
		if (tmp.getKickCount() >= (int)((float)PhotonNetwork.playerList.Length * 0.5f))
		{
			kickPhotonPlayer(tmp.name.ToString());
		}
	}

	private void kickPhotonPlayer(string name)
	{
		UnityEngine.MonoBehaviour.print("KICK " + name + "!!!");
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			if (photonPlayer.ID.ToString() == name && !photonPlayer.isMasterClient)
			{
				PhotonNetwork.CloseConnection(photonPlayer);
				break;
			}
		}
	}

	[RPC]
	private void showChatContent(string content)
	{
		chatContent.Add(content);
		if (chatContent.Count > 10)
		{
			chatContent.RemoveAt(0);
		}
		GameObject.Find("LabelChatContent").GetComponent<UILabel>().text = string.Empty;
		for (int i = 0; i < chatContent.Count; i++)
		{
			GameObject.Find("LabelChatContent").GetComponent<UILabel>().text += chatContent[i];
		}
	}

	private void ShowHUDInfoTopCenter(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopCenter");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	private void ShowHUDInfoTopCenterADD(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopCenter");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text += content;
		}
	}

	private void ShowHUDInfoTopLeft(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopLeft");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	private void ShowHUDInfoTopRight(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopRight");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	private void ShowHUDInfoTopRightMAPNAME(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopRight");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text += content;
		}
	}

	public void ShowHUDInfoCenter(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoCenter");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	public void ShowHUDInfoCenterADD(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoCenter");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text += content;
		}
	}

	public bool isPlayerAllDead()
	{
		int num = 0;
		int num2 = 0;
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			if ((int)photonPlayer.customProperties[PhotonPlayerProperty.isTitan] == 1)
			{
				num++;
				if ((bool)photonPlayer.customProperties[PhotonPlayerProperty.dead])
				{
					num2++;
				}
			}
		}
		if (num == num2)
		{
			return true;
		}
		return false;
	}

	public bool isTeamAllDead(int team)
	{
		int num = 0;
		int num2 = 0;
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			if ((int)photonPlayer.customProperties[PhotonPlayerProperty.isTitan] == 1 && (int)photonPlayer.customProperties[PhotonPlayerProperty.team] == team)
			{
				num++;
				if ((bool)photonPlayer.customProperties[PhotonPlayerProperty.dead])
				{
					num2++;
				}
			}
		}
		if (num == num2)
		{
			return true;
		}
		return false;
	}

	private void SingleShowHUDInfoTopLeft(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopLeft");
		if ((bool)gameObject)
		{
			content = content.Replace("[0]", "[*^_^*]");
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	private void SingleShowHUDInfoTopCenter(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopCenter");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}
}
