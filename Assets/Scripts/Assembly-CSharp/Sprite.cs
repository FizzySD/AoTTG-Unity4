using UnityEngine;

public class Sprite
{
	protected Vector2 LowerLeftUV;

	protected Vector2 UVDimensions;

	public STransform MyTransform;

	public Vector3 v1 = Vector3.zero;

	public Vector3 v2 = Vector3.zero;

	public Vector3 v3 = Vector3.zero;

	public Vector3 v4 = Vector3.zero;

	protected VertexPool.VertexSegment Vertexsegment;

	public Color Color;

	private Vector3 ScaleVector;

	private Quaternion Rotation;

	private Matrix4x4 LocalMat;

	private Matrix4x4 WorldMat;

	protected float ElapsedTime;

	protected float Fps;

	public Camera MainCamera;

	protected bool UVChanged;

	protected bool ColorChanged;

	protected Matrix4x4 LastMat;

	protected Vector3 RotateAxis;

	private STYPE Type;

	private ORIPOINT OriPoint;

	private int UVStretch;

	public Sprite(VertexPool.VertexSegment segment, float width, float height, STYPE type, ORIPOINT oripoint, Camera cam, int uvStretch, float maxFps)
	{
		UVChanged = (ColorChanged = false);
		MyTransform.position = Vector3.zero;
		MyTransform.rotation = Quaternion.identity;
		LocalMat = (WorldMat = Matrix4x4.identity);
		Vertexsegment = segment;
		UVStretch = uvStretch;
		LastMat = Matrix4x4.identity;
		ElapsedTime = 0f;
		Fps = 1f / maxFps;
		OriPoint = oripoint;
		RotateAxis = Vector3.zero;
		SetSizeXZ(width, height);
		RotateAxis.y = 1f;
		Type = type;
		MainCamera = cam;
		ResetSegment();
	}

	public void Init(Color color, Vector2 lowerLeftUV, Vector2 uvDimensions)
	{
		SetUVCoord(lowerLeftUV, uvDimensions);
		SetColor(color);
		SetRotation(Quaternion.identity);
		SetScale(1f, 1f);
		SetRotation(0f);
	}

	public void ResetSegment()
	{
		VertexPool pool = Vertexsegment.Pool;
		int indexStart = Vertexsegment.IndexStart;
		int vertStart = Vertexsegment.VertStart;
		pool.Indices[indexStart] = vertStart;
		pool.Indices[indexStart + 1] = vertStart + 3;
		pool.Indices[indexStart + 2] = vertStart + 1;
		pool.Indices[indexStart + 3] = vertStart + 3;
		pool.Indices[indexStart + 4] = vertStart + 2;
		pool.Indices[indexStart + 5] = vertStart + 1;
		pool.Vertices[vertStart] = Vector3.zero;
		pool.Vertices[vertStart + 1] = Vector3.zero;
		pool.Vertices[vertStart + 2] = Vector3.zero;
		pool.Vertices[vertStart + 3] = Vector3.zero;
		pool.Colors[vertStart] = Color.white;
		pool.Colors[vertStart + 1] = Color.white;
		pool.Colors[vertStart + 2] = Color.white;
		pool.Colors[vertStart + 3] = Color.white;
		pool.UVs[vertStart] = Vector2.zero;
		pool.UVs[vertStart + 1] = Vector2.zero;
		pool.UVs[vertStart + 2] = Vector2.zero;
		pool.UVs[vertStart + 3] = Vector2.zero;
		pool.UVChanged = (pool.IndiceChanged = (pool.ColorChanged = (pool.VertChanged = true)));
	}

	public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
	{
		LowerLeftUV = lowerleft;
		UVDimensions = dimensions;
		UVChanged = true;
	}

	public void SetPosition(Vector3 pos)
	{
		MyTransform.position = pos;
	}

	public void SetRotation(Quaternion q)
	{
		MyTransform.rotation = q;
	}

	public void SetRotationFaceTo(Vector3 dir)
	{
		MyTransform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
	}

	public void SetRotationTo(Vector3 dir)
	{
		if (!(dir == Vector3.zero))
		{
			Quaternion rotation = Quaternion.identity;
			Vector3 vector = dir;
			vector.y = 0f;
			if (vector == Vector3.zero)
			{
				vector = Vector3.up;
			}
			if (OriPoint == ORIPOINT.CENTER)
			{
				Quaternion quaternion = Quaternion.FromToRotation(new Vector3(0f, 0f, 1f), vector);
				Quaternion quaternion2 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion2 * quaternion;
			}
			else if (OriPoint == ORIPOINT.LEFT_UP)
			{
				Quaternion quaternion3 = Quaternion.FromToRotation(LocalMat.MultiplyPoint3x4(v3), vector);
				Quaternion quaternion4 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion4 * quaternion3;
			}
			else if (OriPoint == ORIPOINT.LEFT_BOTTOM)
			{
				Quaternion quaternion5 = Quaternion.FromToRotation(LocalMat.MultiplyPoint3x4(v4), vector);
				Quaternion quaternion6 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion6 * quaternion5;
			}
			else if (OriPoint == ORIPOINT.RIGHT_BOTTOM)
			{
				Quaternion quaternion7 = Quaternion.FromToRotation(LocalMat.MultiplyPoint3x4(v1), vector);
				Quaternion quaternion8 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion8 * quaternion7;
			}
			else if (OriPoint == ORIPOINT.RIGHT_UP)
			{
				Quaternion quaternion9 = Quaternion.FromToRotation(LocalMat.MultiplyPoint3x4(v2), vector);
				Quaternion quaternion10 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion10 * quaternion9;
			}
			else if (OriPoint == ORIPOINT.BOTTOM_CENTER)
			{
				Quaternion quaternion11 = Quaternion.FromToRotation(new Vector3(0f, 0f, 1f), vector);
				Quaternion quaternion12 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion12 * quaternion11;
			}
			else if (OriPoint == ORIPOINT.TOP_CENTER)
			{
				Quaternion quaternion13 = Quaternion.FromToRotation(new Vector3(0f, 0f, -1f), vector);
				Quaternion quaternion14 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion14 * quaternion13;
			}
			else if (OriPoint == ORIPOINT.RIGHT_CENTER)
			{
				Quaternion quaternion15 = Quaternion.FromToRotation(new Vector3(-1f, 0f, 0f), vector);
				Quaternion quaternion16 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion16 * quaternion15;
			}
			else if (OriPoint == ORIPOINT.LEFT_CENTER)
			{
				Quaternion quaternion17 = Quaternion.FromToRotation(new Vector3(1f, 0f, 0f), vector);
				Quaternion quaternion18 = Quaternion.FromToRotation(vector, dir);
				rotation = quaternion18 * quaternion17;
			}
			MyTransform.rotation = rotation;
		}
	}

	public void Reset()
	{
		MyTransform.Reset();
		SetColor(Color.white);
		SetUVCoord(Vector2.zero, Vector2.zero);
		ScaleVector = Vector3.one;
		Rotation = Quaternion.identity;
		VertexPool pool = Vertexsegment.Pool;
		int vertStart = Vertexsegment.VertStart;
		pool.Vertices[vertStart] = Vector3.zero;
		pool.Vertices[vertStart + 1] = Vector3.zero;
		pool.Vertices[vertStart + 2] = Vector3.zero;
		pool.Vertices[vertStart + 3] = Vector3.zero;
	}

	public void SetSizeXZ(float width, float height)
	{
		v1 = new Vector3((0f - width) / 2f, 0f, height / 2f);
		v2 = new Vector3((0f - width) / 2f, 0f, (0f - height) / 2f);
		v3 = new Vector3(width / 2f, 0f, (0f - height) / 2f);
		v4 = new Vector3(width / 2f, 0f, height / 2f);
		Vector3 vector = Vector3.zero;
		if (OriPoint == ORIPOINT.LEFT_UP)
		{
			vector = v3;
		}
		else if (OriPoint == ORIPOINT.LEFT_BOTTOM)
		{
			vector = v4;
		}
		else if (OriPoint == ORIPOINT.RIGHT_BOTTOM)
		{
			vector = v1;
		}
		else if (OriPoint == ORIPOINT.RIGHT_UP)
		{
			vector = v2;
		}
		else if (OriPoint == ORIPOINT.BOTTOM_CENTER)
		{
			vector = new Vector3(0f, 0f, height / 2f);
		}
		else if (OriPoint == ORIPOINT.TOP_CENTER)
		{
			vector = new Vector3(0f, 0f, (0f - height) / 2f);
		}
		else if (OriPoint == ORIPOINT.LEFT_CENTER)
		{
			vector = new Vector3(width / 2f, 0f, 0f);
		}
		else if (OriPoint == ORIPOINT.RIGHT_CENTER)
		{
			vector = new Vector3((0f - width) / 2f, 0f, 0f);
		}
		v1 += vector;
		v2 += vector;
		v3 += vector;
		v4 += vector;
	}

	public void UpdateUV()
	{
		VertexPool pool = Vertexsegment.Pool;
		int vertStart = Vertexsegment.VertStart;
		if (UVDimensions.y > 0f)
		{
			pool.UVs[vertStart] = LowerLeftUV + Vector2.up * UVDimensions.y;
			pool.UVs[vertStart + 1] = LowerLeftUV;
			pool.UVs[vertStart + 2] = LowerLeftUV + Vector2.right * UVDimensions.x;
			pool.UVs[vertStart + 3] = LowerLeftUV + UVDimensions;
		}
		else
		{
			pool.UVs[vertStart] = LowerLeftUV;
			pool.UVs[vertStart + 1] = LowerLeftUV + Vector2.up * UVDimensions.y;
			pool.UVs[vertStart + 2] = LowerLeftUV + UVDimensions;
			pool.UVs[vertStart + 3] = LowerLeftUV + Vector2.right * UVDimensions.x;
		}
		Vertexsegment.Pool.UVChanged = true;
	}

	public void UpdateColor()
	{
		VertexPool pool = Vertexsegment.Pool;
		int vertStart = Vertexsegment.VertStart;
		pool.Colors[vertStart] = Color;
		pool.Colors[vertStart + 1] = Color;
		pool.Colors[vertStart + 2] = Color;
		pool.Colors[vertStart + 3] = Color;
		Vertexsegment.Pool.ColorChanged = true;
	}

	public void Transform()
	{
		LocalMat.SetTRS(Vector3.zero, Rotation, ScaleVector);
		if (Type == STYPE.BILLBOARD)
		{
			Transform transform = MainCamera.transform;
			MyTransform.LookAt(MyTransform.position + transform.rotation * Vector3.up, transform.rotation * Vector3.back);
		}
		WorldMat.SetTRS(MyTransform.position, MyTransform.rotation, Vector3.one);
		Matrix4x4 matrix4x = WorldMat * LocalMat;
		VertexPool pool = Vertexsegment.Pool;
		int vertStart = Vertexsegment.VertStart;
		Vector3 vector = matrix4x.MultiplyPoint3x4(v1);
		Vector3 vector2 = matrix4x.MultiplyPoint3x4(v2);
		Vector3 vector3 = matrix4x.MultiplyPoint3x4(v3);
		Vector3 vector4 = matrix4x.MultiplyPoint3x4(v4);
		if (Type == STYPE.BILLBOARD_SELF)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			float num = 0f;
			if (UVStretch == 0)
			{
				zero = (vector + vector4) / 2f;
				zero2 = (vector2 + vector3) / 2f;
				num = (vector4 - vector).magnitude;
			}
			else
			{
				zero = (vector + vector2) / 2f;
				zero2 = (vector4 + vector3) / 2f;
				num = (vector2 - vector).magnitude;
			}
			Vector3 lhs = zero - zero2;
			Vector3 rhs = MainCamera.transform.position - zero;
			Vector3 vector5 = Vector3.Cross(lhs, rhs);
			vector5.Normalize();
			vector5 *= num * 0.5f;
			Vector3 rhs2 = MainCamera.transform.position - zero2;
			Vector3 vector6 = Vector3.Cross(lhs, rhs2);
			vector6.Normalize();
			vector6 *= num * 0.5f;
			if (UVStretch == 0)
			{
				vector = zero - vector5;
				vector4 = zero + vector5;
				vector2 = zero2 - vector6;
				vector3 = zero2 + vector6;
			}
			else
			{
				vector = zero - vector5;
				vector2 = zero + vector5;
				vector4 = zero2 - vector6;
				vector3 = zero2 + vector6;
			}
		}
		pool.Vertices[vertStart] = vector;
		pool.Vertices[vertStart + 1] = vector2;
		pool.Vertices[vertStart + 2] = vector3;
		pool.Vertices[vertStart + 3] = vector4;
	}

	public void SetRotation(float angle)
	{
		Rotation = Quaternion.AngleAxis(angle, RotateAxis);
	}

	public void SetScale(float width, float height)
	{
		ScaleVector.x = width;
		ScaleVector.z = height;
	}

	public void Update(bool force)
	{
		ElapsedTime += Time.deltaTime;
		if (ElapsedTime > Fps || force)
		{
			Transform();
			if (UVChanged)
			{
				UpdateUV();
			}
			if (ColorChanged)
			{
				UpdateColor();
			}
			UVChanged = (ColorChanged = false);
			if (!force)
			{
				ElapsedTime -= Fps;
			}
		}
	}

	public void SetColor(Color c)
	{
		Color = c;
		ColorChanged = true;
	}
}
