using System;
using System.Collections;
using Photon;
using UnityEngine;

public class TITAN_EREN : Photon.MonoBehaviour
{
	public FengCustomInputs inputManager;

	public Camera currentCamera;

	public float speed = 80f;

	private float gravity = 500f;

	public float maxVelocityChange = 100f;

	public bool canJump = true;

	public float jumpHeight = 2f;

	private bool grounded;

	private float facingDirection;

	private bool justGrounded;

	private bool isAttack;

	public bool isHit;

	private bool isNextAttack;

	private string attackAnimation;

	public bool hasDied;

	private Vector3 dashDirection;

	private GameObject myNetWorkName;

	private float sqrt2 = Mathf.Sqrt(2f);

	private float myR;

	public GameObject bottomObject;

	private Vector3 oldCorePosition;

	private Transform attackBox;

	private ArrayList hitTargets;

	private bool attackChkOnce;

	private float hitPause;

	public GameObject realBody;

	public float lifeTime = 9999f;

	private float lifeTimeMax = 9999f;

	public bool rockLift;

	private bool hasDieSteam;

	private float dieTime;

	private int stepSoundPhase = 2;

	private bool isPlayRoar;

	private bool needFreshCorePosition;

	private bool needRoar;

	private string hitAnimation;

	private int rockPhase;

	private float waitCounter;

	private Vector3 targetCheckPt;

	private ArrayList checkPoints = new ArrayList();

	public GameObject rock;

	private bool isROCKMOVE;

	private bool rockHitGround;

	private bool isHitWhileCarryingRock;

	private void OnDestroy()
	{
		if (GameObject.Find("MultiplayerManager") != null)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().removeET(this);
		}
	}

	private void Start()
	{
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().addET(this);
		if (rockLift)
		{
			rock = GameObject.Find("rock");
			rock.animation["lift"].speed = 0f;
			return;
		}
		currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
		inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
		oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
		myR = sqrt2 * 6f;
		base.animation["hit_annie_1"].speed = 0.8f;
		base.animation["hit_annie_2"].speed = 0.7f;
		base.animation["hit_annie_3"].speed = 0.7f;
	}

	public bool IsGrounded()
	{
		return bottomObject.GetComponent<CheckHitGround>().isGrounded;
	}

	public void playAnimation(string aniName)
	{
		base.animation.Play(aniName);
		if (PhotonNetwork.connected && base.photonView.isMine)
		{
			base.photonView.RPC("netPlayAnimation", PhotonTargets.Others, aniName);
		}
	}

	private void playAnimationAt(string aniName, float normalizedTime)
	{
		base.animation.Play(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
		if (PhotonNetwork.connected && base.photonView.isMine)
		{
			base.photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, aniName, normalizedTime);
		}
	}

	private void crossFade(string aniName, float time)
	{
		base.animation.CrossFade(aniName, time);
		if (PhotonNetwork.connected && base.photonView.isMine)
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
	private void removeMe()
	{
		PhotonNetwork.RemoveRPCs(base.photonView);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void update()
	{
		if ((IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) || rockLift)
		{
			return;
		}
		if (base.animation.IsPlaying("run"))
		{
			if (base.animation["run"].normalizedTime % 1f > 0.3f && base.animation["run"].normalizedTime % 1f < 0.75f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform transform = base.transform.Find("snd_eren_foot");
				transform.GetComponent<AudioSource>().Stop();
				transform.GetComponent<AudioSource>().Play();
			}
			if (base.animation["run"].normalizedTime % 1f > 0.75f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform transform2 = base.transform.Find("snd_eren_foot");
				transform2.GetComponent<AudioSource>().Stop();
				transform2.GetComponent<AudioSource>().Play();
			}
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			return;
		}
		if (hasDied)
		{
			if (!(base.animation["die"].normalizedTime >= 1f) && !(hitAnimation == "hit_annie_3"))
			{
				return;
			}
			if (realBody != null)
			{
				realBody.GetComponent<HERO>().backToHuman();
				realBody.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position + Vector3.up * 2f;
				realBody = null;
			}
			dieTime += Time.deltaTime;
			if (dieTime > 2f && !hasDieSteam)
			{
				hasDieSteam = true;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie1"));
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
					GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie"));
					gameObject3.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
					gameObject3.transform.localScale = base.transform.localScale;
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else if (base.photonView.isMine)
				{
					GameObject gameObject4 = PhotonNetwork.Instantiate("FX/FXtitanDie", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0);
					gameObject4.transform.localScale = base.transform.localScale;
					PhotonNetwork.Destroy(base.photonView);
				}
			}
		}
		else
		{
			if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
			{
				return;
			}
			if (isHit)
			{
				if (base.animation[hitAnimation].normalizedTime >= 1f)
				{
					isHit = false;
					falseAttack();
					playAnimation("idle");
				}
				return;
			}
			if (lifeTime > 0f)
			{
				lifeTime -= Time.deltaTime;
				if (lifeTime <= 0f)
				{
					hasDied = true;
					playAnimation("die");
					return;
				}
			}
			if (grounded && !isAttack && !base.animation.IsPlaying("jump_land") && !isAttack && !base.animation.IsPlaying("born"))
			{
				if (inputManager.isInputDown[InputCode.attack0] || inputManager.isInputDown[InputCode.attack1])
				{
					bool flag = false;
					if ((IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.WOW && inputManager.isInput[InputCode.down]) || inputManager.isInputDown[InputCode.attack1])
					{
						if (IN_GAME_MAIN_CAMERA.cameraMode == CAMERA_TYPE.WOW && inputManager.isInputDown[InputCode.attack1] && inputManager.inputKey[11] == KeyCode.Mouse1)
						{
							flag = true;
						}
						if (flag)
						{
							flag = true;
						}
						else
						{
							attackAnimation = "attack_kick";
						}
					}
					else
					{
						attackAnimation = "attack_combo_001";
					}
					if (!flag)
					{
						playAnimation(attackAnimation);
						base.animation[attackAnimation].time = 0f;
						isAttack = true;
						needFreshCorePosition = true;
						if (attackAnimation == "attack_combo_001" || attackAnimation == "attack_combo_001")
						{
							attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
						}
						else if (attackAnimation == "attack_combo_002")
						{
							attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
						}
						else if (attackAnimation == "attack_kick")
						{
							attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
						}
						hitTargets = new ArrayList();
					}
				}
				if (inputManager.isInputDown[InputCode.salute])
				{
					crossFade("born", 0.1f);
					base.animation["born"].normalizedTime = 0.28f;
					isPlayRoar = false;
				}
			}
			if (!isAttack)
			{
				if ((grounded || base.animation.IsPlaying("idle")) && !base.animation.IsPlaying("jump_start") && !base.animation.IsPlaying("jump_air") && !base.animation.IsPlaying("jump_land") && inputManager.isInput[InputCode.bothRope])
				{
					crossFade("jump_start", 0.1f);
				}
			}
			else
			{
				if (base.animation[attackAnimation].time >= 0.1f && inputManager.isInputDown[InputCode.attack0])
				{
					isNextAttack = true;
				}
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				string text = string.Empty;
				if (attackAnimation == "attack_combo_001")
				{
					num = 0.4f;
					num2 = 0.5f;
					num3 = 0.66f;
					text = "attack_combo_002";
				}
				else if (attackAnimation == "attack_combo_002")
				{
					num = 0.15f;
					num2 = 0.25f;
					num3 = 0.43f;
					text = "attack_combo_003";
				}
				else if (attackAnimation == "attack_combo_003")
				{
					num3 = 0f;
					num = 0.31f;
					num2 = 0.37f;
				}
				else if (attackAnimation == "attack_kick")
				{
					num3 = 0f;
					num = 0.32f;
					num2 = 0.38f;
				}
				else
				{
					num = 0.5f;
					num2 = 0.85f;
				}
				if (hitPause > 0f)
				{
					hitPause -= Time.deltaTime;
					if (hitPause <= 0f)
					{
						base.animation[attackAnimation].speed = 1f;
						hitPause = 0f;
					}
				}
				if (num3 > 0f && isNextAttack && base.animation[attackAnimation].normalizedTime >= num3)
				{
					if (hitTargets.Count > 0)
					{
						Transform transform3 = (Transform)hitTargets[0];
						if ((bool)transform3)
						{
							base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(transform3.position - base.transform.position).eulerAngles.y, 0f);
							facingDirection = base.transform.rotation.eulerAngles.y;
						}
					}
					falseAttack();
					attackAnimation = text;
					crossFade(attackAnimation, 0.1f);
					base.animation[attackAnimation].time = 0f;
					base.animation[attackAnimation].speed = 1f;
					isAttack = true;
					needFreshCorePosition = true;
					if (attackAnimation == "attack_combo_002")
					{
						attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
					}
					else if (attackAnimation == "attack_combo_003")
					{
						attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
					}
					hitTargets = new ArrayList();
				}
				if ((base.animation[attackAnimation].normalizedTime >= num && base.animation[attackAnimation].normalizedTime <= num2) || (!attackChkOnce && base.animation[attackAnimation].normalizedTime >= num))
				{
					if (!attackChkOnce)
					{
						if (attackAnimation == "attack_combo_002")
						{
							playSound("snd_eren_swing2");
						}
						else if (attackAnimation == "attack_combo_001")
						{
							playSound("snd_eren_swing1");
						}
						else if (attackAnimation == "attack_combo_003")
						{
							playSound("snd_eren_swing3");
						}
						attackChkOnce = true;
					}
					Collider[] array = Physics.OverlapSphere(attackBox.transform.position, 8f);
					for (int i = 0; i < array.Length; i++)
					{
						if (!array[i].gameObject.transform.root.GetComponent<TITAN>())
						{
							continue;
						}
						bool flag2 = false;
						for (int j = 0; j < hitTargets.Count; j++)
						{
							if (array[i].gameObject.transform.root == hitTargets[j])
							{
								flag2 = true;
								break;
							}
						}
						if (flag2 || array[i].gameObject.transform.root.GetComponent<TITAN>().hasDie)
						{
							continue;
						}
						base.animation[attackAnimation].speed = 0f;
						if (attackAnimation == "attack_combo_002")
						{
							hitPause = 0.05f;
							array[i].gameObject.transform.root.GetComponent<TITAN>().hitL(base.transform.position, hitPause);
							currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(1f, 0.03f);
						}
						else if (attackAnimation == "attack_combo_001")
						{
							currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(1.2f, 0.04f);
							hitPause = 0.08f;
							array[i].gameObject.transform.root.GetComponent<TITAN>().hitR(base.transform.position, hitPause);
						}
						else if (attackAnimation == "attack_combo_003")
						{
							currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(3f, 0.1f);
							hitPause = 0.3f;
							array[i].gameObject.transform.root.GetComponent<TITAN>().dieHeadBlow(base.transform.position, hitPause);
						}
						else if (attackAnimation == "attack_kick")
						{
							currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(3f, 0.1f);
							hitPause = 0.2f;
							if (array[i].gameObject.transform.root.GetComponent<TITAN>().abnormalType == AbnormalType.TYPE_CRAWLER)
							{
								array[i].gameObject.transform.root.GetComponent<TITAN>().dieBlow(base.transform.position, hitPause);
							}
							else if (array[i].gameObject.transform.root.transform.localScale.x < 2f)
							{
								array[i].gameObject.transform.root.GetComponent<TITAN>().dieBlow(base.transform.position, hitPause);
							}
							else
							{
								array[i].gameObject.transform.root.GetComponent<TITAN>().hitR(base.transform.position, hitPause);
							}
						}
						hitTargets.Add(array[i].gameObject.transform.root);
						if (IN_GAME_MAIN_CAMERA.gametype != 0)
						{
							PhotonNetwork.Instantiate("hitMeatBIG", (array[i].transform.position + attackBox.position) * 0.5f, Quaternion.Euler(270f, 0f, 0f), 0);
						}
						else
						{
							UnityEngine.Object.Instantiate(Resources.Load("hitMeatBIG"), (array[i].transform.position + attackBox.position) * 0.5f, Quaternion.Euler(270f, 0f, 0f));
						}
					}
				}
				if (base.animation[attackAnimation].normalizedTime >= 1f)
				{
					falseAttack();
					playAnimation("idle");
				}
			}
			if (base.animation.IsPlaying("jump_land") && base.animation["jump_land"].normalizedTime >= 1f)
			{
				crossFade("idle", 0.1f);
			}
			if (base.animation.IsPlaying("born"))
			{
				if (base.animation["born"].normalizedTime >= 0.28f && !isPlayRoar)
				{
					isPlayRoar = true;
					playSound("snd_eren_roar");
				}
				if (base.animation["born"].normalizedTime >= 0.5f && base.animation["born"].normalizedTime <= 0.7f)
				{
					currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().startShake(0.5f, 1f);
				}
				if (base.animation["born"].normalizedTime >= 1f)
				{
					crossFade("idle", 0.1f);
					if (IN_GAME_MAIN_CAMERA.gametype != 0)
					{
						if (PhotonNetwork.isMasterClient)
						{
							base.photonView.RPC("netTauntAttack", PhotonTargets.MasterClient, 10f, 500f);
						}
						else
						{
							netTauntAttack(10f, 500f);
						}
					}
					else
					{
						netTauntAttack(10f, 500f);
					}
				}
			}
			showAimUI();
			showSkillCD();
		}
	}

	private void showSkillCD()
	{
		GameObject.Find("skill_cd_eren").GetComponent<UISprite>().fillAmount = lifeTime / lifeTimeMax;
	}

	private void playSound(string sndname)
	{
		playsoundRPC(sndname);
		if (IN_GAME_MAIN_CAMERA.gametype != 0)
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

	private void showAimUI()
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
	}

	public void lateUpdate()
	{
		if ((!IN_GAME_MAIN_CAMERA.isPausing || IN_GAME_MAIN_CAMERA.gametype != 0) && !rockLift && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine))
		{
			Quaternion to = Quaternion.Euler(GameObject.Find("MainCamera").transform.rotation.eulerAngles.x, GameObject.Find("MainCamera").transform.rotation.eulerAngles.y, 0f);
			GameObject.Find("MainCamera").transform.rotation = Quaternion.Lerp(GameObject.Find("MainCamera").transform.rotation, to, Time.deltaTime * 2f);
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
			if ((bool)gameObject.GetComponent<FEMALE_TITAN>())
			{
				gameObject.GetComponent<FEMALE_TITAN>().erenIsHere(base.gameObject);
			}
		}
	}

	private void falseAttack()
	{
		isAttack = false;
		isNextAttack = false;
		hitTargets = new ArrayList();
		attackChkOnce = false;
	}

	private void FixedUpdate()
	{
		if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			return;
		}
		if (rockLift)
		{
			RockUpdate();
		}
		else
		{
			if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
			{
				return;
			}
			if (hitPause > 0f)
			{
				base.rigidbody.velocity = Vector3.zero;
			}
			else if (hasDied)
			{
				base.rigidbody.velocity = Vector3.zero + Vector3.up * base.rigidbody.velocity.y;
				base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
			}
			else
			{
				if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
				{
					return;
				}
				if (base.rigidbody.velocity.magnitude > 50f)
				{
					currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentCamera.GetComponent<Camera>().fieldOfView, Mathf.Min(100f, base.rigidbody.velocity.magnitude), 0.1f);
				}
				else
				{
					currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentCamera.GetComponent<Camera>().fieldOfView, 50f, 0.1f);
				}
				if (bottomObject.GetComponent<CheckHitGround>().isGrounded)
				{
					if (!grounded)
					{
						justGrounded = true;
					}
					grounded = true;
					bottomObject.GetComponent<CheckHitGround>().isGrounded = false;
				}
				else
				{
					grounded = false;
				}
				float num = 0f;
				float num2 = 0f;
				if (!IN_GAME_MAIN_CAMERA.isTyping)
				{
					num2 = (inputManager.isInput[InputCode.up] ? 1f : ((!inputManager.isInput[InputCode.down]) ? 0f : (-1f)));
					num = (inputManager.isInput[InputCode.left] ? (-1f) : ((!inputManager.isInput[InputCode.right]) ? 0f : 1f));
				}
				if (needFreshCorePosition)
				{
					oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
					needFreshCorePosition = false;
				}
				if (isAttack || isHit)
				{
					Vector3 vector = base.transform.position - base.transform.Find("Amarture/Core").position - oldCorePosition;
					oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
					base.rigidbody.velocity = vector / Time.deltaTime + Vector3.up * base.rigidbody.velocity.y;
					base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, facingDirection, 0f), Time.deltaTime * 10f);
					if (justGrounded)
					{
						justGrounded = false;
					}
				}
				else if (grounded)
				{
					Vector3 vector2 = Vector3.zero;
					if (justGrounded)
					{
						justGrounded = false;
						vector2 = base.rigidbody.velocity;
						if (base.animation.IsPlaying("jump_air"))
						{
							GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/boom2_eren"), base.transform.position, Quaternion.Euler(270f, 0f, 0f));
							gameObject.transform.localScale = Vector3.one * 1.5f;
							if (needRoar)
							{
								playAnimation("born");
								needRoar = false;
								isPlayRoar = false;
							}
							else
							{
								playAnimation("jump_land");
							}
						}
					}
					if (!base.animation.IsPlaying("jump_land") && !isAttack && !isHit && !base.animation.IsPlaying("born"))
					{
						Vector3 vector3 = new Vector3(num, 0f, num2);
						float y = currentCamera.transform.rotation.eulerAngles.y;
						float num3 = Mathf.Atan2(num2, num) * 57.29578f;
						num3 = 0f - num3 + 90f;
						float num4 = y + num3;
						float num5 = 0f - num4 + 90f;
						float x = Mathf.Cos(num5 * ((float)Math.PI / 180f));
						float z = Mathf.Sin(num5 * ((float)Math.PI / 180f));
						vector2 = new Vector3(x, 0f, z);
						float num6 = ((vector3.magnitude > 0.95f) ? 1f : ((!(vector3.magnitude < 0.25f)) ? vector3.magnitude : 0f));
						vector2 *= num6;
						vector2 *= speed;
						if (num != 0f || num2 != 0f)
						{
							if (!base.animation.IsPlaying("run") && !base.animation.IsPlaying("jump_start") && !base.animation.IsPlaying("jump_air"))
							{
								crossFade("run", 0.1f);
							}
						}
						else
						{
							if (!base.animation.IsPlaying("idle") && !base.animation.IsPlaying("dash_land") && !base.animation.IsPlaying("dodge") && !base.animation.IsPlaying("jump_start") && !base.animation.IsPlaying("jump_air") && !base.animation.IsPlaying("jump_land"))
							{
								crossFade("idle", 0.1f);
								vector2 *= 0f;
							}
							num4 = -874f;
						}
						if (num4 != -874f)
						{
							facingDirection = num4;
						}
					}
					Vector3 velocity = base.rigidbody.velocity;
					Vector3 force = vector2 - velocity;
					force.x = Mathf.Clamp(force.x, 0f - maxVelocityChange, maxVelocityChange);
					force.z = Mathf.Clamp(force.z, 0f - maxVelocityChange, maxVelocityChange);
					force.y = 0f;
					if (base.animation.IsPlaying("jump_start") && base.animation["jump_start"].normalizedTime >= 1f)
					{
						playAnimation("jump_air");
						force.y += 240f;
					}
					else if (base.animation.IsPlaying("jump_start"))
					{
						force = -base.rigidbody.velocity;
					}
					base.rigidbody.AddForce(force, ForceMode.VelocityChange);
					base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, facingDirection, 0f), Time.deltaTime * 10f);
				}
				else
				{
					if (base.animation.IsPlaying("jump_start") && base.animation["jump_start"].normalizedTime >= 1f)
					{
						playAnimation("jump_air");
						base.rigidbody.AddForce(Vector3.up * 240f, ForceMode.VelocityChange);
					}
					if (!base.animation.IsPlaying("jump") && !isHit)
					{
						Vector3 vector4 = new Vector3(num, 0f, num2);
						float y2 = currentCamera.transform.rotation.eulerAngles.y;
						float num7 = Mathf.Atan2(num2, num) * 57.29578f;
						num7 = 0f - num7 + 90f;
						float num8 = y2 + num7;
						float num9 = 0f - num8 + 90f;
						float x2 = Mathf.Cos(num9 * ((float)Math.PI / 180f));
						float z2 = Mathf.Sin(num9 * ((float)Math.PI / 180f));
						Vector3 force2 = new Vector3(x2, 0f, z2);
						float num10 = ((vector4.magnitude > 0.95f) ? 1f : ((!(vector4.magnitude < 0.25f)) ? vector4.magnitude : 0f));
						force2 *= num10;
						force2 *= speed * 2f;
						if (num != 0f || num2 != 0f)
						{
							base.rigidbody.AddForce(force2, ForceMode.Impulse);
						}
						else
						{
							num8 = -874f;
						}
						if (num8 != -874f)
						{
							facingDirection = num8;
						}
						if (!base.animation.IsPlaying(string.Empty) && !base.animation.IsPlaying("attack3_2") && !base.animation.IsPlaying("attack5"))
						{
							base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, facingDirection, 0f), Time.deltaTime * 6f);
						}
					}
				}
				base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
			}
		}
	}

	public void born()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if ((bool)gameObject.GetComponent<FEMALE_TITAN>())
			{
				gameObject.GetComponent<FEMALE_TITAN>().erenIsHere(base.gameObject);
			}
		}
		if (!bottomObject.GetComponent<CheckHitGround>().isGrounded)
		{
			playAnimation("jump_air");
			needRoar = true;
		}
		else
		{
			needRoar = false;
			playAnimation("born");
			isPlayRoar = false;
		}
		playSound("snd_eren_shift");
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			UnityEngine.Object.Instantiate(Resources.Load("FX/Thunder"), base.transform.position + Vector3.up * 23f, Quaternion.Euler(270f, 0f, 0f));
		}
		else if (base.photonView.isMine)
		{
			PhotonNetwork.Instantiate("FX/Thunder", base.transform.position + Vector3.up * 23f, Quaternion.Euler(270f, 0f, 0f), 0);
		}
		lifeTimeMax = (lifeTime = 30f);
	}

	public void hitByTitan()
	{
		if (!isHit && !hasDied && !base.animation.IsPlaying("born"))
		{
			if (rockLift)
			{
				crossFade("die", 0.1f);
				isHitWhileCarryingRock = true;
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameLose();
				base.photonView.RPC("rockPlayAnimation", PhotonTargets.All, "set");
			}
			else
			{
				isHit = true;
				hitAnimation = "hit_titan";
				falseAttack();
				playAnimation(hitAnimation);
				needFreshCorePosition = true;
			}
		}
	}

	public void hitByFT(int phase)
	{
		if (!hasDied)
		{
			isHit = true;
			hitAnimation = "hit_annie_" + phase;
			falseAttack();
			playAnimation(hitAnimation);
			needFreshCorePosition = true;
			if (phase == 3)
			{
				hasDied = true;
				Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
				GameObject gameObject = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !PhotonNetwork.isMasterClient) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("bloodExplore"), transform.position + Vector3.up * 1f * 4f, Quaternion.Euler(270f, 0f, 0f))) : PhotonNetwork.Instantiate("bloodExplore", transform.position + Vector3.up * 1f * 4f, Quaternion.Euler(270f, 0f, 0f), 0));
				gameObject.transform.localScale = base.transform.localScale;
				gameObject = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !PhotonNetwork.isMasterClient) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("bloodsplatter"), transform.position, Quaternion.Euler(90f + transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z))) : PhotonNetwork.Instantiate("bloodsplatter", transform.position, Quaternion.Euler(90f + transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), 0));
				gameObject.transform.localScale = base.transform.localScale;
				gameObject.transform.parent = transform;
				gameObject = ((IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.MULTIPLAYER || !PhotonNetwork.isMasterClient) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/justSmoke"), transform.position, Quaternion.Euler(270f, 0f, 0f))) : PhotonNetwork.Instantiate("FX/justSmoke", transform.position, Quaternion.Euler(270f, 0f, 0f), 0));
				gameObject.transform.parent = transform;
			}
		}
	}

	public void hitByFTByServer(int phase)
	{
		base.photonView.RPC("hitByFTRPC", PhotonTargets.All, phase);
	}

	[RPC]
	private void hitByFTRPC(int phase)
	{
		if (base.photonView.isMine)
		{
			hitByFT(phase);
		}
	}

	public void hitByTitanByServer()
	{
		base.photonView.RPC("hitByTitanRPC", PhotonTargets.All);
	}

	[RPC]
	private void hitByTitanRPC()
	{
		if (base.photonView.isMine)
		{
			hitByTitan();
		}
	}

	private void RockUpdate()
	{
		if (isHitWhileCarryingRock)
		{
			return;
		}
		if (isROCKMOVE)
		{
			rock.transform.position = base.transform.position;
			rock.transform.rotation = base.transform.rotation;
		}
		if (IN_GAME_MAIN_CAMERA.gametype != 0 && !base.photonView.isMine)
		{
			return;
		}
		if (rockPhase == 0)
		{
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
			waitCounter += Time.deltaTime;
			if (waitCounter > 20f)
			{
				rockPhase++;
				crossFade("idle", 1f);
				waitCounter = 0f;
				setRoute();
			}
		}
		else if (rockPhase == 1)
		{
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
			waitCounter += Time.deltaTime;
			if (waitCounter > 2f)
			{
				rockPhase++;
				crossFade("run", 0.2f);
				waitCounter = 0f;
			}
		}
		else if (rockPhase == 2)
		{
			Vector3 vector = base.transform.forward * 30f;
			Vector3 velocity = base.rigidbody.velocity;
			Vector3 force = vector - velocity;
			force.x = Mathf.Clamp(force.x, 0f - maxVelocityChange, maxVelocityChange);
			force.z = Mathf.Clamp(force.z, 0f - maxVelocityChange, maxVelocityChange);
			force.y = 0f;
			base.rigidbody.AddForce(force, ForceMode.VelocityChange);
			if (base.transform.position.z < -238f)
			{
				base.transform.position = new Vector3(base.transform.position.x, 0f, -238f);
				rockPhase++;
				crossFade("idle", 0.2f);
				waitCounter = 0f;
			}
		}
		else if (rockPhase == 3)
		{
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
			waitCounter += Time.deltaTime;
			if (waitCounter > 1f)
			{
				rockPhase++;
				crossFade("rock_lift", 0.1f);
				base.photonView.RPC("rockPlayAnimation", PhotonTargets.All, "lift");
				waitCounter = 0f;
				targetCheckPt = (Vector3)checkPoints[0];
			}
		}
		else if (rockPhase == 4)
		{
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
			waitCounter += Time.deltaTime;
			if (waitCounter > 4.2f)
			{
				rockPhase++;
				crossFade("rock_walk", 0.1f);
				base.photonView.RPC("rockPlayAnimation", PhotonTargets.All, "move");
				rock.animation["move"].normalizedTime = base.animation["rock_walk"].normalizedTime;
				waitCounter = 0f;
				base.photonView.RPC("startMovingRock", PhotonTargets.All);
			}
		}
		else if (rockPhase == 5)
		{
			if (Vector3.Distance(base.transform.position, targetCheckPt) < 10f)
			{
				if (checkPoints.Count > 0)
				{
					if (checkPoints.Count == 1)
					{
						rockPhase++;
					}
					else
					{
						Vector3 vector2 = (Vector3)checkPoints[0];
						targetCheckPt = vector2;
						checkPoints.RemoveAt(0);
						GameObject[] array = GameObject.FindGameObjectsWithTag("titanRespawn2");
						GameObject gameObject = GameObject.Find("titanRespawnTrost" + (7 - checkPoints.Count));
						if (gameObject != null)
						{
							GameObject[] array2 = array;
							foreach (GameObject gameObject2 in array2)
							{
								if (gameObject2.transform.parent.gameObject == gameObject)
								{
									GameObject gameObject3 = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().spawnTitan(70, gameObject2.transform.position, gameObject2.transform.rotation);
									gameObject3.GetComponent<TITAN>().isAlarm = true;
									gameObject3.GetComponent<TITAN>().chaseDistance = 999999f;
								}
							}
						}
					}
				}
				else
				{
					rockPhase++;
				}
			}
			if (checkPoints.Count > 0 && UnityEngine.Random.Range(0, 3000) < 10 - checkPoints.Count)
			{
				Quaternion quaternion = ((UnityEngine.Random.Range(0, 10) <= 5) ? (base.transform.rotation * Quaternion.Euler(0f, UnityEngine.Random.Range(-30f, 30f), 0f)) : (base.transform.rotation * Quaternion.Euler(0f, UnityEngine.Random.Range(150f, 210f), 0f)));
				Vector3 vector3 = quaternion * new Vector3(UnityEngine.Random.Range(100f, 200f), 0f, 0f);
				Vector3 vector4 = base.transform.position + vector3;
				LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
				LayerMask layerMask2 = layerMask;
				float num = 0f;
				RaycastHit hitInfo;
				if (Physics.Raycast(vector4 + Vector3.up * 500f, -Vector3.up, out hitInfo, 1000f, layerMask2.value))
				{
					num = hitInfo.point.y;
				}
				vector4 += Vector3.up * num;
				GameObject gameObject4 = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().spawnTitan(70, vector4, base.transform.rotation);
				gameObject4.GetComponent<TITAN>().isAlarm = true;
				gameObject4.GetComponent<TITAN>().chaseDistance = 999999f;
			}
			Vector3 vector5 = base.transform.forward * 6f;
			Vector3 velocity2 = base.rigidbody.velocity;
			Vector3 force2 = vector5 - velocity2;
			force2.x = Mathf.Clamp(force2.x, 0f - maxVelocityChange, maxVelocityChange);
			force2.z = Mathf.Clamp(force2.z, 0f - maxVelocityChange, maxVelocityChange);
			force2.y = 0f;
			base.rigidbody.AddForce(force2, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
			Vector3 vector6 = targetCheckPt - base.transform.position;
			float current = (0f - Mathf.Atan2(vector6.z, vector6.x)) * 57.29578f;
			float num2 = 0f - Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
			base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num2, 0f), 0.8f * Time.deltaTime);
		}
		else if (rockPhase == 6)
		{
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
			base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			rockPhase++;
			crossFade("rock_fix_hole", 0.1f);
			base.photonView.RPC("rockPlayAnimation", PhotonTargets.All, "set");
			base.photonView.RPC("endMovingRock", PhotonTargets.All);
		}
		else
		{
			if (rockPhase != 7)
			{
				return;
			}
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
			if (base.animation["rock_fix_hole"].normalizedTime >= 1.2f)
			{
				crossFade("die", 0.1f);
				rockPhase++;
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().gameWin();
			}
			if (base.animation["rock_fix_hole"].normalizedTime >= 0.62f && !rockHitGround)
			{
				rockHitGround = true;
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.Instantiate("FX/boom1_CT_KICK", new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f), 0);
				}
				else
				{
					UnityEngine.Object.Instantiate(Resources.Load("FX/boom1_CT_KICK"), new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f));
				}
			}
		}
	}

	[RPC]
	private void rockPlayAnimation(string anim)
	{
		rock.animation.Play(anim);
		rock.animation[anim].speed = 1f;
	}

	[RPC]
	private void startMovingRock()
	{
		isROCKMOVE = true;
	}

	[RPC]
	private void endMovingRock()
	{
		isROCKMOVE = false;
	}

	public void setRoute()
	{
		GameObject gameObject = GameObject.Find("routeTrost");
		checkPoints = new ArrayList();
		for (int i = 1; i <= 7; i++)
		{
			checkPoints.Add(gameObject.transform.Find("r" + i).position);
		}
		checkPoints.Add("end");
	}
}
