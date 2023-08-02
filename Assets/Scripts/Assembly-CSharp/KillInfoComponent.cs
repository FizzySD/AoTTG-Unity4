using UnityEngine;

public class KillInfoComponent : MonoBehaviour
{
	public GameObject leftTitan;

	public GameObject rightTitan;

	public GameObject labelScore;

	public GameObject labelNameLeft;

	public GameObject labelNameRight;

	public GameObject spriteSkeleton;

	public GameObject spriteSword;

	public GameObject sleftTitan;

	public GameObject srightTitan;

	public GameObject slabelScore;

	public GameObject slabelNameLeft;

	public GameObject slabelNameRight;

	public GameObject sspriteSkeleton;

	public GameObject sspriteSword;

	public GameObject groupBig;

	public GameObject groupSmall;

	private bool start;

	private float timeElapsed;

	private float lifeTime = 8f;

	private float alpha = 1f;

	private float maxScale = 1.5f;

	private int offset = 24;

	private int col;

	private void Start()
	{
		start = true;
		base.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
		base.transform.localPosition = new Vector3(0f, -100f + (float)Screen.height * 0.5f, 0f);
	}

	public void show(bool isTitan1, string name1, bool isTitan2, string name2, int dmg = 0)
	{
		groupBig.SetActive(true);
		groupSmall.SetActive(true);
		if (!isTitan1)
		{
			leftTitan.SetActive(false);
			spriteSkeleton.SetActive(false);
			sleftTitan.SetActive(false);
			sspriteSkeleton.SetActive(false);
			labelNameLeft.transform.position += new Vector3(18f, 0f, 0f);
			slabelNameLeft.transform.position += new Vector3(16f, 0f, 0f);
		}
		else
		{
			spriteSword.SetActive(false);
			sspriteSword.SetActive(false);
			labelNameRight.transform.position -= new Vector3(18f, 0f, 0f);
			slabelNameRight.transform.position -= new Vector3(16f, 0f, 0f);
		}
		if (!isTitan2)
		{
			rightTitan.SetActive(false);
			srightTitan.SetActive(false);
		}
		labelNameLeft.GetComponent<UILabel>().text = name1;
		labelNameRight.GetComponent<UILabel>().text = name2;
		slabelNameLeft.GetComponent<UILabel>().text = name1;
		slabelNameRight.GetComponent<UILabel>().text = name2;
		if (dmg == 0)
		{
			labelScore.GetComponent<UILabel>().text = string.Empty;
			slabelScore.GetComponent<UILabel>().text = string.Empty;
		}
		else
		{
			labelScore.GetComponent<UILabel>().text = dmg.ToString();
			slabelScore.GetComponent<UILabel>().text = dmg.ToString();
			if (dmg > 1000)
			{
				labelScore.GetComponent<UILabel>().color = Color.red;
				slabelScore.GetComponent<UILabel>().color = Color.red;
			}
		}
		groupSmall.SetActive(false);
	}

	public void moveOn()
	{
		col++;
		if (col > 4)
		{
			timeElapsed = lifeTime;
		}
		groupBig.SetActive(false);
		groupSmall.SetActive(true);
	}

	public void destory()
	{
		timeElapsed = lifeTime;
	}

	private void Update()
	{
		if (start)
		{
			timeElapsed += Time.deltaTime;
			if (timeElapsed < 0.2f)
			{
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one * maxScale, Time.deltaTime * 10f);
			}
			else if (timeElapsed < 1f)
			{
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.deltaTime * 10f);
			}
			if (timeElapsed > lifeTime)
			{
				base.transform.position = base.transform.position + new Vector3(0f, Time.deltaTime * 0.15f, 0f);
				alpha = 1f - Time.deltaTime * 45f + lifeTime - timeElapsed;
				setAlpha(alpha);
			}
			else
			{
				float num = (int)(100f - (float)Screen.height * 0.5f + (float)(col * offset));
				base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, new Vector3(0f, 0f - num, 0f), Time.deltaTime * 10f);
			}
			if (timeElapsed > lifeTime + 0.5f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void setAlpha(float alpha)
	{
		if (groupBig.activeInHierarchy)
		{
			labelScore.GetComponent<UILabel>().color = new Color(labelScore.GetComponent<UILabel>().color.r, labelScore.GetComponent<UILabel>().color.g, labelScore.GetComponent<UILabel>().color.b, alpha);
			leftTitan.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			rightTitan.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			labelNameLeft.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, alpha);
			labelNameRight.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, alpha);
			spriteSkeleton.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			spriteSword.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
		}
		if (groupSmall.activeInHierarchy)
		{
			slabelScore.GetComponent<UILabel>().color = new Color(labelScore.GetComponent<UILabel>().color.r, labelScore.GetComponent<UILabel>().color.g, labelScore.GetComponent<UILabel>().color.b, alpha);
			sleftTitan.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			srightTitan.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			slabelNameLeft.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, alpha);
			slabelNameRight.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, alpha);
			sspriteSkeleton.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
			sspriteSword.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, alpha);
		}
	}
}
