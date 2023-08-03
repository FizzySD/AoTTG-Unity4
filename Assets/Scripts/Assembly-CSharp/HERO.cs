using System;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;
using Xft;

public class HERO : Photon.MonoBehaviour
{
	public FengCustomInputs inputManager;

	public Camera currentCamera;

	private string skillId;

	private ParticleSystem sparks;

	private ParticleSystem smoke_3dmg;

	private float invincible = 3f;

	private GameObject myHorse;

	private bool isMounted;

	private bool spinning;

	private string standAnimation = "stand";

	public GameObject speedFX;

	public GameObject speedFX1;

	private ParticleSystem speedFXPS;

	public bool useGun;

	public XWeaponTrail leftbladetrail;

	public XWeaponTrail rightbladetrail;

	public XWeaponTrail leftbladetrail2;

	public XWeaponTrail rightbladetrail2;

	private Transform handL;

	private Transform handR;

	private Transform forearmL;

	private Transform forearmR;

	private Transform upperarmL;

	private Transform upperarmR;

	public GROUP myGroup;

	public int myTeam = 1;

	private bool leftGunHasBullet = true;

	private bool rightGunHasBullet = true;

	private int leftBulletLeft = 7;

	private int rightBulletLeft = 7;

	private int bulletMAX = 7;

	public GameObject hookRefL1;

	public GameObject hookRefR1;

	public GameObject hookRefL2;

	public GameObject hookRefR2;

	public float currentSpeed;

	private Quaternion targetRotation;

	private HERO_STATE _state;

	public HERO_SETUP setup;

	private GameObject skillCD;

	public float skillCDLast;

	public float skillCDDuration;

	public GameObject bulletLeft;

	public GameObject bulletRight;

	public float speed = 10f;

	private float gravity = 20f;

	public float maxVelocityChange = 10f;

	public bool canJump = true;

	public float jumpHeight = 2f;

	private bool grounded;

	private float facingDirection;

	private bool justGrounded;

	private bool isLaunchLeft;

	private bool isLaunchRight;

	private Vector3 launchForce;

	private bool buttonAttackRelease;

	private bool attackReleased;

	private string attackAnimation;

	public GameObject checkBoxLeft;

	public GameObject checkBoxRight;

	public AudioSource slash;

	public AudioSource slashHit;

	public AudioSource rope;

	public AudioSource meatDie;

	public AudioSource audio_hitwall;

	public AudioSource audio_ally;

	private bool QHold;

	private bool EHold;

	public Transform lastHook;

	private bool hasDied;

	private Vector3 dashDirection;

	private int attackLoop;

	private bool attackMove;

	public float myScale = 1f;

	private float wallRunTime;

	private bool wallJump;

	public float totalGas = 100f;

	private float currentGas = 100f;

	private float useGasSpeed = 0.2f;

	public float totalBladeSta = 100f;

	private float currentBladeSta = 100f;

	private int totalBladeNum = 5;

	private int currentBladeNum = 5;

	private bool throwedBlades;

	private float flare1CD;

	private float flare2CD;

	private float flare3CD;

	private float flareTotalCD = 5f;

	private GameObject myNetWorkName;

	private int escapeTimes = 1;

	private GameObject eren_titan;

	private float buffTime;

	private BUFF currentBuff;

	private bool rightArmAim;

	private bool leftArmAim;

	private GameObject gunDummy;

	private GameObject titanWhoGrabMe;

	private int titanWhoGrabMeID;

	public string currentAnimation;

	private float uTapTime = -1f;

	private float dTapTime = -1f;

	private float lTapTime = -1f;

	private float rTapTime = -1f;

	private bool dashU;

	private bool dashD;

	private bool dashL;

	private bool dashR;

	public bool titanForm;

	private bool leanLeft;

	private bool bigLean;

	private bool needLean;

	private bool almostSingleHook;

	private string reloadAnimation = string.Empty;

	private float dashTime;

	private Vector3 dashV;

	private float originVM;

	private bool isLeftHandHooked;

	private bool isRightHandHooked;

	private GameObject hookTarget;

	private GameObject badGuy;

	private bool hookSomeOne;

	private bool hookBySomeOne = true;

	private Vector3 launchPointLeft;

	private Vector3 launchPointRight;

	private float launchElapsedTimeL;

	private float launchElapsedTimeR;

	private Quaternion oldHeadRotation;

	private Quaternion targetHeadRotation;

	private Vector3 gunTarget;

	public bool AlwaysTrue;

	public bool isGrabbed
	{
		get
		{
			return state == HERO_STATE.Grab;
		}
	}

	private HERO_STATE state
	{
		get
		{
			return _state;
		}
		set
		{
			if (_state == HERO_STATE.AirDodge || _state == HERO_STATE.GroundDodge)
			{
				dashTime = 0f;
			}
			UnityEngine.MonoBehaviour.print(string.Concat("state = ", value, " "));
			_state = value;
		}
	}

	public bool isInvincible()
	{
		return invincible > 0f;
	}

	private void Awake()
	{
		setup = base.gameObject.GetComponent<HERO_SETUP>();
		base.rigidbody.freezeRotation = true;
		base.rigidbody.useGravity = false;
		handL = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
		handR = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
		forearmL = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L");
		forearmR = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R");
		upperarmL = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L");
		upperarmR = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
	}

	private void OnDestroy()
	{
		if (myNetWorkName != null)
		{
			UnityEngine.Object.Destroy(myNetWorkName);
		}
		if (gunDummy != null)
		{
			UnityEngine.Object.Destroy(gunDummy);
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
		{
			releaseIfIHookSb();
		}
		if (GameObject.Find("MultiplayerManager") != null)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().removeHero(this);
		}
	}

	private void Start()
	{
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().addHero(this);
		if (LevelInfo.getInfo(FengGameManagerMKII.level).horse && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			myHorse = PhotonNetwork.Instantiate("horse", base.transform.position + Vector3.up * 5f, base.transform.rotation, 0);
			myHorse.GetComponent<Horse>().myHero = base.gameObject;
		}
		bool flag = false;
		if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
		{
			if (Application.srcValue != "aog1.unity3d")
			{
				flag = true;
			}
			if (string.Compare(Application.absoluteURL, "http://fenglee.com/game/aog/aog1.unity3d", true) != 0)
			{
				flag = true;
			}
			if (flag)
			{
				Application.ExternalEval("top.location.href=\"http://fenglee.com/\";");
				UnityEngine.Object.Destroy(base.gameObject);
			}
			Application.ExternalEval("if(window != window.top) {document.location='http://fenglee.com/game/aog/playhere.html'}");
		}
		sparks = base.transform.Find("slideSparks").GetComponent<ParticleSystem>();
		smoke_3dmg = base.transform.Find("3dmg_smoke").GetComponent<ParticleSystem>();
		base.transform.localScale = new Vector3(myScale, myScale, myScale);
		facingDirection = base.transform.rotation.eulerAngles.y;
		targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
		smoke_3dmg.enableEmission = false;
		sparks.enableEmission = false;
		speedFXPS = speedFX1.GetComponent<ParticleSystem>();
		speedFXPS.enableEmission = false;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
		{
			GameObject gameObject = GameObject.Find("UI_IN_GAME");
			myNetWorkName = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI/LabelNameOverHead"));
			myNetWorkName.name = "LabelNameOverHead";
			myNetWorkName.transform.parent = gameObject.GetComponent<UIReferArray>().panels[0].transform;
			myNetWorkName.transform.localScale = new Vector3(14f, 14f, 14f);
			myNetWorkName.GetComponent<UILabel>().text = string.Empty;
			if ((int)base.photonView.owner.customProperties[PhotonPlayerProperty.team] == 2)
			{
				myNetWorkName.GetComponent<UILabel>().text = "[FF0000]AHSS\n[FFFFFF]";
			}
			string text = (string)base.photonView.owner.customProperties[PhotonPlayerProperty.guildName];
			if (text != string.Empty)
			{
				UILabel component = myNetWorkName.GetComponent<UILabel>();
				string text2 = component.text;
				component.text = text2 + "[FFFF00]" + text + "\n[FFFFFF]" + (string)base.photonView.owner.customProperties[PhotonPlayerProperty.name];
			}
			else
			{
				myNetWorkName.GetComponent<UILabel>().text += (string)base.photonView.owner.customProperties[PhotonPlayerProperty.name];
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			base.gameObject.layer = LayerMask.NameToLayer("NetworkObject");
			if (IN_GAME_MAIN_CAMERA.dayLight == DayLight.Night)
			{
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("flashlight"));
				gameObject2.transform.parent = base.transform;
				gameObject2.transform.position = base.transform.position + Vector3.up;
				gameObject2.transform.rotation = Quaternion.Euler(353f, 0f, 0f);
			}
			setup.init();
			setup.myCostume = new HeroCostume();
			setup.myCostume = CostumeConeveter.PhotonDataToHeroCostume(base.photonView.owner);
			setup.setCharacterComponent();
			UnityEngine.Object.Destroy(checkBoxLeft);
			UnityEngine.Object.Destroy(checkBoxRight);
			UnityEngine.Object.Destroy(leftbladetrail);
			UnityEngine.Object.Destroy(rightbladetrail);
			UnityEngine.Object.Destroy(leftbladetrail2);
			UnityEngine.Object.Destroy(rightbladetrail2);
		}
		else
		{
			currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
			inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
		}
	}

	public void setStat()
	{
		skillCDLast = 1.5f;
		skillId = setup.myCostume.stat.skillId;
		if (skillId == "levi")
		{
			skillCDLast = 3.5f;
		}
		customAnimationSpeed();
		if (skillId == "armin")
		{
			skillCDLast = 5f;
		}
		if (skillId == "marco")
		{
			skillCDLast = 10f;
		}
		if (skillId == "jean")
		{
			skillCDLast = 0.001f;
		}
		if (skillId == "eren")
		{
			skillCDLast = 120f;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
			{
				if (LevelInfo.getInfo(FengGameManagerMKII.level).teamTitan || LevelInfo.getInfo(FengGameManagerMKII.level).type == GAMEMODE.RACING || LevelInfo.getInfo(FengGameManagerMKII.level).type == GAMEMODE.PVP_CAPTURE || LevelInfo.getInfo(FengGameManagerMKII.level).type == GAMEMODE.TROST)
				{
					skillId = "petra";
					skillCDLast = 1f;
				}
				else
				{
					int num = 0;
					PhotonPlayer[] playerList = PhotonNetwork.playerList;
					foreach (PhotonPlayer photonPlayer in playerList)
					{
						if ((int)photonPlayer.customProperties[PhotonPlayerProperty.isTitan] == 1 && ((string)photonPlayer.customProperties[PhotonPlayerProperty.character]).ToUpper() == "EREN")
						{
							num++;
						}
					}
					if (num > 1)
					{
						skillId = "petra";
						skillCDLast = 1f;
					}
				}
			}
		}
		if (skillId == "sasha")
		{
			skillCDLast = 20f;
		}
		if (skillId == "petra")
		{
			skillCDLast = 3.5f;
		}
		skillCDDuration = skillCDLast;
		speed = (float)setup.myCostume.stat.SPD / 10f;
		totalGas = (currentGas = setup.myCostume.stat.GAS);
		totalBladeSta = (currentBladeSta = setup.myCostume.stat.BLA);
		base.rigidbody.mass = 0.5f - (float)(setup.myCostume.stat.ACL - 100) * 0.001f;
		GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (float)(-Screen.height) * 0.5f + 5f, 0f);
		skillCD = GameObject.Find("skill_cd_" + skillId);
		skillCD.transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
		GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
		{
			GameObject.Find("bulletL").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL1").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR1").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL2").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR2").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL3").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR3").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL4").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR4").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL5").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR5").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL6").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR6").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletL7").GetComponent<UISprite>().enabled = false;
			GameObject.Find("bulletR7").GetComponent<UISprite>().enabled = false;
		}
		if (setup.myCostume.uniform_type == UNIFORM_TYPE.CasualAHSS)
		{
			standAnimation = "AHSS_stand_gun";
			useGun = true;
			gunDummy = new GameObject();
			gunDummy.name = "gunDummy";
			gunDummy.transform.position = base.transform.position;
			gunDummy.transform.rotation = base.transform.rotation;
			myGroup = GROUP.A;
			setTeam(2);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
			{
				GameObject.Find("bladeCL").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladeCR").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladel1").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader1").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladel2").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader2").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladel3").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader3").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladel4").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader4").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bladel5").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader5").GetComponent<UISprite>().enabled = false;
				GameObject.Find("bulletL").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL1").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR1").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL2").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR2").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL3").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR3").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL4").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR4").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL5").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR5").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL6").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR6").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletL7").GetComponent<UISprite>().enabled = true;
				GameObject.Find("bulletR7").GetComponent<UISprite>().enabled = true;
				skillCD.transform.localPosition = Vector3.up * 5000f;
			}
		}
		else if (setup.myCostume.sex == SEX.FEMALE)
		{
			standAnimation = "stand";
			setTeam(1);
		}
		else
		{
			standAnimation = "stand_levi";
			setTeam(1);
		}
	}

	public void setTeam(int team)
	{
		setMyTeam(team);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			base.photonView.RPC("setMyTeam", PhotonTargets.OthersBuffered, team);
			PhotonNetwork.player.SetCustomProperties(new Hashtable { 
			{
				PhotonPlayerProperty.team,
				team
			} });
		}
	}

	[RPC]
	private void setMyTeam(int val)
	{
		UnityEngine.MonoBehaviour.print("set team " + val);
		myTeam = val;
		checkBoxLeft.GetComponent<TriggerColliderWeapon>().myTeam = val;
		checkBoxRight.GetComponent<TriggerColliderWeapon>().myTeam = val;
	}

	public void setSkillHUDPosition()
	{
		skillCD = GameObject.Find("skill_cd_" + skillId);
		if (skillCD != null)
		{
			skillCD.transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
		}
		if (useGun)
		{
			skillCD.transform.localPosition = Vector3.up * 5000f;
		}
	}

	private void updateLeftMagUI()
	{
		for (int i = 1; i <= bulletMAX; i++)
		{
			GameObject.Find("bulletL" + i).GetComponent<UISprite>().enabled = false;
		}
		for (int j = 1; j <= leftBulletLeft; j++)
		{
			GameObject.Find("bulletL" + j).GetComponent<UISprite>().enabled = true;
		}
	}

	private void updateRightMagUI()
	{
		for (int i = 1; i <= bulletMAX; i++)
		{
			GameObject.Find("bulletR" + i).GetComponent<UISprite>().enabled = false;
		}
		for (int j = 1; j <= rightBulletLeft; j++)
		{
			GameObject.Find("bulletR" + j).GetComponent<UISprite>().enabled = true;
		}
	}

	public bool IsGrounded()
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		return Physics.Raycast(base.gameObject.transform.position + Vector3.up * 0.1f, -Vector3.up, 0.3f, layerMask3.value);
	}

	private bool IsFrontGrounded()
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		return Physics.Raycast(base.gameObject.transform.position + base.gameObject.transform.up * 1f, base.gameObject.transform.forward, 1f, layerMask3.value);
	}

	private bool IsUpFrontGrounded()
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		return Physics.Raycast(base.gameObject.transform.position + base.gameObject.transform.up * 3f, base.gameObject.transform.forward, 1.2f, layerMask3.value);
	}

	public void grabbed(GameObject titan, bool leftHand)
	{
		if (isMounted)
		{
			unmounted();
		}
		state = HERO_STATE.Grab;
		GetComponent<CapsuleCollider>().isTrigger = true;
		falseAttack();
		titanWhoGrabMe = titan;
		if (titanForm && eren_titan != null)
		{
			eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
		}
		if (!useGun && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine))
		{
			leftbladetrail.Deactivate();
			rightbladetrail.Deactivate();
			leftbladetrail2.Deactivate();
			rightbladetrail2.Deactivate();
		}
		smoke_3dmg.enableEmission = false;
		sparks.enableEmission = false;
	}

	[RPC]
	private void netGrabbed(int id, bool leftHand)
	{
		titanWhoGrabMeID = id;
		grabbed(PhotonView.Find(id).gameObject, leftHand);
	}

	public void ungrabbed()
	{
		facingDirection = 0f;
		targetRotation = Quaternion.Euler(0f, 0f, 0f);
		base.transform.parent = null;
		GetComponent<CapsuleCollider>().isTrigger = false;
		state = HERO_STATE.Idle;
	}

	[RPC]
	private void netSetIsGrabbedFalse()
	{
		state = HERO_STATE.Idle;
	}

	[RPC]
	private void netUngrabbed()
	{
		ungrabbed();
		netPlayAnimation(standAnimation);
		falseAttack();
	}

	private void escapeFromGrab()
	{
	}

	public void attackAccordingToMouse()
	{
		if ((double)Input.mousePosition.x < (double)Screen.width * 0.5)
		{
			attackAnimation = "attack2";
		}
		else
		{
			attackAnimation = "attack1";
		}
	}

	public void attackAccordingToTarget(Transform a)
	{
		Vector3 vector = a.position - base.transform.position;
		float current = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
		float num = 0f - Mathf.DeltaAngle(current, base.transform.rotation.eulerAngles.y - 90f);
		if (Mathf.Abs(num) < 90f && vector.magnitude < 6f && a.position.y <= base.transform.position.y + 2f && a.position.y >= base.transform.position.y - 5f)
		{
			attackAnimation = "attack4";
		}
		else if (num > 0f)
		{
			attackAnimation = "attack1";
		}
		else
		{
			attackAnimation = "attack2";
		}
	}

	public void playAnimation(string aniName)
	{
		currentAnimation = aniName;
		base.animation.Play(aniName);
		if (PhotonNetwork.connected && base.photonView.isMine)
		{
			base.photonView.RPC("netPlayAnimation", PhotonTargets.Others, aniName);
		}
	}

	private void playAnimationAt(string aniName, float normalizedTime)
	{
		currentAnimation = aniName;
		base.animation.Play(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
		if (PhotonNetwork.connected && base.photonView.isMine)
		{
			base.photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, aniName, normalizedTime);
		}
	}

	public void crossFade(string aniName, float time)
	{
		currentAnimation = aniName;
		base.animation.CrossFade(aniName, time);
		if (PhotonNetwork.connected && base.photonView.isMine)
		{
			base.photonView.RPC("netCrossFade", PhotonTargets.Others, aniName, time);
		}
	}

	[RPC]
	private void netPlayAnimation(string aniName)
	{
		currentAnimation = aniName;
		if (base.animation != null)
		{
			base.animation.Play(aniName);
		}
	}

	[RPC]
	private void netPlayAnimationAt(string aniName, float normalizedTime)
	{
		currentAnimation = aniName;
		if (base.animation != null)
		{
			base.animation.Play(aniName);
			base.animation[aniName].normalizedTime = normalizedTime;
		}
	}

	[RPC]
	private void netCrossFade(string aniName, float time)
	{
		currentAnimation = aniName;
		if (base.animation != null)
		{
			base.animation.CrossFade(aniName, time);
		}
	}

	private GameObject findNearestTitan()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
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

	private void erenTransform()
	{
		skillCDDuration = skillCDLast;
		if ((bool)bulletLeft)
		{
			bulletLeft.GetComponent<Bullet>().removeMe();
		}
		if ((bool)bulletRight)
		{
			bulletRight.GetComponent<Bullet>().removeMe();
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			eren_titan = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("TITAN_EREN"), base.transform.position, base.transform.rotation);
		}
		else
		{
			eren_titan = PhotonNetwork.Instantiate("TITAN_EREN", base.transform.position, base.transform.rotation, 0);
		}
		eren_titan.GetComponent<TITAN_EREN>().realBody = base.gameObject;
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().flashBlind();
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(eren_titan);
		eren_titan.GetComponent<TITAN_EREN>().born();
		eren_titan.rigidbody.velocity = base.rigidbody.velocity;
		base.rigidbody.velocity = Vector3.zero;
		base.transform.position = eren_titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
		titanForm = true;
		if (IN_GAME_MAIN_CAMERA.gametype != 0)
		{
			base.photonView.RPC("whoIsMyErenTitan", PhotonTargets.Others, eren_titan.GetPhotonView().viewID);
		}
		if (smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			base.photonView.RPC("net3DMGSMOKE", PhotonTargets.Others, false);
		}
		smoke_3dmg.enableEmission = false;
	}

	public void backToHuman()
	{
		base.gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
		base.rigidbody.velocity = Vector3.zero;
		titanForm = false;
		ungrabbed();
		falseAttack();
		skillCDDuration = skillCDLast;
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(base.gameObject);
		if (IN_GAME_MAIN_CAMERA.gametype != 0)
		{
			base.photonView.RPC("backToHumanRPC", PhotonTargets.Others);
		}
	}

	[RPC]
	private void backToHumanRPC()
	{
		titanForm = false;
		eren_titan = null;
		base.gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
	}

	[RPC]
	private void whoIsMyErenTitan(int id)
	{
		eren_titan = PhotonView.Find(id).gameObject;
		titanForm = true;
	}

	private void checkDashDoubleTap()
	{
		if (uTapTime >= 0f)
		{
			uTapTime += Time.deltaTime;
			if (uTapTime > 0.2f)
			{
				uTapTime = -1f;
			}
		}
		if (dTapTime >= 0f)
		{
			dTapTime += Time.deltaTime;
			if (dTapTime > 0.2f)
			{
				dTapTime = -1f;
			}
		}
		if (lTapTime >= 0f)
		{
			lTapTime += Time.deltaTime;
			if (lTapTime > 0.2f)
			{
				lTapTime = -1f;
			}
		}
		if (rTapTime >= 0f)
		{
			rTapTime += Time.deltaTime;
			if (rTapTime > 0.2f)
			{
				rTapTime = -1f;
			}
		}
		if (inputManager.isInputDown[InputCode.up])
		{
			if (uTapTime == -1f)
			{
				uTapTime = 0f;
			}
			if (uTapTime != 0f)
			{
				dashU = true;
			}
		}
		if (inputManager.isInputDown[InputCode.down])
		{
			if (dTapTime == -1f)
			{
				dTapTime = 0f;
			}
			if (dTapTime != 0f)
			{
				dashD = true;
			}
		}
		if (inputManager.isInputDown[InputCode.left])
		{
			if (lTapTime == -1f)
			{
				lTapTime = 0f;
			}
			if (lTapTime != 0f)
			{
				dashL = true;
			}
		}
		if (inputManager.isInputDown[InputCode.right])
		{
			if (rTapTime == -1f)
			{
				rTapTime = 0f;
			}
			if (rTapTime != 0f)
			{
				dashR = true;
			}
		}
	}

	public void update()
	{
        AlwaysTrue = true;
		if (IN_GAME_MAIN_CAMERA.isPausing)
		{
			return;
		}
		if (invincible > 0f)
		{
			invincible -= Time.deltaTime;
		}
		if (hasDied)
		{
			return;
		}
		if (titanForm && eren_titan != null)
		{
			base.transform.position = eren_titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
			base.gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			return;
		}
		if (state == HERO_STATE.Grab && !useGun)
		{
			if (skillId == "jean")
			{
				if (state != HERO_STATE.Attack && (inputManager.isInputDown[InputCode.attack0] || inputManager.isInputDown[InputCode.attack1]) && escapeTimes > 0 && !base.animation.IsPlaying("grabbed_jean"))
				{
					playAnimation("grabbed_jean");
					base.animation["grabbed_jean"].time = 0f;
					escapeTimes--;
				}
				if (!base.animation.IsPlaying("grabbed_jean") || !(base.animation["grabbed_jean"].normalizedTime > 0.64f) || !titanWhoGrabMe.GetComponent<TITAN>())
				{
					return;
				}
				ungrabbed();
				base.rigidbody.velocity = Vector3.up * 30f;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
					return;
				}
				base.photonView.RPC("netSetIsGrabbedFalse", PhotonTargets.All);
				if (PhotonNetwork.isMasterClient)
				{
					titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
				}
				else
				{
					PhotonView.Find(titanWhoGrabMeID).RPC("grabbedTargetEscape", PhotonTargets.MasterClient);
				}
			}
			else
			{
				if (!(skillId == "eren"))
				{
					return;
				}
				showSkillCD();
				if (IN_GAME_MAIN_CAMERA.gametype != 0 || (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE && !IN_GAME_MAIN_CAMERA.isPausing))
				{
					calcSkillCD();
					calcFlareCD();
				}
				if (!inputManager.isInputDown[InputCode.attack1])
				{
					return;
				}
				bool flag = false;
				if (skillCDDuration > 0f || flag)
				{
					flag = true;
					return;
				}
				skillCDDuration = skillCDLast;
				if (!(skillId == "eren") || !titanWhoGrabMe.GetComponent<TITAN>())
				{
					return;
				}
				ungrabbed();
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
				}
				else
				{
					base.photonView.RPC("netSetIsGrabbedFalse", PhotonTargets.All);
					if (PhotonNetwork.isMasterClient)
					{
						titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
					}
					else
					{
						PhotonView.Find(titanWhoGrabMeID).photonView.RPC("grabbedTargetEscape", PhotonTargets.MasterClient);
					}
				}
				erenTransform();
			}
		}
		else
		{
			if (titanForm)
			{
				return;
			}
			bufferUpdate();
			if (!grounded && state != HERO_STATE.AirDodge)
			{
				checkDashDoubleTap();
				if (dashD)
				{
					dashD = false;
					dash(0f, -1f);
					return;
				}
				if (dashU)
				{
					dashU = false;
					dash(0f, 1f);
					return;
				}
				if (dashL)
				{
					dashL = false;
					dash(-1f, 0f);
					return;
				}
				if (dashR)
				{
					dashR = false;
					dash(1f, 0f);
					return;
				}
			}
			if (grounded && (state == HERO_STATE.Idle || state == HERO_STATE.Slide))
			{
				if (inputManager.isInputDown[InputCode.jump] && !base.animation.IsPlaying("jump") && !base.animation.IsPlaying("horse_geton"))
				{
					idle();
					crossFade("jump", 0.1f);
					sparks.enableEmission = false;
				}
				if (inputManager.isInputDown[InputCode.dodge] && !base.animation.IsPlaying("jump") && !base.animation.IsPlaying("horse_geton"))
				{
					dodge();
					return;
				}
			}
			if (state == HERO_STATE.Idle)
			{
				if (inputManager.isInputDown[InputCode.flare1])
				{
					shootFlare(1);
				}
				if (inputManager.isInputDown[InputCode.flare2])
				{
					shootFlare(2);
				}
				if (inputManager.isInputDown[InputCode.flare3])
				{
					shootFlare(3);
				}
				if (inputManager.isInputDown[InputCode.restart])
				{
					suicide();
				}
				if (myHorse != null && isMounted && inputManager.isInputDown[InputCode.dodge])
				{
					getOffHorse();
				}
				if ((base.animation.IsPlaying(standAnimation) || !grounded) && inputManager.isInputDown[InputCode.reload])
				{
					changeBlade();
					return;
				}
				if (base.animation.IsPlaying(standAnimation) && inputManager.isInputDown[InputCode.salute])
				{
					salute();
					return;
				}
				if (!isMounted && (inputManager.isInputDown[InputCode.attack0] || inputManager.isInputDown[InputCode.attack1]) && !useGun)
				{
					bool flag2 = false;
					if (inputManager.isInputDown[InputCode.attack1])
					{
						if (skillCDDuration > 0f || flag2)
						{
							flag2 = true;
						}
						else
						{
							skillCDDuration = skillCDLast;
							if (skillId == "eren")
							{
								erenTransform();
								return;
							}
							if (skillId == "marco")
							{
								if (AlwaysTrue)
								{
									attackAnimation = ((UnityEngine.Random.Range(0, 2) != 0) ? "special_marco_1" : "special_marco_0");
									playAnimation(attackAnimation);
								}
								else
								{
									flag2 = true;
									skillCDDuration = 0f;
								}
							}
							else if (skillId == "armin")
							{
								if (AlwaysTrue)
								{
									attackAnimation = "special_armin";
									playAnimation("special_armin");
								}
								else
								{
									flag2 = true;
									skillCDDuration = 0f;
								}
							}
							else if (skillId == "sasha")
							{
								if (AlwaysTrue)
								{
									attackAnimation = "special_sasha";
									playAnimation("special_sasha");
									currentBuff = BUFF.SpeedUp;
									buffTime = 10f;
								}
								else
								{
									flag2 = true;
									skillCDDuration = 0f;
								}
							}
							else if (skillId == "mikasa")
							{
								attackAnimation = "attack3_1";
								playAnimation("attack3_1");
								base.rigidbody.velocity = Vector3.up * 10f;
							}
							else if (skillId == "levi")
							{
								attackAnimation = "attack5";
								playAnimation("attack5");
								base.rigidbody.velocity += Vector3.up * 5f;
								Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
								LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
								LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
								RaycastHit hitInfo;
								if (Physics.Raycast(ray, out hitInfo, 10000000f, ((LayerMask)((int)layerMask2 | (int)layerMask)).value))
								{
                                    Debug.Log(hitInfo.transform.name);
                                    if ((bool)bulletRight)
									{
										bulletRight.GetComponent<Bullet>().disable();
										releaseIfIHookSb();
									}
									dashDirection = hitInfo.point - base.transform.position;
									launchRightRope(hitInfo, true, 1);
									rope.Play();
								}
								facingDirection = Mathf.Atan2(dashDirection.x, dashDirection.z) * 57.29578f;
								targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
								attackLoop = 3;
							}
							else if (skillId == "petra")
							{
								attackAnimation = "special_petra";
								playAnimation("special_petra");
								base.rigidbody.velocity += Vector3.up * 5f;
								Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
								LayerMask layerMask3 = 1 << LayerMask.NameToLayer("Ground");
								LayerMask layerMask4 = 1 << LayerMask.NameToLayer("EnemyBox");
								RaycastHit hitInfo2;
								if (Physics.Raycast(ray2, out hitInfo2, 10000000f, ((LayerMask)((int)layerMask4 | (int)layerMask3)).value))
								{
                                    Debug.Log(hitInfo2.transform.name);
                                    if ((bool)bulletRight)
									{
										bulletRight.GetComponent<Bullet>().disable();
										releaseIfIHookSb();
									}
									if ((bool)bulletLeft)
									{
										bulletLeft.GetComponent<Bullet>().disable();
										releaseIfIHookSb();
									}
									dashDirection = hitInfo2.point - base.transform.position;
									launchLeftRope(hitInfo2, true);
									launchRightRope(hitInfo2, true);
									rope.Play();
								}
								facingDirection = Mathf.Atan2(dashDirection.x, dashDirection.z) * 57.29578f;
								targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
								attackLoop = 3;
							}
							else
							{
								if (needLean)
								{
									if (leanLeft)
									{
										attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_l1" : "attack1_hook_l2");
									}
									else
									{
										attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_r1" : "attack1_hook_r2");
									}
								}
								else
								{
									attackAnimation = "attack1";
								}
								playAnimation(attackAnimation);
							}
						}
					}
					else if (inputManager.isInputDown[InputCode.attack0])
					{
						if (needLean)
						{
							if (inputManager.isInput[InputCode.left])
							{
								attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_l1" : "attack1_hook_l2");
							}
							else if (inputManager.isInput[InputCode.right])
							{
								attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_r1" : "attack1_hook_r2");
							}
							else if (leanLeft)
							{
								attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_l1" : "attack1_hook_l2");
							}
							else
							{
								attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_r1" : "attack1_hook_r2");
							}
						}
						else if (inputManager.isInput[InputCode.left])
						{
							attackAnimation = "attack2";
						}
						else if (inputManager.isInput[InputCode.right])
						{
							attackAnimation = "attack1";
						}
						else if (lastHook != null)
						{
							if (lastHook.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck") != null)
							{
								attackAccordingToTarget(lastHook.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck"));
							}
							else
							{
								flag2 = true;
							}
						}
						else if ((bool)bulletLeft && bulletLeft.transform.parent != null)
						{
							Transform transform = bulletLeft.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
							if ((bool)transform)
							{
								attackAccordingToTarget(transform);
							}
							else
							{
								attackAccordingToMouse();
							}
						}
						else if ((bool)bulletRight && bulletRight.transform.parent != null)
						{
							Transform transform2 = bulletRight.transform.parent.transform.root.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
							if ((bool)transform2)
							{
								attackAccordingToTarget(transform2);
							}
							else
							{
								attackAccordingToMouse();
							}
						}
						else
						{
							GameObject gameObject = findNearestTitan();
							if ((bool)gameObject)
							{
								Transform transform3 = gameObject.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
								if ((bool)transform3)
								{
									attackAccordingToTarget(transform3);
								}
								else
								{
									attackAccordingToMouse();
								}
							}
							else
							{
								attackAccordingToMouse();
							}
						}
					}
					if (!flag2)
					{
						checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
						checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
						if (grounded)
						{
							base.rigidbody.AddForce(base.gameObject.transform.forward * 200f);
						}
						playAnimation(attackAnimation);
						base.animation[attackAnimation].time = 0f;
						buttonAttackRelease = false;
						state = HERO_STATE.Attack;
						if (grounded || attackAnimation == "attack3_1" || attackAnimation == "attack5" || attackAnimation == "special_petra")
						{
							attackReleased = true;
							buttonAttackRelease = true;
						}
						else
						{
							attackReleased = false;
						}
						sparks.enableEmission = false;
					}
				}
				if (useGun)
				{
					if (inputManager.isInput[InputCode.attack1])
					{
						leftArmAim = true;
						rightArmAim = true;
					}
					else if (inputManager.isInput[InputCode.attack0])
					{
						if (leftGunHasBullet)
						{
							leftArmAim = true;
							rightArmAim = false;
						}
						else
						{
							leftArmAim = false;
							if (rightGunHasBullet)
							{
								rightArmAim = true;
							}
							else
							{
								rightArmAim = false;
							}
						}
					}
					else
					{
						leftArmAim = false;
						rightArmAim = false;
					}
					if (leftArmAim || rightArmAim)
					{
						Ray ray3 = Camera.main.ScreenPointToRay(Input.mousePosition);
						LayerMask layerMask5 = 1 << LayerMask.NameToLayer("Ground");
						LayerMask layerMask6 = 1 << LayerMask.NameToLayer("EnemyBox");
						RaycastHit hitInfo3;
						if (Physics.Raycast(ray3, out hitInfo3, 10000000f, ((LayerMask)((int)layerMask6 | (int)layerMask5)).value))
						{
                            Debug.Log(hitInfo3.transform.name);
                            gunTarget = hitInfo3.point;
						}
					}
					bool flag3 = false;
					bool flag4 = false;
					bool flag5 = false;
					if (inputManager.isInputUp[InputCode.attack1])
					{
						if (leftGunHasBullet && rightGunHasBullet)
						{
							if (grounded)
							{
								attackAnimation = "AHSS_shoot_both";
							}
							else
							{
								attackAnimation = "AHSS_shoot_both_air";
							}
							flag3 = true;
						}
						else if (!leftGunHasBullet && !rightGunHasBullet)
						{
							flag4 = true;
						}
						else
						{
							flag5 = true;
						}
					}
					if (flag5 || inputManager.isInputUp[InputCode.attack0])
					{
						if (grounded)
						{
							if (leftGunHasBullet && rightGunHasBullet)
							{
								if (isLeftHandHooked)
								{
									attackAnimation = "AHSS_shoot_r";
								}
								else
								{
									attackAnimation = "AHSS_shoot_l";
								}
							}
							else if (leftGunHasBullet)
							{
								attackAnimation = "AHSS_shoot_l";
							}
							else if (rightGunHasBullet)
							{
								attackAnimation = "AHSS_shoot_r";
							}
						}
						else if (leftGunHasBullet && rightGunHasBullet)
						{
							if (isLeftHandHooked)
							{
								attackAnimation = "AHSS_shoot_r_air";
							}
							else
							{
								attackAnimation = "AHSS_shoot_l_air";
							}
						}
						else if (leftGunHasBullet)
						{
							attackAnimation = "AHSS_shoot_l_air";
						}
						else if (rightGunHasBullet)
						{
							attackAnimation = "AHSS_shoot_r_air";
						}
						if (leftGunHasBullet || rightGunHasBullet)
						{
							flag3 = true;
						}
						else
						{
							flag4 = true;
						}
					}
					if (flag3)
					{
						state = HERO_STATE.Attack;
						crossFade(attackAnimation, 0.05f);
						gunDummy.transform.position = base.transform.position;
						gunDummy.transform.rotation = base.transform.rotation;
						gunDummy.transform.LookAt(gunTarget);
						attackReleased = false;
						facingDirection = gunDummy.transform.rotation.eulerAngles.y;
						targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
					}
					else if (flag4 && (grounded || LevelInfo.getInfo(FengGameManagerMKII.level).type != GAMEMODE.PVP_AHSS))
					{
						changeBlade();
					}
				}
			}
			else if (state == HERO_STATE.Attack)
			{
				if (!useGun)
				{
					if (!inputManager.isInput[InputCode.attack0])
					{
						buttonAttackRelease = true;
					}
					if (!attackReleased)
					{
						if (buttonAttackRelease)
						{
							continueAnimation();
							attackReleased = true;
						}
						else if (base.animation[attackAnimation].normalizedTime >= 0.32f)
						{
							pauseAnimation();
						}
					}
					if (attackAnimation == "attack3_1" && currentBladeSta > 0f)
					{
						if (base.animation[attackAnimation].normalizedTime >= 0.8f)
						{
							if (!checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
							{
								checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
								leftbladetrail2.Activate();
								rightbladetrail2.Activate();
								if (QualitySettings.GetQualityLevel() >= 2)
								{
									leftbladetrail.Activate();
									rightbladetrail.Activate();
								}
								base.rigidbody.velocity = -Vector3.up * 30f;
							}
							if (!checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me)
							{
								checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
								slash.Play();
							}
						}
						else if (checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
						{
							checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
							checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
							checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
							checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
							leftbladetrail.StopSmoothly(0.1f);
							rightbladetrail.StopSmoothly(0.1f);
							leftbladetrail2.StopSmoothly(0.1f);
							rightbladetrail2.StopSmoothly(0.1f);
						}
					}
					else
					{
						float num2;
						float num;
						if (currentBladeSta == 0f)
						{
							num2 = (num = -1f);
						}
						else if (attackAnimation == "attack5")
						{
							num2 = 0.35f;
							num = 0.5f;
						}
						else if (attackAnimation == "special_petra")
						{
							num2 = 0.35f;
							num = 0.48f;
						}
						else if (attackAnimation == "special_armin")
						{
							num2 = 0.25f;
							num = 0.35f;
						}
						else if (attackAnimation == "attack4")
						{
							num2 = 0.6f;
							num = 0.9f;
						}
						else if (attackAnimation == "special_sasha")
						{
							num2 = (num = -1f);
						}
						else
						{
							num2 = 0.5f;
							num = 0.85f;
						}
						if (base.animation[attackAnimation].normalizedTime > num2 && base.animation[attackAnimation].normalizedTime < num)
						{
							if (!checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
							{
								checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = true;
								slash.Play();
								leftbladetrail2.Activate();
								rightbladetrail2.Activate();
								if (QualitySettings.GetQualityLevel() >= 2)
								{
									leftbladetrail.Activate();
									rightbladetrail.Activate();
								}
							}
							if (!checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me)
							{
								checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = true;
							}
						}
						else if (checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me)
						{
							checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
							checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
							checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
							checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
							leftbladetrail2.StopSmoothly(0.1f);
							rightbladetrail2.StopSmoothly(0.1f);
							if (QualitySettings.GetQualityLevel() >= 2)
							{
								leftbladetrail.StopSmoothly(0.1f);
								rightbladetrail.StopSmoothly(0.1f);
							}
						}
						if (attackLoop > 0 && base.animation[attackAnimation].normalizedTime > num)
						{
							attackLoop--;
							playAnimationAt(attackAnimation, num2);
						}
					}
					if (base.animation[attackAnimation].normalizedTime >= 1f)
					{
						if (attackAnimation == "special_marco_0" || attackAnimation == "special_marco_1")
						{
							if (IN_GAME_MAIN_CAMERA.gametype != 0)
							{
								if (!PhotonNetwork.isMasterClient)
								{
									base.photonView.RPC("netTauntAttack", PhotonTargets.MasterClient, 5f, 100f);
								}
								else
								{
									netTauntAttack(5f);
								}
							}
							else
							{
								netTauntAttack(5f);
							}
							falseAttack();
							idle();
						}
						else if (attackAnimation == "special_armin")
						{
							if (IN_GAME_MAIN_CAMERA.gametype != 0)
							{
								if (!PhotonNetwork.isMasterClient)
								{
									base.photonView.RPC("netlaughAttack", PhotonTargets.MasterClient);
								}
								else
								{
									netlaughAttack();
								}
							}
							else
							{
								GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
								GameObject[] array2 = array;
								foreach (GameObject gameObject2 in array2)
								{
									if (Vector3.Distance(gameObject2.transform.position, base.transform.position) < 50f && Vector3.Angle(gameObject2.transform.forward, base.transform.position - gameObject2.transform.position) < 90f && (bool)gameObject2.GetComponent<TITAN>())
									{
										gameObject2.GetComponent<TITAN>().beLaughAttacked();
									}
								}
							}
							falseAttack();
							idle();
						}
						else if (attackAnimation == "attack3_1")
						{
							base.rigidbody.velocity -= Vector3.up * Time.deltaTime * 30f;
						}
						else
						{
							falseAttack();
							idle();
						}
					}
					if (base.animation.IsPlaying("attack3_2") && base.animation["attack3_2"].normalizedTime >= 1f)
					{
						falseAttack();
						idle();
					}
				}
				else
				{
					base.transform.rotation = Quaternion.Lerp(base.transform.rotation, gunDummy.transform.rotation, Time.deltaTime * 30f);
					UnityEngine.MonoBehaviour.print(attackAnimation);
					if (!attackReleased && base.animation[attackAnimation].normalizedTime > 0.167f)
					{
						attackReleased = true;
						bool flag6 = false;
						if (attackAnimation == "AHSS_shoot_both" || attackAnimation == "AHSS_shoot_both_air")
						{
							flag6 = true;
							leftGunHasBullet = false;
							rightGunHasBullet = false;
							base.rigidbody.AddForce(-base.transform.forward * 1000f, ForceMode.Acceleration);
						}
						else
						{
							if (attackAnimation == "AHSS_shoot_l" || attackAnimation == "AHSS_shoot_l_air")
							{
								leftGunHasBullet = false;
							}
							else
							{
								rightGunHasBullet = false;
							}
							base.rigidbody.AddForce(-base.transform.forward * 600f, ForceMode.Acceleration);
						}
						base.rigidbody.AddForce(Vector3.up * 200f, ForceMode.Acceleration);
						string text = "FX/shotGun";
						if (flag6)
						{
							text = "FX/shotGun 1";
						}
						if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
						{
							GameObject gameObject3 = PhotonNetwork.Instantiate(text, base.transform.position + base.transform.up * 0.8f - base.transform.right * 0.1f, base.transform.rotation, 0);
							if ((bool)gameObject3.GetComponent<EnemyfxIDcontainer>())
							{
								gameObject3.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = base.photonView.viewID;
							}
						}
						else
						{
							GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(text), base.transform.position + base.transform.up * 0.8f - base.transform.right * 0.1f, base.transform.rotation);
						}
					}
					if (base.animation[attackAnimation].normalizedTime >= 1f)
					{
						falseAttack();
						idle();
					}
					if (!base.animation.IsPlaying(attackAnimation))
					{
						falseAttack();
						idle();
					}
				}
			}
			else if (state == HERO_STATE.ChangeBlade)
			{
				if (useGun)
				{
					if (base.animation[reloadAnimation].normalizedTime > 0.22f)
					{
						if (!leftGunHasBullet && setup.part_blade_l.activeSelf)
						{
							setup.part_blade_l.SetActive(false);
							Transform transform4 = setup.part_blade_l.transform;
							GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_l"), transform4.position, transform4.rotation);
							gameObject4.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
							Vector3 force = -base.transform.forward * 10f + base.transform.up * 5f - base.transform.right;
							gameObject4.rigidbody.AddForce(force, ForceMode.Impulse);
							Vector3 torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
							gameObject4.rigidbody.AddTorque(torque, ForceMode.Acceleration);
						}
						if (!rightGunHasBullet && setup.part_blade_r.activeSelf)
						{
							setup.part_blade_r.SetActive(false);
							Transform transform5 = setup.part_blade_r.transform;
							GameObject gameObject5 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_r"), transform5.position, transform5.rotation);
							gameObject5.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
							Vector3 force2 = -base.transform.forward * 10f + base.transform.up * 5f + base.transform.right;
							gameObject5.rigidbody.AddForce(force2, ForceMode.Impulse);
							Vector3 torque2 = new Vector3(UnityEngine.Random.Range(-300, 300), UnityEngine.Random.Range(-300, 300), UnityEngine.Random.Range(-300, 300));
							gameObject5.rigidbody.AddTorque(torque2, ForceMode.Acceleration);
						}
					}
					if (base.animation[reloadAnimation].normalizedTime > 0.62f && !throwedBlades)
					{
						throwedBlades = true;
						if (leftBulletLeft > 0 && !leftGunHasBullet)
						{
							leftBulletLeft--;
							setup.part_blade_l.SetActive(true);
							leftGunHasBullet = true;
						}
						if (rightBulletLeft > 0 && !rightGunHasBullet)
						{
							setup.part_blade_r.SetActive(true);
							rightBulletLeft--;
							rightGunHasBullet = true;
						}
						updateRightMagUI();
						updateLeftMagUI();
					}
					if (base.animation[reloadAnimation].normalizedTime > 1f)
					{
						idle();
					}
				}
				else
				{
					if (!grounded)
					{
						if (base.animation[reloadAnimation].normalizedTime >= 0.2f && !throwedBlades)
						{
							throwedBlades = true;
							if (setup.part_blade_l.activeSelf)
							{
								throwBlades();
							}
						}
						if (base.animation[reloadAnimation].normalizedTime >= 0.56f && currentBladeNum > 0)
						{
							setup.part_blade_l.SetActive(true);
							setup.part_blade_r.SetActive(true);
							currentBladeSta = totalBladeSta;
						}
					}
					else
					{
						if (base.animation[reloadAnimation].normalizedTime >= 0.13f && !throwedBlades)
						{
							throwedBlades = true;
							if (setup.part_blade_l.activeSelf)
							{
								throwBlades();
							}
						}
						if (base.animation[reloadAnimation].normalizedTime >= 0.37f && currentBladeNum > 0)
						{
							setup.part_blade_l.SetActive(true);
							setup.part_blade_r.SetActive(true);
							currentBladeSta = totalBladeSta;
						}
					}
					if (base.animation[reloadAnimation].normalizedTime >= 1f)
					{
						idle();
					}
				}
			}
			else if (state == HERO_STATE.Salute)
			{
				if (base.animation["salute"].normalizedTime >= 1f)
				{
					idle();
				}
			}
			else if (state == HERO_STATE.GroundDodge)
			{
				if (base.animation.IsPlaying("dodge"))
				{
					if (!grounded && base.animation["dodge"].normalizedTime > 0.6f)
					{
						idle();
					}
					if (base.animation["dodge"].normalizedTime >= 1f)
					{
						idle();
					}
				}
			}
			else if (state == HERO_STATE.Land)
			{
				if (base.animation.IsPlaying("dash_land") && base.animation["dash_land"].normalizedTime >= 1f)
				{
					idle();
				}
			}
			else if (state == HERO_STATE.FillGas)
			{
				if (base.animation.IsPlaying("supply") && base.animation["supply"].normalizedTime >= 1f)
				{
					currentBladeSta = totalBladeSta;
					currentBladeNum = totalBladeNum;
					currentGas = totalGas;
					if (!useGun)
					{
						setup.part_blade_l.SetActive(true);
						setup.part_blade_r.SetActive(true);
					}
					else
					{
						leftBulletLeft = (rightBulletLeft = bulletMAX);
						leftGunHasBullet = (rightGunHasBullet = true);
						setup.part_blade_l.SetActive(true);
						setup.part_blade_r.SetActive(true);
						updateRightMagUI();
						updateLeftMagUI();
					}
					idle();
				}
			}
			else if (state == HERO_STATE.Slide)
			{
				if (!grounded)
				{
					idle();
				}
			}
			else if (state == HERO_STATE.AirDodge)
			{
				if (dashTime > 0f)
				{
					dashTime -= Time.deltaTime;
					if (currentSpeed > originVM)
					{
						base.rigidbody.AddForce(-base.rigidbody.velocity * Time.deltaTime * 1.7f, ForceMode.VelocityChange);
					}
				}
				else
				{
					dashTime = 0f;
					idle();
				}
			}
			if (inputManager.isInput[InputCode.leftRope] && !base.animation.IsPlaying("attack3_1") && !base.animation.IsPlaying("attack5") && !base.animation.IsPlaying("special_petra") && state != HERO_STATE.ChangeBlade && state != HERO_STATE.Grab)
			{
				if ((bool)bulletLeft)
				{
					QHold = true;
				}
				else
				{
					Ray ray4 = Camera.main.ScreenPointToRay(Input.mousePosition);
					LayerMask layerMask7 = 1 << LayerMask.NameToLayer("Ground");
					LayerMask layerMask8 = 1 << LayerMask.NameToLayer("EnemyBox");
					RaycastHit hitInfo4;
					if (Physics.Raycast(ray4, out hitInfo4, 10000f, ((LayerMask)((int)layerMask8 | (int)layerMask7)).value))
					{
                        Debug.Log(hitInfo4.transform.name);
                        launchLeftRope(hitInfo4, true);
						rope.Play();
					}
				}
			}
			else
			{
				QHold = false;
			}
			if (inputManager.isInput[InputCode.rightRope] && ((!base.animation.IsPlaying("attack3_1") && !base.animation.IsPlaying("attack5") && !base.animation.IsPlaying("special_petra")) || state == HERO_STATE.Idle))
			{
				if ((bool)bulletRight)
				{
					EHold = true;
				}
				else
				{
					Ray ray5 = Camera.main.ScreenPointToRay(Input.mousePosition);
					LayerMask layerMask9 = 1 << LayerMask.NameToLayer("Ground");
					LayerMask layerMask10 = 1 << LayerMask.NameToLayer("EnemyBox");
					RaycastHit hitInfo5;
					if (Physics.Raycast(ray5, out hitInfo5, 10000f, ((LayerMask)((int)layerMask10 | (int)layerMask9)).value))
					{
                        Debug.Log(hitInfo5.transform.name);
                        launchRightRope(hitInfo5, true);
						rope.Play();
					}
				}
			}
			else
			{
				EHold = false;
			}
			if (inputManager.isInput[InputCode.bothRope] && ((!base.animation.IsPlaying("attack3_1") && !base.animation.IsPlaying("attack5") && !base.animation.IsPlaying("special_petra")) || state == HERO_STATE.Idle))
			{
				QHold = true;
				EHold = true;
				if (!bulletLeft && !bulletRight)
				{
					Ray ray6 = Camera.main.ScreenPointToRay(Input.mousePosition);
					LayerMask layerMask11 = 1 << LayerMask.NameToLayer("Ground");
					LayerMask layerMask12 = 1 << LayerMask.NameToLayer("EnemyBox");
					RaycastHit hitInfo6;
					if (Physics.Raycast(ray6, out hitInfo6, 1000000f, ((LayerMask)((int)layerMask12 | (int)layerMask11)).value))
					{
						Debug.Log(hitInfo6.transform.name);
						launchLeftRope(hitInfo6, false);
						launchRightRope(hitInfo6, false);
						rope.Play();
					}
				}
			}
			if (IN_GAME_MAIN_CAMERA.gametype != 0 || (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE && !IN_GAME_MAIN_CAMERA.isPausing))
			{
				calcSkillCD();
				calcFlareCD();
			}
			if (!useGun)
			{
				if (leftbladetrail.gameObject.GetActive())
				{
					leftbladetrail.update();
					rightbladetrail.update();
				}
				if (leftbladetrail2.gameObject.GetActive())
				{
					leftbladetrail2.update();
					rightbladetrail2.update();
				}
				if (leftbladetrail.gameObject.GetActive())
				{
					leftbladetrail.lateUpdate();
					rightbladetrail.lateUpdate();
				}
				if (leftbladetrail2.gameObject.GetActive())
				{
					leftbladetrail2.lateUpdate();
					rightbladetrail2.lateUpdate();
				}
			}
			if (!IN_GAME_MAIN_CAMERA.isPausing)
			{
				showSkillCD();
				showFlareCD();
				showGas();
				showAimUI();
			}
		}
	}

	public string getDebugInfo()
	{
		string text = "\n";
		text = "Left:" + isLeftHandHooked + " ";
		if (isLeftHandHooked && bulletLeft != null)
		{
			Vector3 vector = bulletLeft.transform.position - base.transform.position;
			text += (int)(Mathf.Atan2(vector.x, vector.z) * 57.29578f);
		}
		string text2 = text;
		text = text2 + "\nRight:" + isRightHandHooked + " ";
		if (isRightHandHooked && bulletRight != null)
		{
			Vector3 vector2 = bulletRight.transform.position - base.transform.position;
			text += (int)(Mathf.Atan2(vector2.x, vector2.z) * 57.29578f);
		}
		text = text + "\nfacingDirection:" + (int)facingDirection;
		text = text + "\nActual facingDirection:" + (int)base.transform.rotation.eulerAngles.y;
		text = text + "\nState:" + state;
		text += "\n\n\n\n\n";
		if (state == HERO_STATE.Attack)
		{
			targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
		}
		return text;
	}

	private float getLeanAngle(Vector3 p, bool left)
	{
		if (!useGun && state == HERO_STATE.Attack)
		{
			return 0f;
		}
		float num = p.y - base.transform.position.y;
		float num2 = Vector3.Distance(p, base.transform.position);
		float num3 = Mathf.Acos(num / num2) * 57.29578f;
		num3 *= 0.1f;
		num3 *= 1f + Mathf.Pow(base.rigidbody.velocity.magnitude, 0.2f);
		Vector3 vector = p - base.transform.position;
		float current = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		float target = Mathf.Atan2(base.rigidbody.velocity.x, base.rigidbody.velocity.z) * 57.29578f;
		float num4 = Mathf.DeltaAngle(current, target);
		num3 += Mathf.Abs(num4 * 0.5f);
		if (state != HERO_STATE.Attack)
		{
			num3 = Mathf.Min(num3, 80f);
		}
		if (num4 > 0f)
		{
			leanLeft = true;
		}
		else
		{
			leanLeft = false;
		}
		if (useGun)
		{
			return num3 * (float)((!(num4 < 0f)) ? 1 : (-1));
		}
		float num5 = 0f;
		num5 = (((!left || !(num4 < 0f)) && (left || !(num4 > 0f))) ? 0.5f : 0.1f);
		return num3 * ((!(num4 < 0f)) ? num5 : (0f - num5));
	}

	private void bodyLean()
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			return;
		}
		float z = 0f;
		needLean = false;
		if (!useGun && state == HERO_STATE.Attack && attackAnimation != "attack3_1" && attackAnimation != "attack3_2")
		{
			float y = base.rigidbody.velocity.y;
			float x = base.rigidbody.velocity.x;
			float z2 = base.rigidbody.velocity.z;
			float x2 = Mathf.Sqrt(x * x + z2 * z2);
			float num = Mathf.Atan2(y, x2) * 57.29578f;
			targetRotation = Quaternion.Euler((0f - num) * (1f - Vector3.Angle(base.rigidbody.velocity, base.transform.forward) / 90f), facingDirection, 0f);
			if ((isLeftHandHooked && bulletLeft != null) || (isRightHandHooked && bulletRight != null))
			{
				base.transform.rotation = targetRotation;
			}
			return;
		}
		if (isLeftHandHooked && bulletLeft != null && isRightHandHooked && bulletRight != null)
		{
			if (almostSingleHook)
			{
				needLean = true;
				z = getLeanAngle(bulletRight.transform.position, true);
			}
		}
		else if (isLeftHandHooked && bulletLeft != null)
		{
			needLean = true;
			z = getLeanAngle(bulletLeft.transform.position, true);
		}
		else if (isRightHandHooked && bulletRight != null)
		{
			needLean = true;
			z = getLeanAngle(bulletRight.transform.position, false);
		}
		if (needLean)
		{
			float num2 = 0f;
			if (!useGun && state != HERO_STATE.Attack)
			{
				num2 = currentSpeed * 0.1f;
				num2 = Mathf.Min(num2, 20f);
			}
			targetRotation = Quaternion.Euler(0f - num2, facingDirection, z);
		}
		else if (state != HERO_STATE.Attack)
		{
			targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
		}
	}

	private void setHookedPplDirection()
	{
		almostSingleHook = false;
		if (isRightHandHooked && isLeftHandHooked)
		{
			if (!(bulletLeft != null) || !(bulletRight != null))
			{
				return;
			}
			Vector3 normal = bulletLeft.transform.position - bulletRight.transform.position;
			if (normal.sqrMagnitude < 4f)
			{
				Vector3 vector = (bulletLeft.transform.position + bulletRight.transform.position) * 0.5f - base.transform.position;
				facingDirection = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				if (useGun && state != HERO_STATE.Attack)
				{
					float current = (0f - Mathf.Atan2(base.rigidbody.velocity.z, base.rigidbody.velocity.x)) * 57.29578f;
					float target = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
					float num = 0f - Mathf.DeltaAngle(current, target);
					facingDirection += num;
				}
				almostSingleHook = true;
				return;
			}
			Vector3 to = base.transform.position - bulletLeft.transform.position;
			Vector3 to2 = base.transform.position - bulletRight.transform.position;
			Vector3 vector2 = (bulletLeft.transform.position + bulletRight.transform.position) * 0.5f;
			Vector3 from = base.transform.position - vector2;
			if (Vector3.Angle(from, to) < 30f && Vector3.Angle(from, to2) < 30f)
			{
				almostSingleHook = true;
				Vector3 vector3 = vector2 - base.transform.position;
				facingDirection = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
				return;
			}
			almostSingleHook = false;
			Vector3 tangent = base.transform.forward;
			Vector3.OrthoNormalize(ref normal, ref tangent);
			facingDirection = Mathf.Atan2(tangent.x, tangent.z) * 57.29578f;
			float current2 = Mathf.Atan2(to.x, to.z) * 57.29578f;
			float num2 = Mathf.DeltaAngle(current2, facingDirection);
			if (num2 > 0f)
			{
				facingDirection += 180f;
			}
			return;
		}
		almostSingleHook = true;
		Vector3 zero = Vector3.zero;
		if (isRightHandHooked && bulletRight != null)
		{
			zero = bulletRight.transform.position - base.transform.position;
		}
		else
		{
			if (!isLeftHandHooked || !(bulletLeft != null))
			{
				return;
			}
			zero = bulletLeft.transform.position - base.transform.position;
		}
		facingDirection = Mathf.Atan2(zero.x, zero.z) * 57.29578f;
		if (state != HERO_STATE.Attack)
		{
			float current3 = (0f - Mathf.Atan2(base.rigidbody.velocity.z, base.rigidbody.velocity.x)) * 57.29578f;
			float target2 = (0f - Mathf.Atan2(zero.z, zero.x)) * 57.29578f;
			float num3 = 0f - Mathf.DeltaAngle(current3, target2);
			if (useGun)
			{
				facingDirection += num3;
				return;
			}
			float num4 = 0f;
			num4 = (((!isLeftHandHooked || !(num3 < 0f)) && (!isRightHandHooked || !(num3 > 0f))) ? 0.1f : (-0.1f));
			facingDirection += num3 * num4;
		}
	}

	private void idle()
	{
		if (state == HERO_STATE.Attack)
		{
			falseAttack();
		}
		state = HERO_STATE.Idle;
		crossFade(standAnimation, 0.1f);
	}

	private void bufferUpdate()
	{
		if (!(buffTime > 0f))
		{
			return;
		}
		buffTime -= Time.deltaTime;
		if (buffTime <= 0f)
		{
			buffTime = 0f;
			if (currentBuff == BUFF.SpeedUp && base.animation.IsPlaying("run_sasha"))
			{
				crossFade("run", 0.1f);
			}
			currentBuff = BUFF.NoBuff;
		}
	}

	private void salute()
	{
		state = HERO_STATE.Salute;
		crossFade("salute", 0.1f);
	}

	private void changeBlade()
	{
		if (useGun && !grounded && LevelInfo.getInfo(FengGameManagerMKII.level).type == GAMEMODE.PVP_AHSS)
		{
			return;
		}
		state = HERO_STATE.ChangeBlade;
		throwedBlades = false;
		if (useGun)
		{
			if (!leftGunHasBullet && !rightGunHasBullet)
			{
				if (grounded)
				{
					reloadAnimation = "AHSS_gun_reload_both";
				}
				else
				{
					reloadAnimation = "AHSS_gun_reload_both_air";
				}
			}
			else if (!leftGunHasBullet)
			{
				if (grounded)
				{
					reloadAnimation = "AHSS_gun_reload_l";
				}
				else
				{
					reloadAnimation = "AHSS_gun_reload_l_air";
				}
			}
			else if (!rightGunHasBullet)
			{
				if (grounded)
				{
					reloadAnimation = "AHSS_gun_reload_r";
				}
				else
				{
					reloadAnimation = "AHSS_gun_reload_r_air";
				}
			}
			else
			{
				if (grounded)
				{
					reloadAnimation = "AHSS_gun_reload_both";
				}
				else
				{
					reloadAnimation = "AHSS_gun_reload_both_air";
				}
				leftGunHasBullet = (rightGunHasBullet = false);
			}
			crossFade(reloadAnimation, 0.05f);
		}
		else
		{
			if (!grounded)
			{
				reloadAnimation = "changeBlade_air";
			}
			else
			{
				reloadAnimation = "changeBlade";
			}
			crossFade(reloadAnimation, 0.1f);
		}
	}

	private void dodge(bool offTheWall = false)
	{
		if (myHorse != null && !isMounted && Vector3.Distance(myHorse.transform.position, base.transform.position) < 15f)
		{
			getOnHorse();
			return;
		}
		state = HERO_STATE.GroundDodge;
		if (!offTheWall)
		{
			float num = (inputManager.isInput[InputCode.up] ? 1f : ((!inputManager.isInput[InputCode.down]) ? 0f : (-1f)));
			float num2 = (inputManager.isInput[InputCode.left] ? (-1f) : ((!inputManager.isInput[InputCode.right]) ? 0f : 1f));
			float globalFacingDirection = getGlobalFacingDirection(num2, num);
			if (num2 != 0f || num != 0f)
			{
				facingDirection = globalFacingDirection + 180f;
				targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
			}
			crossFade("dodge", 0.1f);
		}
		else
		{
			playAnimation("dodge");
			playAnimationAt("dodge", 0.2f);
		}
		sparks.enableEmission = false;
	}

	private void dash(float horizontal, float vertical)
	{
		UnityEngine.MonoBehaviour.print(dashTime + " " + currentGas);
		if (!(dashTime > 0f) && !(currentGas <= 0f) && !isMounted)
		{
			useGas(totalGas * 0.04f);
			facingDirection = getGlobalFacingDirection(horizontal, vertical);
			dashV = getGlobaleFacingVector3(facingDirection);
			originVM = currentSpeed;
			Quaternion rotation = Quaternion.Euler(0f, facingDirection, 0f);
			base.rigidbody.rotation = rotation;
			targetRotation = rotation;
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				UnityEngine.Object.Instantiate(Resources.Load("FX/boost_smoke"), base.transform.position, base.transform.rotation);
			}
			else
			{
				PhotonNetwork.Instantiate("FX/boost_smoke", base.transform.position, base.transform.rotation, 0);
			}
			dashTime = 0.5f;
			crossFade("dash", 0.1f);
			base.animation["dash"].time = 0.1f;
			state = HERO_STATE.AirDodge;
			falseAttack();
			base.rigidbody.AddForce(dashV * 40f, ForceMode.VelocityChange);
		}
	}

	private void showAimUI()
	{
		if (Screen.showCursor)
		{
			GameObject gameObject = GameObject.Find("cross1");
			GameObject gameObject2 = GameObject.Find("cross2");
			GameObject gameObject3 = GameObject.Find("crossL1");
			GameObject gameObject4 = GameObject.Find("crossL2");
			GameObject gameObject5 = GameObject.Find("crossR1");
			GameObject gameObject6 = GameObject.Find("crossR2");
			GameObject gameObject7 = GameObject.Find("LabelDistance");
			Transform obj = gameObject.transform;
			Vector3 vector = Vector3.up * 10000f;
			gameObject6.transform.localPosition = vector;
			vector = vector;
			gameObject5.transform.localPosition = vector;
			vector = vector;
			gameObject4.transform.localPosition = vector;
			vector = vector;
			gameObject3.transform.localPosition = vector;
			vector = vector;
			gameObject7.transform.localPosition = vector;
			vector = vector;
			gameObject2.transform.localPosition = vector;
			obj.localPosition = vector;
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, 10000000f, layerMask3.value))
		{
			return;
		}
		GameObject gameObject8 = GameObject.Find("cross1");
		GameObject gameObject9 = GameObject.Find("cross2");
		gameObject8.transform.localPosition = Input.mousePosition;
		gameObject8.transform.localPosition -= new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
		gameObject9.transform.localPosition = gameObject8.transform.localPosition;
		float magnitude = (hitInfo.point - base.transform.position).magnitude;
		GameObject gameObject10 = GameObject.Find("LabelDistance");
		string text = ((!(magnitude > 1000f)) ? ((int)magnitude).ToString() : "???");
		gameObject10.GetComponent<UILabel>().text = text;
		if (magnitude > 120f)
		{
			gameObject8.transform.localPosition += Vector3.up * 10000f;
			gameObject10.transform.localPosition = gameObject9.transform.localPosition;
		}
		else
		{
			gameObject9.transform.localPosition += Vector3.up * 10000f;
			gameObject10.transform.localPosition = gameObject8.transform.localPosition;
		}
		gameObject10.transform.localPosition -= new Vector3(0f, 15f, 0f);
		Vector3 vector2 = new Vector3(0f, 0.4f, 0f);
		vector2 -= base.transform.right * 0.3f;
		Vector3 vector3 = new Vector3(0f, 0.4f, 0f);
		vector3 += base.transform.right * 0.3f;
		float num = ((!(hitInfo.distance > 50f)) ? (hitInfo.distance * 0.05f) : (hitInfo.distance * 0.3f));
		Vector3 vector4 = hitInfo.point - base.transform.right * num - (base.transform.position + vector2);
		Vector3 vector5 = hitInfo.point + base.transform.right * num - (base.transform.position + vector3);
		vector4.Normalize();
		vector5.Normalize();
		vector4 *= 1000000f;
		vector5 *= 1000000f;
		RaycastHit hitInfo2;
		if (Physics.Linecast(base.transform.position + vector2, base.transform.position + vector2 + vector4, out hitInfo2, layerMask3.value))
		{
			GameObject gameObject11 = GameObject.Find("crossL1");
			gameObject11.transform.localPosition = currentCamera.WorldToScreenPoint(hitInfo2.point);
			gameObject11.transform.localPosition -= new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
			gameObject11.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(gameObject11.transform.localPosition.y - (Input.mousePosition.y - (float)Screen.height * 0.5f), gameObject11.transform.localPosition.x - (Input.mousePosition.x - (float)Screen.width * 0.5f)) * 57.29578f + 180f);
			GameObject gameObject12 = GameObject.Find("crossL2");
			gameObject12.transform.localPosition = gameObject11.transform.localPosition;
			gameObject12.transform.localRotation = gameObject11.transform.localRotation;
			if (hitInfo2.distance > 120f)
			{
				gameObject11.transform.localPosition += Vector3.up * 10000f;
			}
			else
			{
				gameObject12.transform.localPosition += Vector3.up * 10000f;
			}
		}
		if (Physics.Linecast(base.transform.position + vector3, base.transform.position + vector3 + vector5, out hitInfo2, layerMask3.value))
		{
			GameObject gameObject13 = GameObject.Find("crossR1");
			gameObject13.transform.localPosition = currentCamera.WorldToScreenPoint(hitInfo2.point);
			gameObject13.transform.localPosition -= new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
			gameObject13.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(gameObject13.transform.localPosition.y - (Input.mousePosition.y - (float)Screen.height * 0.5f), gameObject13.transform.localPosition.x - (Input.mousePosition.x - (float)Screen.width * 0.5f)) * 57.29578f);
			GameObject gameObject14 = GameObject.Find("crossR2");
			gameObject14.transform.localPosition = gameObject13.transform.localPosition;
			gameObject14.transform.localRotation = gameObject13.transform.localRotation;
			if (hitInfo2.distance > 120f)
			{
				gameObject13.transform.localPosition += Vector3.up * 10000f;
			}
			else
			{
				gameObject14.transform.localPosition += Vector3.up * 10000f;
			}
		}
	}

	private void calcSkillCD()
	{
		if (skillCDDuration > 0f)
		{
			skillCDDuration -= Time.deltaTime;
			if (skillCDDuration < 0f)
			{
				skillCDDuration = 0f;
			}
		}
	}

	private void calcFlareCD()
	{
		if (flare1CD > 0f)
		{
			flare1CD -= Time.deltaTime;
			if (flare1CD < 0f)
			{
				flare1CD = 0f;
			}
		}
		if (flare2CD > 0f)
		{
			flare2CD -= Time.deltaTime;
			if (flare2CD < 0f)
			{
				flare2CD = 0f;
			}
		}
		if (flare3CD > 0f)
		{
			flare3CD -= Time.deltaTime;
			if (flare3CD < 0f)
			{
				flare3CD = 0f;
			}
		}
	}

	private void showFlareCD()
	{
		if ((bool)GameObject.Find("UIflare1"))
		{
			GameObject.Find("UIflare1").GetComponent<UISprite>().fillAmount = (flareTotalCD - flare1CD) / flareTotalCD;
			GameObject.Find("UIflare2").GetComponent<UISprite>().fillAmount = (flareTotalCD - flare2CD) / flareTotalCD;
			GameObject.Find("UIflare3").GetComponent<UISprite>().fillAmount = (flareTotalCD - flare3CD) / flareTotalCD;
		}
	}

	private void showSkillCD()
	{
		if ((bool)skillCD)
		{
			skillCD.GetComponent<UISprite>().fillAmount = (skillCDLast - skillCDDuration) / skillCDLast;
		}
	}

	private void showGas()
	{
		float num = currentGas / totalGas;
		float num2 = currentBladeSta / totalBladeSta;
		GameObject gameObject = GameObject.Find("gasL1");
		gameObject.GetComponent<UISprite>().fillAmount = currentGas / totalGas;
		GameObject gameObject2 = GameObject.Find("gasR1");
		gameObject2.GetComponent<UISprite>().fillAmount = currentGas / totalGas;
		if (!useGun)
		{
			GameObject gameObject3 = GameObject.Find("bladeCL");
			gameObject3.GetComponent<UISprite>().fillAmount = currentBladeSta / totalBladeSta;
			GameObject gameObject4 = GameObject.Find("bladeCR");
			gameObject4.GetComponent<UISprite>().fillAmount = currentBladeSta / totalBladeSta;
			if (num <= 0f)
			{
				GameObject.Find("gasL").GetComponent<UISprite>().color = Color.red;
				GameObject.Find("gasR").GetComponent<UISprite>().color = Color.red;
			}
			else if (num < 0.3f)
			{
				GameObject.Find("gasL").GetComponent<UISprite>().color = Color.yellow;
				GameObject.Find("gasR").GetComponent<UISprite>().color = Color.yellow;
			}
			else
			{
				GameObject.Find("gasL").GetComponent<UISprite>().color = Color.white;
				GameObject.Find("gasR").GetComponent<UISprite>().color = Color.white;
			}
			if (num2 <= 0f)
			{
				GameObject.Find("bladel1").GetComponent<UISprite>().color = Color.red;
				GameObject.Find("blader1").GetComponent<UISprite>().color = Color.red;
			}
			else if (num2 < 0.3f)
			{
				GameObject.Find("bladel1").GetComponent<UISprite>().color = Color.yellow;
				GameObject.Find("blader1").GetComponent<UISprite>().color = Color.yellow;
			}
			else
			{
				GameObject.Find("bladel1").GetComponent<UISprite>().color = Color.white;
				GameObject.Find("blader1").GetComponent<UISprite>().color = Color.white;
			}
			if (currentBladeNum <= 4)
			{
				GameObject.Find("bladel5").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader5").GetComponent<UISprite>().enabled = false;
			}
			else
			{
				GameObject.Find("bladel5").GetComponent<UISprite>().enabled = true;
				GameObject.Find("blader5").GetComponent<UISprite>().enabled = true;
			}
			if (currentBladeNum <= 3)
			{
				GameObject.Find("bladel4").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader4").GetComponent<UISprite>().enabled = false;
			}
			else
			{
				GameObject.Find("bladel4").GetComponent<UISprite>().enabled = true;
				GameObject.Find("blader4").GetComponent<UISprite>().enabled = true;
			}
			if (currentBladeNum <= 2)
			{
				GameObject.Find("bladel3").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader3").GetComponent<UISprite>().enabled = false;
			}
			else
			{
				GameObject.Find("bladel3").GetComponent<UISprite>().enabled = true;
				GameObject.Find("blader3").GetComponent<UISprite>().enabled = true;
			}
			if (currentBladeNum <= 1)
			{
				GameObject.Find("bladel2").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader2").GetComponent<UISprite>().enabled = false;
			}
			else
			{
				GameObject.Find("bladel2").GetComponent<UISprite>().enabled = true;
				GameObject.Find("blader2").GetComponent<UISprite>().enabled = true;
			}
			if (currentBladeNum <= 0)
			{
				GameObject.Find("bladel1").GetComponent<UISprite>().enabled = false;
				GameObject.Find("blader1").GetComponent<UISprite>().enabled = false;
			}
			else
			{
				GameObject.Find("bladel1").GetComponent<UISprite>().enabled = true;
				GameObject.Find("blader1").GetComponent<UISprite>().enabled = true;
			}
		}
		else
		{
			if (leftGunHasBullet)
			{
				GameObject.Find("bulletL").GetComponent<UISprite>().enabled = true;
			}
			else
			{
				GameObject.Find("bulletL").GetComponent<UISprite>().enabled = false;
			}
			if (rightGunHasBullet)
			{
				GameObject.Find("bulletR").GetComponent<UISprite>().enabled = true;
			}
			else
			{
				GameObject.Find("bulletR").GetComponent<UISprite>().enabled = false;
			}
		}
	}

	private void useGas(float amount = 0f)
	{
		if (amount == 0f)
		{
			amount = useGasSpeed;
		}
		if (currentGas > 0f)
		{
			currentGas -= amount;
			if (currentGas < 0f)
			{
				currentGas = 0f;
			}
		}
	}

	public void getSupply()
	{
		if ((base.animation.IsPlaying(standAnimation) || base.animation.IsPlaying("run") || base.animation.IsPlaying("run_sasha")) && (currentBladeSta != totalBladeSta || currentBladeNum != totalBladeNum || currentGas != totalGas || leftBulletLeft != bulletMAX || rightBulletLeft != bulletMAX))
		{
			state = HERO_STATE.FillGas;
			crossFade("supply", 0.1f);
		}
	}

	public void fillGas()
	{
		currentGas = totalGas;
	}

	public void useBlade(int amount = 0)
	{
		if (amount == 0)
		{
			amount = 1;
		}
		amount *= 1;
		if (!(currentBladeSta > 0f))
		{
			return;
		}
		currentBladeSta -= amount;
		if (currentBladeSta <= 0f)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
			{
				leftbladetrail.Deactivate();
				rightbladetrail.Deactivate();
				leftbladetrail2.Deactivate();
				rightbladetrail2.Deactivate();
				checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
				checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
			}
			currentBladeSta = 0f;
			throwBlades();
		}
	}

	private void throwBlades()
	{
		Transform transform = setup.part_blade_l.transform;
		Transform transform2 = setup.part_blade_r.transform;
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_blade_l"), transform.position, transform.rotation);
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_blade_r"), transform2.position, transform2.rotation);
		gameObject.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		gameObject2.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		Vector3 force = base.transform.forward + base.transform.up * 2f - base.transform.right;
		gameObject.rigidbody.AddForce(force, ForceMode.Impulse);
		Vector3 force2 = base.transform.forward + base.transform.up * 2f + base.transform.right;
		gameObject2.rigidbody.AddForce(force2, ForceMode.Impulse);
		Vector3 torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
		torque.Normalize();
		gameObject.rigidbody.AddTorque(torque);
		torque = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
		torque.Normalize();
		gameObject2.rigidbody.AddTorque(torque);
		setup.part_blade_l.SetActive(false);
		setup.part_blade_r.SetActive(false);
		currentBladeNum--;
		if (currentBladeNum == 0)
		{
			currentBladeSta = 0f;
		}
		if (state == HERO_STATE.Attack)
		{
			falseAttack();
		}
	}

	private void launchLeftRope(RaycastHit hit, bool single, int mode = 0)
	{
		if (currentGas != 0f)
		{
			useGas(0f);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				bulletLeft = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("hook"));
			}
			else if (base.photonView.isMine)
			{
				bulletLeft = PhotonNetwork.Instantiate("hook", base.transform.position, base.transform.rotation, 0);
			}
			GameObject gameObject = ((!useGun) ? hookRefL1 : hookRefL2);
			string launcher_ref = ((!useGun) ? "hookRefL1" : "hookRefL2");
			bulletLeft.transform.position = gameObject.transform.position;
			Bullet component = bulletLeft.GetComponent<Bullet>();
			float num = (single ? 0f : ((!(hit.distance > 50f)) ? (hit.distance * 0.05f) : (hit.distance * 0.3f)));
			Vector3 vector = hit.point - base.transform.right * num - bulletLeft.transform.position;
			vector.Normalize();
			if (mode == 1)
			{
				component.launch(vector * 3f, base.rigidbody.velocity, launcher_ref, true, base.gameObject, true);
			}
			else
			{
				component.launch(vector * 3f, base.rigidbody.velocity, launcher_ref, true, base.gameObject);
			}
			launchPointLeft = Vector3.zero;
		}
	}

	private void launchRightRope(RaycastHit hit, bool single, int mode = 0)
	{
		if (currentGas != 0f)
		{
			useGas(0f);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				bulletRight = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("hook"));
			}
			else if (base.photonView.isMine)
			{
				bulletRight = PhotonNetwork.Instantiate("hook", base.transform.position, base.transform.rotation, 0);
			}
			GameObject gameObject = ((!useGun) ? hookRefR1 : hookRefR2);
			string launcher_ref = ((!useGun) ? "hookRefR1" : "hookRefR2");
			bulletRight.transform.position = gameObject.transform.position;
			Bullet component = bulletRight.GetComponent<Bullet>();
			float num = (single ? 0f : ((!(hit.distance > 50f)) ? (hit.distance * 0.05f) : (hit.distance * 0.3f)));
			Vector3 vector = hit.point + base.transform.right * num - bulletRight.transform.position;
			vector.Normalize();
			if (mode == 1)
			{
				component.launch(vector * 5f, base.rigidbody.velocity, launcher_ref, false, base.gameObject, true);
			}
			else
			{
				component.launch(vector * 3f, base.rigidbody.velocity, launcher_ref, false, base.gameObject);
			}
			launchPointRight = Vector3.zero;
		}
	}

	public void falseAttack()
	{
		attackMove = false;
		if (useGun)
		{
			if (!attackReleased)
			{
				continueAnimation();
				attackReleased = true;
			}
			return;
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
		{
			checkBoxLeft.GetComponent<TriggerColliderWeapon>().active_me = false;
			checkBoxRight.GetComponent<TriggerColliderWeapon>().active_me = false;
			checkBoxLeft.GetComponent<TriggerColliderWeapon>().clearHits();
			checkBoxRight.GetComponent<TriggerColliderWeapon>().clearHits();
			leftbladetrail.StopSmoothly(0.2f);
			rightbladetrail.StopSmoothly(0.2f);
			leftbladetrail2.StopSmoothly(0.2f);
			rightbladetrail2.StopSmoothly(0.2f);
		}
		attackLoop = 0;
		if (!attackReleased)
		{
			continueAnimation();
			attackReleased = true;
		}
	}

	private void FixedUpdate()
	{
		if (titanForm || (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE))
		{
			return;
		}
		currentSpeed = base.rigidbody.velocity.magnitude;
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			return;
		}
		if (state == HERO_STATE.Grab)
		{
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			return;
		}
		if (IsGrounded())
		{
			if (!grounded)
			{
				justGrounded = true;
			}
			grounded = true;
		}
		else
		{
			grounded = false;
		}
		if (hookSomeOne)
		{
			if (hookTarget != null)
			{
				Vector3 vector = hookTarget.transform.position - base.transform.position;
				float magnitude = vector.magnitude;
				if (magnitude > 2f)
				{
					base.rigidbody.AddForce(vector.normalized * Mathf.Pow(magnitude, 0.15f) * 30f - base.rigidbody.velocity * 0.95f, ForceMode.VelocityChange);
				}
			}
			else
			{
				hookSomeOne = false;
			}
		}
		else if (hookBySomeOne && badGuy != null)
		{
			if (badGuy != null)
			{
				Vector3 vector2 = badGuy.transform.position - base.transform.position;
				float magnitude2 = vector2.magnitude;
				if (magnitude2 > 5f)
				{
					base.rigidbody.AddForce(vector2.normalized * Mathf.Pow(magnitude2, 0.15f) * 0.2f, ForceMode.Impulse);
				}
			}
			else
			{
				hookBySomeOne = false;
			}
		}
		float num = 0f;
		float num2 = 0f;
		if (!IN_GAME_MAIN_CAMERA.isTyping)
		{
			num2 = (inputManager.isInput[InputCode.up] ? 1f : ((!inputManager.isInput[InputCode.down]) ? 0f : (-1f)));
			num = (inputManager.isInput[InputCode.left] ? (-1f) : ((!inputManager.isInput[InputCode.right]) ? 0f : 1f));
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		isLeftHandHooked = false;
		isRightHandHooked = false;
		if (isLaunchLeft)
		{
			if (bulletLeft != null && bulletLeft.GetComponent<Bullet>().isHooked())
			{
				isLeftHandHooked = true;
				Vector3 vector3 = bulletLeft.transform.position - base.transform.position;
				vector3.Normalize();
				vector3 *= 10f;
				if (!isLaunchRight)
				{
					vector3 *= 2f;
				}
				if (Vector3.Angle(base.rigidbody.velocity, vector3) > 90f && inputManager.isInput[InputCode.jump])
				{
					flag2 = true;
					flag = true;
				}
				if (!flag2)
				{
					base.rigidbody.AddForce(vector3);
					if (Vector3.Angle(base.rigidbody.velocity, vector3) > 90f)
					{
						base.rigidbody.AddForce(-base.rigidbody.velocity * 2f, ForceMode.Acceleration);
					}
				}
			}
			launchElapsedTimeL += Time.deltaTime;
			if (QHold && currentGas > 0f)
			{
				useGas(useGasSpeed * Time.deltaTime);
			}
			else if (launchElapsedTimeL > 0.3f)
			{
				isLaunchLeft = false;
				if (bulletLeft != null)
				{
					Bullet component = bulletLeft.GetComponent<Bullet>();
					component.disable();
					releaseIfIHookSb();
					bulletLeft = null;
					flag2 = false;
				}
			}
		}
		if (isLaunchRight)
		{
			if (bulletRight != null && bulletRight.GetComponent<Bullet>().isHooked())
			{
				isRightHandHooked = true;
				Vector3 vector4 = bulletRight.transform.position - base.transform.position;
				vector4.Normalize();
				vector4 *= 10f;
				if (!isLaunchLeft)
				{
					vector4 *= 2f;
				}
				if (Vector3.Angle(base.rigidbody.velocity, vector4) > 90f && inputManager.isInput[InputCode.jump])
				{
					flag3 = true;
					flag = true;
				}
				if (!flag3)
				{
					base.rigidbody.AddForce(vector4);
					if (Vector3.Angle(base.rigidbody.velocity, vector4) > 90f)
					{
						base.rigidbody.AddForce(-base.rigidbody.velocity * 2f, ForceMode.Acceleration);
					}
				}
			}
			launchElapsedTimeR += Time.deltaTime;
			if (EHold && currentGas > 0f)
			{
				useGas(useGasSpeed * Time.deltaTime);
			}
			else if (launchElapsedTimeR > 0.3f)
			{
				isLaunchRight = false;
				if (bulletRight != null)
				{
					Bullet component2 = bulletRight.GetComponent<Bullet>();
					component2.disable();
					releaseIfIHookSb();
					bulletRight = null;
					flag3 = false;
				}
			}
		}
		if (grounded)
		{
			Vector3 vector5 = Vector3.zero;
			if (state == HERO_STATE.Attack)
			{
				if (attackAnimation == "attack5")
				{
					if (base.animation[attackAnimation].normalizedTime > 0.4f && base.animation[attackAnimation].normalizedTime < 0.61f)
					{
						base.rigidbody.AddForce(base.gameObject.transform.forward * 200f);
					}
				}
				else if (attackAnimation == "special_petra")
				{
					if (base.animation[attackAnimation].normalizedTime > 0.35f && base.animation[attackAnimation].normalizedTime < 0.48f)
					{
						base.rigidbody.AddForce(base.gameObject.transform.forward * 200f);
					}
				}
				else if (base.animation.IsPlaying("attack3_2"))
				{
					vector5 = Vector3.zero;
				}
				else if (base.animation.IsPlaying("attack1") || base.animation.IsPlaying("attack2"))
				{
					base.rigidbody.AddForce(base.gameObject.transform.forward * 200f);
				}
				if (base.animation.IsPlaying("attack3_2"))
				{
					vector5 = Vector3.zero;
				}
			}
			if (justGrounded)
			{
				if (state != HERO_STATE.Attack || (!(attackAnimation == "attack3_1") && !(attackAnimation == "attack5") && !(attackAnimation == "special_petra")))
				{
					if (state != HERO_STATE.Attack && num == 0f && num2 == 0f && !bulletLeft && !bulletRight && state != HERO_STATE.FillGas)
					{
						state = HERO_STATE.Land;
						crossFade("dash_land", 0.01f);
					}
					else
					{
						buttonAttackRelease = true;
						if (state != HERO_STATE.Attack && base.rigidbody.velocity.x * base.rigidbody.velocity.x + base.rigidbody.velocity.z * base.rigidbody.velocity.z > speed * speed * 1.5f && state != HERO_STATE.FillGas)
						{
							state = HERO_STATE.Slide;
							crossFade("slide", 0.05f);
							facingDirection = Mathf.Atan2(base.rigidbody.velocity.x, base.rigidbody.velocity.z) * 57.29578f;
							targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
							sparks.enableEmission = true;
						}
					}
				}
				justGrounded = false;
				vector5 = base.rigidbody.velocity;
			}
			if (state == HERO_STATE.Attack && attackAnimation == "attack3_1" && base.animation[attackAnimation].normalizedTime >= 1f)
			{
				playAnimation("attack3_2");
				resetAnimationSpeed();
				Vector3 zero = Vector3.zero;
				base.rigidbody.velocity = zero;
				vector5 = zero;
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.2f, 0.3f);
			}
			if (state == HERO_STATE.GroundDodge)
			{
				if (base.animation["dodge"].normalizedTime >= 0.2f && base.animation["dodge"].normalizedTime < 0.8f)
				{
					vector5 = -base.transform.forward * 2.4f * speed;
				}
				if (base.animation["dodge"].normalizedTime > 0.8f)
				{
					vector5 = base.rigidbody.velocity;
					vector5 *= 0.9f;
				}
			}
			else if (state == HERO_STATE.Idle)
			{
				Vector3 vector6 = new Vector3(num, 0f, num2);
				float num3 = getGlobalFacingDirection(num, num2);
				vector5 = getGlobaleFacingVector3(num3);
				float num4 = ((vector6.magnitude > 0.95f) ? 1f : ((!(vector6.magnitude < 0.25f)) ? vector6.magnitude : 0f));
				vector5 *= num4;
				vector5 *= speed;
				if (buffTime > 0f && currentBuff == BUFF.SpeedUp)
				{
					vector5 *= 4f;
				}
				if (num != 0f || num2 != 0f)
				{
					if (!base.animation.IsPlaying("run") && !base.animation.IsPlaying("jump") && !base.animation.IsPlaying("run_sasha") && (!base.animation.IsPlaying("horse_geton") || !(base.animation["horse_geton"].normalizedTime < 0.5f)))
					{
						if (buffTime > 0f && currentBuff == BUFF.SpeedUp)
						{
							crossFade("run_sasha", 0.1f);
						}
						else
						{
							crossFade("run", 0.1f);
						}
					}
				}
				else
				{
					if (!base.animation.IsPlaying(standAnimation) && state != HERO_STATE.Land && !base.animation.IsPlaying("jump") && !base.animation.IsPlaying("horse_geton") && !base.animation.IsPlaying("grabbed"))
					{
						crossFade(standAnimation, 0.1f);
						vector5 *= 0f;
					}
					num3 = -874f;
				}
				if (num3 != -874f)
				{
					facingDirection = num3;
					targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
				}
			}
			else if (state == HERO_STATE.Land)
			{
				vector5 = base.rigidbody.velocity;
				vector5 *= 0.96f;
			}
			else if (state == HERO_STATE.Slide)
			{
				vector5 = base.rigidbody.velocity;
				vector5 *= 0.99f;
				if (currentSpeed < speed * 1.2f)
				{
					idle();
					sparks.enableEmission = false;
				}
			}
			Vector3 velocity = base.rigidbody.velocity;
			Vector3 force = vector5 - velocity;
			force.x = Mathf.Clamp(force.x, 0f - maxVelocityChange, maxVelocityChange);
			force.z = Mathf.Clamp(force.z, 0f - maxVelocityChange, maxVelocityChange);
			force.y = 0f;
			if (base.animation.IsPlaying("jump") && base.animation["jump"].normalizedTime > 0.18f)
			{
				force.y += 8f;
			}
			if (base.animation.IsPlaying("horse_geton") && base.animation["horse_geton"].normalizedTime > 0.18f && base.animation["horse_geton"].normalizedTime < 1f)
			{
				float num5 = 6f;
				force = -base.rigidbody.velocity;
				force.y = num5;
				float num6 = Vector3.Distance(myHorse.transform.position, base.transform.position);
				float num7 = 0.6f * gravity * num6 / (2f * num5);
				force += num7 * (myHorse.transform.position - base.transform.position).normalized;
			}
			if (state != HERO_STATE.Attack || !useGun)
			{
				base.rigidbody.AddForce(force, ForceMode.VelocityChange);
				base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, facingDirection, 0f), Time.deltaTime * 10f);
			}
		}
		else
		{
			if (sparks.enableEmission)
			{
				sparks.enableEmission = false;
			}
			if (myHorse != null && (base.animation.IsPlaying("horse_geton") || base.animation.IsPlaying("air_fall")) && base.rigidbody.velocity.y < 0f && Vector3.Distance(myHorse.transform.position + Vector3.up * 1.65f, base.transform.position) < 0.5f)
			{
				base.transform.position = myHorse.transform.position + Vector3.up * 1.65f;
				base.transform.rotation = myHorse.transform.rotation;
				isMounted = true;
				crossFade("horse_idle", 0.1f);
				myHorse.GetComponent<Horse>().mounted();
			}
			if ((state == HERO_STATE.Idle && !base.animation.IsPlaying("dash") && !base.animation.IsPlaying("wallrun") && !base.animation.IsPlaying("toRoof") && !base.animation.IsPlaying("horse_geton") && !base.animation.IsPlaying("horse_getoff") && !base.animation.IsPlaying("air_release") && !isMounted && (!base.animation.IsPlaying("air_hook_l_just") || !(base.animation["air_hook_l_just"].normalizedTime < 1f)) && (!base.animation.IsPlaying("air_hook_r_just") || !(base.animation["air_hook_r_just"].normalizedTime < 1f))) || base.animation["dash"].normalizedTime >= 0.99f)
			{
				if (!isLeftHandHooked && !isRightHandHooked && (base.animation.IsPlaying("air_hook_l") || base.animation.IsPlaying("air_hook_r") || base.animation.IsPlaying("air_hook")) && base.rigidbody.velocity.y > 20f)
				{
					base.animation.CrossFade("air_release");
				}
				else
				{
					bool flag4 = Mathf.Abs(base.rigidbody.velocity.x) + Mathf.Abs(base.rigidbody.velocity.z) > 25f;
					bool flag5 = base.rigidbody.velocity.y < 0f;
					if (!flag4)
					{
						if (flag5)
						{
							if (!base.animation.IsPlaying("air_fall"))
							{
								crossFade("air_fall", 0.2f);
							}
						}
						else if (!base.animation.IsPlaying("air_rise"))
						{
							crossFade("air_rise", 0.2f);
						}
					}
					else if (!isLeftHandHooked && !isRightHandHooked)
					{
						float current = (0f - Mathf.Atan2(base.rigidbody.velocity.z, base.rigidbody.velocity.x)) * 57.29578f;
						float num8 = 0f - Mathf.DeltaAngle(current, base.transform.rotation.eulerAngles.y - 90f);
						if (Mathf.Abs(num8) < 45f)
						{
							if (!base.animation.IsPlaying("air2"))
							{
								crossFade("air2", 0.2f);
							}
						}
						else if (num8 < 135f && num8 > 0f)
						{
							if (!base.animation.IsPlaying("air2_right"))
							{
								crossFade("air2_right", 0.2f);
							}
						}
						else if (num8 > -135f && num8 < 0f)
						{
							if (!base.animation.IsPlaying("air2_left"))
							{
								crossFade("air2_left", 0.2f);
							}
						}
						else if (!base.animation.IsPlaying("air2_backward"))
						{
							crossFade("air2_backward", 0.2f);
						}
					}
					else if (useGun)
					{
						if (!isRightHandHooked)
						{
							if (!base.animation.IsPlaying("AHSS_hook_forward_l"))
							{
								crossFade("AHSS_hook_forward_l", 0.1f);
							}
						}
						else if (!isLeftHandHooked)
						{
							if (!base.animation.IsPlaying("AHSS_hook_forward_r"))
							{
								crossFade("AHSS_hook_forward_r", 0.1f);
							}
						}
						else if (!base.animation.IsPlaying("AHSS_hook_forward_both"))
						{
							crossFade("AHSS_hook_forward_both", 0.1f);
						}
					}
					else if (!isRightHandHooked)
					{
						if (!base.animation.IsPlaying("air_hook_l"))
						{
							crossFade("air_hook_l", 0.1f);
						}
					}
					else if (!isLeftHandHooked)
					{
						if (!base.animation.IsPlaying("air_hook_r"))
						{
							crossFade("air_hook_r", 0.1f);
						}
					}
					else if (!base.animation.IsPlaying("air_hook"))
					{
						crossFade("air_hook", 0.1f);
					}
				}
			}
			if (state == HERO_STATE.Idle && base.animation.IsPlaying("air_release") && base.animation["air_release"].normalizedTime >= 1f)
			{
				crossFade("air_rise", 0.2f);
			}
			if (base.animation.IsPlaying("horse_getoff") && base.animation["horse_getoff"].normalizedTime >= 1f)
			{
				crossFade("air_rise", 0.2f);
			}
			if (base.animation.IsPlaying("toRoof"))
			{
				if (base.animation["toRoof"].normalizedTime < 0.22f)
				{
					base.rigidbody.velocity = Vector3.zero;
					base.rigidbody.AddForce(new Vector3(0f, gravity * base.rigidbody.mass, 0f));
				}
				else
				{
					if (!wallJump)
					{
						wallJump = true;
						base.rigidbody.AddForce(Vector3.up * 8f, ForceMode.Impulse);
					}
					base.rigidbody.AddForce(base.transform.forward * 0.05f, ForceMode.Impulse);
				}
				if (base.animation["toRoof"].normalizedTime >= 1f)
				{
					playAnimation("air_rise");
				}
			}
			else if (state == HERO_STATE.Idle && isPressDirectionTowardsHero(num, num2) && !inputManager.isInput[InputCode.jump] && !inputManager.isInput[InputCode.leftRope] && !inputManager.isInput[InputCode.rightRope] && !inputManager.isInput[InputCode.bothRope] && IsFrontGrounded() && !base.animation.IsPlaying("wallrun") && !base.animation.IsPlaying("dodge"))
			{
				crossFade("wallrun", 0.1f);
				wallRunTime = 0f;
			}
			else if (base.animation.IsPlaying("wallrun"))
			{
				base.rigidbody.AddForce(Vector3.up * speed - base.rigidbody.velocity, ForceMode.VelocityChange);
				wallRunTime += Time.deltaTime;
				if (wallRunTime > 1f || (num2 == 0f && num == 0f))
				{
					base.rigidbody.AddForce(-base.transform.forward * speed * 0.75f, ForceMode.Impulse);
					dodge(true);
				}
				else if (!IsUpFrontGrounded())
				{
					wallJump = false;
					crossFade("toRoof", 0.1f);
				}
				else if (!IsFrontGrounded())
				{
					crossFade("air_fall", 0.1f);
				}
			}
			else if (!base.animation.IsPlaying("attack5") && !base.animation.IsPlaying("special_petra") && !base.animation.IsPlaying("dash") && !base.animation.IsPlaying("jump"))
			{
				Vector3 vector7 = new Vector3(num, 0f, num2);
				float num9 = getGlobalFacingDirection(num, num2);
				Vector3 globaleFacingVector = getGlobaleFacingVector3(num9);
				float num10 = ((vector7.magnitude > 0.95f) ? 1f : ((!(vector7.magnitude < 0.25f)) ? vector7.magnitude : 0f));
				globaleFacingVector *= num10;
				globaleFacingVector *= (float)setup.myCostume.stat.ACL / 10f * 2f;
				if (num == 0f && num2 == 0f)
				{
					if (state == HERO_STATE.Attack)
					{
						globaleFacingVector *= 0f;
					}
					num9 = -874f;
				}
				if (num9 != -874f)
				{
					facingDirection = num9;
					targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
				}
				if (!flag2 && !flag3 && !isMounted && inputManager.isInput[InputCode.jump] && currentGas > 0f)
				{
					if (num != 0f || num2 != 0f)
					{
						base.rigidbody.AddForce(globaleFacingVector, ForceMode.Acceleration);
					}
					else
					{
						base.rigidbody.AddForce(base.transform.forward * globaleFacingVector.magnitude, ForceMode.Acceleration);
					}
					flag = true;
				}
			}
			if (base.animation.IsPlaying("air_fall") && currentSpeed < 0.2f && IsFrontGrounded())
			{
				crossFade("onWall", 0.3f);
			}
		}
		spinning = false;
		if (flag2 && flag3)
		{
			float num11 = currentSpeed + 0.1f;
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			Vector3 current2 = (bulletRight.transform.position + bulletLeft.transform.position) * 0.5f - base.transform.position;
			float value = Input.GetAxis("Mouse ScrollWheel") * 5555f;
			value = Mathf.Clamp(value, -0.8f, 0.8f);
			float num12 = 1f + value;
			Vector3 vector8 = Vector3.RotateTowards(current2, base.rigidbody.velocity, 1.5393804f * num12, 1.5393804f * num12);
			vector8.Normalize();
			spinning = true;
			base.rigidbody.velocity = vector8 * num11;
		}
		else if (flag2)
		{
			float num13 = currentSpeed + 0.1f;
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			Vector3 current3 = bulletLeft.transform.position - base.transform.position;
			float value2 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
			value2 = Mathf.Clamp(value2, -0.8f, 0.8f);
			float num14 = 1f + value2;
			Vector3 vector9 = Vector3.RotateTowards(current3, base.rigidbody.velocity, 1.5393804f * num14, 1.5393804f * num14);
			vector9.Normalize();
			spinning = true;
			base.rigidbody.velocity = vector9 * num13;
		}
		else if (flag3)
		{
			float num15 = currentSpeed + 0.1f;
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			Vector3 current4 = bulletRight.transform.position - base.transform.position;
			float value3 = Input.GetAxis("Mouse ScrollWheel") * 5555f;
			value3 = Mathf.Clamp(value3, -0.8f, 0.8f);
			float num16 = 1f + value3;
			Vector3 vector10 = Vector3.RotateTowards(current4, base.rigidbody.velocity, 1.5393804f * num16, 1.5393804f * num16);
			vector10.Normalize();
			spinning = true;
			base.rigidbody.velocity = vector10 * num15;
		}
		if (state == HERO_STATE.Attack && (attackAnimation == "attack5" || attackAnimation == "special_petra") && base.animation[attackAnimation].normalizedTime > 0.4f && !attackMove)
		{
			attackMove = true;
			if (launchPointRight.magnitude > 0f)
			{
				Vector3 force2 = launchPointRight - base.transform.position;
				force2.Normalize();
				force2 *= 13f;
				base.rigidbody.AddForce(force2, ForceMode.Impulse);
			}
			if (attackAnimation == "special_petra" && launchPointLeft.magnitude > 0f)
			{
				Vector3 force3 = launchPointLeft - base.transform.position;
				force3.Normalize();
				force3 *= 13f;
				base.rigidbody.AddForce(force3, ForceMode.Impulse);
				if ((bool)bulletRight)
				{
					bulletRight.GetComponent<Bullet>().disable();
					releaseIfIHookSb();
				}
				if ((bool)bulletLeft)
				{
					bulletLeft.GetComponent<Bullet>().disable();
					releaseIfIHookSb();
				}
			}
			base.rigidbody.AddForce(Vector3.up * 2f, ForceMode.Impulse);
		}
		bool flag6 = false;
		if (bulletLeft != null || bulletRight != null)
		{
			if ((bool)bulletLeft && bulletLeft.transform.position.y > base.gameObject.transform.position.y && isLaunchLeft && bulletLeft.GetComponent<Bullet>().isHooked())
			{
				flag6 = true;
			}
			if ((bool)bulletRight && bulletRight.transform.position.y > base.gameObject.transform.position.y && isLaunchRight && bulletRight.GetComponent<Bullet>().isHooked())
			{
				flag6 = true;
			}
		}
		if (flag6)
		{
			base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
		}
		else
		{
			base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
		}
		if (currentSpeed > 10f)
		{
			currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentCamera.GetComponent<Camera>().fieldOfView, Mathf.Min(100f, currentSpeed + 40f), 0.1f);
		}
		else
		{
			currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentCamera.GetComponent<Camera>().fieldOfView, 50f, 0.1f);
		}
		if (flag)
		{
			useGas(useGasSpeed * Time.deltaTime);
			if (!smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
			{
				base.photonView.RPC("net3DMGSMOKE", PhotonTargets.Others, true);
			}
			smoke_3dmg.enableEmission = true;
		}
		else
		{
			if (smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
			{
				base.photonView.RPC("net3DMGSMOKE", PhotonTargets.Others, false);
			}
			smoke_3dmg.enableEmission = false;
		}
		if (currentSpeed > 80f)
		{
			if (!speedFXPS.enableEmission)
			{
				speedFXPS.enableEmission = false;
			}
			speedFXPS.startSpeed = currentSpeed;
			speedFX.transform.LookAt(base.transform.position + base.rigidbody.velocity);
		}
		else if (speedFXPS.enableEmission)
		{
			speedFXPS.enableEmission = false;
		}
	}

	private bool isPressDirectionTowardsHero(float h, float v)
	{
		if (h == 0f && v == 0f)
		{
			return false;
		}
		float globalFacingDirection = getGlobalFacingDirection(h, v);
		if (Mathf.Abs(Mathf.DeltaAngle(globalFacingDirection, base.transform.rotation.eulerAngles.y)) < 45f)
		{
			return true;
		}
		return false;
	}

	private void customAnimationSpeed()
	{
		base.animation["attack5"].speed = 1.85f;
		base.animation["changeBlade"].speed = 1.2f;
		base.animation["air_release"].speed = 0.6f;
		base.animation["changeBlade_air"].speed = 0.8f;
		base.animation["AHSS_gun_reload_both"].speed = 0.38f;
		base.animation["AHSS_gun_reload_both_air"].speed = 0.5f;
		base.animation["AHSS_gun_reload_l"].speed = 0.4f;
		base.animation["AHSS_gun_reload_l_air"].speed = 0.5f;
		base.animation["AHSS_gun_reload_r"].speed = 0.4f;
		base.animation["AHSS_gun_reload_r_air"].speed = 0.5f;
	}

	public void pauseAnimation()
	{
		foreach (AnimationState item in base.animation)
		{
			item.speed = 0f;
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			base.photonView.RPC("netPauseAnimation", PhotonTargets.Others);
		}
	}

	public void resetAnimationSpeed()
	{
		foreach (AnimationState item in base.animation)
		{
			item.speed = 1f;
		}
		customAnimationSpeed();
	}

	public void continueAnimation()
	{
		foreach (AnimationState item in base.animation)
		{
			if (item.speed == 1f)
			{
				return;
			}
			item.speed = 1f;
		}
		customAnimationSpeed();
		playAnimation(currentPlayingClipName());
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && base.photonView.isMine)
		{
			base.photonView.RPC("netContinueAnimation", PhotonTargets.Others);
		}
	}

	[RPC]
	private void netPauseAnimation()
	{
		foreach (AnimationState item in base.animation)
		{
			item.speed = 0f;
		}
	}

	[RPC]
	private void netContinueAnimation()
	{
		foreach (AnimationState item in base.animation)
		{
			if (item.speed == 1f)
			{
				return;
			}
			item.speed = 1f;
		}
		playAnimation(currentPlayingClipName());
	}

	public string currentPlayingClipName()
	{
		foreach (AnimationState item in base.animation)
		{
			if (base.animation.IsPlaying(item.name))
			{
				return item.name;
			}
		}
		return string.Empty;
	}

	public void hookToHuman(GameObject target, Vector3 hookPosition)
	{
		releaseIfIHookSb();
		hookTarget = target;
		hookSomeOne = true;
		if ((bool)target.GetComponent<HERO>())
		{
			target.GetComponent<HERO>().hookedByHuman(base.photonView.viewID, hookPosition);
		}
		launchForce = hookPosition - base.transform.position;
		float num = Mathf.Pow(launchForce.magnitude, 0.1f);
		if (grounded)
		{
			base.rigidbody.AddForce(Vector3.up * Mathf.Min(launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
		}
		base.rigidbody.AddForce(launchForce * num * 0.1f, ForceMode.Impulse);
	}

	public void hookedByHuman(int hooker, Vector3 hookPosition)
	{
		base.photonView.RPC("RPCHookedByHuman", base.photonView.owner, hooker, hookPosition);
	}

	[RPC]
	private void RPCHookedByHuman(int hooker, Vector3 hookPosition)
	{
		hookBySomeOne = true;
		badGuy = PhotonView.Find(hooker).gameObject;
		if (Vector3.Distance(hookPosition, base.transform.position) < 15f)
		{
			launchForce = PhotonView.Find(hooker).gameObject.transform.position - base.transform.position;
			base.rigidbody.AddForce(-base.rigidbody.velocity * 0.9f, ForceMode.VelocityChange);
			float num = Mathf.Pow(launchForce.magnitude, 0.1f);
			if (grounded)
			{
				base.rigidbody.AddForce(Vector3.up * Mathf.Min(launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
			}
			base.rigidbody.AddForce(launchForce * num * 0.1f, ForceMode.Impulse);
			if (state != HERO_STATE.Grab)
			{
				dashTime = 1f;
				crossFade("dash", 0.05f);
				base.animation["dash"].time = 0.1f;
				state = HERO_STATE.AirDodge;
				falseAttack();
				facingDirection = Mathf.Atan2(launchForce.x, launchForce.z) * 57.29578f;
				Quaternion quaternion = Quaternion.Euler(0f, facingDirection, 0f);
				base.gameObject.transform.rotation = quaternion;
				quaternion = quaternion;
				base.rigidbody.rotation = quaternion;
				targetRotation = quaternion;
			}
		}
		else
		{
			hookBySomeOne = false;
			badGuy = null;
			PhotonView.Find(hooker).RPC("hookFail", PhotonView.Find(hooker).owner);
		}
	}

	[RPC]
	public void hookFail()
	{
		hookTarget = null;
		hookSomeOne = false;
	}

	[RPC]
	public void badGuyReleaseMe()
	{
		hookBySomeOne = false;
		badGuy = null;
	}

	private void releaseIfIHookSb()
	{
		if (hookSomeOne && hookTarget != null)
		{
			hookTarget.GetPhotonView().RPC("badGuyReleaseMe", hookTarget.GetPhotonView().owner);
			hookTarget = null;
			hookSomeOne = false;
		}
	}

	public void launch(Vector3 des, bool left = true, bool leviMode = false)
	{
		if (isMounted)
		{
			unmounted();
		}
		if (state != HERO_STATE.Attack)
		{
			idle();
		}
		Vector3 vector = des - base.transform.position;
		if (left)
		{
			launchPointLeft = des;
		}
		else
		{
			launchPointRight = des;
		}
		vector.Normalize();
		vector *= 20f;
		if (bulletLeft != null && bulletRight != null && bulletLeft.GetComponent<Bullet>().isHooked() && bulletRight.GetComponent<Bullet>().isHooked())
		{
			vector *= 0.8f;
		}
		leviMode = ((base.animation.IsPlaying("attack5") || base.animation.IsPlaying("special_petra")) ? true : false);
		if (!leviMode)
		{
			falseAttack();
			idle();
			if (useGun)
			{
				crossFade("AHSS_hook_forward_both", 0.1f);
			}
			else if (left && !isRightHandHooked)
			{
				crossFade("air_hook_l_just", 0.1f);
			}
			else if (!left && !isLeftHandHooked)
			{
				crossFade("air_hook_r_just", 0.1f);
			}
			else
			{
				crossFade("dash", 0.1f);
				base.animation["dash"].time = 0f;
			}
		}
		if (left)
		{
			isLaunchLeft = true;
		}
		if (!left)
		{
			isLaunchRight = true;
		}
		launchForce = vector;
		if (!leviMode)
		{
			if (vector.y < 30f)
			{
				launchForce += Vector3.up * (30f - vector.y);
			}
			if (des.y >= base.transform.position.y)
			{
				launchForce += Vector3.up * (des.y - base.transform.position.y) * 10f;
			}
			base.rigidbody.AddForce(launchForce);
		}
		facingDirection = Mathf.Atan2(launchForce.x, launchForce.z) * 57.29578f;
		Quaternion quaternion = Quaternion.Euler(0f, facingDirection, 0f);
		base.gameObject.transform.rotation = quaternion;
		quaternion = quaternion;
		base.rigidbody.rotation = quaternion;
		targetRotation = quaternion;
		if (left)
		{
			launchElapsedTimeL = 0f;
		}
		else
		{
			launchElapsedTimeR = 0f;
		}
		if (leviMode)
		{
			launchElapsedTimeR = -100f;
		}
		if (base.animation.IsPlaying("special_petra"))
		{
			launchElapsedTimeR = -100f;
			launchElapsedTimeL = -100f;
			if ((bool)bulletRight)
			{
				bulletRight.GetComponent<Bullet>().disable();
				releaseIfIHookSb();
			}
			if ((bool)bulletLeft)
			{
				bulletLeft.GetComponent<Bullet>().disable();
				releaseIfIHookSb();
			}
		}
		sparks.enableEmission = false;
	}

	public void lateUpdate()
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && myNetWorkName != null)
		{
			if (titanForm && eren_titan != null)
			{
				myNetWorkName.transform.localPosition = Vector3.up * Screen.height * 2f;
			}
			Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y + 2f, base.transform.position.z);
			GameObject gameObject = GameObject.Find("MainCamera");
			LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
			LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
			LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
			if (Vector3.Angle(gameObject.transform.forward, vector - gameObject.transform.position) > 90f || Physics.Linecast(vector, gameObject.transform.position, layerMask3))
			{
				myNetWorkName.transform.localPosition = Vector3.up * Screen.height * 2f;
			}
			else
			{
				Vector2 vector2 = GameObject.Find("MainCamera").GetComponent<Camera>().WorldToScreenPoint(vector);
				myNetWorkName.transform.localPosition = new Vector3((int)(vector2.x - (float)Screen.width * 0.5f), (int)(vector2.y - (float)Screen.height * 0.5f), 0f);
			}
		}
		if (titanForm)
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.cameraTilt == 1 && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine))
		{
			Vector3 vector3 = Vector3.zero;
			Vector3 vector4 = Vector3.zero;
			if (isLaunchLeft && bulletLeft != null && bulletLeft.GetComponent<Bullet>().isHooked())
			{
				vector3 = bulletLeft.transform.position;
			}
			if (isLaunchRight && bulletRight != null && bulletRight.GetComponent<Bullet>().isHooked())
			{
				vector4 = bulletRight.transform.position;
			}
			Vector3 vector5 = Vector3.zero;
			if (vector3.magnitude != 0f && vector4.magnitude == 0f)
			{
				vector5 = vector3;
			}
			else if (vector3.magnitude == 0f && vector4.magnitude != 0f)
			{
				vector5 = vector4;
			}
			else if (vector3.magnitude != 0f && vector4.magnitude != 0f)
			{
				vector5 = (vector3 + vector4) * 0.5f;
			}
			Vector3 vector6 = Vector3.Project(vector5 - base.transform.position, GameObject.Find("MainCamera").transform.up);
			Vector3 vector7 = Vector3.Project(vector5 - base.transform.position, GameObject.Find("MainCamera").transform.right);
			Quaternion to2;
			if (vector5.magnitude > 0f)
			{
				Vector3 to = vector6 + vector7;
				float num = Vector3.Angle(vector5 - base.transform.position, base.rigidbody.velocity);
				num *= 0.005f;
				to2 = Quaternion.Euler(GameObject.Find("MainCamera").transform.rotation.eulerAngles.x, GameObject.Find("MainCamera").transform.rotation.eulerAngles.y, (!((GameObject.Find("MainCamera").transform.right + vector7.normalized).magnitude < 1f)) ? ((0f - Vector3.Angle(vector6, to)) * num) : (Vector3.Angle(vector6, to) * num));
			}
			else
			{
				to2 = Quaternion.Euler(GameObject.Find("MainCamera").transform.rotation.eulerAngles.x, GameObject.Find("MainCamera").transform.rotation.eulerAngles.y, 0f);
			}
			GameObject.Find("MainCamera").transform.rotation = Quaternion.Lerp(GameObject.Find("MainCamera").transform.rotation, to2, Time.deltaTime * 2f);
		}
		if (state == HERO_STATE.Grab && (bool)titanWhoGrabMe)
		{
			if ((bool)titanWhoGrabMe.GetComponent<TITAN>())
			{
				base.transform.position = titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.position;
				base.transform.rotation = titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.rotation;
			}
			else if ((bool)titanWhoGrabMe.GetComponent<FEMALE_TITAN>())
			{
				base.transform.position = titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.position;
				base.transform.rotation = titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.rotation;
			}
		}
		if (useGun)
		{
			if (leftArmAim || rightArmAim)
			{
				Vector3 vector8 = gunTarget - base.transform.position;
				float current = (0f - Mathf.Atan2(vector8.z, vector8.x)) * 57.29578f;
				float num2 = 0f - Mathf.DeltaAngle(current, base.transform.rotation.eulerAngles.y - 90f);
				headMovement();
				if (!isLeftHandHooked && leftArmAim && num2 < 40f && num2 > -90f)
				{
					leftArmAimTo(gunTarget);
				}
				if (!isRightHandHooked && rightArmAim && num2 > -40f && num2 < 90f)
				{
					rightArmAimTo(gunTarget);
				}
			}
			else if (!grounded)
			{
				handL.localRotation = Quaternion.Euler(90f, 0f, 0f);
				handR.localRotation = Quaternion.Euler(-90f, 0f, 0f);
			}
			if (isLeftHandHooked && bulletLeft != null)
			{
				leftArmAimTo(bulletLeft.transform.position);
			}
			if (isRightHandHooked && bulletRight != null)
			{
				rightArmAimTo(bulletRight.transform.position);
			}
		}
		setHookedPplDirection();
		bodyLean();
		if (!base.animation.IsPlaying("attack3_2") && !base.animation.IsPlaying("attack5") && !base.animation.IsPlaying("special_petra"))
		{
			base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, targetRotation, Time.deltaTime * 6f);
		}
	}

	private void leftArmAimTo(Vector3 target)
	{
		float num = target.x - upperarmL.transform.position.x;
		float y = target.y - upperarmL.transform.position.y;
		float num2 = target.z - upperarmL.transform.position.z;
		float x = Mathf.Sqrt(num * num + num2 * num2);
		handL.localRotation = Quaternion.Euler(90f, 0f, 0f);
		forearmL.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		upperarmL.rotation = Quaternion.Euler(0f, 90f + Mathf.Atan2(num, num2) * 57.29578f, (0f - Mathf.Atan2(y, x)) * 57.29578f);
	}

	private void rightArmAimTo(Vector3 target)
	{
		float num = target.x - upperarmR.transform.position.x;
		float y = target.y - upperarmR.transform.position.y;
		float num2 = target.z - upperarmR.transform.position.z;
		float x = Mathf.Sqrt(num * num + num2 * num2);
		handR.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		forearmR.localRotation = Quaternion.Euler(90f, 0f, 0f);
		upperarmR.rotation = Quaternion.Euler(180f, 90f + Mathf.Atan2(num, num2) * 57.29578f, Mathf.Atan2(y, x) * 57.29578f);
	}

	private void headMovement()
	{
		Transform transform = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
		Transform transform2 = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck");
		float x = Mathf.Sqrt((gunTarget.x - base.transform.position.x) * (gunTarget.x - base.transform.position.x) + (gunTarget.z - base.transform.position.z) * (gunTarget.z - base.transform.position.z));
		targetHeadRotation = transform.rotation;
		Vector3 vector = gunTarget - base.transform.position;
		float current = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
		float value = 0f - Mathf.DeltaAngle(current, base.transform.rotation.eulerAngles.y - 90f);
		value = Mathf.Clamp(value, -40f, 40f);
		float y = transform2.position.y - gunTarget.y;
		float value2 = Mathf.Atan2(y, x) * 57.29578f;
		value2 = Mathf.Clamp(value2, -40f, 30f);
		targetHeadRotation = Quaternion.Euler(transform.rotation.eulerAngles.x + value2, transform.rotation.eulerAngles.y + value, transform.rotation.eulerAngles.z);
		oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 60f);
		transform.rotation = oldHeadRotation;
	}

	private float CalculateJumpVerticalSpeed()
	{
		return Mathf.Sqrt(2f * jumpHeight * gravity);
	}

	public void shootFlare(int type)
	{
		bool flag = false;
		if (type == 1 && flare1CD == 0f)
		{
			flare1CD = flareTotalCD;
			flag = true;
		}
		if (type == 2 && flare2CD == 0f)
		{
			flare2CD = flareTotalCD;
			flag = true;
		}
		if (type == 3 && flare3CD == 0f)
		{
			flare3CD = flareTotalCD;
			flag = true;
		}
		if (flag)
		{
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/flareBullet" + type), base.transform.position, base.transform.rotation);
				gameObject.GetComponent<FlareMovement>().dontShowHint();
				UnityEngine.Object.Destroy(gameObject, 25f);
			}
			else
			{
				GameObject gameObject2 = PhotonNetwork.Instantiate("FX/flareBullet" + type, base.transform.position, base.transform.rotation, 0);
				gameObject2.GetComponent<FlareMovement>().dontShowHint();
			}
		}
	}

	public bool HasDied()
	{
		return hasDied || isInvincible();
	}

	public void markDie()
	{
		hasDied = true;
		state = HERO_STATE.Die;
	}

	[RPC]
	public void netDie(Vector3 v, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true)
	{
		if (base.photonView.isMine && titanForm && eren_titan != null)
		{
			eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
		}
		if ((bool)bulletLeft)
		{
			bulletLeft.GetComponent<Bullet>().removeMe();
		}
		if ((bool)bulletRight)
		{
			bulletRight.GetComponent<Bullet>().removeMe();
		}
		meatDie.Play();
		if (!useGun && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine))
		{
			leftbladetrail.Deactivate();
			rightbladetrail.Deactivate();
			leftbladetrail2.Deactivate();
			rightbladetrail2.Deactivate();
		}
		falseAttack();
		breakApart(v, isBite);
		if (base.photonView.isMine)
		{
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().myRespawnTime = 0f;
		}
		hasDied = true;
		Transform transform = base.transform.Find("audio_die");
		transform.parent = null;
		transform.GetComponent<AudioSource>().Play();
		base.gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
		if (base.photonView.isMine)
		{
			PhotonNetwork.RemoveRPCs(base.photonView);
			PhotonNetwork.player.SetCustomProperties(new Hashtable { 
			{
				PhotonPlayerProperty.dead,
				true
			} });
			PhotonNetwork.player.SetCustomProperties(new Hashtable { 
			{
				PhotonPlayerProperty.deaths,
				(int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths] + 1
			} });
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("someOneIsDead", PhotonTargets.MasterClient, (!(titanName == string.Empty)) ? 1 : 0);
			if (viewID != -1)
			{
				PhotonView photonView = PhotonView.Find(viewID);
				if (photonView != null)
				{
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(killByTitan, (string)photonView.owner.customProperties[PhotonPlayerProperty.name], false, (string)PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
					photonView.owner.SetCustomProperties(new Hashtable { 
					{
						PhotonPlayerProperty.kills,
						(int)photonView.owner.customProperties[PhotonPlayerProperty.kills] + 1
					} });
				}
			}
			else
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo((!(titanName == string.Empty)) ? true : false, titanName, false, (string)PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
			}
		}
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
	}

	public void die(Vector3 v, bool isBite)
	{
		if (!(invincible > 0f))
		{
			if (titanForm && eren_titan != null)
			{
				eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
			}
			if ((bool)bulletLeft)
			{
				bulletLeft.GetComponent<Bullet>().removeMe();
			}
			if ((bool)bulletRight)
			{
				bulletRight.GetComponent<Bullet>().removeMe();
			}
			meatDie.Play();
			if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine) && !useGun)
			{
				leftbladetrail.Deactivate();
				rightbladetrail.Deactivate();
				leftbladetrail2.Deactivate();
				rightbladetrail2.Deactivate();
			}
			breakApart(v, isBite);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameLose();
			falseAttack();
			hasDied = true;
			Transform transform = base.transform.Find("audio_die");
			transform.parent = null;
			transform.GetComponent<AudioSource>().Play();
			if (PlayerPrefs.HasKey("EnableSS") && PlayerPrefs.GetInt("EnableSS") == 1)
			{
				GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().startSnapShot(base.transform.position, 0);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void breakApart(Vector3 v, bool isBite)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), base.transform.position, base.transform.rotation);
		gameObject.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
		gameObject.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, base.animation[currentAnimation].normalizedTime, BODY_PARTS.ARM_R);
		if (!isBite)
		{
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), base.transform.position, base.transform.rotation);
			GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), base.transform.position, base.transform.rotation);
			GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/AOTTG_HERO_body"), base.transform.position, base.transform.rotation);
			gameObject2.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
			gameObject3.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
			gameObject4.gameObject.GetComponent<HERO_SETUP>().myCostume = setup.myCostume;
			gameObject2.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, base.animation[currentAnimation].normalizedTime, BODY_PARTS.UPPER);
			gameObject3.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, base.animation[currentAnimation].normalizedTime, BODY_PARTS.LOWER);
			gameObject4.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation, base.animation[currentAnimation].normalizedTime, BODY_PARTS.ARM_L);
			applyForceToBody(gameObject2, v);
			applyForceToBody(gameObject3, v);
			applyForceToBody(gameObject4, v);
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
			{
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(gameObject2, false);
			}
		}
		else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
		{
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(gameObject, false);
		}
		applyForceToBody(gameObject, v);
		Transform transform = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L").transform;
		Transform transform2 = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R").transform;
		GameObject gameObject5;
		GameObject gameObject6;
		GameObject gameObject7;
		GameObject gameObject8;
		GameObject gameObject9;
		if (useGun)
		{
			gameObject5 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_l"), transform.position, transform.rotation);
			gameObject6 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_r"), transform2.position, transform2.rotation);
			gameObject7 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_3dmg_2"), base.transform.position, base.transform.rotation);
			gameObject8 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_mag_l"), base.transform.position, base.transform.rotation);
			gameObject9 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_gun_mag_r"), base.transform.position, base.transform.rotation);
		}
		else
		{
			gameObject5 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_blade_l"), transform.position, transform.rotation);
			gameObject6 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_blade_r"), transform2.position, transform2.rotation);
			gameObject7 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_3dmg"), base.transform.position, base.transform.rotation);
			gameObject8 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_3dmg_gas_l"), base.transform.position, base.transform.rotation);
			gameObject9 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Character_parts/character_3dmg_gas_r"), base.transform.position, base.transform.rotation);
		}
		gameObject5.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		gameObject6.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		gameObject7.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		gameObject8.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		gameObject9.renderer.material = CharacterMaterials.materials[setup.myCostume._3dmg_texture];
		applyForceToBody(gameObject5, v);
		applyForceToBody(gameObject6, v);
		applyForceToBody(gameObject7, v);
		applyForceToBody(gameObject8, v);
		applyForceToBody(gameObject9, v);
	}

	private void applyForceToBody(GameObject GO, Vector3 v)
	{
		GO.rigidbody.AddForce(v);
		GO.rigidbody.AddTorque(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
	}

	[RPC]
	private void netDie2(int viewID = -1, string titanName = "")
	{
		if (base.photonView.isMine)
		{
			PhotonNetwork.RemoveRPCs(base.photonView);
			if (titanForm && eren_titan != null)
			{
				eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
			}
		}
		meatDie.Play();
		if ((bool)bulletLeft)
		{
			bulletLeft.GetComponent<Bullet>().removeMe();
		}
		if ((bool)bulletRight)
		{
			bulletRight.GetComponent<Bullet>().removeMe();
		}
		Transform transform = base.transform.Find("audio_die");
		transform.parent = null;
		transform.GetComponent<AudioSource>().Play();
		if (base.photonView.isMine)
		{
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().myRespawnTime = 0f;
		}
		falseAttack();
		hasDied = true;
		base.gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			PhotonNetwork.RemoveRPCs(base.photonView);
			PhotonNetwork.player.SetCustomProperties(new Hashtable { 
			{
				PhotonPlayerProperty.dead,
				true
			} });
			PhotonNetwork.player.SetCustomProperties(new Hashtable { 
			{
				PhotonPlayerProperty.deaths,
				(int)PhotonNetwork.player.customProperties[PhotonPlayerProperty.deaths] + 1
			} });
			if (viewID != -1)
			{
				PhotonView photonView = PhotonView.Find(viewID);
				if (photonView != null)
				{
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(true, (string)photonView.owner.customProperties[PhotonPlayerProperty.name], false, (string)PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
					photonView.owner.SetCustomProperties(new Hashtable { 
					{
						PhotonPlayerProperty.kills,
						(int)photonView.owner.customProperties[PhotonPlayerProperty.kills] + 1
					} });
				}
			}
			else
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(true, titanName, false, (string)PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
			}
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("someOneIsDead", PhotonTargets.MasterClient, (!(titanName == string.Empty)) ? 1 : 0);
		}
		GameObject gameObject = ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("hitMeat2"))) : PhotonNetwork.Instantiate("hitMeat2", base.transform.position, Quaternion.Euler(270f, 0f, 0f), 0));
		gameObject.transform.position = base.transform.position;
		if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
	}

	public void die2(Transform tf)
	{
		if (!(invincible > 0f))
		{
			if (titanForm && eren_titan != null)
			{
				eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
			}
			if ((bool)bulletLeft)
			{
				bulletLeft.GetComponent<Bullet>().removeMe();
			}
			if ((bool)bulletRight)
			{
				bulletRight.GetComponent<Bullet>().removeMe();
			}
			Transform transform = base.transform.Find("audio_die");
			transform.parent = null;
			transform.GetComponent<AudioSource>().Play();
			meatDie.Play();
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
			currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameLose();
			falseAttack();
			hasDied = true;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("hitMeat2"));
			gameObject.transform.position = base.transform.position;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	[RPC]
	private void netTauntAttack(float tauntTime, float distance = 100f)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (Vector3.Distance(gameObject.transform.position, base.transform.position) < distance && (bool)gameObject.GetComponent<TITAN>())
			{
				gameObject.GetComponent<TITAN>().beTauntedBy(base.gameObject, tauntTime);
			}
		}
	}

	[RPC]
	private void netlaughAttack()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (Vector3.Distance(gameObject.transform.position, base.transform.position) < 50f && Vector3.Angle(gameObject.transform.forward, base.transform.position - gameObject.transform.position) < 90f && (bool)gameObject.GetComponent<TITAN>())
			{
				gameObject.GetComponent<TITAN>().beLaughAttacked();
			}
		}
	}

	[RPC]
	private void net3DMGSMOKE(bool ifON)
	{
		if (smoke_3dmg != null)
		{
			smoke_3dmg.enableEmission = ifON;
		}
	}

	[RPC]
	private void showHitDamage()
	{
		GameObject gameObject = GameObject.Find("LabelScore");
		if ((bool)gameObject)
		{
			speed = Mathf.Max(10f, speed);
			gameObject.GetComponent<UILabel>().text = speed.ToString();
			gameObject.transform.localScale = Vector3.zero;
			speed = (int)(speed * 0.1f);
			speed = Mathf.Clamp(speed, 40f, 150f);
			iTween.Stop(gameObject);
			iTween.ScaleTo(gameObject, iTween.Hash("x", speed, "y", speed, "z", speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f));
			iTween.ScaleTo(gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f));
		}
	}

	[RPC]
	public void blowAway(Vector3 force)
	{
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
		{
			base.rigidbody.AddForce(force, ForceMode.Impulse);
			base.transform.LookAt(base.transform.position);
		}
	}

	[RPC]
	private void killObject()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void suicide()
	{
		if (IN_GAME_MAIN_CAMERA.gametype != 0)
		{
			netDie(base.rigidbody.velocity * 50f, false, -1, string.Empty);
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().needChooseSide = true;
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().justSuicide = true;
		}
	}

	private float getGlobalFacingDirection(float horizontal, float vertical)
	{
		if (vertical == 0f && horizontal == 0f)
		{
			return base.transform.rotation.eulerAngles.y;
		}
		float y = currentCamera.transform.rotation.eulerAngles.y;
		float num = Mathf.Atan2(vertical, horizontal) * 57.29578f;
		num = 0f - num + 90f;
		return y + num;
	}

	private Vector3 getGlobaleFacingVector3(float horizontal, float vertical)
	{
		float num = 0f - getGlobalFacingDirection(horizontal, vertical) + 90f;
		float x = Mathf.Cos(num * ((float)Math.PI / 180f));
		float z = Mathf.Sin(num * ((float)Math.PI / 180f));
		return new Vector3(x, 0f, z);
	}

	private Vector3 getGlobaleFacingVector3(float resultAngle)
	{
		float num = 0f - resultAngle + 90f;
		float x = Mathf.Cos(num * ((float)Math.PI / 180f));
		float z = Mathf.Sin(num * ((float)Math.PI / 180f));
		return new Vector3(x, 0f, z);
	}

	private void getOnHorse()
	{
		playAnimation("horse_geton");
		facingDirection = myHorse.transform.rotation.eulerAngles.y;
		targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
	}

	private void getOffHorse()
	{
		playAnimation("horse_getoff");
		base.rigidbody.AddForce(Vector3.up * 10f - base.transform.forward * 2f - base.transform.right * 1f, ForceMode.VelocityChange);
		unmounted();
	}

	private void unmounted()
	{
		myHorse.GetComponent<Horse>().unmounted();
		isMounted = false;
	}
}
