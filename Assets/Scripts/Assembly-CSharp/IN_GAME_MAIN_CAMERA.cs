using UnityEngine;

public class IN_GAME_MAIN_CAMERA : MonoBehaviour
{
	public enum RotationAxes
	{
		MouseXAndY = 0,
		MouseX = 1,
		MouseY = 2
	}

	public static GAMETYPE gametype = GAMETYPE.STOP;

	public static GAMEMODE gamemode;

	public static int difficulty;

	public static bool triggerAutoLock;

	public static int level;

	public static int character = 1;

	public static bool isCheating;

	public static bool isPausing;

	public static bool isTyping;

	public static float sensitivityMulti = 0.5f;

	public static int invertY = 1;

	public static int cameraTilt = 1;

	public static STEREO_3D_TYPE stereoType;

	public static DayLight dayLight = DayLight.Dawn;

	public FengCustomInputs inputManager;

	public RotationAxes axes;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	private float rotationY;

	public GameObject main_object;

	public bool gameOver;

	public AudioSource bgmusic;

	private float xSpeed = -3f;

	private float ySpeed = -0.8f;

	private float verticalRotationOffset;

	private Vector3 verticalHeightOffset = Vector3.zero;

	private float flashDuration;

	private bool needSetHUD;

	private GameObject lockTarget;

	public Material skyBoxDAY;

	public Material skyBoxDAWN;

	public Material skyBoxNIGHT;

	public bool spectatorMode;

	private GameObject locker;

	public static bool usingTitan;

	public static float cameraDistance = 0.6f;

	private int currentPeekPlayerIndex;

	private Vector3 lockCameraPosition;

	private float R;

	private float duration;

	private float decay;

	private bool flip;

	private float closestDistance;

	private Transform target;

	private float distance = 10f;

	private float height = 5f;

	private float heightDamping = 2f;

	private Transform head;

	private float distanceMulti;

	private float distanceOffsetMulti;

	private float heightMulti;

	private bool lockAngle;

	private int snapShotDmg;

	private Texture2D snapshot1;

	private Texture2D snapshot2;

	private Texture2D snapshot3;

	private bool startSnapShotFrameCount;

	private float snapShotStartCountDownTime;

	private float snapShotInterval = 0.02f;

	private Vector3 snapShotTargetPosition;

	private GameObject snapShotTarget;

	private int snapShotCount;

	private float snapShotCountDown;

	private bool hasSnapShot;

	public GameObject snapShotCamera;

	public float timer;

	public static CAMERA_TYPE cameraMode;

	public Texture texture;

	public int score;

	public int lastScore;

	public float justHit;

	public static string singleCharacter;

	private void Awake()
	{
		isTyping = false;
		isPausing = false;
		base.name = "MainCamera";
		if (PlayerPrefs.HasKey("GameQuality"))
		{

		}
	}

	private void Start()
	{
		GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().addCamera(this);
		isPausing = false;
		sensitivityMulti = PlayerPrefs.GetFloat("MouseSensitivity");
		invertY = PlayerPrefs.GetInt("invertMouseY");
		inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
		setDayLight(dayLight);
		locker = GameObject.Find("locker");
		if (PlayerPrefs.HasKey("cameraTilt"))
		{
			cameraTilt = PlayerPrefs.GetInt("cameraTilt");
		}
		else
		{
			cameraTilt = 1;
		}
		if (PlayerPrefs.HasKey("cameraDistance"))
		{
			cameraDistance = PlayerPrefs.GetFloat("cameraDistance") + 0.3f;
		}
		createSnapShotRT();
	}

	public void createSnapShotRT()
	{
		if (snapShotCamera.GetComponent<Camera>().targetTexture != null)
		{
			snapShotCamera.GetComponent<Camera>().targetTexture.Release();
		}
		if (QualitySettings.GetQualityLevel() > 3)
		{
			snapShotCamera.GetComponent<Camera>().targetTexture = new RenderTexture((int)((float)Screen.width * 0.8f), (int)((float)Screen.height * 0.8f), 24);
		}
		else
		{
			snapShotCamera.GetComponent<Camera>().targetTexture = new RenderTexture((int)((float)Screen.width * 0.4f), (int)((float)Screen.height * 0.4f), 24);
		}
	}

	public void setDayLight(DayLight val)
	{
		dayLight = val;
		if (dayLight == DayLight.Night)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("flashlight"));
			gameObject.transform.parent = base.transform;
			gameObject.transform.position = base.transform.position;
			gameObject.transform.rotation = Quaternion.Euler(353f, 0f, 0f);
			RenderSettings.ambientLight = FengColor.nightAmbientLight;
			GameObject.Find("mainLight").GetComponent<Light>().color = FengColor.nightLight;
			base.gameObject.GetComponent<Skybox>().material = skyBoxNIGHT;
		}
		if (dayLight == DayLight.Day)
		{
			RenderSettings.ambientLight = FengColor.dayAmbientLight;
			GameObject.Find("mainLight").GetComponent<Light>().color = FengColor.dayLight;
			base.gameObject.GetComponent<Skybox>().material = skyBoxDAY;
		}
		if (dayLight == DayLight.Dawn)
		{
			RenderSettings.ambientLight = FengColor.dawnAmbientLight;
			GameObject.Find("mainLight").GetComponent<Light>().color = FengColor.dawnAmbientLight;
			base.gameObject.GetComponent<Skybox>().material = skyBoxDAWN;
		}
		snapShotCamera.gameObject.GetComponent<Skybox>().material = base.gameObject.GetComponent<Skybox>().material;
	}

	public void setHUDposition()
	{
		GameObject gameObject = GameObject.Find("Flare");
		gameObject.transform.localPosition = new Vector3((int)((float)(-Screen.width) * 0.5f) + 14, (int)((float)(-Screen.height) * 0.5f), 0f);
		gameObject = GameObject.Find("LabelInfoBottomRight");
		gameObject.transform.localPosition = new Vector3((int)((float)Screen.width * 0.5f), (int)((float)(-Screen.height) * 0.5f), 0f);
		gameObject.GetComponent<UILabel>().text = "Pause : " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.pause] + " ";
		gameObject = GameObject.Find("LabelInfoTopCenter");
		gameObject.transform.localPosition = new Vector3(0f, (int)((float)Screen.height * 0.5f), 0f);
		gameObject = GameObject.Find("LabelInfoTopRight");
		gameObject.transform.localPosition = new Vector3((int)((float)Screen.width * 0.5f), (int)((float)Screen.height * 0.5f), 0f);
		GameObject.Find("LabelNetworkStatus").transform.localPosition = new Vector3((int)((float)(-Screen.width) * 0.5f), (int)((float)Screen.height * 0.5f), 0f);
		gameObject = GameObject.Find("LabelInfoTopLeft");
		gameObject.transform.localPosition = new Vector3((int)((float)(-Screen.width) * 0.5f), (int)((float)Screen.height * 0.5f - 20f), 0f);
		gameObject = GameObject.Find("Chatroom");
		gameObject.transform.localPosition = new Vector3((int)((float)(-Screen.width) * 0.5f), (int)((float)(-Screen.height) * 0.5f), 0f);
		if ((bool)GameObject.Find("Chatroom"))
		{
			GameObject.Find("Chatroom").GetComponent<InRoomChat>().setPosition();
		}
		if (!usingTitan || gametype == GAMETYPE.SINGLE)
		{
			GameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (int)((float)(-Screen.height) * 0.5f + 5f), 0f);
			GameObject.Find("GasUI").transform.localPosition = GameObject.Find("skill_cd_bottom").transform.localPosition;
			GameObject.Find("stamina_titan").transform.localPosition = new Vector3(0f, 9999f, 0f);
			GameObject.Find("stamina_titan_bottom").transform.localPosition = new Vector3(0f, 9999f, 0f);
		}
		else
		{
			Vector3 localPosition = new Vector3(0f, 9999f, 0f);
			GameObject.Find("skill_cd_bottom").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_armin").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_eren").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_jean").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_levi").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_marco").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_mikasa").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_petra").transform.localPosition = localPosition;
			GameObject.Find("skill_cd_sasha").transform.localPosition = localPosition;
			GameObject.Find("GasUI").transform.localPosition = localPosition;
			GameObject.Find("stamina_titan").transform.localPosition = new Vector3(-160f, (int)((float)(-Screen.height) * 0.5f + 15f), 0f);
			GameObject.Find("stamina_titan_bottom").transform.localPosition = new Vector3(-160f, (int)((float)(-Screen.height) * 0.5f + 15f), 0f);
		}
		if (main_object != null && main_object.GetComponent<HERO>() != null)
		{
			if (gametype == GAMETYPE.SINGLE)
			{
				main_object.GetComponent<HERO>().setSkillHUDPosition();
			}
			else if (main_object.GetPhotonView() != null && main_object.GetPhotonView().isMine)
			{
				main_object.GetComponent<HERO>().setSkillHUDPosition();
			}
		}
		if (stereoType == STEREO_3D_TYPE.SIDE_BY_SIDE)
		{
			base.gameObject.GetComponent<Camera>().aspect = Screen.width / Screen.height;
		}
		createSnapShotRT();
	}

	public void flashBlind()
	{
		GameObject gameObject = GameObject.Find("flash");
		gameObject.GetComponent<UISprite>().alpha = 1f;
		flashDuration = 2f;
	}

	private float getSensitivityMultiWithDeltaTime()
	{
		return sensitivityMulti * Time.deltaTime * 62f;
	}

	private float getSensitivityMulti()
	{
		return sensitivityMulti;
	}

	private int getReverse()
	{
		return invertY;
	}

	private void reset()
	{
		if (gametype == GAMETYPE.SINGLE)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().restartGameSingle();
		}
	}

	public void setSpectorMode(bool val)
	{
		spectatorMode = val;
		GameObject.Find("MainCamera").GetComponent<SpectatorMovement>().disable = !val;
		GameObject.Find("MainCamera").GetComponent<MouseLook>().disable = !val;
	}

	private void camareMovement()
	{
		distanceOffsetMulti = cameraDistance * (200f - base.camera.fieldOfView) / 150f;
		base.transform.position = ((!(head != null)) ? main_object.transform.position : head.transform.position);
		base.transform.position += Vector3.up * heightMulti;
		base.transform.position -= Vector3.up * (0.6f - cameraDistance) * 2f;
		if (cameraMode == CAMERA_TYPE.WOW)
		{
			if (Input.GetKey(KeyCode.Mouse1))
			{
				float angle = Input.GetAxis("Mouse X") * 10f * getSensitivityMulti();
				float angle2 = (0f - Input.GetAxis("Mouse Y")) * 10f * getSensitivityMulti() * (float)getReverse();
				base.transform.RotateAround(base.transform.position, Vector3.up, angle);
				base.transform.RotateAround(base.transform.position, base.transform.right, angle2);
			}
			base.transform.position -= base.transform.forward * distance * distanceMulti * distanceOffsetMulti;
		}
		else if (cameraMode == CAMERA_TYPE.ORIGINAL)
		{
			float num = 0f;
			if (Input.mousePosition.x < (float)Screen.width * 0.4f)
			{
				num = (0f - ((float)Screen.width * 0.4f - Input.mousePosition.x) / (float)Screen.width * 0.4f) * getSensitivityMultiWithDeltaTime() * 150f;
				base.transform.RotateAround(base.transform.position, Vector3.up, num);
			}
			else if (Input.mousePosition.x > (float)Screen.width * 0.6f)
			{
				num = (Input.mousePosition.x - (float)Screen.width * 0.6f) / (float)Screen.width * 0.4f * getSensitivityMultiWithDeltaTime() * 150f;
				base.transform.RotateAround(base.transform.position, Vector3.up, num);
			}
			float x = 140f * ((float)Screen.height * 0.6f - Input.mousePosition.y) / (float)Screen.height * 0.5f;
			base.transform.rotation = Quaternion.Euler(x, base.transform.rotation.eulerAngles.y, base.transform.rotation.eulerAngles.z);
			base.transform.position -= base.transform.forward * distance * distanceMulti * distanceOffsetMulti;
		}
		else if (cameraMode == CAMERA_TYPE.TPS)
		{
			if (!inputManager.menuOn)
			{
				Screen.lockCursor = true;
			}
			float angle3 = Input.GetAxis("Mouse X") * 10f * getSensitivityMulti();
			float num2 = (0f - Input.GetAxis("Mouse Y")) * 10f * getSensitivityMulti() * (float)getReverse();
			base.transform.RotateAround(base.transform.position, Vector3.up, angle3);
			float num3 = base.transform.rotation.eulerAngles.x % 360f;
			float num4 = num3 + num2;
			if ((!(num2 > 0f) || ((!(num3 < 260f) || !(num4 > 260f)) && (!(num3 < 80f) || !(num4 > 80f)))) && (!(num2 < 0f) || ((!(num3 > 280f) || !(num4 < 280f)) && (!(num3 > 100f) || !(num4 < 100f)))))
			{
				base.transform.RotateAround(base.transform.position, base.transform.right, num2);
			}
			base.transform.position -= base.transform.forward * distance * distanceMulti * distanceOffsetMulti;
		}
		if (cameraDistance < 0.65f)
		{
			base.transform.position += base.transform.right * Mathf.Max((0.6f - cameraDistance) * 2f, 0.65f);
		}
	}

	public void update()
	{
		if (flashDuration > 0f)
		{
			flashDuration -= Time.deltaTime;
			if (flashDuration <= 0f)
			{
				flashDuration = 0f;
			}
			GameObject gameObject = GameObject.Find("flash");
			gameObject.GetComponent<UISprite>().alpha = flashDuration * 0.5f;
		}
		if (gametype == GAMETYPE.STOP)
		{
			Screen.showCursor = true;
			Screen.lockCursor = false;
			return;
		}
		if (gametype != 0 && gameOver)
		{
			if (inputManager.isInputDown[InputCode.attack1])
			{
				if (spectatorMode)
				{
					setSpectorMode(false);
				}
				else
				{
					setSpectorMode(true);
				}
			}
			if (inputManager.isInputDown[InputCode.flare1])
			{
				currentPeekPlayerIndex++;
				int num = GameObject.FindGameObjectsWithTag("Player").Length;
				if (currentPeekPlayerIndex >= num)
				{
					currentPeekPlayerIndex = 0;
				}
				if (num > 0)
				{
					setMainObject(GameObject.FindGameObjectsWithTag("Player")[currentPeekPlayerIndex]);
					setSpectorMode(false);
					lockAngle = false;
				}
			}
			if (inputManager.isInputDown[InputCode.flare2])
			{
				currentPeekPlayerIndex--;
				int num2 = GameObject.FindGameObjectsWithTag("Player").Length;
				if (currentPeekPlayerIndex >= num2)
				{
					currentPeekPlayerIndex = 0;
				}
				if (currentPeekPlayerIndex < 0)
				{
					currentPeekPlayerIndex = num2;
				}
				if (num2 > 0)
				{
					setMainObject(GameObject.FindGameObjectsWithTag("Player")[currentPeekPlayerIndex]);
					setSpectorMode(false);
					lockAngle = false;
				}
			}
			if (spectatorMode)
			{
				return;
			}
		}
		if (inputManager.isInputDown[InputCode.pause])
		{
			if (isPausing)
			{
				if (main_object != null)
				{
					Vector3 position = base.transform.position;
					position = ((!(head != null)) ? main_object.transform.position : head.transform.position);
					position += Vector3.up * heightMulti;
					base.transform.position = Vector3.Lerp(base.transform.position, position - base.transform.forward * 5f, 0.2f);
				}
				return;
			}
			isPausing = !isPausing;
			if (isPausing)
			{
				if (gametype == GAMETYPE.SINGLE)
				{
					Time.timeScale = 0f;
				}
				GameObject gameObject2 = GameObject.Find("UI_IN_GAME");
				NGUITools.SetActive(gameObject2.GetComponent<UIReferArray>().panels[0], false);
				NGUITools.SetActive(gameObject2.GetComponent<UIReferArray>().panels[1], true);
				NGUITools.SetActive(gameObject2.GetComponent<UIReferArray>().panels[2], false);
				NGUITools.SetActive(gameObject2.GetComponent<UIReferArray>().panels[3], false);
				GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().showKeyMap();
				GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().justUPDATEME();
				GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = true;
				Screen.showCursor = true;
				Screen.lockCursor = false;
			}
		}
		if (needSetHUD)
		{
			needSetHUD = false;
			setHUDposition();
		}
		if (inputManager.isInputDown[InputCode.fullscreen])
		{
			Screen.fullScreen = !Screen.fullScreen;
			if (Screen.fullScreen)
			{
				Screen.SetResolution(960, 600, false);
			}
			else
			{
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
			}
			needSetHUD = true;
		}
		if (inputManager.isInputDown[InputCode.restart])
		{
			reset();
		}
		if (!main_object)
		{
			return;
		}
		if (inputManager.isInputDown[InputCode.camera])
		{
			if (cameraMode == CAMERA_TYPE.ORIGINAL)
			{
				cameraMode = CAMERA_TYPE.WOW;
				Screen.lockCursor = false;
			}
			else if (cameraMode == CAMERA_TYPE.WOW)
			{
				cameraMode = CAMERA_TYPE.TPS;
				Screen.lockCursor = true;
			}
			else if (cameraMode == CAMERA_TYPE.TPS)
			{
				cameraMode = CAMERA_TYPE.ORIGINAL;
				Screen.lockCursor = false;
			}
			verticalRotationOffset = 0f;
		}
		if (inputManager.isInputDown[InputCode.hideCursor])
		{
			Screen.showCursor = !Screen.showCursor;
		}
		if (inputManager.isInputDown[InputCode.focus])
		{
			triggerAutoLock = !triggerAutoLock;
			if (triggerAutoLock)
			{
				lockTarget = findNearestTitan();
				if (!(closestDistance < 9999f))
				{
					lockTarget = null;
					triggerAutoLock = false;
				}
			}
		}
		if (gameOver && lockAngle && main_object != null)
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, main_object.transform.rotation, 0.2f);
			base.transform.position = Vector3.Lerp(base.transform.position, main_object.transform.position - main_object.transform.forward * 5f, 0.2f);
		}
		else
		{
			camareMovement();
		}
		if (triggerAutoLock && lockTarget != null)
		{
			float z = base.transform.eulerAngles.z;
			Transform transform = lockTarget.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
			Vector3 vector = transform.position - ((!(head != null)) ? main_object.transform.position : head.transform.position);
			vector.Normalize();
			lockCameraPosition = ((!(head != null)) ? main_object.transform.position : head.transform.position);
			lockCameraPosition -= vector * distance * distanceMulti * distanceOffsetMulti;
			lockCameraPosition += Vector3.up * 3f * heightMulti * distanceOffsetMulti;
			base.transform.position = Vector3.Lerp(base.transform.position, lockCameraPosition, Time.deltaTime * 4f);
			if (head != null)
			{
				base.transform.LookAt(head.transform.position * 0.8f + transform.position * 0.2f);
			}
			else
			{
				base.transform.LookAt(main_object.transform.position * 0.8f + transform.position * 0.2f);
			}
			base.transform.localEulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, z);
			Vector2 vector2 = base.camera.WorldToScreenPoint(transform.position - transform.forward * lockTarget.transform.localScale.x);
			locker.transform.localPosition = new Vector3(vector2.x - (float)Screen.width * 0.5f, vector2.y - (float)Screen.height * 0.5f, 0f);
			if ((bool)lockTarget.GetComponent<TITAN>() && lockTarget.GetComponent<TITAN>().hasDie)
			{
				lockTarget = null;
			}
		}
		else
		{
			locker.transform.localPosition = new Vector3(0f, (float)(-Screen.height) * 0.5f - 50f, 0f);
		}
		Vector3 end = ((!(head != null)) ? main_object.transform.position : head.transform.position);
		Vector3 normalized = (((!(head != null)) ? main_object.transform.position : head.transform.position) - base.transform.position).normalized;
		end -= distance * normalized * distanceMulti;
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask | (int)layerMask2;
		RaycastHit hitInfo;
		if (head != null)
		{
			if (Physics.Linecast(head.transform.position, end, out hitInfo, layerMask))
			{
				base.transform.position = hitInfo.point;
			}
			else if (Physics.Linecast(head.transform.position - normalized * distanceMulti * 3f, end, out hitInfo, layerMask2))
			{
				base.transform.position = hitInfo.point;
			}
			Debug.DrawLine(head.transform.position - normalized * distanceMulti * 3f, end, Color.red);
		}
		else if (Physics.Linecast(main_object.transform.position + Vector3.up, end, out hitInfo, layerMask3))
		{
			base.transform.position = hitInfo.point;
		}
		shakeUpdate();
	}

	private void shakeUpdate()
	{
		if (duration > 0f)
		{
			duration -= Time.deltaTime;
			if (flip)
			{
				base.gameObject.transform.position += Vector3.up * R;
			}
			else
			{
				base.gameObject.transform.position -= Vector3.up * R;
			}
			flip = !flip;
			R *= decay;
		}
	}

	public void startShake(float R, float duration, float decay = 0.95f)
	{
		if (this.duration < duration)
		{
			this.R = R;
			this.duration = duration;
			this.decay = decay;
		}
	}

	private GameObject findNearestTitan()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		GameObject result = null;
		float num = (closestDistance = float.PositiveInfinity);
		Vector3 position = main_object.transform.position;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float magnitude = (gameObject.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position - position).magnitude;
			if (magnitude < num && (!gameObject.GetComponent<TITAN>() || !gameObject.GetComponent<TITAN>().hasDie))
			{
				result = gameObject;
				num = (closestDistance = magnitude);
			}
		}
		return result;
	}

	public GameObject setMainObject(GameObject obj, bool resetRotation = true, bool lockAngle = false)
	{
		main_object = obj;
		if (obj == null)
		{
			head = null;
			distanceMulti = (heightMulti = 1f);
		}
		else if ((bool)main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head"))
		{
			head = main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
			distanceMulti = ((!(head == null)) ? (Vector3.Distance(head.transform.position, main_object.transform.position) * 0.2f) : 1f);
			heightMulti = ((!(head == null)) ? (Vector3.Distance(head.transform.position, main_object.transform.position) * 0.33f) : 1f);
			if (resetRotation)
			{
				base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		else if ((bool)main_object.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head"))
		{
			head = main_object.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
			distanceMulti = (heightMulti = 0.64f);
			if (resetRotation)
			{
				base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		else
		{
			head = null;
			distanceMulti = (heightMulti = 1f);
			if (resetRotation)
			{
				base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		this.lockAngle = lockAngle;
		return obj;
	}

	public GameObject setMainObjectASTITAN(GameObject obj)
	{
		main_object = obj;
		if ((bool)main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head"))
		{
			head = main_object.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
			distanceMulti = ((!(head == null)) ? (Vector3.Distance(head.transform.position, main_object.transform.position) * 0.4f) : 1f);
			heightMulti = ((!(head == null)) ? (Vector3.Distance(head.transform.position, main_object.transform.position) * 0.45f) : 1f);
			base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		}
		return obj;
	}

	public void snapShotUpdate()
	{
		if (startSnapShotFrameCount)
		{
			snapShotStartCountDownTime -= Time.deltaTime;
			if (snapShotStartCountDownTime <= 0f)
			{
				snapShot2(1);
				startSnapShotFrameCount = false;
			}
		}
		if (!hasSnapShot)
		{
			return;
		}
		snapShotCountDown -= Time.deltaTime;
		if (snapShotCountDown <= 0f)
		{
			GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().enabled = false;
			hasSnapShot = false;
			snapShotCountDown = 0f;
		}
		else if (snapShotCountDown < 1f)
		{
			GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().mainTexture = snapshot3;
		}
		else if (snapShotCountDown < 1.5f)
		{
			GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().mainTexture = snapshot2;
		}
		if (snapShotCount < 3)
		{
			snapShotInterval -= Time.deltaTime;
			if (snapShotInterval <= 0f)
			{
				snapShotInterval = 0.05f;
				snapShotCount++;
				snapShot2(snapShotCount);
			}
		}
	}

	public void startSnapShot(Vector3 p, int dmg, GameObject target = null, float startTime = 0.02f)
	{
		snapShotCount = 1;
		startSnapShotFrameCount = true;
		snapShotTargetPosition = p;
		snapShotTarget = target;
		snapShotStartCountDownTime = startTime;
		snapShotInterval = 0.05f + Random.Range(0f, 0.03f);
		snapShotDmg = dmg;
	}

	private Texture2D RTImage(Camera cam)
	{
		RenderTexture renderTexture = RenderTexture.active;
		RenderTexture.active = cam.targetTexture;
		cam.Render();
		Texture2D texture2D = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
		int num = (int)((float)cam.targetTexture.width * 0.04f);
		int num2 = (int)((float)cam.targetTexture.width * 0.02f);
		texture2D.ReadPixels(new Rect(num, num, cam.targetTexture.width - num, cam.targetTexture.height - num), num2, num2);
		texture2D.Apply();
		RenderTexture.active = renderTexture;
		return texture2D;
	}

	public void snapShot2(int index)
	{
		snapShotCamera.transform.position = ((!(head != null)) ? main_object.transform.position : head.transform.position);
		snapShotCamera.transform.position += Vector3.up * heightMulti;
		snapShotCamera.transform.position -= Vector3.up * 1.1f;
		Vector3 position;
		Vector3 vector = (position = snapShotCamera.transform.position);
		Vector3 vector2 = (vector + snapShotTargetPosition) * 0.5f;
		snapShotCamera.transform.position = vector2;
		vector = vector2;
		snapShotCamera.transform.LookAt(snapShotTargetPosition);
		if (index == 3)
		{
			snapShotCamera.transform.RotateAround(base.transform.position, Vector3.up, Random.Range(-180f, 180f));
		}
		else
		{
			snapShotCamera.transform.RotateAround(base.transform.position, Vector3.up, Random.Range(-20f, 20f));
		}
		snapShotCamera.transform.LookAt(vector);
		snapShotCamera.transform.RotateAround(vector, base.transform.right, Random.Range(-20f, 20f));
		float num = Vector3.Distance(snapShotTargetPosition, position);
		if (snapShotTarget != null && (bool)snapShotTarget.GetComponent<TITAN>())
		{
			num += (float)(index - 1) * snapShotTarget.transform.localScale.x * 10f;
		}
		snapShotCamera.transform.position -= snapShotCamera.transform.forward * Random.Range(num + 3f, num + 10f);
		snapShotCamera.transform.LookAt(vector);
		snapShotCamera.transform.RotateAround(vector, base.transform.forward, Random.Range(-30f, 30f));
		Vector3 end = ((!(head != null)) ? main_object.transform.position : head.transform.position);
		Vector3 vector3 = ((!(head != null)) ? main_object.transform.position : head.transform.position) - snapShotCamera.transform.position;
		end -= vector3;
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask | (int)layerMask2;
		RaycastHit hitInfo;
		if (head != null)
		{
			if (Physics.Linecast(head.transform.position, end, out hitInfo, layerMask))
			{
				snapShotCamera.transform.position = hitInfo.point;
			}
			else if (Physics.Linecast(head.transform.position - vector3 * distanceMulti * 3f, end, out hitInfo, layerMask3))
			{
				snapShotCamera.transform.position = hitInfo.point;
			}
		}
		else if (Physics.Linecast(main_object.transform.position + Vector3.up, end, out hitInfo, layerMask3))
		{
			snapShotCamera.transform.position = hitInfo.point;
		}
		switch (index)
		{
		case 1:
			snapshot1 = RTImage(snapShotCamera.GetComponent<Camera>());
			SnapShotSaves.addIMG(snapshot1, snapShotDmg);
			break;
		case 2:
			snapshot2 = RTImage(snapShotCamera.GetComponent<Camera>());
			SnapShotSaves.addIMG(snapshot2, snapShotDmg);
			break;
		case 3:
			snapshot3 = RTImage(snapShotCamera.GetComponent<Camera>());
			SnapShotSaves.addIMG(snapshot3, snapShotDmg);
			break;
		}
		snapShotCount = index;
		hasSnapShot = true;
		snapShotCountDown = 2f;
		if (index == 1)
		{
			GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().mainTexture = snapshot1;
			GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().transform.localScale = new Vector3((float)Screen.width * 0.4f, (float)Screen.height * 0.4f, 1f);
			GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().transform.localPosition = new Vector3((float)(-Screen.width) * 0.225f, (float)Screen.height * 0.225f, 0f);
			GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().transform.rotation = Quaternion.Euler(0f, 0f, 10f);
			if (PlayerPrefs.HasKey("showSSInGame") && PlayerPrefs.GetInt("showSSInGame") == 1)
			{
				GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().enabled = true;
			}
			else
			{
				GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[0].transform.Find("snapshot1").GetComponent<UITexture>().enabled = false;
			}
		}
	}
}
