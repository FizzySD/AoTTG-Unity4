using Photon;
using UnityEngine;

public class SmoothSyncMovement2 : Photon.MonoBehaviour
{
	public float SmoothingDelay = 5f;

	public bool disabled;

	private Vector3 correctPlayerPos = Vector3.zero;

	private Quaternion correctPlayerRot = Quaternion.identity;

	public void Awake()
	{
		if (base.photonView == null || base.photonView.observed != this)
		{
			Debug.LogWarning(string.Concat(this, " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used."));
		}
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
		{
			base.enabled = false;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
		}
		else
		{
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}

	public void Update()
	{
		if (!disabled && !base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, correctPlayerPos, Time.deltaTime * SmoothingDelay);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, correctPlayerRot, Time.deltaTime * SmoothingDelay);
		}
	}
}
