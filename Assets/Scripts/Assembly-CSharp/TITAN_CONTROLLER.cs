using UnityEngine;

public class TITAN_CONTROLLER : MonoBehaviour
{
	public FengCustomInputs inputManager;

	public float targetDirection;

	public Camera currentCamera;

	public bool isAttackDown;

	public bool isJumpDown;

	public bool isAttackIIDown;

	public bool isWALKDown;

	public bool isSuicide;

	private void Start()
	{
		inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
		currentCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		int num = (inputManager.isInput[InputCode.up] ? 1 : (inputManager.isInput[InputCode.down] ? (-1) : 0));
		int num2 = (inputManager.isInput[InputCode.left] ? (-1) : (inputManager.isInput[InputCode.right] ? 1 : 0));
		if (num2 != 0 || num != 0)
		{
			float y = currentCamera.transform.rotation.eulerAngles.y;
			float num3 = Mathf.Atan2(num, num2) * 57.29578f;
			num3 = 0f - num3 + 90f;
			float num4 = y + num3;
			targetDirection = num4;
		}
		else
		{
			targetDirection = -874f;
		}
		isAttackDown = false;
		isJumpDown = false;
		isAttackIIDown = false;
		isSuicide = false;
		if (inputManager.isInputDown[InputCode.attack0])
		{
			isAttackDown = true;
		}
		if (inputManager.isInputDown[InputCode.attack1])
		{
			isAttackIIDown = true;
		}
		if (inputManager.isInputDown[InputCode.bothRope])
		{
			isJumpDown = true;
		}
		if (inputManager.isInputDown[InputCode.restart])
		{
			isSuicide = true;
		}
		isWALKDown = inputManager.isInput[InputCode.jump];
	}
}
