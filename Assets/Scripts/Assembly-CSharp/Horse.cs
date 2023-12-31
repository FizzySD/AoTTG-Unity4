using Photon;
using UnityEngine;

public class Horse : Photon.MonoBehaviour
{
	public GameObject myHero;

	public GameObject dust;

	private string State = "idle";

	private float speed = 45f;

	private TITAN_CONTROLLER controller;

	private Vector3 setPoint;

	private float awayTimer;

	private float timeElapsed;

	private void Start()
	{
		controller = base.gameObject.GetComponent<TITAN_CONTROLLER>();
	}

	public void mounted()
	{
		State = "mounted";
		base.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = true;
	}

	public void unmounted()
	{
		State = "idle";
		base.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = false;
	}

	private void followed()
	{
		if (!(myHero == null))
		{
			State = "follow";
			setPoint = myHero.transform.position + Vector3.right * Random.Range(-6, 6) + Vector3.forward * Random.Range(-6, 6);
			setPoint.y = getHeight(setPoint + Vector3.up * 5f);
			awayTimer = 0f;
		}
	}

	private float getHeight(Vector3 pt)
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = layerMask;
		RaycastHit hitInfo;
		if (Physics.Raycast(pt, -Vector3.up, out hitInfo, 1000f, layerMask2.value))
		{
			return hitInfo.point.y;
		}
		return 0f;
	}

	private void LateUpdate()
	{
		if (myHero == null && base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		if (State == "mounted")
		{
			if (myHero == null)
			{
				unmounted();
				return;
			}
			myHero.transform.position = base.transform.position + Vector3.up * 1.68f;
			myHero.transform.rotation = base.transform.rotation;
			myHero.rigidbody.velocity = base.rigidbody.velocity;
			if (controller.targetDirection != -874f)
			{
				base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, controller.targetDirection, 0f), 100f * Time.deltaTime / (base.rigidbody.velocity.magnitude + 20f));
				if (controller.isWALKDown)
				{
					base.rigidbody.AddForce(base.transform.forward * speed * 0.6f, ForceMode.Acceleration);
					if (!(base.rigidbody.velocity.magnitude < speed * 0.6f))
					{
						base.rigidbody.AddForce((0f - speed) * 0.6f * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
					}
				}
				else
				{
					base.rigidbody.AddForce(base.transform.forward * speed, ForceMode.Acceleration);
					if (!(base.rigidbody.velocity.magnitude < speed))
					{
						base.rigidbody.AddForce((0f - speed) * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
					}
				}
				if (base.rigidbody.velocity.magnitude > 8f)
				{
					if (!base.animation.IsPlaying("horse_Run"))
					{
						crossFade("horse_Run", 0.1f);
					}
					if (!myHero.animation.IsPlaying("horse_Run"))
					{
						myHero.GetComponent<HERO>().crossFade("horse_run", 0.1f);
					}
					if (!dust.GetComponent<ParticleSystem>().enableEmission)
					{
						dust.GetComponent<ParticleSystem>().enableEmission = true;
						base.photonView.RPC("setDust", PhotonTargets.Others, true);
					}
				}
				else
				{
					if (!base.animation.IsPlaying("horse_WALK"))
					{
						crossFade("horse_WALK", 0.1f);
					}
					if (!myHero.animation.IsPlaying("horse_idle"))
					{
						myHero.GetComponent<HERO>().crossFade("horse_idle", 0.1f);
					}
					if (dust.GetComponent<ParticleSystem>().enableEmission)
					{
						dust.GetComponent<ParticleSystem>().enableEmission = false;
						base.photonView.RPC("setDust", PhotonTargets.Others, false);
					}
				}
			}
			else
			{
				toIdleAnimation();
				if (base.rigidbody.velocity.magnitude > 15f)
				{
					if (!myHero.animation.IsPlaying("horse_Run"))
					{
						myHero.GetComponent<HERO>().crossFade("horse_run", 0.1f);
					}
				}
				else if (!myHero.animation.IsPlaying("horse_idle"))
				{
					myHero.GetComponent<HERO>().crossFade("horse_idle", 0.1f);
				}
			}
			if ((controller.isAttackDown || controller.isAttackIIDown) && IsGrounded())
			{
				base.rigidbody.AddForce(Vector3.up * 25f, ForceMode.VelocityChange);
			}
		}
		else if (State == "follow")
		{
			if (myHero == null)
			{
				unmounted();
				return;
			}
			if (base.rigidbody.velocity.magnitude > 8f)
			{
				if (!base.animation.IsPlaying("horse_Run"))
				{
					crossFade("horse_Run", 0.1f);
				}
				if (!dust.GetComponent<ParticleSystem>().enableEmission)
				{
					dust.GetComponent<ParticleSystem>().enableEmission = true;
					base.photonView.RPC("setDust", PhotonTargets.Others, true);
				}
			}
			else
			{
				if (!base.animation.IsPlaying("horse_WALK"))
				{
					crossFade("horse_WALK", 0.1f);
				}
				if (dust.GetComponent<ParticleSystem>().enableEmission)
				{
					dust.GetComponent<ParticleSystem>().enableEmission = false;
					base.photonView.RPC("setDust", PhotonTargets.Others, false);
				}
			}
			float num = 0f - Mathf.DeltaAngle(FengMath.getHorizontalAngle(base.transform.position, setPoint), base.gameObject.transform.rotation.eulerAngles.y - 90f);
			base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num, 0f), 200f * Time.deltaTime / (base.rigidbody.velocity.magnitude + 20f));
			if (Vector3.Distance(setPoint, base.transform.position) < 20f)
			{
				base.rigidbody.AddForce(base.transform.forward * speed * 0.7f, ForceMode.Acceleration);
				if (!(base.rigidbody.velocity.magnitude < speed))
				{
					base.rigidbody.AddForce((0f - speed) * 0.7f * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
				}
			}
			else
			{
				base.rigidbody.AddForce(base.transform.forward * speed, ForceMode.Acceleration);
				if (!(base.rigidbody.velocity.magnitude < speed))
				{
					base.rigidbody.AddForce((0f - speed) * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
				}
			}
			timeElapsed += Time.deltaTime;
			if (timeElapsed > 0.6f)
			{
				timeElapsed = 0f;
				if (Vector3.Distance(myHero.transform.position, setPoint) > 20f)
				{
					followed();
				}
			}
			if (Vector3.Distance(myHero.transform.position, base.transform.position) < 5f)
			{
				unmounted();
			}
			if (Vector3.Distance(setPoint, base.transform.position) < 5f)
			{
				unmounted();
			}
			awayTimer += Time.deltaTime;
			if (awayTimer > 6f)
			{
				awayTimer = 0f;
				LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
				LayerMask layerMask2 = layerMask;
				if (Physics.Linecast(base.transform.position + Vector3.up, myHero.transform.position + Vector3.up, layerMask2.value))
				{
					base.transform.position = new Vector3(myHero.transform.position.x, getHeight(myHero.transform.position + Vector3.up * 5f), myHero.transform.position.z);
				}
			}
		}
		else if (State == "idle")
		{
			toIdleAnimation();
			if (myHero != null && Vector3.Distance(myHero.transform.position, base.transform.position) > 20f)
			{
				followed();
			}
		}
		base.rigidbody.AddForce(new Vector3(0f, -50f * base.rigidbody.mass, 0f));
	}

	public bool IsGrounded()
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("EnemyBox");
		LayerMask layerMask3 = (int)layerMask2 | (int)layerMask;
		return Physics.Raycast(base.gameObject.transform.position + Vector3.up * 0.1f, -Vector3.up, 0.3f, layerMask3.value);
	}

	private void toIdleAnimation()
	{
		if (base.rigidbody.velocity.magnitude > 0.1f)
		{
			if (base.rigidbody.velocity.magnitude > 15f)
			{
				if (!base.animation.IsPlaying("horse_Run"))
				{
					crossFade("horse_Run", 0.1f);
				}
				if (!dust.GetComponent<ParticleSystem>().enableEmission)
				{
					dust.GetComponent<ParticleSystem>().enableEmission = true;
					base.photonView.RPC("setDust", PhotonTargets.Others, true);
				}
			}
			else
			{
				if (!base.animation.IsPlaying("horse_WALK"))
				{
					crossFade("horse_WALK", 0.1f);
				}
				if (dust.GetComponent<ParticleSystem>().enableEmission)
				{
					dust.GetComponent<ParticleSystem>().enableEmission = false;
					base.photonView.RPC("setDust", PhotonTargets.Others, false);
				}
			}
			return;
		}
		if (base.animation.IsPlaying("horse_idle1") && base.animation["horse_idle1"].normalizedTime >= 1f)
		{
			crossFade("horse_idle0", 0.1f);
		}
		if (base.animation.IsPlaying("horse_idle2") && base.animation["horse_idle2"].normalizedTime >= 1f)
		{
			crossFade("horse_idle0", 0.1f);
		}
		if (base.animation.IsPlaying("horse_idle3") && base.animation["horse_idle3"].normalizedTime >= 1f)
		{
			crossFade("horse_idle0", 0.1f);
		}
		if (!base.animation.IsPlaying("horse_idle0") && !base.animation.IsPlaying("horse_idle1") && !base.animation.IsPlaying("horse_idle2") && !base.animation.IsPlaying("horse_idle3"))
		{
			crossFade("horse_idle0", 0.1f);
		}
		if (base.animation.IsPlaying("horse_idle0"))
		{
			int num = Random.Range(0, 10000);
			if (num < 10)
			{
				crossFade("horse_idle1", 0.1f);
			}
			else if (num < 20)
			{
				crossFade("horse_idle2", 0.1f);
			}
			else if (num < 30)
			{
				crossFade("horse_idle3", 0.1f);
			}
		}
		if (dust.GetComponent<ParticleSystem>().enableEmission)
		{
			dust.GetComponent<ParticleSystem>().enableEmission = false;
			base.photonView.RPC("setDust", PhotonTargets.Others, false);
		}
		base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
	}

	[RPC]
	private void setDust(bool enable)
	{
		if (dust.GetComponent<ParticleSystem>().enableEmission)
		{
			dust.GetComponent<ParticleSystem>().enableEmission = enable;
		}
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
}
