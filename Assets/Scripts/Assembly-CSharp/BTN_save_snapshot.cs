using System;
using System.Collections;
using UnityEngine;

public class BTN_save_snapshot : MonoBehaviour
{
	public GameObject targetTexture;

	public GameObject info;

	public GameObject[] thingsNeedToHide;

	private void OnClick()
	{
		GameObject[] array = thingsNeedToHide;
		foreach (GameObject gameObject in array)
		{
			gameObject.transform.position += Vector3.up * 10000f;
		}
		StartCoroutine(ScreenshotEncode());
		info.GetComponent<UILabel>().text = "trying..";
	}

	private IEnumerator ScreenshotEncode()
	{
		yield return new WaitForEndOfFrame();
		float r = (float)Screen.height / 600f;
		Texture2D texture = new Texture2D((int)(r * targetTexture.transform.localScale.x), (int)(r * targetTexture.transform.localScale.y), TextureFormat.RGB24, false);
		texture.ReadPixels(new Rect((float)Screen.width * 0.5f - (float)texture.width * 0.5f, (float)Screen.height * 0.5f - (float)texture.height * 0.5f - r * 0f, texture.width, texture.height), 0, 0);
		texture.Apply();
		yield return 0;
		GameObject[] array = thingsNeedToHide;
		foreach (GameObject go in array)
		{
			go.transform.position -= Vector3.up * 10000f;
		}
		string img_name = "aottg_ss-" + DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + DateTime.Today.Year + "-" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".png";
		Application.ExternalCall("SaveImg", img_name, texture.width, texture.height, Convert.ToBase64String(texture.EncodeToPNG()));
		UnityEngine.Object.DestroyObject(texture);
	}
}
