using UnityEngine;

public class AuiSprite : MonoBehaviour
{
	public const string szSpriteShader = "TransparentSprite";

	public const string szSpriteSize = "_SpriteSize";

	public const string szOriginalSize = "_OriginalSize";

	public const string szSpriteName = "_sprite";

	public const string szColor = "_Color";

	private static string[] filenameTextImage = new string[3] { "pki_text_english", "pki_text_korean", "pki_text_japanese" };

	public Material[] materials;

	public int curFrame;

	public Rect crop = new Rect(0f, 0f, 1f, 1f);

	public bool isCrop;

	public bool isVisible = true;

	protected Material[] cropMaterials;

	protected GameObject sprObject;

	protected Transform sprTrasform;

	protected MeshRenderer sprRenderer;

	protected int frameCount;

	protected Vector2[] texOffset;

	protected Vector2[] texScale;

	protected Vector3 orgSize = new Vector3(0f, 0f, 0f);

	public int spriteCount
	{
		get
		{
			return frameCount;
		}
	}

	public bool visible
	{
		get
		{
			return isVisible;
		}
		set
		{
			isVisible = value;
			SetFrame(curFrame);
		}
	}

	private void Start()
	{
		OnStart();
	}

	protected void OnStart()
	{
		if (UserSetting.language == Language.english || materials == null)
		{
			return;
		}
		for (int i = 0; i < materials.Length; i++)
		{
			if (!(materials[i] != null) || !(materials[i].mainTexture != null) || !materials[i].mainTexture.name.Equals(filenameTextImage[0]))
			{
				continue;
			}
			Material material = ResourceManager.Load(string.Concat("interface/images/Sprite_pki_text_", UserSetting.language, "_Materials"), materials[i].name, typeof(Material)) as Material;
			if (material != null)
			{
				materials[i] = material;
				if (i == curFrame)
				{
					SetFrame(curFrame);
				}
			}
		}
	}

	public Mesh GetSpriteMesh()
	{
		return ResourceManager.Load("BaseMesh", "msh_guiplane", typeof(Mesh)) as Mesh;
	}

	public void MakeSpriteObject()
	{
		MakeSpriteObject(false);
	}

	public void MakeSpriteObject(bool isPreview)
	{
		if (materials != null)
		{
			frameCount = materials.Length;
		}
		string text = base.name + "_sprite";
		int childCount = base.transform.GetChildCount();
		for (int num = childCount - 1; num >= 0; num--)
		{
			GameObject obj = base.transform.GetChild(num).gameObject;
			if (isPreview)
			{
				Object.DestroyImmediate(obj);
			}
			else
			{
				Object.Destroy(obj);
			}
		}
		sprObject = new GameObject(text);
		sprTrasform = sprObject.transform;
		sprTrasform.parent = base.transform;
		MeshFilter meshFilter = sprObject.AddComponent<MeshFilter>();
		meshFilter.mesh = GetSpriteMesh();
		sprRenderer = sprObject.AddComponent<MeshRenderer>();
		sprObject.layer = base.gameObject.layer;
		if (isCrop)
		{
			texOffset = new Vector2[frameCount];
			texScale = new Vector2[frameCount];
			cropMaterials = new Material[frameCount];
			for (int i = 0; i < frameCount; i++)
			{
				texOffset[i] = materials[i].mainTextureOffset;
				texScale[i] = materials[i].mainTextureScale;
				Material original = materials[i];
				cropMaterials[i] = Object.Instantiate(original) as Material;
			}
		}
		SetFrame(curFrame);
	}

	public void SetFrame(int frame)
	{
		if (sprObject == null)
		{
			MakeSpriteObject();
		}
		if (materials.Length > 0)
		{
			if (frame > frameCount)
			{
				frame = frameCount - 1;
			}
			Material material = materials[frame];
			Vector4 vector = new Vector4(0f, 0f, 0f, 0f);
			Vector4 vector2 = new Vector4(0f, 0f, 0f, 0f);
			if (material.shader.name.Equals("TransparentSprite"))
			{
				vector = material.GetVector("_SpriteSize");
				vector2 = material.GetVector("_OriginalSize");
			}
			orgSize = new Vector3(vector2.z, vector2.w, 1f);
			Vector3 localPosition = new Vector3(vector.x, vector.y, 0f);
			Vector3 localScale = new Vector3(vector.z, vector.w, 1f);
			if (isCrop && texOffset != null && texScale != null)
			{
				Vector3 localScale2 = new Vector3(localScale.x * crop.width, localScale.y * crop.height, 1f);
				Vector3 localPosition2 = new Vector3(localPosition.x - localScale.x * (1f - crop.width) * 0.5f, localPosition.y - localScale.y * (1f - crop.height) * 0.5f, 0f);
				Vector2 mainTextureOffset = new Vector2(0f, 0f);
				mainTextureOffset.x = texOffset[frame].x + texScale[frame].x * crop.x;
				mainTextureOffset.y = texOffset[frame].y + texScale[frame].y * crop.y;
				Vector2 mainTextureScale = new Vector2(0f, 0f);
				mainTextureScale.x = texScale[frame].x * crop.width;
				mainTextureScale.y = texScale[frame].y * crop.height;
				sprTrasform.localPosition = localPosition2;
				sprTrasform.localScale = localScale2;
				cropMaterials[frame].mainTextureOffset = mainTextureOffset;
				cropMaterials[frame].mainTextureScale = mainTextureScale;
				sprRenderer.material = cropMaterials[frame];
			}
			else
			{
				sprTrasform.localPosition = localPosition;
				sprTrasform.localScale = localScale;
				sprRenderer.material = material;
			}
			sprTrasform.localRotation = Quaternion.identity;
			curFrame = frame;
			sprObject.active = isVisible;
			base.gameObject.active = isVisible;
		}
	}
}
