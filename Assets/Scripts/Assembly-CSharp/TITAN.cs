using System.Collections;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class TITAN : Photon.MonoBehaviour
{
	public bool hasDie;

	public GameObject myHero;

	private TitanState state;

	public float speed = 7f;

	private float gravity = 120f;

	public float maxVelocityChange = 10f;

	public GameObject currentCamera;

	public float chaseDistance = 80f;

	public float attackDistance = 13f;

	public float attackWait = 1f;

	public float myDistance;

	public static float minusDistance = 99999f;

	public static GameObject minusDistanceEnemy;

	public float myLevel = 1f;

	public bool isAlarm;

	public AbnormalType abnormalType;

	private Vector3 oldCorePosition;

	private int attackCount;

	private float attackEndWait;

	private Vector3 abnorma_jump_bite_horizon_v;

	private float hitPause;

	public int activeRad = int.MaxValue;

	private Vector3 spawnPt;

	public ArrayList checkPoints = new ArrayList();

	private Vector3 targetCheckPt;

	private float targetR;

	private float angle;

	private float between2;

	private GameObject throwRock;

	public GROUP myGroup = GROUP.T;

	public PVPcheckPoint PVPfromCheckPt;

	public int myDifficulty;

	public TITAN_CONTROLLER controller;

	public bool nonAI;

	private float stamina = 320f;

	private float maxStamina = 320f;

	private string runAnimation;

	private Vector3 headscale = Vector3.one;

	public GameObject mainMaterial;

	private float tauntTime;

	private GameObject whoHasTauntMe;

	private bool hasDieSteam;

	private float rockInterval;

	private float random_run_time;

	private Transform head;

	private Transform neck;

	private float stuckTime;

	private bool stuck;

	private float stuckTurnAngle;

	private bool needFreshCorePosition;

	private int stepSoundPhase = 2;

	public bool asClientLookTarget;

	private Quaternion oldHeadRotation;

	private Quaternion targetHeadRotation;

	private bool grounded;

	private bool attacked;

	private string attackAnimation;

	private string hitAnimation;

	private string nextAttackAnimation;

	private bool isAttackMoveByCore;

	private string fxName;

	private Vector3 fxPosition;

	private Quaternion fxRotation;

	private float attackCheckTime;

	private float attackCheckTimeA;

	private float attackCheckTimeB;

	private Transform currentGrabHand;

	private bool isGrabHandLeft;

	private GameObject grabbedTarget;

	private bool nonAIcombo;

	private bool leftHandAttack;

	private float sbtime;

	private float turnDeg;

	private float desDeg;

	private string turnAnimation;

	public GameObject grabTF;

	private float getdownTime;

	private float dieTime;

	private void Awake()
	{
		base.rigidbody.freezeRotation = true;
		base.rigidbody.useGravity = false;
	}

	private void playAnimation(string aniName)
	{
		base.animation.Play(aniName);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			base.photonView.RPC("netPlayAnimation", PhotonTargets.Others, aniName);
		}
	}

	private void playAnimationAt(string aniName, float normalizedTime)
	{
		base.animation.Play(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			base.photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, aniName, normalizedTime);
		}
	}

	private void crossFade(string aniName, float time)
	{
		base.animation.CrossFade(aniName, time);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
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

	[RPC]
	private void netSetLevel(float level, int AI, int skinColor)
	{
		setLevel(level, AI, skinColor);
	}

	private void setLevel(float level, int AI, int skinColor)
	{
		myLevel = level;
		myLevel = Mathf.Clamp(myLevel, 0.7f, 3f);
		attackWait += Random.Range(0f, 2f);
		chaseDistance += myLevel * 10f;
		base.transform.localScale = new Vector3(myLevel, myLevel, myLevel);
		float a = Mathf.Pow(2f / myLevel, 0.35f);
		a = Mathf.Min(a, 1.25f);
		headscale = new Vector3(a, a, a);
		head = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
		head.localScale = headscale;
		if (skinColor != 0)
		{
			Material material = mainMaterial.GetComponent<SkinnedMeshRenderer>().material;
			Color color;
			switch (skinColor)
			{
			case 1:
				color = FengColor.titanSkin1;
				break;
			case 2:
				color = FengColor.titanSkin2;
				break;
			default:
				color = FengColor.titanSkin3;
				break;
			}
			material.color = color;
		}
		float value = 1.4f - (myLevel - 0.7f) * 0.15f;
		value = Mathf.Clamp(value, 0.9f, 1.5f);
		foreach (AnimationState item in base.animation)
		{
			item.speed = value;
		}
		base.rigidbody.mass *= myLevel;
		base.rigidbody.rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
		if (myLevel > 1f)
		{
			speed *= Mathf.Sqrt(myLevel);
		}
		myDifficulty = AI;
		if (myDifficulty == 1 || myDifficulty == 2)
		{
			foreach (AnimationState item2 in base.animation)
			{
				item2.speed = value * 1.05f;
			}
			if (nonAI)
			{
				speed *= 1.1f;
			}
			else
			{
				speed *= 1.4f;
			}
			chaseDistance *= 1.15f;
		}
		if (myDifficulty == 2)
		{
			foreach (AnimationState item3 in base.animation)
			{
				item3.speed = value * 1.05f;
			}
			if (nonAI)
			{
				speed *= 1.1f;
			}
			else
			{
				speed *= 1.5f;
			}
			chaseDistance *= 1.3f;
		}
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.ENDLESS_TITAN || IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
		{
			chaseDistance = 999999f;
		}
		if (nonAI)
		{
			if (abnormalType == AbnormalType.TYPE_CRAWLER)
			{
				speed = Mathf.Min(70f, speed);
			}
			else
			{
				speed = Mathf.Min(60f, speed);
			}
		}
		attackDistance = Vector3.Distance(base.transform.position, base.transform.Find("ap_front_ground").position) * 1.65f;
	}

	public void setAbnormalType(AbnormalType type, bool forceCrawler = false)
	{
		int num = 0;
		float num2 = 0.02f * (float)(IN_GAME_MAIN_CAMERA.difficulty + 1);
		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_AHSS)
		{
			num2 = 100f;
		}
		switch (type)
		{
		case AbnormalType.NORMAL:
			num = ((Random.Range(0f, 1f) < num2) ? 4 : 0);
			break;
		case AbnormalType.TYPE_I:
			num = ((!(Random.Range(0f, 1f) < num2)) ? 1 : 4);
			break;
		case AbnormalType.TYPE_JUMPER:
			num = ((!(Random.Range(0f, 1f) < num2)) ? 2 : 4);
			break;
		case AbnormalType.TYPE_CRAWLER:
			num = 3;
			if (GameObject.Find("Crawler") != null && Random.Range(0, 1000) > 5)
			{
				num = 2;
			}
			break;
		case AbnormalType.TYPE_PUNK:
			num = 4;
			break;
		}
		if (forceCrawler)
		{
			num = 3;
		}
		if (num == 4)
		{
			if (!LevelInfo.getInfo(FengGameManagerMKII.level).punk)
			{
				num = 1;
			}
			else
			{
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE && getPunkNumber() >= 3)
				{
					num = 1;
				}
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE)
				{
					int wave = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().wave;
					if (wave != 5 && wave != 10 && wave != 15 && wave != 20)
					{
						num = 1;
					}
				}
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			base.photonView.RPC("netSetAbnormalType", PhotonTargets.AllBuffered, num);
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			netSetAbnormalType(num);
		}
	}

	private int getPunkNumber()
	{
		int num = 0;
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if ((bool)gameObject.GetComponent<TITAN>() && gameObject.GetComponent<TITAN>().name == "Punk")
			{
				num++;
			}
		}
		return num;
	}

	[RPC]
	private void netSetAbnormalType(int type)
	{
		switch (type)
		{
		case 0:
			abnormalType = AbnormalType.NORMAL;
			base.name = "Titan";
			runAnimation = "run_walk";
			GetComponent<TITAN_SETUP>().setHair();
			break;
		case 1:
			abnormalType = AbnormalType.TYPE_I;
			base.name = "Aberrant";
			runAnimation = "run_abnormal";
			GetComponent<TITAN_SETUP>().setHair();
			break;
		case 2:
			abnormalType = AbnormalType.TYPE_JUMPER;
			base.name = "Jumper";
			runAnimation = "run_abnormal";
			GetComponent<TITAN_SETUP>().setHair();
			break;
		case 3:
			abnormalType = AbnormalType.TYPE_CRAWLER;
			base.name = "Crawler";
			runAnimation = "crawler_run";
			GetComponent<TITAN_SETUP>().setHair();
			break;
		case 4:
			abnormalType = AbnormalType.TYPE_PUNK;
			base.name = "Punk";
			runAnimation = "run_abnormal_1";
			GetComponent<TITAN_SETUP>().setPunkHair();
			break;
		}
		if (abnormalType == AbnormalType.TYPE_I || abnormalType == AbnormalType.TYPE_JUMPER || abnormalType == AbnormalType.TYPE_PUNK)
		{
			speed = 18f;
			if (myLevel > 1f)
			{
				speed *= Mathf.Sqrt(myLevel);
			}
			if (myDifficulty == 1)
			{
				speed *= 1.4f;
			}
			if (myDifficulty == 2)
			{
				speed *= 1.6f;
			}
			base.animation["turnaround1"].speed = 2f;
			base.animation["turnaround2"].speed = 2f;
		}
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			chaseDistance += 50f;
			speed = 25f;
			if (myLevel > 1f)
			{
				speed *= Mathf.Sqrt(myLevel);
			}
			if (myDifficulty == 1)
			{
				speed *= 2f;
			}
			if (myDifficulty == 2)
			{
				speed *= 2.2f;
			}
			base.transform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().height = 10f;
			base.transform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().radius = 5f;
			base.transform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0f, 5.05f, 0f);
		}
		if (nonAI)
		{
			if (abnormalType == AbnormalType.TYPE_CRAWLER)
			{
				speed = Mathf.Min(70f, speed);
			}
			else
			{
				speed = Mathf.Min(60f, speed);
			}
			base.animation["attack_jumper_0"].speed = 7f;
			base.animation["attack_crawler_jump_0"].speed = 4f;
		}
		base.animation["attack_combo_1"].speed = 1f;
		base.animation["attack_combo_2"].speed = 1f;
		base.animation["attack_combo_3"].speed = 1f;
		base.animation["attack_quick_turn_l"].speed = 1f;
		base.animation["attack_quick_turn_r"].speed = 1f;
		base.animation["attack_anti_AE_l"].speed = 1.1f;
		base.animation["attack_anti_AE_low_l"].speed = 1.1f;
		base.animation["attack_anti_AE_r"].speed = 1.1f;
		base.animation["attack_anti_AE_low_r"].speed = 1.1f;
		idle();
	}

	private void setmyLevel()
	{
		base.animation.cullingType = AnimationCullingType.BasedOnRenderers;
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			base.photonView.RPC("netSetLevel", PhotonTargets.AllBuffered, myLevel, GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().difficulty, Random.Range(0, 4));
			base.animation.cullingType = AnimationCullingType.AlwaysAnimate;
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			setLevel(myLevel, IN_GAME_MAIN_CAMERA.difficulty, Random.Range(0, 4));
		}
	}

	private void OnDestroy()
	{
		if (GameObject.Find("MultiplayerManager") != null)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().removeTitan(this);
		}
	}

	private void Start()
	{
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().addTitan(this);
		currentCamera = GameObject.Find("MainCamera");
		runAnimation = "run_walk";
		grabTF = new GameObject();
		grabTF.name = "titansTmpGrabTF";
		head = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
		neck = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
		oldHeadRotation = head.rotation;
		if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || base.photonView.isMine)
		{
			myLevel = Random.Range(0.5f, 3.5f);
			spawnPt = base.transform.position;
			setmyLevel();
			setAbnormalType(abnormalType);
			if (myHero == null)
			{
				findNearestHero();
			}
			controller = base.gameObject.GetComponent<TITAN_CONTROLLER>();
		}
	}

	private void findNearestHero()
	{
		GameObject gameObject = myHero;
		myHero = getNearestHero();
		if (myHero != gameObject && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			if (myHero == null)
			{
				base.photonView.RPC("setMyTarget", PhotonTargets.Others, -1);
			}
			else
			{
				base.photonView.RPC("setMyTarget", PhotonTargets.Others, myHero.GetPhotonView().viewID);
			}
		}
		oldHeadRotation = head.rotation;
	}

	[RPC]
	private void setMyTarget(int ID)
	{
		if (ID == -1)
		{
			myHero = null;
		}
		PhotonView photonView = PhotonView.Find(ID);
		if (photonView != null)
		{
			myHero = photonView.gameObject;
		}
	}

	private void findNearestFacingHero()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject gameObject = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		float num2 = 0f;
		float num3 = ((abnormalType != 0) ? 180f : 100f);
		float num4 = 0f;
		GameObject[] array2 = array;
		foreach (GameObject gameObject2 in array2)
		{
			float sqrMagnitude = (gameObject2.transform.position - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				Vector3 vector = gameObject2.transform.position - base.transform.position;
				num2 = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
				num4 = 0f - Mathf.DeltaAngle(num2, base.gameObject.transform.rotation.eulerAngles.y - 90f);
				if (Mathf.Abs(num4) < num3)
				{
					gameObject = gameObject2;
					num = sqrMagnitude;
				}
			}
		}
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject3 = myHero;
		myHero = gameObject;
		if (gameObject3 != myHero && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
		{
			if (myHero == null)
			{
				base.photonView.RPC("setMyTarget", PhotonTargets.Others, -1);
			}
			else
			{
				base.photonView.RPC("setMyTarget", PhotonTargets.Others, myHero.GetPhotonView().viewID);
			}
		}
		tauntTime = 5f;
	}

	private GameObject getNearestHero()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float sqrMagnitude = (gameObject.transform.position - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				result = gameObject;
				num = sqrMagnitude;
			}
		}
		return result;
	}

	public bool IsGrounded()
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyAABB");
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		return Physics.Raycast(base.gameObject.transform.position + Vector3.up * 0.1f, -Vector3.up, 0.3f, layerMask3.value);
	}

	private GameObject checkIfHitHand(Transform hand)
	{
		float num = 2.4f * myLevel;
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
			}
			else if ((bool)gameObject.GetComponent<HERO>() && !gameObject.GetComponent<HERO>().isInvincible())
			{
				return gameObject;
			}
		}
		return null;
	}

	private GameObject checkIfHitHead(Transform head, float rad)
	{
		float num = rad * myLevel;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (!gameObject.GetComponent<TITAN_EREN>() && (!gameObject.GetComponent<HERO>() || !gameObject.GetComponent<HERO>().isInvincible()))
			{
				float num2 = gameObject.GetComponent<CapsuleCollider>().height * 0.5f;
				if (Vector3.Distance(gameObject.transform.position + Vector3.up * num2, head.position + Vector3.up * 1.5f * myLevel) < num + num2)
				{
					return gameObject;
				}
			}
		}
		return null;
	}

	private GameObject checkIfHitCrawlerMouth(Transform head, float rad)
	{
		float num = rad * myLevel;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (!gameObject.GetComponent<TITAN_EREN>() && (!gameObject.GetComponent<HERO>() || !gameObject.GetComponent<HERO>().isInvincible()))
			{
				float num2 = gameObject.GetComponent<CapsuleCollider>().height * 0.5f;
				if (Vector3.Distance(gameObject.transform.position + Vector3.up * num2, head.position - Vector3.up * 1.5f * myLevel) < num + num2)
				{
					return gameObject;
				}
			}
		}
		return null;
	}

	public void beTauntedBy(GameObject target, float tauntTime)
	{
		whoHasTauntMe = target;
		this.tauntTime = tauntTime;
		isAlarm = true;
	}

	public void beLaughAttacked()
	{
		if (!hasDie && abnormalType != AbnormalType.TYPE_CRAWLER)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
			{
				base.photonView.RPC("laugh", PhotonTargets.All, 0f);
			}
			else if (state == TitanState.idle || state == TitanState.turn || state == TitanState.chase)
			{
				laugh();
			}
		}
	}

	[RPC]
	private void laugh(float sbtime = 0f)
	{
		if (state == TitanState.idle || state == TitanState.turn || state == TitanState.chase)
		{
			this.sbtime = sbtime;
			state = TitanState.laugh;
			crossFade("laugh", 0.2f);
		}
	}

	private string[] GetAttackStrategy()
	{
		string[] array = null;
		int num = 0;
		if (isAlarm || !(myHero.transform.position.y + 3f > neck.position.y + 10f * myLevel))
		{
			if (myHero.transform.position.y > neck.position.y - 3f * myLevel)
			{
				if (myDistance < attackDistance * 0.5f)
				{
					if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkOverHead").position) < 3.6f * myLevel)
					{
						array = ((!(between2 > 0f)) ? new string[1] { "grab_head_front_l" } : new string[1] { "grab_head_front_r" });
					}
					else if (Mathf.Abs(between2) < 90f)
					{
						if (Mathf.Abs(between2) < 30f)
						{
							if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkFront").position) < 2.5f * myLevel)
							{
								array = new string[3] { "attack_bite", "attack_bite", "attack_slap_face" };
							}
						}
						else if (between2 > 0f)
						{
							if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkFrontRight").position) < 2.5f * myLevel)
							{
								array = new string[1] { "attack_bite_r" };
							}
						}
						else if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkFrontLeft").position) < 2.5f * myLevel)
						{
							array = new string[1] { "attack_bite_l" };
						}
					}
					else if (between2 > 0f)
					{
						if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkBackRight").position) < 2.8f * myLevel)
						{
							array = new string[3] { "grab_head_back_r", "grab_head_back_r", "attack_slap_back" };
						}
					}
					else if (Vector3.Distance(myHero.transform.position, base.transform.Find("chkBackLeft").position) < 2.8f * myLevel)
					{
						array = new string[3] { "grab_head_back_l", "grab_head_back_l", "attack_slap_back" };
					}
				}
				if (array == null)
				{
					if (abnormalType == AbnormalType.NORMAL || abnormalType == AbnormalType.TYPE_PUNK)
					{
						if ((myDifficulty > 0 || Random.Range(0, 1000) < 3) && Mathf.Abs(between2) < 60f)
						{
							array = new string[1] { "attack_combo" };
						}
					}
					else if ((abnormalType == AbnormalType.TYPE_I || abnormalType == AbnormalType.TYPE_JUMPER) && (myDifficulty > 0 || Random.Range(0, 100) < 50))
					{
						array = new string[1] { "attack_abnormal_jump" };
					}
				}
			}
			else
			{
				switch ((Mathf.Abs(between2) < 90f) ? ((between2 > 0f) ? 1 : 2) : ((!(between2 > 0f)) ? 3 : 4))
				{
				case 2:
					array = ((!(myDistance < attackDistance * 0.25f)) ? ((!(myDistance < attackDistance * 0.5f)) ? ((abnormalType != AbnormalType.TYPE_PUNK) ? ((abnormalType != 0) ? new string[1] { "attack_abnormal_jump" } : ((myDifficulty <= 0) ? new string[5] { "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_combo" } : new string[3] { "attack_front_ground", "attack_combo", "attack_combo" })) : new string[3] { "attack_combo", "attack_combo", "attack_abnormal_jump" }) : ((abnormalType != AbnormalType.TYPE_PUNK) ? ((abnormalType != 0) ? new string[3] { "grab_ground_front_l", "grab_ground_front_l", "attack_abnormal_jump" } : new string[3] { "grab_ground_front_l", "grab_ground_front_l", "attack_stomp" }) : new string[3] { "grab_ground_front_l", "grab_ground_front_l", "attack_abnormal_jump" })) : ((abnormalType != AbnormalType.TYPE_PUNK) ? ((abnormalType != 0) ? new string[1] { "attack_kick" } : new string[2] { "attack_front_ground", "attack_stomp" }) : new string[2] { "attack_kick", "attack_stomp" }));
					break;
				case 1:
					array = ((!(myDistance < attackDistance * 0.25f)) ? ((!(myDistance < attackDistance * 0.5f)) ? ((abnormalType != AbnormalType.TYPE_PUNK) ? ((abnormalType != 0) ? new string[1] { "attack_abnormal_jump" } : ((myDifficulty <= 0) ? new string[5] { "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_front_ground", "attack_combo" } : new string[3] { "attack_front_ground", "attack_combo", "attack_combo" })) : new string[3] { "attack_combo", "attack_combo", "attack_abnormal_jump" }) : ((abnormalType != AbnormalType.TYPE_PUNK) ? ((abnormalType != 0) ? new string[3] { "grab_ground_front_r", "grab_ground_front_r", "attack_abnormal_jump" } : new string[3] { "grab_ground_front_r", "grab_ground_front_r", "attack_stomp" }) : new string[3] { "grab_ground_front_r", "grab_ground_front_r", "attack_abnormal_jump" })) : ((abnormalType != AbnormalType.TYPE_PUNK) ? ((abnormalType != 0) ? new string[1] { "attack_kick" } : new string[2] { "attack_front_ground", "attack_stomp" }) : new string[2] { "attack_kick", "attack_stomp" }));
					break;
				case 3:
					if (myDistance < attackDistance * 0.5f)
					{
						array = ((abnormalType != 0) ? new string[1] { "grab_ground_back_l" } : new string[1] { "grab_ground_back_l" });
					}
					break;
				case 4:
					if (myDistance < attackDistance * 0.5f)
					{
						array = ((abnormalType != 0) ? new string[1] { "grab_ground_back_r" } : new string[1] { "grab_ground_back_r" });
					}
					break;
				}
			}
		}
		return array;
	}

	public void update()
	{
		if ((IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || myDifficulty < 0 || (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine))
		{
			return;
		}
		if (!nonAI)
		{
			if (activeRad < int.MaxValue && (state == TitanState.idle || state == TitanState.wander || state == TitanState.chase))
			{
				if (checkPoints.Count > 1)
				{
					if (Vector3.Distance((Vector3)checkPoints[0], base.transform.position) > (float)activeRad)
					{
						toCheckPoint((Vector3)checkPoints[0], 10f);
					}
				}
				else if (Vector3.Distance(spawnPt, base.transform.position) > (float)activeRad)
				{
					toCheckPoint(spawnPt, 10f);
				}
			}
			if (whoHasTauntMe != null)
			{
				tauntTime -= Time.deltaTime;
				if (tauntTime <= 0f)
				{
					whoHasTauntMe = null;
				}
				myHero = whoHasTauntMe;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
				{
					base.photonView.RPC("setMyTarget", PhotonTargets.Others, myHero.GetPhotonView().viewID);
				}
			}
		}
		if (hasDie)
		{
			dieTime += Time.deltaTime;
			if (dieTime > 2f && !hasDieSteam)
			{
				hasDieSteam = true;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("FX/FXtitanDie1"));
					gameObject.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
					gameObject.transform.localScale = base.transform.localScale;
				}
				else if (base.photonView.isMine)
				{
					GameObject gameObject2 = PhotonNetwork.Instantiate("FX/FXtitanDie1", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0);
					gameObject2.transform.localScale = base.transform.localScale;
				}
			}
			if (dieTime > 5f)
			{
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					GameObject gameObject3 = (GameObject)Object.Instantiate(Resources.Load("FX/FXtitanDie"));
					gameObject3.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
					gameObject3.transform.localScale = base.transform.localScale;
					Object.Destroy(base.gameObject);
				}
				else if (base.photonView.isMine)
				{
					GameObject gameObject4 = PhotonNetwork.Instantiate("FX/FXtitanDie", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0);
					gameObject4.transform.localScale = base.transform.localScale;
					PhotonNetwork.Destroy(base.gameObject);
					myDifficulty = -1;
				}
			}
			return;
		}
		if (state == TitanState.hit)
		{
			if (hitPause > 0f)
			{
				hitPause -= Time.deltaTime;
				if (hitPause <= 0f)
				{
					base.animation[hitAnimation].speed = 1f;
					hitPause = 0f;
				}
			}
			if (base.animation[hitAnimation].normalizedTime >= 1f)
			{
				idle();
			}
		}
		if (!nonAI)
		{
			if (myHero == null)
			{
				findNearestHero();
			}
			if ((state == TitanState.idle || state == TitanState.chase || state == TitanState.wander) && whoHasTauntMe == null && Random.Range(0, 100) < 10)
			{
				findNearestFacingHero();
			}
			if (myHero == null)
			{
				myDistance = float.MaxValue;
			}
			else
			{
				myDistance = Mathf.Sqrt((myHero.transform.position.x - base.transform.position.x) * (myHero.transform.position.x - base.transform.position.x) + (myHero.transform.position.z - base.transform.position.z) * (myHero.transform.position.z - base.transform.position.z));
			}
		}
		else
		{
			if (stamina < maxStamina)
			{
				if (base.animation.IsPlaying("idle"))
				{
					stamina += Time.deltaTime * 30f;
				}
				if (base.animation.IsPlaying("crawler_idle"))
				{
					stamina += Time.deltaTime * 35f;
				}
				if (base.animation.IsPlaying("run_walk"))
				{
					stamina += Time.deltaTime * 10f;
				}
			}
			if (base.animation.IsPlaying("run_abnormal_1"))
			{
				stamina -= Time.deltaTime * 5f;
			}
			if (base.animation.IsPlaying("crawler_run"))
			{
				stamina -= Time.deltaTime * 15f;
			}
			if (stamina < 0f)
			{
				stamina = 0f;
			}
			if (!IN_GAME_MAIN_CAMERA.isPausing)
			{
				GameObject.Find("stamina_titan").transform.localScale = new Vector3(stamina, 16f);
			}
		}
		if (state == TitanState.laugh)
		{
			if (base.animation["laugh"].normalizedTime >= 1f)
			{
				idle(2f);
			}
		}
		else if (state == TitanState.idle)
		{
			if (nonAI)
			{
				if (IN_GAME_MAIN_CAMERA.isPausing)
				{
					return;
				}
				if (abnormalType != AbnormalType.TYPE_CRAWLER)
				{
					if (controller.isAttackDown && stamina > 25f)
					{
						stamina -= 25f;
						attack("combo_1");
					}
					else if (controller.isAttackIIDown && stamina > 50f)
					{
						stamina -= 50f;
						attack("abnormal_jump");
					}
					else if (controller.isJumpDown && stamina > 15f)
					{
						stamina -= 15f;
						attack("jumper_0");
					}
				}
				else if (controller.isAttackDown && stamina > 40f)
				{
					stamina -= 40f;
					attack("crawler_jump_0");
				}
				if (controller.isSuicide)
				{
					suicide();
				}
				return;
			}
			if (sbtime > 0f)
			{
				sbtime -= Time.deltaTime;
				return;
			}
			if (!isAlarm)
			{
				if (abnormalType != AbnormalType.TYPE_PUNK && abnormalType != AbnormalType.TYPE_CRAWLER && Random.Range(0f, 1f) < 0.005f)
				{
					sitdown();
					return;
				}
				if (Random.Range(0f, 1f) < 0.02f)
				{
					wander();
					return;
				}
				if (Random.Range(0f, 1f) < 0.01f)
				{
					turn(Random.Range(30, 120));
					return;
				}
				if (Random.Range(0f, 1f) < 0.01f)
				{
					turn(Random.Range(-30, -120));
					return;
				}
			}
			angle = 0f;
			between2 = 0f;
			if (myDistance < chaseDistance || whoHasTauntMe != null)
			{
				Vector3 vector = myHero.transform.position - base.transform.position;
				angle = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
				between2 = 0f - Mathf.DeltaAngle(angle, base.gameObject.transform.rotation.eulerAngles.y - 90f);
				if (!(myDistance < attackDistance))
				{
					if (isAlarm || Mathf.Abs(between2) < 90f)
					{
						chase();
						return;
					}
					if (!isAlarm && myDistance < chaseDistance * 0.1f)
					{
						chase();
						return;
					}
				}
			}
			if (longRangeAttackCheck())
			{
				return;
			}
			if (myDistance < chaseDistance)
			{
				if (abnormalType == AbnormalType.TYPE_JUMPER && (myDistance > attackDistance || myHero.transform.position.y > head.position.y + 4f * myLevel) && Mathf.Abs(between2) < 120f && Vector3.Distance(base.transform.position, myHero.transform.position) < 1.5f * myHero.transform.position.y)
				{
					attack("jumper_0");
					return;
				}
				if (abnormalType == AbnormalType.TYPE_CRAWLER && myDistance < attackDistance * 3f && Mathf.Abs(between2) < 90f && myHero.transform.position.y < neck.position.y + 30f * myLevel && myHero.transform.position.y > neck.position.y + 10f * myLevel)
				{
					attack("crawler_jump_0");
					return;
				}
			}
			if (abnormalType == AbnormalType.TYPE_PUNK && myDistance < 90f && Mathf.Abs(between2) > 90f)
			{
				if (Random.Range(0f, 1f) < 0.4f)
				{
					randomRun(base.transform.position + new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f)), 10f);
				}
				if (Random.Range(0f, 1f) < 0.2f)
				{
					recover();
				}
				else if (Random.Range(0, 2) == 0)
				{
					attack("quick_turn_l");
				}
				else
				{
					attack("quick_turn_r");
				}
				return;
			}
			if (myDistance < attackDistance)
			{
				if (abnormalType == AbnormalType.TYPE_CRAWLER)
				{
					if (!(myHero.transform.position.y + 3f > neck.position.y + 20f * myLevel) && Random.Range(0f, 1f) < 0.1f)
					{
						chase();
					}
					return;
				}
				string text = string.Empty;
				string[] attackStrategy = GetAttackStrategy();
				if (attackStrategy != null)
				{
					text = attackStrategy[Random.Range(0, attackStrategy.Length)];
				}
				if ((abnormalType == AbnormalType.TYPE_JUMPER || abnormalType == AbnormalType.TYPE_I) && Mathf.Abs(between2) > 40f)
				{
					if (text.Contains("grab") || text.Contains("kick") || text.Contains("slap") || text.Contains("bite"))
					{
						if (Random.Range(0, 100) < 30)
						{
							turn(between2);
							return;
						}
					}
					else if (Random.Range(0, 100) < 90)
					{
						turn(between2);
						return;
					}
				}
				if (executeAttack(text))
				{
					return;
				}
				if (abnormalType == AbnormalType.NORMAL)
				{
					if (Random.Range(0, 100) < 30 && Mathf.Abs(between2) > 45f)
					{
						turn(between2);
						return;
					}
				}
				else if (Mathf.Abs(between2) > 45f)
				{
					turn(between2);
					return;
				}
			}
			if (!(PVPfromCheckPt != null))
			{
				return;
			}
			if (PVPfromCheckPt.state == CheckPointState.Titan)
			{
				if (Random.Range(0, 100) > 48)
				{
					GameObject chkPtNext = PVPfromCheckPt.chkPtNext;
					if (chkPtNext != null && (chkPtNext.GetComponent<PVPcheckPoint>().state != CheckPointState.Titan || Random.Range(0, 100) < 20))
					{
						toPVPCheckPoint(chkPtNext.transform.position, 5 + Random.Range(0, 10));
						PVPfromCheckPt = chkPtNext.GetComponent<PVPcheckPoint>();
					}
				}
				else
				{
					GameObject chkPtNext = PVPfromCheckPt.chkPtPrevious;
					if (chkPtNext != null && (chkPtNext.GetComponent<PVPcheckPoint>().state != CheckPointState.Titan || Random.Range(0, 100) < 5))
					{
						toPVPCheckPoint(chkPtNext.transform.position, 5 + Random.Range(0, 10));
						PVPfromCheckPt = chkPtNext.GetComponent<PVPcheckPoint>();
					}
				}
			}
			else
			{
				toPVPCheckPoint(PVPfromCheckPt.transform.position, 5 + Random.Range(0, 10));
			}
		}
		else if (state == TitanState.attack)
		{
			if (attackAnimation == "combo")
			{
				if (nonAI)
				{
					if (controller.isAttackDown)
					{
						nonAIcombo = true;
					}
					if (!nonAIcombo && base.animation["attack_" + attackAnimation].normalizedTime >= 0.385f)
					{
						idle();
						return;
					}
				}
				if (base.animation["attack_" + attackAnimation].normalizedTime >= 0.11f && base.animation["attack_" + attackAnimation].normalizedTime <= 0.16f)
				{
					GameObject gameObject5 = checkIfHitHand(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001"));
					if (gameObject5 != null)
					{
						Vector3 position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
						{
							gameObject5.GetComponent<HERO>().die((gameObject5.transform.position - position) * 15f * myLevel, false);
						}
						else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject5.GetComponent<HERO>().HasDied())
						{
							gameObject5.GetComponent<HERO>().markDie();
							gameObject5.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (gameObject5.transform.position - position) * 15f * myLevel, false, (!nonAI) ? (-1) : base.photonView.viewID, base.name, true);
						}
					}
				}
				if (base.animation["attack_" + attackAnimation].normalizedTime >= 0.27f && base.animation["attack_" + attackAnimation].normalizedTime <= 0.32f)
				{
					GameObject gameObject6 = checkIfHitHand(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001"));
					if (gameObject6 != null)
					{
						Vector3 position2 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
						{
							gameObject6.GetComponent<HERO>().die((gameObject6.transform.position - position2) * 15f * myLevel, false);
						}
						else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject6.GetComponent<HERO>().HasDied())
						{
							gameObject6.GetComponent<HERO>().markDie();
							gameObject6.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (gameObject6.transform.position - position2) * 15f * myLevel, false, (!nonAI) ? (-1) : base.photonView.viewID, base.name, true);
						}
					}
				}
			}
			if (attackCheckTimeA != 0f && base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA && base.animation["attack_" + attackAnimation].normalizedTime <= attackCheckTimeB)
			{
				if (leftHandAttack)
				{
					GameObject gameObject7 = checkIfHitHand(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001"));
					if (gameObject7 != null)
					{
						Vector3 position3 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
						{
							gameObject7.GetComponent<HERO>().die((gameObject7.transform.position - position3) * 15f * myLevel, false);
						}
						else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject7.GetComponent<HERO>().HasDied())
						{
							gameObject7.GetComponent<HERO>().markDie();
							gameObject7.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (gameObject7.transform.position - position3) * 15f * myLevel, false, (!nonAI) ? (-1) : base.photonView.viewID, base.name, true);
						}
					}
				}
				else
				{
					GameObject gameObject8 = checkIfHitHand(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001"));
					if (gameObject8 != null)
					{
						Vector3 position4 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
						{
							gameObject8.GetComponent<HERO>().die((gameObject8.transform.position - position4) * 15f * myLevel, false);
						}
						else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject8.GetComponent<HERO>().HasDied())
						{
							gameObject8.GetComponent<HERO>().markDie();
							gameObject8.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (gameObject8.transform.position - position4) * 15f * myLevel, false, (!nonAI) ? (-1) : base.photonView.viewID, base.name, true);
						}
					}
				}
			}
			if (!attacked && attackCheckTime != 0f && base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTime)
			{
				attacked = true;
				fxPosition = base.transform.Find("ap_" + attackAnimation).position;
				GameObject gameObject9 = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine) ? ((GameObject)Object.Instantiate(Resources.Load("FX/" + fxName), fxPosition, fxRotation)) : PhotonNetwork.Instantiate("FX/" + fxName, fxPosition, fxRotation, 0));
				if (nonAI)
				{
					gameObject9.transform.localScale = base.transform.localScale * 1.5f;
					if ((bool)gameObject9.GetComponent<EnemyfxIDcontainer>())
					{
						gameObject9.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = base.photonView.viewID;
					}
				}
				else
				{
					gameObject9.transform.localScale = base.transform.localScale;
				}
				if ((bool)gameObject9.GetComponent<EnemyfxIDcontainer>())
				{
					gameObject9.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
				}
				float b = 1f - Vector3.Distance(currentCamera.transform.position, gameObject9.transform.position) * 0.05f;
				b = Mathf.Min(1f, b);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(b, b);
			}
			if (attackAnimation == "throw")
			{
				if (!attacked && base.animation["attack_" + attackAnimation].normalizedTime >= 0.11f)
				{
					attacked = true;
					Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
					if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
					{
						throwRock = PhotonNetwork.Instantiate("FX/rockThrow", transform.position, transform.rotation, 0);
					}
					else
					{
						throwRock = (GameObject)Object.Instantiate(Resources.Load("FX/rockThrow"), transform.position, transform.rotation);
					}
					throwRock.transform.localScale = base.transform.localScale;
					throwRock.transform.position -= throwRock.transform.forward * 2.5f * myLevel;
					if ((bool)throwRock.GetComponent<EnemyfxIDcontainer>())
					{
						if (nonAI)
						{
							throwRock.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = base.photonView.viewID;
						}
						throwRock.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
					}
					throwRock.transform.parent = transform;
					if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
					{
						throwRock.GetPhotonView().RPC("initRPC", PhotonTargets.Others, base.photonView.viewID, base.transform.localScale, throwRock.transform.localPosition, myLevel);
					}
				}
				if (base.animation["attack_" + attackAnimation].normalizedTime >= 0.11f)
				{
					float y = Mathf.Atan2(myHero.transform.position.x - base.transform.position.x, myHero.transform.position.z - base.transform.position.z) * 57.29578f;
					base.gameObject.transform.rotation = Quaternion.Euler(0f, y, 0f);
				}
				if (throwRock != null && base.animation["attack_" + attackAnimation].normalizedTime >= 0.62f)
				{
					float num = 1f;
					float num2 = -20f;
					Vector3 vector2;
					if (myHero != null)
					{
						vector2 = (myHero.transform.position - throwRock.transform.position) / num + myHero.rigidbody.velocity;
						float num3 = myHero.transform.position.y + 2f * myLevel;
						float num4 = num3 - throwRock.transform.position.y;
						vector2 = new Vector3(vector2.x, num4 / num - 0.5f * num2 * num, vector2.z);
					}
					else
					{
						vector2 = base.transform.forward * 60f + Vector3.up * 10f;
					}
					throwRock.GetComponent<RockThrow>().launch(vector2);
					throwRock.transform.parent = null;
					throwRock = null;
				}
			}
			if (attackAnimation == "jumper_0" || attackAnimation == "crawler_jump_0")
			{
				if (!attacked)
				{
					if (base.animation["attack_" + attackAnimation].normalizedTime >= 0.68f)
					{
						attacked = true;
						if (myHero == null || nonAI)
						{
							float num5 = 120f;
							Vector3 velocity = base.transform.forward * speed + Vector3.up * num5;
							if (nonAI && abnormalType == AbnormalType.TYPE_CRAWLER)
							{
								num5 = 100f;
								float a = speed * 2.5f;
								a = Mathf.Min(a, 100f);
								velocity = base.transform.forward * a + Vector3.up * num5;
							}
							base.rigidbody.velocity = velocity;
						}
						else
						{
							float y2 = myHero.rigidbody.velocity.y;
							float num6 = -20f;
							float num7 = gravity;
							float y3 = neck.position.y;
							float num8 = (num6 - num7) * 0.5f;
							float num9 = y2;
							float num10 = myHero.transform.position.y - y3;
							float num11 = Mathf.Abs((Mathf.Sqrt(num9 * num9 - 4f * num8 * num10) - num9) / (2f * num8));
							Vector3 vector3 = myHero.transform.position + myHero.rigidbody.velocity * num11 + Vector3.up * 0.5f * num6 * num11 * num11;
							float y4 = vector3.y;
							float num12;
							if (num10 < 0f || y4 - y3 < 0f)
							{
								num12 = 60f;
								float a2 = speed * 2.5f;
								a2 = Mathf.Min(a2, 100f);
								Vector3 velocity2 = base.transform.forward * a2 + Vector3.up * num12;
								base.rigidbody.velocity = velocity2;
								return;
							}
							float num13 = y4 - y3;
							float num14 = Mathf.Sqrt(2f * num13 / gravity);
							num12 = gravity * num14;
							num12 = Mathf.Max(30f, num12);
							Vector3 vector4 = (vector3 - base.transform.position) / num11;
							abnorma_jump_bite_horizon_v = new Vector3(vector4.x, 0f, vector4.z);
							Vector3 velocity3 = base.rigidbody.velocity;
							Vector3 force = new Vector3(abnorma_jump_bite_horizon_v.x, velocity3.y, abnorma_jump_bite_horizon_v.z) - velocity3;
							base.rigidbody.AddForce(force, ForceMode.VelocityChange);
							base.rigidbody.AddForce(Vector3.up * num12, ForceMode.VelocityChange);
							float num15 = Vector2.Angle(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(myHero.transform.position.x, myHero.transform.position.z));
							num15 = Mathf.Atan2(myHero.transform.position.x - base.transform.position.x, myHero.transform.position.z - base.transform.position.z) * 57.29578f;
							base.gameObject.transform.rotation = Quaternion.Euler(0f, num15, 0f);
						}
					}
					else
					{
						base.rigidbody.velocity = Vector3.zero;
					}
				}
				if (!(base.animation["attack_" + attackAnimation].normalizedTime >= 1f))
				{
					return;
				}
				Debug.DrawLine(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * myLevel, base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * myLevel + Vector3.up * 3f * myLevel, Color.green);
				Debug.DrawLine(base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * myLevel, base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").position + Vector3.up * 1.5f * myLevel + Vector3.forward * 3f * myLevel, Color.green);
				GameObject gameObject10 = checkIfHitHead(head, 3f);
				if (gameObject10 != null)
				{
					Vector3 position5 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
					if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
					{
						gameObject10.GetComponent<HERO>().die((gameObject10.transform.position - position5) * 15f * myLevel, false);
					}
					else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject10.GetComponent<HERO>().HasDied())
					{
						gameObject10.GetComponent<HERO>().markDie();
						gameObject10.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (gameObject10.transform.position - position5) * 15f * myLevel, true, (!nonAI) ? (-1) : base.photonView.viewID, base.name, true);
					}
					if (abnormalType == AbnormalType.TYPE_CRAWLER)
					{
						attackAnimation = "crawler_jump_1";
					}
					else
					{
						attackAnimation = "jumper_1";
					}
					playAnimation("attack_" + attackAnimation);
				}
				if (Mathf.Abs(base.rigidbody.velocity.y) < 0.5f || base.rigidbody.velocity.y < 0f || IsGrounded())
				{
					if (abnormalType == AbnormalType.TYPE_CRAWLER)
					{
						attackAnimation = "crawler_jump_1";
					}
					else
					{
						attackAnimation = "jumper_1";
					}
					playAnimation("attack_" + attackAnimation);
				}
			}
			else if (attackAnimation == "jumper_1" || attackAnimation == "crawler_jump_1")
			{
				if (base.animation["attack_" + attackAnimation].normalizedTime >= 1f && grounded)
				{
					if (abnormalType == AbnormalType.TYPE_CRAWLER)
					{
						attackAnimation = "crawler_jump_2";
					}
					else
					{
						attackAnimation = "jumper_2";
					}
					crossFade("attack_" + attackAnimation, 0.1f);
					fxPosition = base.transform.position;
					GameObject gameObject11 = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine) ? ((GameObject)Object.Instantiate(Resources.Load("FX/boom2"), fxPosition, fxRotation)) : PhotonNetwork.Instantiate("FX/boom2", fxPosition, fxRotation, 0));
					gameObject11.transform.localScale = base.transform.localScale * 1.6f;
					float b2 = 1f - Vector3.Distance(currentCamera.transform.position, gameObject11.transform.position) * 0.05f;
					b2 = Mathf.Min(1f, b2);
					currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(b2, b2);
				}
			}
			else if (attackAnimation == "jumper_2" || attackAnimation == "crawler_jump_2")
			{
				if (base.animation["attack_" + attackAnimation].normalizedTime >= 1f)
				{
					idle();
				}
			}
			else if (base.animation.IsPlaying("tired"))
			{
				if (base.animation["tired"].normalizedTime >= 1f + Mathf.Max(attackEndWait * 2f, 3f))
				{
					idle(Random.Range(attackWait - 1f, 3f));
				}
			}
			else
			{
				if (!(base.animation["attack_" + attackAnimation].normalizedTime >= 1f + attackEndWait))
				{
					return;
				}
				if (nextAttackAnimation != null)
				{
					attack(nextAttackAnimation);
				}
				else if (attackAnimation == "quick_turn_l" || attackAnimation == "quick_turn_r")
				{
					base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y + 180f, base.transform.rotation.eulerAngles.z);
					idle(Random.Range(0.5f, 1f));
					playAnimation("idle");
				}
				else if (abnormalType == AbnormalType.TYPE_I || abnormalType == AbnormalType.TYPE_JUMPER)
				{
					attackCount++;
					if (attackCount > 3 && attackAnimation == "abnormal_getup")
					{
						attackCount = 0;
						crossFade("tired", 0.5f);
					}
					else
					{
						idle(Random.Range(attackWait - 1f, 3f));
					}
				}
				else
				{
					idle(Random.Range(attackWait - 1f, 3f));
				}
			}
		}
		else if (state == TitanState.grab)
		{
			if (base.animation["grab_" + attackAnimation].normalizedTime >= attackCheckTimeA && base.animation["grab_" + attackAnimation].normalizedTime <= attackCheckTimeB && grabbedTarget == null)
			{
				GameObject gameObject12 = checkIfHitHand(currentGrabHand);
				if (gameObject12 != null)
				{
					if (isGrabHandLeft)
					{
						eatSetL(gameObject12);
						grabbedTarget = gameObject12;
					}
					else
					{
						eatSet(gameObject12);
						grabbedTarget = gameObject12;
					}
				}
			}
			if (base.animation["grab_" + attackAnimation].normalizedTime >= 1f)
			{
				if ((bool)grabbedTarget)
				{
					eat();
				}
				else
				{
					idle(Random.Range(attackWait - 1f, 2f));
				}
			}
		}
		else if (state == TitanState.eat)
		{
			if (!attacked && base.animation[attackAnimation].normalizedTime >= 0.48f)
			{
				attacked = true;
				justEatHero(grabbedTarget, currentGrabHand);
			}
			if (grabbedTarget == null)
			{
			}
			if (base.animation[attackAnimation].normalizedTime >= 1f)
			{
				idle();
			}
		}
		else if (state == TitanState.chase)
		{
			if (myHero == null)
			{
				idle();
			}
			else
			{
				if (longRangeAttackCheck())
				{
					return;
				}
				if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.PVP_CAPTURE && PVPfromCheckPt != null && myDistance > chaseDistance)
				{
					idle();
				}
				else if (abnormalType == AbnormalType.TYPE_CRAWLER)
				{
					Vector3 vector5 = myHero.transform.position - base.transform.position;
					float current = (0f - Mathf.Atan2(vector5.z, vector5.x)) * 57.29578f;
					float f = 0f - Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
					if (myDistance < attackDistance * 3f && Random.Range(0f, 1f) < 0.1f && Mathf.Abs(f) < 90f && myHero.transform.position.y < neck.position.y + 30f * myLevel && myHero.transform.position.y > neck.position.y + 10f * myLevel)
					{
						attack("crawler_jump_0");
						return;
					}
					GameObject gameObject13 = checkIfHitCrawlerMouth(head, 2.2f);
					if (gameObject13 != null)
					{
						Vector3 position6 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
						{
							gameObject13.GetComponent<HERO>().die((gameObject13.transform.position - position6) * 15f * myLevel, false);
						}
						else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
						{
							if ((bool)gameObject13.GetComponent<TITAN_EREN>())
							{
								gameObject13.GetComponent<TITAN_EREN>().hitByTitan();
							}
							else if (!gameObject13.GetComponent<HERO>().HasDied())
							{
								gameObject13.GetComponent<HERO>().markDie();
								gameObject13.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (gameObject13.transform.position - position6) * 15f * myLevel, true, (!nonAI) ? (-1) : base.photonView.viewID, base.name, true);
							}
						}
					}
					if (myDistance < attackDistance && Random.Range(0f, 1f) < 0.02f)
					{
						idle(Random.Range(0.05f, 0.2f));
					}
				}
				else if (abnormalType == AbnormalType.TYPE_JUMPER && ((myDistance > attackDistance && myHero.transform.position.y > head.position.y + 4f * myLevel) || myHero.transform.position.y > head.position.y + 4f * myLevel) && Vector3.Distance(base.transform.position, myHero.transform.position) < 1.5f * myHero.transform.position.y)
				{
					attack("jumper_0");
				}
				else if (myDistance < attackDistance)
				{
					idle(Random.Range(0.05f, 0.2f));
				}
			}
		}
		else if (state == TitanState.wander)
		{
			float num16 = 0f;
			float num17 = 0f;
			if (myDistance < chaseDistance || whoHasTauntMe != null)
			{
				Vector3 vector6 = myHero.transform.position - base.transform.position;
				num16 = (0f - Mathf.Atan2(vector6.z, vector6.x)) * 57.29578f;
				num17 = 0f - Mathf.DeltaAngle(num16, base.gameObject.transform.rotation.eulerAngles.y - 90f);
				if (isAlarm || Mathf.Abs(num17) < 90f)
				{
					chase();
					return;
				}
				if (!isAlarm && myDistance < chaseDistance * 0.1f)
				{
					chase();
					return;
				}
			}
			if (Random.Range(0f, 1f) < 0.01f)
			{
				idle();
			}
		}
		else if (state == TitanState.turn)
		{
			base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, desDeg, 0f), Time.deltaTime * Mathf.Abs(turnDeg) * 0.015f);
			if (base.animation[turnAnimation].normalizedTime >= 1f)
			{
				idle();
			}
		}
		else if (state == TitanState.hit_eye)
		{
			if (base.animation.IsPlaying("sit_hit_eye") && base.animation["sit_hit_eye"].normalizedTime >= 1f)
			{
				remainSitdown();
			}
			else if (base.animation.IsPlaying("hit_eye") && base.animation["hit_eye"].normalizedTime >= 1f)
			{
				if (nonAI)
				{
					idle();
				}
				else
				{
					attack("combo_1");
				}
			}
		}
		else if (state == TitanState.to_check_point)
		{
			if (checkPoints.Count <= 0 && myDistance < attackDistance)
			{
				string decidedAction = string.Empty;
				string[] attackStrategy2 = GetAttackStrategy();
				if (attackStrategy2 != null)
				{
					decidedAction = attackStrategy2[Random.Range(0, attackStrategy2.Length)];
				}
				if (executeAttack(decidedAction))
				{
					return;
				}
			}
			if (!(Vector3.Distance(base.transform.position, targetCheckPt) < targetR))
			{
				return;
			}
			if (checkPoints.Count > 0)
			{
				if (checkPoints.Count == 1)
				{
					if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.BOSS_FIGHT_CT)
					{
						GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameLose();
						checkPoints = new ArrayList();
						idle();
					}
					return;
				}
				if (checkPoints.Count == 4)
				{
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendChatContentInfo("<color=#A8FF24>*WARNING!* An abnormal titan is approaching the north gate!</color>");
				}
				Vector3 vector7 = (Vector3)checkPoints[0];
				targetCheckPt = vector7;
				checkPoints.RemoveAt(0);
			}
			else
			{
				idle();
			}
		}
		else if (state == TitanState.to_pvp_pt)
		{
			if (myDistance < chaseDistance * 0.7f)
			{
				chase();
			}
			if (Vector3.Distance(base.transform.position, targetCheckPt) < targetR)
			{
				idle();
			}
		}
		else if (state == TitanState.random_run)
		{
			random_run_time -= Time.deltaTime;
			if (Vector3.Distance(base.transform.position, targetCheckPt) < targetR || random_run_time <= 0f)
			{
				idle();
			}
		}
		else if (state == TitanState.down)
		{
			getdownTime -= Time.deltaTime;
			if (base.animation.IsPlaying("sit_hunt_down") && base.animation["sit_hunt_down"].normalizedTime >= 1f)
			{
				playAnimation("sit_idle");
			}
			if (getdownTime <= 0f)
			{
				crossFade("sit_getup", 0.1f);
			}
			if (base.animation.IsPlaying("sit_getup") && base.animation["sit_getup"].normalizedTime >= 1f)
			{
				idle();
			}
		}
		else if (state == TitanState.sit)
		{
			getdownTime -= Time.deltaTime;
			angle = 0f;
			between2 = 0f;
			if (myDistance < chaseDistance || whoHasTauntMe != null)
			{
				if (myDistance < 50f)
				{
					isAlarm = true;
				}
				else
				{
					Vector3 vector8 = myHero.transform.position - base.transform.position;
					angle = (0f - Mathf.Atan2(vector8.z, vector8.x)) * 57.29578f;
					between2 = 0f - Mathf.DeltaAngle(angle, base.gameObject.transform.rotation.eulerAngles.y - 90f);
					if (Mathf.Abs(between2) < 100f)
					{
						isAlarm = true;
					}
				}
			}
			if (base.animation.IsPlaying("sit_down") && base.animation["sit_down"].normalizedTime >= 1f)
			{
				playAnimation("sit_idle");
			}
			if ((getdownTime <= 0f || isAlarm) && base.animation.IsPlaying("sit_idle"))
			{
				crossFade("sit_getup", 0.1f);
			}
			if (base.animation.IsPlaying("sit_getup") && base.animation["sit_getup"].normalizedTime >= 1f)
			{
				idle();
			}
		}
		else if (state == TitanState.recover)
		{
			getdownTime -= Time.deltaTime;
			if (getdownTime <= 0f)
			{
				idle();
			}
			if (base.animation.IsPlaying("idle_recovery") && base.animation["idle_recovery"].normalizedTime >= 1f)
			{
				idle();
			}
		}
	}

	private bool simpleHitTestLineAndBall(Vector3 line, Vector3 ball, float R)
	{
		Vector3 vector = Vector3.Project(ball, line);
		if ((ball - vector).magnitude > R)
		{
			return false;
		}
		if (Vector3.Dot(line, vector) < 0f)
		{
			return false;
		}
		if (vector.sqrMagnitude > line.sqrMagnitude)
		{
			return false;
		}
		return true;
	}

	private bool longRangeAttackCheck()
	{
		if (abnormalType != AbnormalType.TYPE_PUNK)
		{
			return false;
		}
		if (myHero != null && myHero.rigidbody != null)
		{
			Vector3 vector = myHero.rigidbody.velocity * Time.deltaTime * 30f;
			if (vector.sqrMagnitude > 10f)
			{
				if (simpleHitTestLineAndBall(vector, base.transform.Find("chkAeLeft").position - myHero.transform.position, 5f * myLevel))
				{
					attack("anti_AE_l");
					return true;
				}
				if (simpleHitTestLineAndBall(vector, base.transform.Find("chkAeLLeft").position - myHero.transform.position, 5f * myLevel))
				{
					attack("anti_AE_low_l");
					return true;
				}
				if (simpleHitTestLineAndBall(vector, base.transform.Find("chkAeRight").position - myHero.transform.position, 5f * myLevel))
				{
					attack("anti_AE_r");
					return true;
				}
				if (simpleHitTestLineAndBall(vector, base.transform.Find("chkAeLRight").position - myHero.transform.position, 5f * myLevel))
				{
					attack("anti_AE_low_r");
					return true;
				}
			}
			Vector3 vector2 = myHero.transform.position - base.transform.position;
			float current = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f;
			float f = 0f - Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
			if (rockInterval > 0f)
			{
				rockInterval -= Time.deltaTime;
			}
			else if (Mathf.Abs(f) < 5f)
			{
				Vector3 vector3 = myHero.transform.position + vector;
				float sqrMagnitude = (vector3 - base.transform.position).sqrMagnitude;
				if (sqrMagnitude > 8000f && sqrMagnitude < 90000f)
				{
					attack("throw");
					rockInterval = 2f;
					return true;
				}
			}
		}
		return false;
	}

	private bool executeAttack(string decidedAction)
	{
		switch (decidedAction)
		{
		case "grab_ground_front_l":
			grab("ground_front_l");
			return true;
		case "grab_ground_front_r":
			grab("ground_front_r");
			return true;
		case "grab_ground_back_l":
			grab("ground_back_l");
			return true;
		case "grab_ground_back_r":
			grab("ground_back_r");
			return true;
		case "grab_head_front_l":
			grab("head_front_l");
			return true;
		case "grab_head_front_r":
			grab("head_front_r");
			return true;
		case "grab_head_back_l":
			grab("head_back_l");
			return true;
		case "grab_head_back_r":
			grab("head_back_r");
			return true;
		case "attack_abnormal_jump":
			attack("abnormal_jump");
			return true;
		case "attack_combo":
			attack("combo_1");
			return true;
		case "attack_front_ground":
			attack("front_ground");
			return true;
		case "attack_kick":
			attack("kick");
			return true;
		case "attack_slap_back":
			attack("slap_back");
			return true;
		case "attack_slap_face":
			attack("slap_face");
			return true;
		case "attack_stomp":
			attack("stomp");
			return true;
		case "attack_bite":
			attack("bite");
			return true;
		case "attack_bite_l":
			attack("bite_l");
			return true;
		case "attack_bite_r":
			attack("bite_r");
			return true;
		default:
			return false;
		}
	}

	public void setRoute(GameObject route)
	{
		checkPoints = new ArrayList();
		for (int i = 1; i <= 10; i++)
		{
			checkPoints.Add(route.transform.Find("r" + i).position);
		}
		checkPoints.Add("end");
	}

	public void toCheckPoint(Vector3 targetPt, float r)
	{
		state = TitanState.to_check_point;
		targetCheckPt = targetPt;
		targetR = r;
		crossFade(runAnimation, 0.5f);
	}

	public void randomRun(Vector3 targetPt, float r)
	{
		state = TitanState.random_run;
		targetCheckPt = targetPt;
		targetR = r;
		random_run_time = Random.Range(1f, 2f);
		crossFade(runAnimation, 0.5f);
	}

	public void toPVPCheckPoint(Vector3 targetPt, float r)
	{
		state = TitanState.to_pvp_pt;
		targetCheckPt = targetPt;
		targetR = r;
		crossFade(runAnimation, 0.5f);
	}

	public void hitL(Vector3 attacker, float hitPauseTime)
	{
		if (abnormalType != AbnormalType.TYPE_CRAWLER)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				hit("hit_eren_L", attacker, hitPauseTime);
				return;
			}
			base.photonView.RPC("hitLRPC", PhotonTargets.All, attacker, hitPauseTime);
		}
	}

	[RPC]
	private void hitLRPC(Vector3 attacker, float hitPauseTime)
	{
		if (base.photonView.isMine)
		{
			float magnitude = (attacker - base.transform.position).magnitude;
			if (magnitude < 80f)
			{
				hit("hit_eren_L", attacker, hitPauseTime);
			}
		}
	}

	private void hit(string animationName, Vector3 attacker, float hitPauseTime)
	{
		state = TitanState.hit;
		hitAnimation = animationName;
		hitPause = hitPauseTime;
		playAnimation(hitAnimation);
		base.animation[hitAnimation].time = 0f;
		base.animation[hitAnimation].speed = 0f;
		base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - base.transform.position).eulerAngles.y, 0f);
		needFreshCorePosition = true;
		if (base.photonView.isMine && grabbedTarget != null)
		{
			grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
		}
	}

	public void hitR(Vector3 attacker, float hitPauseTime)
	{
		if (abnormalType != AbnormalType.TYPE_CRAWLER)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				hit("hit_eren_R", attacker, hitPauseTime);
				return;
			}
			base.photonView.RPC("hitRRPC", PhotonTargets.All, attacker, hitPauseTime);
		}
	}

	[RPC]
	private void hitRRPC(Vector3 attacker, float hitPauseTime)
	{
		if (base.photonView.isMine && !hasDie)
		{
			float magnitude = (attacker - base.transform.position).magnitude;
			if (magnitude < 80f)
			{
				hit("hit_eren_R", attacker, hitPauseTime);
			}
		}
	}

	public void dieBlow(Vector3 attacker, float hitPauseTime)
	{
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			dieBlowFunc(attacker, hitPauseTime);
			if (GameObject.FindGameObjectsWithTag("titan").Length <= 1)
			{
				GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			}
		}
		else
		{
			base.photonView.RPC("dieBlowRPC", PhotonTargets.All, attacker, hitPauseTime);
		}
	}

	[RPC]
	private void dieBlowRPC(Vector3 attacker, float hitPauseTime)
	{
		if (base.photonView.isMine)
		{
			float magnitude = (attacker - base.transform.position).magnitude;
			if (magnitude < 80f)
			{
				dieBlowFunc(attacker, hitPauseTime);
			}
		}
	}

	public void dieBlowFunc(Vector3 attacker, float hitPauseTime)
	{
		if (hasDie)
		{
			return;
		}
		base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - base.transform.position).eulerAngles.y, 0f);
		hasDie = true;
		hitAnimation = "die_blow";
		hitPause = hitPauseTime;
		playAnimation(hitAnimation);
		base.animation[hitAnimation].time = 0f;
		base.animation[hitAnimation].speed = 0f;
		needFreshCorePosition = true;
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().oneTitanDown(string.Empty);
		if (base.photonView.isMine)
		{
			if (grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			if (nonAI)
			{
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
				PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					PhotonPlayerProperty.dead,
					true
				} });
				PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					PhotonPlayerProperty.deaths,
					(int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths] + 1
				} });
			}
		}
	}

	public void dieHeadBlow(Vector3 attacker, float hitPauseTime)
	{
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			dieHeadBlowFunc(attacker, hitPauseTime);
			if (GameObject.FindGameObjectsWithTag("titan").Length <= 1)
			{
				GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			}
		}
		else
		{
			base.photonView.RPC("dieHeadBlowRPC", PhotonTargets.All, attacker, hitPauseTime);
		}
	}

	[RPC]
	private void dieHeadBlowRPC(Vector3 attacker, float hitPauseTime)
	{
		if (base.photonView.isMine)
		{
			float magnitude = (attacker - base.transform.position).magnitude;
			if (magnitude < 80f)
			{
				dieHeadBlowFunc(attacker, hitPauseTime);
			}
		}
	}

	private void playSound(string sndname)
	{
		playsoundRPC(sndname);
		if (base.photonView.isMine)
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

	public void dieHeadBlowFunc(Vector3 attacker, float hitPauseTime)
	{
		if (hasDie)
		{
			return;
		}
		playSound("snd_titan_head_blow");
		base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - base.transform.position).eulerAngles.y, 0f);
		hasDie = true;
		hitAnimation = "die_headOff";
		hitPause = hitPauseTime;
		playAnimation(hitAnimation);
		base.animation[hitAnimation].time = 0f;
		base.animation[hitAnimation].speed = 0f;
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().oneTitanDown(string.Empty);
		needFreshCorePosition = true;
		GameObject gameObject = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine) ? ((GameObject)Object.Instantiate(Resources.Load("bloodExplore"), head.position + Vector3.up * 1f * myLevel, Quaternion.Euler(270f, 0f, 0f))) : PhotonNetwork.Instantiate("bloodExplore", head.position + Vector3.up * 1f * myLevel, Quaternion.Euler(270f, 0f, 0f), 0));
		gameObject.transform.localScale = base.transform.localScale;
		gameObject = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine) ? ((GameObject)Object.Instantiate(Resources.Load("bloodsplatter"), head.position, Quaternion.Euler(270f + neck.rotation.eulerAngles.x, neck.rotation.eulerAngles.y, neck.rotation.eulerAngles.z))) : PhotonNetwork.Instantiate("bloodsplatter", head.position, Quaternion.Euler(270f + neck.rotation.eulerAngles.x, neck.rotation.eulerAngles.y, neck.rotation.eulerAngles.z), 0));
		gameObject.transform.localScale = base.transform.localScale;
		gameObject.transform.parent = neck;
		gameObject = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine) ? ((GameObject)Object.Instantiate(Resources.Load("FX/justSmoke"), neck.position, Quaternion.Euler(270f, 0f, 0f))) : PhotonNetwork.Instantiate("FX/justSmoke", neck.position, Quaternion.Euler(270f, 0f, 0f), 0));
		gameObject.transform.parent = neck;
		if (base.photonView.isMine)
		{
			if (grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			if (nonAI)
			{
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
				PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					PhotonPlayerProperty.dead,
					true
				} });
				PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					PhotonPlayerProperty.deaths,
					(int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths] + 1
				} });
			}
		}
	}

	private void justEatHero(GameObject target, Transform hand)
	{
		if (target == null)
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			if (!target.GetComponent<HERO>().HasDied())
			{
				target.GetComponent<HERO>().markDie();
				if (nonAI)
				{
					target.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, base.photonView.viewID, base.name);
				}
				else
				{
					target.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, -1, base.name);
				}
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			target.GetComponent<HERO>().die2(hand);
		}
	}

	private void FixedUpdate()
	{
		if ((IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine))
		{
			return;
		}
		base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
		if (needFreshCorePosition)
		{
			oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
			needFreshCorePosition = false;
		}
		if (hasDie)
		{
			if (!(hitPause > 0f) && base.animation.IsPlaying("die_headOff"))
			{
				Vector3 vector = base.transform.position - base.transform.Find("Amarture/Core").position - oldCorePosition;
				base.rigidbody.velocity = vector / Time.deltaTime + Vector3.up * base.rigidbody.velocity.y;
			}
			oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
		}
		else if ((state == TitanState.attack && isAttackMoveByCore) || state == TitanState.hit)
		{
			Vector3 vector2 = base.transform.position - base.transform.Find("Amarture/Core").position - oldCorePosition;
			base.rigidbody.velocity = vector2 / Time.deltaTime + Vector3.up * base.rigidbody.velocity.y;
			oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
		}
		if (hasDie)
		{
			if (hitPause > 0f)
			{
				hitPause -= Time.deltaTime;
				if (hitPause <= 0f)
				{
					base.animation[hitAnimation].speed = 1f;
					hitPause = 0f;
				}
			}
			else if (base.animation.IsPlaying("die_blow"))
			{
				if (base.animation["die_blow"].normalizedTime < 0.55f)
				{
					base.rigidbody.velocity = -base.transform.forward * 300f + Vector3.up * base.rigidbody.velocity.y;
				}
				else if (base.animation["die_blow"].normalizedTime < 0.83f)
				{
					base.rigidbody.velocity = -base.transform.forward * 100f + Vector3.up * base.rigidbody.velocity.y;
				}
				else
				{
					base.rigidbody.velocity = Vector3.up * base.rigidbody.velocity.y;
				}
			}
			return;
		}
		if (nonAI && !IN_GAME_MAIN_CAMERA.isPausing && (state == TitanState.idle || (state == TitanState.attack && attackAnimation == "jumper_1")))
		{
			Vector3 vector3 = Vector3.zero;
			if (controller.targetDirection != -874f)
			{
				bool flag = false;
				if (stamina < 5f)
				{
					flag = true;
				}
				else if (stamina < 40f && !base.animation.IsPlaying("run_abnormal") && !base.animation.IsPlaying("crawler_run"))
				{
					flag = true;
				}
				vector3 = ((!controller.isWALKDown && !flag) ? (base.transform.forward * speed * Mathf.Sqrt(myLevel)) : (base.transform.forward * speed * Mathf.Sqrt(myLevel) * 0.2f));
				base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, controller.targetDirection, 0f), speed * 0.15f * Time.deltaTime);
				if (state == TitanState.idle)
				{
					if (controller.isWALKDown || flag)
					{
						if (abnormalType == AbnormalType.TYPE_CRAWLER)
						{
							if (!base.animation.IsPlaying("crawler_run"))
							{
								crossFade("crawler_run", 0.1f);
							}
						}
						else if (!base.animation.IsPlaying("run_walk"))
						{
							crossFade("run_walk", 0.1f);
						}
					}
					else if (abnormalType == AbnormalType.TYPE_CRAWLER)
					{
						if (!base.animation.IsPlaying("crawler_run"))
						{
							crossFade("crawler_run", 0.1f);
						}
						GameObject gameObject = checkIfHitCrawlerMouth(head, 2.2f);
						if (gameObject != null)
						{
							Vector3 position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
							if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
							{
								gameObject.GetComponent<HERO>().die((gameObject.transform.position - position) * 15f * myLevel, false);
							}
							else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine && !gameObject.GetComponent<HERO>().HasDied())
							{
								gameObject.GetComponent<HERO>().markDie();
								gameObject.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (gameObject.transform.position - position) * 15f * myLevel, true, (!nonAI) ? (-1) : base.photonView.viewID, base.name, true);
							}
						}
					}
					else if (!base.animation.IsPlaying("run_abnormal"))
					{
						crossFade("run_abnormal", 0.1f);
					}
				}
			}
			else if (state == TitanState.idle)
			{
				if (abnormalType == AbnormalType.TYPE_CRAWLER)
				{
					if (!base.animation.IsPlaying("crawler_idle"))
					{
						crossFade("crawler_idle", 0.1f);
					}
				}
				else if (!base.animation.IsPlaying("idle"))
				{
					crossFade("idle", 0.1f);
				}
				vector3 = Vector3.zero;
			}
			if (state == TitanState.idle)
			{
				Vector3 velocity = base.rigidbody.velocity;
				Vector3 force = vector3 - velocity;
				force.x = Mathf.Clamp(force.x, 0f - maxVelocityChange, maxVelocityChange);
				force.z = Mathf.Clamp(force.z, 0f - maxVelocityChange, maxVelocityChange);
				force.y = 0f;
				base.rigidbody.AddForce(force, ForceMode.VelocityChange);
			}
			else if (state == TitanState.attack && attackAnimation == "jumper_0")
			{
				Vector3 velocity2 = base.rigidbody.velocity;
				Vector3 force2 = vector3 * 0.8f - velocity2;
				force2.x = Mathf.Clamp(force2.x, 0f - maxVelocityChange, maxVelocityChange);
				force2.z = Mathf.Clamp(force2.z, 0f - maxVelocityChange, maxVelocityChange);
				force2.y = 0f;
				base.rigidbody.AddForce(force2, ForceMode.VelocityChange);
			}
		}
		if ((abnormalType == AbnormalType.TYPE_I || abnormalType == AbnormalType.TYPE_JUMPER) && !nonAI && state == TitanState.attack && attackAnimation == "jumper_0")
		{
			Vector3 vector4 = base.transform.forward * speed * myLevel * 0.5f;
			Vector3 velocity3 = base.rigidbody.velocity;
			if (!(base.animation["attack_jumper_0"].normalizedTime > 0.28f) || !(base.animation["attack_jumper_0"].normalizedTime < 0.8f))
			{
				vector4 = Vector3.zero;
			}
			Vector3 force3 = vector4 - velocity3;
			force3.x = Mathf.Clamp(force3.x, 0f - maxVelocityChange, maxVelocityChange);
			force3.z = Mathf.Clamp(force3.z, 0f - maxVelocityChange, maxVelocityChange);
			force3.y = 0f;
			base.rigidbody.AddForce(force3, ForceMode.VelocityChange);
		}
		if (state != TitanState.chase && state != TitanState.wander && state != TitanState.to_check_point && state != TitanState.to_pvp_pt && state != TitanState.random_run)
		{
			return;
		}
		Vector3 vector5 = base.transform.forward * speed;
		Vector3 velocity4 = base.rigidbody.velocity;
		Vector3 force4 = vector5 - velocity4;
		force4.x = Mathf.Clamp(force4.x, 0f - maxVelocityChange, maxVelocityChange);
		force4.z = Mathf.Clamp(force4.z, 0f - maxVelocityChange, maxVelocityChange);
		force4.y = 0f;
		base.rigidbody.AddForce(force4, ForceMode.VelocityChange);
		if (!stuck && abnormalType != AbnormalType.TYPE_CRAWLER && !nonAI)
		{
			if (base.animation.IsPlaying(runAnimation) && base.rigidbody.velocity.magnitude < speed * 0.5f)
			{
				stuck = true;
				stuckTime = 2f;
				stuckTurnAngle = (float)Random.Range(0, 2) * 140f - 70f;
			}
			if (state == TitanState.chase && myHero != null && myDistance > attackDistance && myDistance < 150f)
			{
				float num = 0.05f;
				if (myDifficulty > 1)
				{
					num += 0.05f;
				}
				if (abnormalType != 0)
				{
					num += 0.1f;
				}
				if (Random.Range(0f, 1f) < num)
				{
					stuck = true;
					stuckTime = 1f;
					float num2 = Random.Range(20f, 50f);
					stuckTurnAngle = (float)Random.Range(0, 2) * num2 * 2f - num2;
				}
			}
		}
		float num3 = 0f;
		if (state == TitanState.wander)
		{
			num3 = base.transform.rotation.eulerAngles.y - 90f;
		}
		else if (state == TitanState.to_check_point || state == TitanState.to_pvp_pt || state == TitanState.random_run)
		{
			Vector3 vector6 = targetCheckPt - base.transform.position;
			num3 = (0f - Mathf.Atan2(vector6.z, vector6.x)) * 57.29578f;
		}
		else
		{
			if (myHero == null)
			{
				return;
			}
			Vector3 vector7 = myHero.transform.position - base.transform.position;
			num3 = (0f - Mathf.Atan2(vector7.z, vector7.x)) * 57.29578f;
		}
		if (stuck)
		{
			stuckTime -= Time.deltaTime;
			if (stuckTime < 0f)
			{
				stuck = false;
			}
			if (stuckTurnAngle > 0f)
			{
				stuckTurnAngle -= Time.deltaTime * 10f;
			}
			else
			{
				stuckTurnAngle += Time.deltaTime * 10f;
			}
			num3 += stuckTurnAngle;
		}
		float num4 = 0f - Mathf.DeltaAngle(num3, base.gameObject.transform.rotation.eulerAngles.y - 90f);
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num4, 0f), speed * 0.3f * Time.deltaTime / myLevel);
		}
		else
		{
			base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num4, 0f), speed * 0.5f * Time.deltaTime / myLevel);
		}
	}

	public void lateUpdate()
	{
		if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			return;
		}
		if (base.animation.IsPlaying("run_walk"))
		{
			if (base.animation["run_walk"].normalizedTime % 1f > 0.1f && base.animation["run_walk"].normalizedTime % 1f < 0.6f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform transform = base.transform.Find("snd_titan_foot");
				transform.GetComponent<AudioSource>().Stop();
				transform.GetComponent<AudioSource>().Play();
			}
			if (base.animation["run_walk"].normalizedTime % 1f > 0.6f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform transform2 = base.transform.Find("snd_titan_foot");
				transform2.GetComponent<AudioSource>().Stop();
				transform2.GetComponent<AudioSource>().Play();
			}
		}
		if (base.animation.IsPlaying("crawler_run"))
		{
			if (base.animation["crawler_run"].normalizedTime % 1f > 0.1f && base.animation["crawler_run"].normalizedTime % 1f < 0.56f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform transform3 = base.transform.Find("snd_titan_foot");
				transform3.GetComponent<AudioSource>().Stop();
				transform3.GetComponent<AudioSource>().Play();
			}
			if (base.animation["crawler_run"].normalizedTime % 1f > 0.56f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform transform4 = base.transform.Find("snd_titan_foot");
				transform4.GetComponent<AudioSource>().Stop();
				transform4.GetComponent<AudioSource>().Play();
			}
		}
		if (base.animation.IsPlaying("run_abnormal"))
		{
			if (base.animation["run_abnormal"].normalizedTime % 1f > 0.47f && base.animation["run_abnormal"].normalizedTime % 1f < 0.95f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform transform5 = base.transform.Find("snd_titan_foot");
				transform5.GetComponent<AudioSource>().Stop();
				transform5.GetComponent<AudioSource>().Play();
			}
			if ((base.animation["run_abnormal"].normalizedTime % 1f > 0.95f || base.animation["run_abnormal"].normalizedTime % 1f < 0.47f) && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform transform6 = base.transform.Find("snd_titan_foot");
				transform6.GetComponent<AudioSource>().Stop();
				transform6.GetComponent<AudioSource>().Play();
			}
		}
		headMovement();
		grounded = false;
	}

	public void headMovement()
	{
		if (!hasDie)
		{
			if (IN_GAME_MAIN_CAMERA.gametype != 0)
			{
				if (base.photonView.isMine)
				{
					targetHeadRotation = head.rotation;
					bool flag = false;
					if (abnormalType != AbnormalType.TYPE_CRAWLER && state != TitanState.attack && state != TitanState.down && state != TitanState.hit && state != TitanState.recover && state != TitanState.eat && state != TitanState.hit_eye && !hasDie && myDistance < 100f && myHero != null)
					{
						Vector3 vector = myHero.transform.position - base.transform.position;
						angle = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
						float value = 0f - Mathf.DeltaAngle(angle, base.transform.rotation.eulerAngles.y - 90f);
						value = Mathf.Clamp(value, -40f, 40f);
						float y = neck.position.y + myLevel * 2f - myHero.transform.position.y;
						float value2 = Mathf.Atan2(y, myDistance) * 57.29578f;
						value2 = Mathf.Clamp(value2, -40f, 30f);
						targetHeadRotation = Quaternion.Euler(head.rotation.eulerAngles.x + value2, head.rotation.eulerAngles.y + value, head.rotation.eulerAngles.z);
						if (!asClientLookTarget)
						{
							asClientLookTarget = true;
							base.photonView.RPC("setIfLookTarget", PhotonTargets.Others, true);
						}
						flag = true;
					}
					if (!flag && asClientLookTarget)
					{
						asClientLookTarget = false;
						base.photonView.RPC("setIfLookTarget", PhotonTargets.Others, false);
					}
					if (state == TitanState.attack || state == TitanState.hit || state == TitanState.hit_eye)
					{
						oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 20f);
					}
					else
					{
						oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 10f);
					}
				}
				else
				{
					targetHeadRotation = head.rotation;
					if (asClientLookTarget && myHero != null)
					{
						Vector3 vector2 = myHero.transform.position - base.transform.position;
						angle = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f;
						float value3 = 0f - Mathf.DeltaAngle(angle, base.transform.rotation.eulerAngles.y - 90f);
						value3 = Mathf.Clamp(value3, -40f, 40f);
						float y2 = neck.position.y + myLevel * 2f - myHero.transform.position.y;
						float value4 = Mathf.Atan2(y2, myDistance) * 57.29578f;
						value4 = Mathf.Clamp(value4, -40f, 30f);
						targetHeadRotation = Quaternion.Euler(head.rotation.eulerAngles.x + value4, head.rotation.eulerAngles.y + value3, head.rotation.eulerAngles.z);
					}
					if (!hasDie)
					{
						oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 10f);
					}
				}
			}
			else
			{
				targetHeadRotation = head.rotation;
				if (abnormalType != AbnormalType.TYPE_CRAWLER && state != TitanState.attack && state != TitanState.down && state != TitanState.hit && state != TitanState.recover && state != TitanState.hit_eye && !hasDie && myDistance < 100f && myHero != null)
				{
					Vector3 vector3 = myHero.transform.position - base.transform.position;
					angle = (0f - Mathf.Atan2(vector3.z, vector3.x)) * 57.29578f;
					float value5 = 0f - Mathf.DeltaAngle(angle, base.transform.rotation.eulerAngles.y - 90f);
					value5 = Mathf.Clamp(value5, -40f, 40f);
					float y3 = neck.position.y + myLevel * 2f - myHero.transform.position.y;
					float value6 = Mathf.Atan2(y3, myDistance) * 57.29578f;
					value6 = Mathf.Clamp(value6, -40f, 30f);
					targetHeadRotation = Quaternion.Euler(head.rotation.eulerAngles.x + value6, head.rotation.eulerAngles.y + value5, head.rotation.eulerAngles.z);
				}
				if (state == TitanState.attack || state == TitanState.hit || state == TitanState.hit_eye)
				{
					oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 20f);
				}
				else
				{
					oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 10f);
				}
			}
			head.rotation = oldHeadRotation;
		}
		if (!base.animation.IsPlaying("die_headOff"))
		{
			head.localScale = headscale;
		}
	}

	[RPC]
	private void setIfLookTarget(bool bo)
	{
		asClientLookTarget = bo;
	}

	private void OnCollisionStay()
	{
		grounded = true;
	}

	private void attack(string type)
	{
		state = TitanState.attack;
		attacked = false;
		isAlarm = true;
		if (attackAnimation == type)
		{
			attackAnimation = type;
			playAnimationAt("attack_" + type, 0f);
		}
		else
		{
			attackAnimation = type;
			playAnimationAt("attack_" + type, 0f);
		}
		nextAttackAnimation = null;
		fxName = null;
		isAttackMoveByCore = false;
		attackCheckTime = 0f;
		attackCheckTimeA = 0f;
		attackCheckTimeB = 0f;
		attackEndWait = 0f;
		fxRotation = Quaternion.Euler(270f, 0f, 0f);
		switch (type)
		{
		case "abnormal_getup":
			attackCheckTime = 0f;
			fxName = string.Empty;
			break;
		case "abnormal_jump":
			nextAttackAnimation = "abnormal_getup";
			if (nonAI)
			{
				attackEndWait = 0f;
			}
			else
			{
				attackEndWait = ((myDifficulty <= 0) ? Random.Range(1f, 4f) : Random.Range(0f, 1f));
			}
			attackCheckTime = 0.75f;
			fxName = "boom4";
			fxRotation = Quaternion.Euler(270f, base.transform.rotation.eulerAngles.y, 0f);
			break;
		case "combo_1":
			nextAttackAnimation = "combo_2";
			attackCheckTimeA = 0.54f;
			attackCheckTimeB = 0.76f;
			nonAIcombo = false;
			isAttackMoveByCore = true;
			leftHandAttack = false;
			break;
		case "combo_2":
			if (abnormalType != AbnormalType.TYPE_PUNK)
			{
				nextAttackAnimation = "combo_3";
			}
			attackCheckTimeA = 0.37f;
			attackCheckTimeB = 0.57f;
			nonAIcombo = false;
			isAttackMoveByCore = true;
			leftHandAttack = true;
			break;
		case "combo_3":
			nonAIcombo = false;
			isAttackMoveByCore = true;
			attackCheckTime = 0.21f;
			fxName = "boom1";
			break;
		case "front_ground":
			fxName = "boom1";
			attackCheckTime = 0.45f;
			break;
		case "kick":
			fxName = "boom5";
			fxRotation = base.transform.rotation;
			attackCheckTime = 0.43f;
			break;
		case "slap_back":
			fxName = "boom3";
			attackCheckTime = 0.66f;
			break;
		case "slap_face":
			fxName = "boom3";
			attackCheckTime = 0.655f;
			break;
		case "stomp":
			fxName = "boom2";
			attackCheckTime = 0.42f;
			break;
		case "bite":
			fxName = "bite";
			attackCheckTime = 0.6f;
			break;
		case "bite_l":
			fxName = "bite";
			attackCheckTime = 0.4f;
			break;
		case "bite_r":
			fxName = "bite";
			attackCheckTime = 0.4f;
			break;
		case "jumper_0":
			abnorma_jump_bite_horizon_v = Vector3.zero;
			break;
		case "crawler_jump_0":
			abnorma_jump_bite_horizon_v = Vector3.zero;
			break;
		case "anti_AE_l":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = true;
			break;
		case "anti_AE_r":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = false;
			break;
		case "anti_AE_low_l":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = true;
			break;
		case "anti_AE_low_r":
			attackCheckTimeA = 0.31f;
			attackCheckTimeB = 0.4f;
			leftHandAttack = false;
			break;
		case "quick_turn_l":
			attackCheckTimeA = 2f;
			attackCheckTimeB = 2f;
			isAttackMoveByCore = true;
			break;
		case "quick_turn_r":
			attackCheckTimeA = 2f;
			attackCheckTimeB = 2f;
			isAttackMoveByCore = true;
			break;
		case "throw":
			isAlarm = true;
			chaseDistance = 99999f;
			break;
		}
		needFreshCorePosition = true;
	}

	private void grab(string type)
	{
		state = TitanState.grab;
		attacked = false;
		isAlarm = true;
		attackAnimation = type;
		crossFade("grab_" + type, 0.1f);
		isGrabHandLeft = true;
		grabbedTarget = null;
		switch (type)
		{
		case "ground_back_l":
			attackCheckTimeA = 0.34f;
			attackCheckTimeB = 0.49f;
			break;
		case "ground_back_r":
			attackCheckTimeA = 0.34f;
			attackCheckTimeB = 0.49f;
			isGrabHandLeft = false;
			break;
		case "ground_front_l":
			attackCheckTimeA = 0.37f;
			attackCheckTimeB = 0.6f;
			break;
		case "ground_front_r":
			attackCheckTimeA = 0.37f;
			attackCheckTimeB = 0.6f;
			isGrabHandLeft = false;
			break;
		case "head_back_l":
			attackCheckTimeA = 0.45f;
			attackCheckTimeB = 0.5f;
			isGrabHandLeft = false;
			break;
		case "head_back_r":
			attackCheckTimeA = 0.45f;
			attackCheckTimeB = 0.5f;
			break;
		case "head_front_l":
			attackCheckTimeA = 0.38f;
			attackCheckTimeB = 0.55f;
			break;
		case "head_front_r":
			attackCheckTimeA = 0.38f;
			attackCheckTimeB = 0.55f;
			isGrabHandLeft = false;
			break;
		}
		if (isGrabHandLeft)
		{
			currentGrabHand = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
		}
		else
		{
			currentGrabHand = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		}
	}

	private void eat()
	{
		state = TitanState.eat;
		attacked = false;
		if (isGrabHandLeft)
		{
			attackAnimation = "eat_l";
			crossFade("eat_l", 0.1f);
		}
		else
		{
			attackAnimation = "eat_r";
			crossFade("eat_r", 0.1f);
		}
	}

	private void chase()
	{
		state = TitanState.chase;
		isAlarm = true;
		crossFade(runAnimation, 0.5f);
	}

	private void idle(float sbtime = 0f)
	{
		stuck = false;
		this.sbtime = sbtime;
		if (myDifficulty == 2 && (abnormalType == AbnormalType.TYPE_JUMPER || abnormalType == AbnormalType.TYPE_I))
		{
			this.sbtime = Random.Range(0f, 1.5f);
		}
		else if (myDifficulty >= 1)
		{
			this.sbtime = 0f;
		}
		this.sbtime = Mathf.Max(0.5f, this.sbtime);
		if (abnormalType == AbnormalType.TYPE_PUNK)
		{
			this.sbtime = 0.1f;
			if (myDifficulty == 1)
			{
				this.sbtime += 0.4f;
			}
		}
		state = TitanState.idle;
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			crossFade("crawler_idle", 0.2f);
		}
		else
		{
			crossFade("idle", 0.2f);
		}
	}

	private void wander(float sbtime = 0f)
	{
		state = TitanState.wander;
		crossFade(runAnimation, 0.5f);
	}

	private void turn(float d)
	{
		if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			if (d > 0f)
			{
				turnAnimation = "crawler_turnaround_R";
			}
			else
			{
				turnAnimation = "crawler_turnaround_L";
			}
		}
		else if (d > 0f)
		{
			turnAnimation = "turnaround2";
		}
		else
		{
			turnAnimation = "turnaround1";
		}
		playAnimation(turnAnimation);
		base.animation[turnAnimation].time = 0f;
		d = Mathf.Clamp(d, -120f, 120f);
		turnDeg = d;
		desDeg = base.gameObject.transform.rotation.eulerAngles.y + turnDeg;
		state = TitanState.turn;
	}

	private void eatSet(GameObject grabTarget)
	{
		if ((IN_GAME_MAIN_CAMERA.gametype != 0 && (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine)) || !grabTarget.GetComponent<HERO>().isGrabbed)
		{
			grabToRight();
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
			{
				base.photonView.RPC("grabToRight", PhotonTargets.Others);
				grabTarget.GetPhotonView().RPC("netPlayAnimation", PhotonTargets.All, "grabbed");
				grabTarget.GetPhotonView().RPC("netGrabbed", PhotonTargets.All, base.photonView.viewID, false);
			}
			else
			{
				grabTarget.GetComponent<HERO>().grabbed(base.gameObject, false);
				grabTarget.GetComponent<HERO>().animation.Play("grabbed");
			}
		}
	}

	private void eatSetL(GameObject grabTarget)
	{
		if ((IN_GAME_MAIN_CAMERA.gametype != 0 && (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !base.photonView.isMine)) || !grabTarget.GetComponent<HERO>().isGrabbed)
		{
			grabToLeft();
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
			{
				base.photonView.RPC("grabToLeft", PhotonTargets.Others);
				grabTarget.GetPhotonView().RPC("netPlayAnimation", PhotonTargets.All, "grabbed");
				grabTarget.GetPhotonView().RPC("netGrabbed", PhotonTargets.All, base.photonView.viewID, true);
			}
			else
			{
				grabTarget.GetComponent<HERO>().grabbed(base.gameObject, true);
				grabTarget.GetComponent<HERO>().animation.Play("grabbed");
			}
		}
	}

	[RPC]
	public void grabToRight()
	{
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		grabTF.transform.parent = transform;
		grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
		grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
		grabTF.transform.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localPosition += Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
		grabTF.transform.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localRotation = Quaternion.Euler(grabTF.transform.localRotation.eulerAngles.x, grabTF.transform.localRotation.eulerAngles.y + 180f, grabTF.transform.localRotation.eulerAngles.z);
	}

	[RPC]
	public void grabToLeft()
	{
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
		grabTF.transform.parent = transform;
		grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
		grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
		grabTF.transform.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localPosition -= Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
		grabTF.transform.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localRotation = Quaternion.Euler(grabTF.transform.localRotation.eulerAngles.x, grabTF.transform.localRotation.eulerAngles.y + 180f, grabTF.transform.localRotation.eulerAngles.z + 180f);
	}

	[RPC]
	public void grabbedTargetEscape()
	{
		grabbedTarget = null;
	}

	private void justHitEye()
	{
		if (state != TitanState.hit_eye)
		{
			if (state == TitanState.down || state == TitanState.sit)
			{
				playAnimation("sit_hit_eye");
			}
			else
			{
				playAnimation("hit_eye");
			}
			state = TitanState.hit_eye;
		}
	}

	public void hitEye()
	{
		if (!hasDie)
		{
			justHitEye();
		}
	}

	[RPC]
	public void hitEyeRPC(int viewID)
	{
		if (hasDie)
		{
			return;
		}
		float magnitude = (PhotonView.Find(viewID).gameObject.transform.position - neck.position).magnitude;
		if (magnitude < 20f)
		{
			if (base.photonView.isMine && grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			if (!hasDie)
			{
				justHitEye();
			}
		}
	}

	private void getDown()
	{
		state = TitanState.down;
		isAlarm = true;
		playAnimation("sit_hunt_down");
		getdownTime = Random.Range(3f, 5f);
	}

	private void sitdown()
	{
		state = TitanState.sit;
		playAnimation("sit_down");
		getdownTime = Random.Range(10f, 30f);
	}

	private void remainSitdown()
	{
		state = TitanState.sit;
		playAnimation("sit_idle");
		getdownTime = Random.Range(10f, 30f);
	}

	private void recover()
	{
		state = TitanState.recover;
		playAnimation("idle_recovery");
		getdownTime = Random.Range(2f, 5f);
	}

	public void hitAnkle()
	{
		if (!hasDie && state != TitanState.down)
		{
			if (grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			getDown();
		}
	}

	[RPC]
	public void hitAnkleRPC(int viewID)
	{
		if (hasDie || state == TitanState.down)
		{
			return;
		}
		PhotonView photonView = PhotonView.Find(viewID);
		if (photonView == null)
		{
			return;
		}
		float magnitude = (photonView.gameObject.transform.position - base.transform.position).magnitude;
		if (magnitude < 20f)
		{
			if (base.photonView.isMine && grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			getDown();
		}
	}

	public bool die()
	{
		if (hasDie)
		{
			return false;
		}
		hasDie = true;
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().oneTitanDown(string.Empty);
		dieAnimation();
		return true;
	}

	private void dieAnimation()
	{
		if (base.animation.IsPlaying("sit_idle") || base.animation.IsPlaying("sit_hit_eye"))
		{
			crossFade("sit_die", 0.1f);
		}
		else if (abnormalType == AbnormalType.TYPE_CRAWLER)
		{
			crossFade("crawler_die", 0.2f);
		}
		else if (abnormalType == AbnormalType.NORMAL)
		{
			crossFade("die_front", 0.05f);
		}
		else if ((base.animation.IsPlaying("attack_abnormal_jump") && base.animation["attack_abnormal_jump"].normalizedTime > 0.7f) || (base.animation.IsPlaying("attack_abnormal_getup") && base.animation["attack_abnormal_getup"].normalizedTime < 0.7f) || base.animation.IsPlaying("tired"))
		{
			crossFade("die_ground", 0.2f);
		}
		else
		{
			crossFade("die_back", 0.05f);
		}
	}

	[RPC]
	public void titanGetHit(int viewID, int speed)
	{
		PhotonView photonView = PhotonView.Find(viewID);
		if (photonView == null)
		{
			return;
		}
		float magnitude = (photonView.gameObject.transform.position - neck.position).magnitude;
		if (magnitude < 30f && !hasDie)
		{
			base.photonView.RPC("netDie", PhotonTargets.OthersBuffered);
			if (grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			netDie();
			if (nonAI)
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().titanGetKill(photonView.owner, speed, (string)PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
			}
			else
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().titanGetKill(photonView.owner, speed, base.name);
			}
		}
	}

	[RPC]
	private void netDie()
	{
		asClientLookTarget = false;
		if (!hasDie)
		{
			hasDie = true;
			if (nonAI)
			{
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
				PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					PhotonPlayerProperty.dead,
					true
				} });
				PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					PhotonPlayerProperty.deaths,
					(int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths] + 1
				} });
			}
			dieAnimation();
		}
	}

	public void suicide()
	{
		netDie();
		if (nonAI)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(false, string.Empty, true, (string)PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
		}
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().needChooseSide = true;
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().justSuicide = true;
	}
}
