using UnityEngine;

public class CommunityLink : MonoBehaviour
{
	public AuiButton linkTwitter;

	public AuiButton linkFacebook;

	public AuiButton linkYoutube;

	private void Start()
	{
		linkTwitter.onButtonClick = OnGoTwitter;
		linkFacebook.onButtonClick = OnGoFacebook;
		linkYoutube.onButtonClick = OnGoYoutube;
	}

	private void OnGoTwitter(AuiButton sender)
	{
		Application.OpenURL("https://mobile.twitter.com/45castles");
	}

	private void OnGoFacebook(AuiButton sender)
	{
		Application.OpenURL("http://m.facebook.com/45castles");
	}

	private void OnGoYoutube(AuiButton sender)
	{
		Application.OpenURL("http://m.youtube.com/user/AlphaCloudInc");
	}
}
