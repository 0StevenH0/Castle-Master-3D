using UnityEngine;

public class AuiSpriteNumber : MonoBehaviour
{
	public enum NumberAlign
	{
		left = 0,
		right = 1,
		center = 2
	}

	public AuiSprite[] numberSprite;

	public NumberAlign align;

	public float numberWidth = 10f;

	public int numberValue;

	public bool isVisible = true;

	public bool isZeroFill;

	public bool visible
	{
		get
		{
			return isVisible;
		}
		set
		{
			isVisible = value;
			SetValue(numberValue);
		}
	}

	public void Start()
	{
		SetValue(numberValue);
	}

	public void SetValue(int num)
	{
		numberValue = num;
		if (numberValue < 0)
		{
			numberValue = 0;
		}
		string text = numberValue.ToString();
		if (isZeroFill)
		{
			int num2 = numberSprite.Length - text.Length;
			if (num2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					text = "0" + text;
				}
			}
		}
		int num3 = text.Length;
		int num4 = numberSprite.Length;
		if (num3 > num4)
		{
			num3 = num4;
		}
		float num5 = numberWidth * (float)num3;
		Vector3 localPosition = numberSprite[0].transform.localPosition;
		if (align == NumberAlign.left)
		{
			localPosition.x = num5;
		}
		else if (align == NumberAlign.right)
		{
			localPosition.x = numberWidth;
		}
		else
		{
			localPosition.x = num5 * 0.5f + numberWidth * 0.5f;
		}
		for (int j = 0; j < num3; j++)
		{
			int frame = int.Parse(text.Substring(num3 - (j + 1), 1));
			numberSprite[j].SetFrame(frame);
			localPosition.x -= numberWidth;
			numberSprite[j].transform.localPosition = localPosition;
			numberSprite[j].visible = isVisible;
		}
		for (int k = num3; k < num4; k++)
		{
			numberSprite[k].visible = false;
		}
	}
}
