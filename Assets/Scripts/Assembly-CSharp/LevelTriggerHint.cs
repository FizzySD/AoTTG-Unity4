using UnityEngine;

public class LevelTriggerHint : MonoBehaviour
{
	public string content;

	public HintType myhint;

	private bool on;

	private void Start()
	{
		if (!LevelInfo.getInfo(FengGameManagerMKII.level).hint)
		{
			base.enabled = false;
		}
		if (!(content != string.Empty))
		{
			switch (myhint)
			{
			case HintType.DODGE:
				content = "Press [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.dodge] + "[-] to Dodge.";
				break;
			case HintType.ATTACK:
				content = "Press [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.attack0] + "[-] to Attack. \nPress [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.attack1] + "[-] to use special attack.\n***You can only kill a titan by slashing his [FA5858]NAPE[-].***\n\n";
				break;
			case HintType.MOVE:
				content = "Hello soldier!\nWelcome to Attack On Titan Tribute Game!\n Press [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.up] + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.left] + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.down] + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.right] + "[-] to Move.";
				break;
			case HintType.TELE:
				content = "Move to [82FA58]green warp point[-] to proceed.";
				break;
			case HintType.CAMA:
				content = "Press [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.camera] + "[-] to change camera mode\nPress [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.hideCursor] + "[-] to hide or show the cursor.";
				break;
			case HintType.JUMP:
				content = "Press [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.jump] + "[-] to Jump.";
				break;
			case HintType.JUMP2:
				content = "Press [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.up] + "[-] towards a wall to perform a wall-run.";
				break;
			case HintType.HOOK:
				content = "Press and Hold[F7D358] " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.leftRope] + "[-] or [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.rightRope] + "[-] to launch your grapple.\nNow Try hooking to the [>3<] box. ";
				break;
			case HintType.HOOK2:
				content = "Press and Hold[F7D358] " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.bothRope] + "[-] to launch both of your grapples at the same Time.\n\nNow aim between the two black blocks. \nYou will see the mark '<' and '>' appearing on the blocks. \nThen press " + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.bothRope] + " to hook the blocks.";
				break;
			case HintType.SUPPLY:
				content = "Press [F7D358]" + GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().inputString[InputCode.reload] + "[-] to reload your blades.\n Move to the supply station to refill your gas and blades.";
				break;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			on = true;
		}
	}

	private void Update()
	{
		if (on)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().ShowHUDInfoCenter(content + "\n\n\n\n\n");
			on = false;
		}
	}
}
