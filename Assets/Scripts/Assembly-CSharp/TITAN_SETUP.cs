using Photon;
using UnityEngine;

public class TITAN_SETUP : Photon.MonoBehaviour
{
	public GameObject eye;

	private GameObject part_hair;

	private CostumeHair hair;

	private int hairType;

	private GameObject hair_go_ref;

	private void Awake()
	{
		CostumeHair.init();
		CharacterMaterials.init();
		HeroCostume.init();
		hair_go_ref = new GameObject();
		eye.transform.parent = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
		hair_go_ref.transform.position = eye.transform.position + Vector3.up * 3.5f + base.transform.forward * 5.2f;
		hair_go_ref.transform.rotation = eye.transform.rotation;
		hair_go_ref.transform.RotateAround(eye.transform.position, base.transform.right, -20f);
		hair_go_ref.transform.localScale = new Vector3(210f, 210f, 210f);
		hair_go_ref.transform.parent = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
	}

	public void setHair()
	{
		Object.Destroy(part_hair);
		int num = Random.Range(0, CostumeHair.hairsM.Length);
		if (num == 3)
		{
			num = 9;
		}
		hairType = num;
		hair = CostumeHair.hairsM[num];
		if (hair.hair == string.Empty)
		{
			hair = CostumeHair.hairsM[9];
			hairType = 9;
		}
		part_hair = (GameObject)Object.Instantiate(Resources.Load("Character/" + hair.hair));
		part_hair.transform.parent = hair_go_ref.transform.parent;
		part_hair.transform.position = hair_go_ref.transform.position;
		part_hair.transform.rotation = hair_go_ref.transform.rotation;
		part_hair.transform.localScale = hair_go_ref.transform.localScale;
		part_hair.renderer.material = CharacterMaterials.materials[hair.texture];
		part_hair.renderer.material.color = HeroCostume.costume[Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
		int num2 = Random.Range(1, 8);
		setFacialTexture(eye, num2);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			base.photonView.RPC("setHairPRC", PhotonTargets.OthersBuffered, hairType, num2, part_hair.renderer.material.color.r, part_hair.renderer.material.color.g, part_hair.renderer.material.color.b);
		}
	}

	public void setFacialTexture(GameObject go, int id)
	{
		if (id >= 0)
		{
			float num = 0.25f;
			float num2 = 0.125f;
			float x = num2 * (float)(int)((float)id / 8f);
			float y = (0f - num) * (float)(id % 4);
			go.renderer.material.mainTextureOffset = new Vector2(x, y);
		}
	}

	public void setPunkHair()
	{
		Object.Destroy(part_hair);
		hair = CostumeHair.hairsM[3];
		hairType = 3;
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Character/" + hair.hair));
		gameObject.transform.parent = hair_go_ref.transform.parent;
		gameObject.transform.position = hair_go_ref.transform.position;
		gameObject.transform.rotation = hair_go_ref.transform.rotation;
		gameObject.transform.localScale = hair_go_ref.transform.localScale;
		gameObject.renderer.material = CharacterMaterials.materials[hair.texture];
		int num = Random.Range(1, 4);
		if (num == 1)
		{
			gameObject.renderer.material.color = FengColor.hairPunk1;
		}
		if (num == 2)
		{
			gameObject.renderer.material.color = FengColor.hairPunk2;
		}
		if (num == 3)
		{
			gameObject.renderer.material.color = FengColor.hairPunk3;
		}
		part_hair = gameObject;
		setFacialTexture(eye, 0);
		if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
		{
			base.photonView.RPC("setHairPRC", PhotonTargets.OthersBuffered, hairType, 0, part_hair.renderer.material.color.r, part_hair.renderer.material.color.g, part_hair.renderer.material.color.b);
		}
	}

	[RPC]
	private void setHairPRC(int type, int eye_type, float c1, float c2, float c3)
	{
		Object.Destroy(part_hair);
		hair = CostumeHair.hairsM[type];
		hairType = type;
		if (hair.hair != string.Empty)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Character/" + hair.hair));
			gameObject.transform.parent = hair_go_ref.transform.parent;
			gameObject.transform.position = hair_go_ref.transform.position;
			gameObject.transform.rotation = hair_go_ref.transform.rotation;
			gameObject.transform.localScale = hair_go_ref.transform.localScale;
			gameObject.renderer.material = CharacterMaterials.materials[hair.texture];
			gameObject.renderer.material.color = new Color(c1, c2, c3);
			part_hair = gameObject;
		}
		setFacialTexture(eye, eye_type);
	}
}
