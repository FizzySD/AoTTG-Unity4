using System.Collections;
using Photon;
using UnityEngine;

public class COLOSSAL_TITAN : Photon.MonoBehaviour
{
	public bool hasDie;

	public GameObject myHero;

	private string state = "idle";

	public float myDistance;

	public static float minusDistance = 99999f;

	public static GameObject minusDistanceEnemy;

	private int attackCount;

	public GameObject bottomObject;

	private float tauntTime;

	public int NapeArmor = 10000;

	public int NapeArmorTotal = 10000;

	private string actionName;

	private Transform checkHitCapsuleStart;

	private Transform checkHitCapsuleEnd;

	private Vector3 checkHitCapsuleEndOld;

	private float checkHitCapsuleR;

	private float attackCheckTime;

	private float attackCheckTimeA;

	private float attackCheckTimeB;

	private bool attackChkOnce;

	private string attackAnimation;

	private int attackPattern = -1;

	public GameObject door_broken;

	public GameObject door_closed;

	public GameObject neckSteamObject;

	public GameObject sweepSmokeObject;

	private float waitTime = 2f;

	private bool isSteamNeed;

	private void OnDestroy()
	{
		if (GameObject.Find("MultiplayerManager") != null)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().removeCT(this);
		}
	}

	private void Start()
	{
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().addCT(this);
		if (myHero == null)
		{
			findNearestHero();
		}
		base.name = "COLOSSAL_TITAN";
		NapeArmor = 1000;
		bool flag = false;
		if (LevelInfo.getInfo(FengGameManagerMKII.level).respawnMode == RespawnMode.NEVER)
		{
			flag = true;
		}
		if (IN_GAME_MAIN_CAMERA.difficulty == 0)
		{
			NapeArmor = ((!flag) ? 5000 : 2000);
		}
		else if (IN_GAME_MAIN_CAMERA.difficulty == 1)
		{
			NapeArmor = ((!flag) ? 8000 : 3500);
			foreach (AnimationState item in base.animation)
			{
				item.speed = 1.02f;
			}
		}
		else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
		{
			NapeArmor = ((!flag) ? 12000 : 5000);
			foreach (AnimationState item2 in base.animation)
			{
				item2.speed = 1.05f;
			}
		}
		NapeArmorTotal = NapeArmor;
		state = "wait";
		base.transform.position += -Vector3.up * 10000f;
		if (FengGameManagerMKII.LAN)
		{
			GetComponent<PhotonView>().enabled = false;
		}
		else
		{
			GetComponent<NetworkView>().enabled = false;
		}
		door_broken = GameObject.Find("door_broke");
		door_closed = GameObject.Find("door_fine");
		door_broken.SetActiveRecursively(false);
		door_closed.SetActiveRecursively(true);
	}

	private RaycastHit[] checkHitCapsule(Vector3 start, Vector3 end, float r)
	{
		return Physics.SphereCastAll(start, r, end - start, Vector3.Distance(start, end));
	}

	private void Awake()
	{
		base.rigidbody.freezeRotation = true;
		base.rigidbody.useGravity = false;
		base.rigidbody.isKinematic = true;
	}

	private void playAnimation(string aniName)
	{
		base.animation.Play(aniName);
		if (!FengGameManagerMKII.LAN && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("netPlayAnimation", PhotonTargets.Others, aniName);
		}
	}

	private void playAnimationAt(string aniName, float normalizedTime)
	{
		base.animation.Play(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
		if (!FengGameManagerMKII.LAN && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, aniName, normalizedTime);
		}
	}

	private void crossFade(string aniName, float time)
	{
		base.animation.CrossFade(aniName, time);
		if (!FengGameManagerMKII.LAN && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("netCrossFade", PhotonTargets.Others, aniName, time);
		}
	}

	[RPC]
	private void netPlayAnimation(string aniName)
	{
		base.animation.Play(aniName);
	}

	[RPC]
	private void netPlayAnimationAt(string aniName, float normalizedTime)
	{
		base.animation.Play(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
	}

	[RPC]
	private void netCrossFade(string aniName, float time)
	{
		base.animation.CrossFade(aniName, time);
	}

	private void findNearestHero()
	{
		myHero = getNearestHero();
	}

	private GameObject getNearestHero()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject result = null;
		float num = float.PositiveInfinity;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if ((!gameObject.GetComponent<HERO>() || !gameObject.GetComponent<HERO>().HasDied()) && (!gameObject.GetComponent<TITAN_EREN>() || !gameObject.GetComponent<TITAN_EREN>().hasDied))
			{
				float num2 = Mathf.Sqrt((gameObject.transform.position.x - base.transform.position.x) * (gameObject.transform.position.x - base.transform.position.x) + (gameObject.transform.position.z - base.transform.position.z) * (gameObject.transform.position.z - base.transform.position.z));
				if (gameObject.transform.position.y - base.transform.position.y < 450f && num2 < num)
				{
					result = gameObject;
					num = num2;
				}
			}
		}
		return result;
	}

	private GameObject checkIfHitHand(Transform hand)
	{
		float num = 30f;
		Collider[] array = Physics.OverlapSphere(hand.GetComponent<SphereCollider>().transform.position, num + 1f);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (!(collider.transform.root.tag == "Player"))
			{
				continue;
			}
			GameObject gameObject = collider.transform.root.gameObject;
			if ((bool)gameObject.GetComponent<TITAN_EREN>())
			{
				if (!gameObject.GetComponent<TITAN_EREN>().isHit)
				{
					gameObject.GetComponent<TITAN_EREN>().hitByTitan();
				}
				return gameObject;
			}
			if ((bool)gameObject.GetComponent<HERO>() && !gameObject.GetComponent<HERO>().isInvincible())
			{
				return gameObject;
			}
		}
		return null;
	}

	public void update()
	{
		if (state == "null")
		{
			return;
		}
		if (state == "wait")
		{
			waitTime -= Time.deltaTime;
			if (waitTime <= 0f)
			{
				base.transform.position = new Vector3(30f, 0f, 784f);
				Object.Instantiate(Resources.Load("FX/ThunderCT"), base.transform.position + Vector3.up * 350f, Quaternion.Euler(270f, 0f, 0f));
				GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().flashBlind();
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					idle();
				}
				else if ((!FengGameManagerMKII.LAN) ? base.photonView.isMine : base.networkView.isMine)
				{
					idle();
				}
				else
				{
					state = "null";
				}
			}
		}
		else if (state == "idle")
		{
			if (attackPattern == -1)
			{
				slap("r1");
				attackPattern++;
				return;
			}
			if (attackPattern == 0)
			{
				attack_sweep(string.Empty);
				attackPattern++;
				return;
			}
			if (attackPattern == 1)
			{
				steam();
				attackPattern++;
				return;
			}
			if (attackPattern == 2)
			{
				kick();
				attackPattern++;
				return;
			}
			if (isSteamNeed || hasDie)
			{
				steam();
				isSteamNeed = false;
				return;
			}
			if (myHero == null)
			{
				findNearestHero();
				return;
			}
			Vector3 vector = myHero.transform.position - base.transform.position;
			float current = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
			float f = 0f - Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
			myDistance = Mathf.Sqrt((myHero.transform.position.x - base.transform.position.x) * (myHero.transform.position.x - base.transform.position.x) + (myHero.transform.position.z - base.transform.position.z) * (myHero.transform.position.z - base.transform.position.z));
			float num = myHero.transform.position.y - base.transform.position.y;
			if (myDistance < 85f && Random.Range(0, 100) < 5)
			{
				steam();
				return;
			}
			if (num > 310f && num < 350f)
			{
				if (Vector3.Distance(myHero.transform.position, base.transform.Find("APL1").position) < 40f)
				{
					slap("l1");
					return;
				}
				if (Vector3.Distance(myHero.transform.position, base.transform.Find("APL2").position) < 40f)
				{
					slap("l2");
					return;
				}
				if (Vector3.Distance(myHero.transform.position, base.transform.Find("APR1").position) < 40f)
				{
					slap("r1");
					return;
				}
				if (Vector3.Distance(myHero.transform.position, base.transform.Find("APR2").position) < 40f)
				{
					slap("r2");
					return;
				}
				if (myDistance < 150f && Mathf.Abs(f) < 80f)
				{
					attack_sweep(string.Empty);
					return;
				}
			}
			if (num < 300f && Mathf.Abs(f) < 80f && myDistance < 85f)
			{
				attack_sweep("_vertical");
				return;
			}
			switch (Random.Range(0, 7))
			{
			case 0:
				slap("l1");
				break;
			case 1:
				slap("l2");
				break;
			case 2:
				slap("r1");
				break;
			case 3:
				slap("r2");
				break;
			case 4:
				attack_sweep(string.Empty);
				break;
			case 5:
				attack_sweep("_vertical");
				break;
			case 6:
				steam();
				break;
			}
		}
		else if (state == "attack_sweep")
		{
			if (attackCheckTimeA != 0f && ((base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA && base.animation["attack_" + attackAnimation].normalizedTime <= attackCheckTimeB) || (!attackChkOnce && base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA)))
			{
				if (!attackChkOnce)
				{
					attackChkOnce = true;
				}
				RaycastHit[] array = checkHitCapsule(checkHitCapsuleStart.position, checkHitCapsuleEnd.position, checkHitCapsuleR);
				foreach (RaycastHit raycastHit in array)
				{
					GameObject gameObject = raycastHit.collider.gameObject;
					if (gameObject.tag == "Player")
					{
						killPlayer(gameObject);
					}
					if (gameObject.tag == "erenHitbox" && attackAnimation == "combo_3" && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && ((!FengGameManagerMKII.LAN) ? PhotonNetwork.isMasterClient : Network.isServer))
					{
						gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByFTByServer(3);
					}
				}
				array = checkHitCapsule(checkHitCapsuleEndOld, checkHitCapsuleEnd.position, checkHitCapsuleR);
				foreach (RaycastHit raycastHit2 in array)
				{
					GameObject gameObject2 = raycastHit2.collider.gameObject;
					if (gameObject2.tag == "Player")
					{
						killPlayer(gameObject2);
					}
				}
				checkHitCapsuleEndOld = checkHitCapsuleEnd.position;
			}
			if (base.animation["attack_" + attackAnimation].normalizedTime >= 1f)
			{
				sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = false;
				sweepSmokeObject.GetComponent<ParticleSystem>().Stop();
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && !FengGameManagerMKII.LAN)
				{
					base.photonView.RPC("stopSweepSmoke", PhotonTargets.Others);
				}
				findNearestHero();
				idle();
				playAnimation("idle");
			}
		}
		else if (state == "kick")
		{
			if (!attackChkOnce && base.animation[actionName].normalizedTime >= attackCheckTime)
			{
				attackChkOnce = true;
				door_broken.SetActiveRecursively(true);
				door_closed.SetActiveRecursively(false);
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && !FengGameManagerMKII.LAN)
				{
					base.photonView.RPC("changeDoor", PhotonTargets.OthersBuffered);
				}
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
				{
					if (FengGameManagerMKII.LAN)
					{
						Network.Instantiate(Resources.Load("FX/boom1_CT_KICK"), base.transform.position + base.transform.forward * 120f + base.transform.right * 30f, Quaternion.Euler(270f, 0f, 0f), 0);
						Network.Instantiate(Resources.Load("rock"), base.transform.position + base.transform.forward * 120f + base.transform.right * 30f, Quaternion.Euler(0f, 0f, 0f), 0);
					}
					else
					{
						PhotonNetwork.Instantiate("FX/boom1_CT_KICK", base.transform.position + base.transform.forward * 120f + base.transform.right * 30f, Quaternion.Euler(270f, 0f, 0f), 0);
						PhotonNetwork.Instantiate("rock", base.transform.position + base.transform.forward * 120f + base.transform.right * 30f, Quaternion.Euler(0f, 0f, 0f), 0);
					}
				}
				else
				{
					Object.Instantiate(Resources.Load("FX/boom1_CT_KICK"), base.transform.position + base.transform.forward * 120f + base.transform.right * 30f, Quaternion.Euler(270f, 0f, 0f));
					Object.Instantiate(Resources.Load("rock"), base.transform.position + base.transform.forward * 120f + base.transform.right * 30f, Quaternion.Euler(0f, 0f, 0f));
				}
			}
			if (base.animation[actionName].normalizedTime >= 1f)
			{
				findNearestHero();
				idle();
				playAnimation("idle");
			}
		}
		else if (state == "slap")
		{
			if (!attackChkOnce && base.animation["attack_slap_" + attackAnimation].normalizedTime >= attackCheckTime)
			{
				attackChkOnce = true;
				GameObject gameObject3;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
				{
					gameObject3 = ((!FengGameManagerMKII.LAN) ? PhotonNetwork.Instantiate("FX/boom1", checkHitCapsuleStart.position, Quaternion.Euler(270f, 0f, 0f), 0) : ((GameObject)Network.Instantiate(Resources.Load("FX/boom1"), checkHitCapsuleStart.position, Quaternion.Euler(270f, 0f, 0f), 0)));
					if ((bool)gameObject3.GetComponent<EnemyfxIDcontainer>())
					{
						gameObject3.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
					}
				}
				else
				{
					gameObject3 = (GameObject)Object.Instantiate(Resources.Load("FX/boom1"), checkHitCapsuleStart.position, Quaternion.Euler(270f, 0f, 0f));
				}
				gameObject3.transform.localScale = new Vector3(5f, 5f, 5f);
			}
			if (base.animation["attack_slap_" + attackAnimation].normalizedTime >= 1f)
			{
				findNearestHero();
				idle();
				playAnimation("idle");
			}
		}
		else if (state == "steam")
		{
			if (!attackChkOnce && base.animation[actionName].normalizedTime >= attackCheckTime)
			{
				attackChkOnce = true;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
				{
					if (FengGameManagerMKII.LAN)
					{
						Network.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + base.transform.up * 185f, Quaternion.Euler(270f, 0f, 0f), 0);
						Network.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + base.transform.up * 303f, Quaternion.Euler(270f, 0f, 0f), 0);
						Network.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + base.transform.up * 50f, Quaternion.Euler(270f, 0f, 0f), 0);
					}
					else
					{
						PhotonNetwork.Instantiate("FX/colossal_steam", base.transform.position + base.transform.up * 185f, Quaternion.Euler(270f, 0f, 0f), 0);
						PhotonNetwork.Instantiate("FX/colossal_steam", base.transform.position + base.transform.up * 303f, Quaternion.Euler(270f, 0f, 0f), 0);
						PhotonNetwork.Instantiate("FX/colossal_steam", base.transform.position + base.transform.up * 50f, Quaternion.Euler(270f, 0f, 0f), 0);
					}
				}
				else
				{
					Object.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + base.transform.forward * 185f, Quaternion.Euler(270f, 0f, 0f));
					Object.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + base.transform.forward * 303f, Quaternion.Euler(270f, 0f, 0f));
					Object.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + base.transform.forward * 50f, Quaternion.Euler(270f, 0f, 0f));
				}
			}
			if (!(base.animation[actionName].normalizedTime >= 1f))
			{
				return;
			}
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
			{
				if (FengGameManagerMKII.LAN)
				{
					Network.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + base.transform.up * 185f, Quaternion.Euler(270f, 0f, 0f), 0);
					Network.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + base.transform.up * 303f, Quaternion.Euler(270f, 0f, 0f), 0);
					Network.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + base.transform.up * 50f, Quaternion.Euler(270f, 0f, 0f), 0);
				}
				else
				{
					GameObject gameObject4 = PhotonNetwork.Instantiate("FX/colossal_steam_dmg", base.transform.position + base.transform.up * 185f, Quaternion.Euler(270f, 0f, 0f), 0);
					if ((bool)gameObject4.GetComponent<EnemyfxIDcontainer>())
					{
						gameObject4.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
					}
					gameObject4 = PhotonNetwork.Instantiate("FX/colossal_steam_dmg", base.transform.position + base.transform.up * 303f, Quaternion.Euler(270f, 0f, 0f), 0);
					if ((bool)gameObject4.GetComponent<EnemyfxIDcontainer>())
					{
						gameObject4.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
					}
					gameObject4 = PhotonNetwork.Instantiate("FX/colossal_steam_dmg", base.transform.position + base.transform.up * 50f, Quaternion.Euler(270f, 0f, 0f), 0);
					if ((bool)gameObject4.GetComponent<EnemyfxIDcontainer>())
					{
						gameObject4.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
					}
				}
			}
			else
			{
				Object.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + base.transform.forward * 185f, Quaternion.Euler(270f, 0f, 0f));
				Object.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + base.transform.forward * 303f, Quaternion.Euler(270f, 0f, 0f));
				Object.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + base.transform.forward * 50f, Quaternion.Euler(270f, 0f, 0f));
			}
			if (hasDie)
			{
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					Object.Destroy(base.gameObject);
				}
				else if (FengGameManagerMKII.LAN)
				{
					if (!base.networkView.isMine)
					{
					}
				}
				else if (PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.Destroy(base.photonView);
				}
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameWin();
			}
			findNearestHero();
			idle();
			playAnimation("idle");
		}
		else if (!(state == string.Empty))
		{
		}
	}

	[RPC]
	private void changeDoor()
	{
		door_broken.SetActiveRecursively(true);
		door_closed.SetActiveRecursively(false);
	}

	private void idle()
	{
		state = "idle";
		crossFade("idle", 0.2f);
	}

	private void callTitanHAHA()
	{
		attackCount++;
		int num = 4;
		int num2 = 7;
		if (IN_GAME_MAIN_CAMERA.difficulty != 0)
		{
			if (IN_GAME_MAIN_CAMERA.difficulty == 1)
			{
				num = 4;
				num2 = 6;
			}
			else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
			{
				num = 3;
				num2 = 5;
			}
		}
		if (attackCount % num == 0)
		{
			callTitan();
		}
		if ((double)NapeArmor < (double)NapeArmorTotal * 0.3)
		{
			if (attackCount % (int)((float)num2 * 0.5f) == 0)
			{
				callTitan(true);
			}
		}
		else if (attackCount % num2 == 0)
		{
			callTitan(true);
		}
	}

	private void attack_sweep(string type = "")
	{
		callTitanHAHA();
		state = "attack_sweep";
		attackAnimation = "sweep" + type;
		attackCheckTimeA = 0.4f;
		attackCheckTimeB = 0.57f;
		checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R");
		checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		checkHitCapsuleR = 20f;
		crossFade("attack_" + attackAnimation, 0.1f);
		attackChkOnce = false;
		sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = true;
		sweepSmokeObject.GetComponent<ParticleSystem>().Play();
		if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER)
		{
			return;
		}
		if (FengGameManagerMKII.LAN)
		{
			if (Network.peerType != NetworkPeerType.Server)
			{
			}
		}
		else if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("startSweepSmoke", PhotonTargets.Others);
		}
	}

	private void kick()
	{
		state = "kick";
		actionName = "attack_kick_wall";
		attackCheckTime = 0.64f;
		attackChkOnce = false;
		crossFade(actionName, 0.1f);
	}

	private void slap(string type)
	{
		callTitanHAHA();
		state = "slap";
		attackAnimation = type;
		if (type == "r1" || type == "r2")
		{
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		}
		if (type == "l1" || type == "l2")
		{
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
		}
		attackCheckTime = 0.57f;
		attackChkOnce = false;
		crossFade("attack_slap_" + attackAnimation, 0.1f);
	}

	private void steam()
	{
		callTitanHAHA();
		state = "steam";
		actionName = "attack_steam";
		attackCheckTime = 0.45f;
		crossFade(actionName, 0.1f);
		attackChkOnce = false;
	}

	private void killPlayer(GameObject hitHero)
	{
		if (!(hitHero != null))
		{
			return;
		}
		Vector3 position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			if (!hitHero.GetComponent<HERO>().HasDied())
			{
				hitHero.GetComponent<HERO>().die((hitHero.transform.position - position) * 15f * 4f, false);
			}
		}
		else
		{
			if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER)
			{
				return;
			}
			if (FengGameManagerMKII.LAN)
			{
				if (!hitHero.GetComponent<HERO>().HasDied())
				{
					hitHero.GetComponent<HERO>().markDie();
				}
			}
			else if (!hitHero.GetComponent<HERO>().HasDied())
			{
				hitHero.GetComponent<HERO>().markDie();
				hitHero.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (hitHero.transform.position - position) * 15f * 4f, false, -1, "Colossal Titan", true);
			}
		}
	}

	private void playSound(string sndname)
	{
		playsoundRPC(sndname);
		if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER)
		{
			return;
		}
		if (FengGameManagerMKII.LAN)
		{
			if (Network.peerType != NetworkPeerType.Server)
			{
			}
		}
		else if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("playsoundRPC", PhotonTargets.Others, sndname);
		}
	}

	[RPC]
	private void playsoundRPC(string sndname)
	{
		Transform transform = base.transform.Find(sndname);
		transform.GetComponent<AudioSource>().Play();
	}

	[RPC]
	private void startNeckSteam()
	{
		neckSteamObject.GetComponent<ParticleSystem>().Stop();
		neckSteamObject.GetComponent<ParticleSystem>().Play();
	}

	[RPC]
	private void startSweepSmoke()
	{
		sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = true;
		sweepSmokeObject.GetComponent<ParticleSystem>().Play();
	}

	[RPC]
	private void stopSweepSmoke()
	{
		sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = false;
		sweepSmokeObject.GetComponent<ParticleSystem>().Stop();
	}

	public void beTauntedBy(GameObject target, float tauntTime)
	{
	}

	private void neckSteam()
	{
		neckSteamObject.GetComponent<ParticleSystem>().Stop();
		neckSteamObject.GetComponent<ParticleSystem>().Play();
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
		{
			if (FengGameManagerMKII.LAN)
			{
				if (Network.peerType != NetworkPeerType.Server)
				{
				}
			}
			else if (PhotonNetwork.isMasterClient)
			{
				base.photonView.RPC("startNeckSteam", PhotonTargets.Others);
			}
		}
		isSteamNeed = true;
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
		float radius = 30f;
		Collider[] array = Physics.OverlapSphere(transform.transform.position - base.transform.forward * 10f, radius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.transform.root.tag == "Player")
			{
				GameObject gameObject = collider.transform.root.gameObject;
				if (!gameObject.GetComponent<TITAN_EREN>() && (bool)gameObject.GetComponent<HERO>())
				{
					blowPlayer(gameObject, transform);
				}
			}
		}
	}

	public void blowPlayer(GameObject player, Transform neck)
	{
		Vector3 vector = -(neck.position + base.transform.forward * 50f - player.transform.position);
		float num = 20f;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			player.GetComponent<HERO>().blowAway(vector.normalized * num + Vector3.up * 1f);
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			player.GetComponent<HERO>().photonView.RPC("blowAway", PhotonTargets.All, vector.normalized * num + Vector3.up * 1f);
		}
	}

	[RPC]
	public void titanGetHit(int viewID, int speed)
	{
		if (FengGameManagerMKII.LAN)
		{
		}
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
		PhotonView photonView = PhotonView.Find(viewID);
		if (photonView == null)
		{
			return;
		}
		float magnitude = (photonView.gameObject.transform.position - transform.transform.position).magnitude;
		if (!(magnitude < 40f))
		{
			return;
		}
		NapeArmor -= speed;
		neckSteam();
		if (NapeArmor <= 0)
		{
			NapeArmor = 0;
			if (!hasDie)
			{
				if (FengGameManagerMKII.LAN)
				{
					netDie();
					return;
				}
				base.photonView.RPC("netDie", PhotonTargets.OthersBuffered);
				netDie();
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().titanGetKill(photonView.owner, speed, base.name);
			}
		}
		else
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(false, (string)photonView.owner.customProperties[PhotonPlayerProperty.name], true, "Colossal Titan's neck", speed);
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("netShowDamage", photonView.owner, speed);
		}
	}

	[RPC]
	private void removeMe()
	{
		Object.Destroy(base.gameObject);
	}

	[RPC]
	public void netDie()
	{
		if (!hasDie)
		{
			hasDie = true;
		}
	}

	private void callTitan(bool special = false)
	{
		if (!special && GameObject.FindGameObjectsWithTag("titan").Length > 6)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("titanRespawn");
		ArrayList arrayList = new ArrayList();
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.transform.parent.name == "titanRespawnCT")
			{
				arrayList.Add(gameObject);
			}
		}
		GameObject gameObject2 = (GameObject)arrayList[Random.Range(0, arrayList.Count)];
		string[] array3 = new string[1] { "TITAN_VER3.1" };
		GameObject gameObject3 = ((!FengGameManagerMKII.LAN) ? PhotonNetwork.Instantiate(array3[Random.Range(0, array3.Length)], gameObject2.transform.position, gameObject2.transform.rotation, 0) : ((GameObject)Network.Instantiate(Resources.Load(array3[Random.Range(0, array3.Length)]), gameObject2.transform.position, gameObject2.transform.rotation, 0)));
		if (special)
		{
			GameObject[] array4 = GameObject.FindGameObjectsWithTag("route");
			GameObject gameObject4 = array4[Random.Range(0, array4.Length)];
			while (gameObject4.name != "routeCT")
			{
				gameObject4 = array4[Random.Range(0, array4.Length)];
			}
			gameObject3.GetComponent<TITAN>().setRoute(gameObject4);
			gameObject3.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_I);
			gameObject3.GetComponent<TITAN>().activeRad = 0;
			gameObject3.GetComponent<TITAN>().toCheckPoint((Vector3)gameObject3.GetComponent<TITAN>().checkPoints[0], 10f);
		}
		else
		{
			float num = 0.7f;
			float num2 = 0.7f;
			if (IN_GAME_MAIN_CAMERA.difficulty != 0)
			{
				if (IN_GAME_MAIN_CAMERA.difficulty == 1)
				{
					num = 0.4f;
					num2 = 0.7f;
				}
				else if (IN_GAME_MAIN_CAMERA.difficulty == 2)
				{
					num = -1f;
					num2 = 0.7f;
				}
			}
			if (GameObject.FindGameObjectsWithTag("titan").Length == 5)
			{
				gameObject3.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
			}
			else if (!(Random.Range(0f, 1f) < num))
			{
				if (Random.Range(0f, 1f) < num2)
				{
					gameObject3.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_JUMPER);
				}
				else
				{
					gameObject3.GetComponent<TITAN>().setAbnormalType(AbnormalType.TYPE_CRAWLER);
				}
			}
			gameObject3.GetComponent<TITAN>().activeRad = 200;
		}
		if (FengGameManagerMKII.LAN)
		{
			GameObject gameObject5 = (GameObject)Network.Instantiate(Resources.Load("FX/FXtitanSpawn"), gameObject3.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0);
			gameObject5.transform.localScale = gameObject3.transform.localScale;
		}
		else
		{
			GameObject gameObject6 = PhotonNetwork.Instantiate("FX/FXtitanSpawn", gameObject3.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0);
			gameObject6.transform.localScale = gameObject3.transform.localScale;
		}
	}
}
