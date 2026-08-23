public class PlayMessage
{
	public enum MessagLevel
	{
		normal = 0,
		warning = 1,
		alert = 2
	}

	public const int msgNotify = 0;

	public const int msgTakeTax = 1;

	public const int msgAttackMonster = 2;

	public const int msgFinishRedeploy = 3;

	public const int msgFinishRecruit = 4;

	public const int msgLordEscape = 5;

	public const int msgFortuneGood = 6;

	public const int msgFortuneBad = 7;

	public const int msgFinishUpgrade = 8;

	public const int msgLordLoyaltyDown = 9;

	public int type;

	public MessagLevel level;

	public int day;

	public int hour;

	public string shortMsg = string.Empty;

	public string content = string.Empty;

	public bool isShown;
}
