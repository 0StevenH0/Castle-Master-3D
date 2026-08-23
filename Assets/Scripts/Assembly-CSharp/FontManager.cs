using UnityEngine;

public class FontManager
{
	public enum FontColor
	{
		white = 0,
		black = 1,
		red = 2,
		blue = 3,
		green = 4,
		yellow = 5
	}

	public enum FontSize
	{
		small = 0,
		middle = 1,
		big = 2
	}

	public enum FontType
	{
		normal = 0
	}

	private const string baseFontPath = "interface/fontdata";

	private const string strFont = "font_";

	private const string strMtrl = "mtrl_";

	public static int[] defaultCharacterSize = new int[3] { 16, 24, 32 };

	public static TextMesh CreateText(string text, FontType type, FontSize size, FontColor color)
	{
		GameObject gameObject = new GameObject("BaseText");
		TextMesh textMesh = gameObject.AddComponent<TextMesh>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		gameObject.layer = LayerManager.layerUI;
		textMesh.font = ResourceManager.Load("interface/fontdata", "font_" + type.ToString() + "_" + size, typeof(Font)) as Font;
		textMesh.characterSize = defaultCharacterSize[(int)size];
		textMesh.anchor = TextAnchor.MiddleCenter;
		textMesh.alignment = TextAlignment.Center;
		textMesh.text = text;
		meshRenderer.material = ResourceManager.Load("interface/fontdata", "mtrl_font_" + type.ToString() + "_" + size.ToString() + "_" + color, typeof(Material)) as Material;
		return textMesh;
	}

	public static Font GetFont(FontType type, FontSize size, Language lang)
	{
		Font font = null;
		string filename = "font_" + type.ToString() + "_" + size;
		if (font == null)
		{
			font = ResourceManager.Load("interface/fontdata", filename, typeof(Font)) as Font;
		}
		return font;
	}

	public static Material GetFontMaterial(FontType type, FontSize size, FontColor color, Language lang)
	{
		Material material = null;
		string filename = "mtrl_font_" + type.ToString() + "_" + size.ToString() + "_" + color;
		if (material == null)
		{
			material = ResourceManager.Load("interface/fontdata", filename, typeof(Material)) as Material;
		}
		return material;
	}
}
