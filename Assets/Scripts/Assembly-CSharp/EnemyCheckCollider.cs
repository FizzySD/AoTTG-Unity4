using Photon;
using UnityEngine;

public class EnemyCheckCollider : Photon.MonoBehaviour
{
	public bool active_me;

	public int dmg = 1;

	public bool isThisBite;

	private int count;

	private void Start()
	{
		active_me = true;
		count = 0;
	}

	private void FixedUpdate()
	{
		if (count > 1)
		{
			active_me = false;
		}
		else
		{
			count++;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && !base.transform.root.gameObject.GetPhotonView().isMine) || !active_me)
		{
			return;
		}
		if (other.gameObject.tag == "playerHitbox")
		{
			float b = 1f - Vector3.Distance(other.gameObject.transform.position, base.transform.position) * 0.05f;
			b = Mathf.Min(1f, b);
			HitBox component = other.gameObject.GetComponent<HitBox>();
			if (!(component != null) || !(component.transform.root != null))
			{
				return;
			}
			if (dmg == 0)
			{
				Vector3 vector = component.transform.root.transform.position - base.transform.position;
				float num = 0f;
				if ((bool)base.gameObject.GetComponent<SphereCollider>())
				{
					num = base.transform.localScale.x * base.gameObject.GetComponent<SphereCollider>().radius;
				}
				if ((bool)base.gameObject.GetComponent<CapsuleCollider>())
				{
					num = base.transform.localScale.x * base.gameObject.GetComponent<CapsuleCollider>().height;
				}
				float num2 = 5f;
				if (num > 0f)
				{
					num2 = Mathf.Max(5f, num - vector.magnitude);
				}
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					component.transform.root.GetComponent<HERO>().blowAway(vector.normalized * num2 + Vector3.up * 1f);
				}
				else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER)
				{
					component.transform.root.GetComponent<HERO>().photonView.RPC("blowAway", PhotonTargets.All, vector.normalized * num2 + Vector3.up * 1f);
				}
			}
			else
			{
				if (component.transform.root.GetComponent<HERO>().isInvincible())
				{
					return;
				}
				if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
				{
					if (!component.transform.root.GetComponent<HERO>().isGrabbed)
					{
						component.transform.root.GetComponent<HERO>().die((component.transform.root.transform.position - base.transform.position).normalized * b * 1000f + Vector3.up * 50f, isThisBite);
					}
				}
				else if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && !component.transform.root.GetComponent<HERO>().HasDied() && !component.transform.root.GetComponent<HERO>().isGrabbed)
				{
					component.transform.root.GetComponent<HERO>().markDie();
					int num3 = -1;
					string text = string.Empty;
					if (base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
					{
						num3 = base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
						text = base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
					}
					component.transform.root.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (component.transform.root.position - base.transform.position).normalized * b * 1000f + Vector3.up * 50f, isThisBite, num3, text, true);
				}
			}
		}
		else if (other.gameObject.tag == "erenHitbox" && dmg > 0 && !other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().isHit)
		{
			other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByTitan();
		}
	}
}
