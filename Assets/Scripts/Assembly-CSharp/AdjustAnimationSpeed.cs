using UnityEngine;

public class AdjustAnimationSpeed : MonoBehaviour
{
	public const float defaultSpeed = 0.25f;

	private void Start()
	{
		if (base.GetComponent<Animation>() != null)
		{
			foreach (AnimationState item in base.GetComponent<Animation>())
			{
				item.speed = 0.25f;
			}
		}
		Animation[] componentsInChildren = base.gameObject.GetComponentsInChildren<Animation>();
		Animation[] array = componentsInChildren;
		foreach (Animation animation in array)
		{
			foreach (AnimationState item2 in animation)
			{
				item2.speed = 0.25f;
			}
		}
	}
}
