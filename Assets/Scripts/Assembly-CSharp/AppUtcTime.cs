using System;
using UnityEngine;

public class AppUtcTime : MonoBehaviour
{
	public delegate void OnUpdateUtcTime(bool isSucceed);

	public static DateTime utcNow = DateTime.UtcNow;

	public void UpdateUtcTime(OnUpdateUtcTime rst)
	{
		utcNow = DateTime.UtcNow;
		rst(true);
	}
}
