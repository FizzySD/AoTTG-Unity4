using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
	public FengCustomInputs inputManager;

	private float speed = 100f;

	public bool disable;

	private void Start()
	{
		inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
	}

	private void Update()
	{
		if (!disable)
		{
			float num = (inputManager.isInput[InputCode.up] ? 1f : ((!inputManager.isInput[InputCode.down]) ? 0f : (-1f)));
			float num2 = (inputManager.isInput[InputCode.left] ? (-1f) : ((!inputManager.isInput[InputCode.right]) ? 0f : 1f));
			if (num > 0f)
			{
				base.transform.position += base.transform.forward * speed * Time.deltaTime;
			}
			if (num < 0f)
			{
				base.transform.position -= base.transform.forward * speed * Time.deltaTime;
			}
			if (num2 > 0f)
			{
				base.transform.position += base.transform.right * speed * Time.deltaTime;
			}
			if (num2 < 0f)
			{
				base.transform.position -= base.transform.right * speed * Time.deltaTime;
			}
			if (inputManager.isInput[InputCode.leftRope])
			{
				base.transform.position -= base.transform.up * speed * Time.deltaTime;
			}
			if (inputManager.isInput[InputCode.rightRope])
			{
				base.transform.position += base.transform.up * speed * Time.deltaTime;
			}
		}
	}
}
