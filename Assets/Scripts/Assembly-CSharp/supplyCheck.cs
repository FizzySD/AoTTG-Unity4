using UnityEngine;

public class supplyCheck : MonoBehaviour
{
	private float stepTime = 1f;

	private float elapsedTime;

	private void Start()
	{
	}

	private void Update()
	{
		elapsedTime += Time.deltaTime;
		if (!(elapsedTime > stepTime))
		{
			return;
		}
		elapsedTime -= stepTime;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (!(gameObject.GetComponent<HERO>() != null))
			{
				continue;
			}
			if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE)
			{
				if (Vector3.Distance(gameObject.transform.position, base.transform.position) < 1.5f)
				{
					gameObject.GetComponent<HERO>().getSupply();
				}
			}
			else if (gameObject.GetPhotonView().isMine && Vector3.Distance(gameObject.transform.position, base.transform.position) < 1.5f)
			{
				gameObject.GetComponent<HERO>().getSupply();
			}
		}
	}
}
