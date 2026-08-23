using UnityEngine;
using UnityEngine.SceneManagement;

public class ProcStartup : MonoBehaviour
{
	private void Start()
	{
		SceneManager.LoadScene("Scene Title");
	}
}
