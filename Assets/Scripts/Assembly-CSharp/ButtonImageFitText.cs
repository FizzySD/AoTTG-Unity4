using UnityEngine;
using UnityEngine.UI;

public class ButtonImageFitText : MonoBehaviour
{
	public Text text;

	public Image image;

	private void Start()
	{
		MonoBehaviour.print(text.flexibleWidth + " " + text.minWidth + " " + text.preferredWidth);
	}

	private void Update()
	{
	}
}
