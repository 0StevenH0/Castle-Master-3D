using UnityEngine;

public class UVAnimation : MonoBehaviour
{
	public enum UVAnimationType
	{
		tile = 0,
		flowU = 1,
		flowV = 2
	}

	public UVAnimationType type;

	public float tileX = 1f;

	public float tileY = 1f;

	public float framesPerSecond = 10f;

	public float delay;

	public float flowSpeed = 1f;

	public float flowOffset;

	private float passTime;

	private int curFrame;

	private float curDelay;

	private void Start()
	{
		if (type != 0)
		{
			Material material = base.GetComponent<Renderer>().material;
			base.GetComponent<Renderer>().sharedMaterial = new Material(material);
			if (type == UVAnimationType.flowU)
			{
				ChangeOffsetU(flowOffset);
			}
			else if (type == UVAnimationType.flowV)
			{
				ChangeOffsetV(flowOffset);
			}
		}
		Play();
	}

	public void Play()
	{
		passTime = 0f;
		curFrame = 0;
		curDelay = delay;
	}

	private void Update()
	{
		if (curDelay > 0f)
		{
			curDelay -= Time.deltaTime;
			if (!(curDelay <= 0f))
			{
				return;
			}
			curDelay = 0f;
		}
		if (type == UVAnimationType.tile)
		{
			passTime += Time.deltaTime * framesPerSecond;
			int num = (int)passTime;
			if (num > 0)
			{
				passTime -= num;
			}
			curFrame += num;
			if ((float)curFrame >= tileX * tileY)
			{
				curFrame %= (int)(tileX * tileY);
			}
			num = curFrame;
			Vector2 scale = new Vector2(1f / tileX, 1f / tileY);
			int num2 = num % (int)tileX;
			int num3 = num / (int)tileX;
			Vector2 offset = new Vector2((float)num2 * scale.x, 1f - scale.y - (float)num3 * scale.y);
			base.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
			base.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
		}
		else if (type == UVAnimationType.flowU)
		{
			ChangeOffsetU(Time.deltaTime * flowSpeed);
		}
		else if (type == UVAnimationType.flowV)
		{
			ChangeOffsetV(Time.deltaTime * flowSpeed);
		}
	}

	private void ChangeOffsetU(float move)
	{
		Vector2 textureOffset = base.GetComponent<Renderer>().sharedMaterial.GetTextureOffset("_MainTex");
		textureOffset.x += move;
		if (textureOffset.x > 1f)
		{
			textureOffset.x -= 1f;
		}
		if (textureOffset.x < -1f)
		{
			textureOffset.x += 1f;
		}
		base.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", textureOffset);
	}

	private void ChangeOffsetV(float move)
	{
		Vector2 textureOffset = base.GetComponent<Renderer>().sharedMaterial.GetTextureOffset("_MainTex");
		textureOffset.y += move;
		if (textureOffset.y > 1f)
		{
			textureOffset.y -= 1f;
		}
		if (textureOffset.y < -1f)
		{
			textureOffset.y += 1f;
		}
		base.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", textureOffset);
	}
}
