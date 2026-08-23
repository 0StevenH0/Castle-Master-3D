using System.Collections.Generic;

public class MessageManager
{
	private const int maxMsg = 99;

	private int newMsg;

	private List<PlayMessage> msgList = new List<PlayMessage>();

	public void Add(int type, PlayMessage.MessagLevel mlevel, string content)
	{
		Add(type, mlevel, content, content);
	}

	public void Add(int type, PlayMessage.MessagLevel mlevel, string content, string shortMsg)
	{
		PlayMessage playMessage = new PlayMessage();
		playMessage.type = type;
		playMessage.level = mlevel;
		playMessage.day = PlayInfo.gameTime.day;
		playMessage.hour = PlayInfo.gameTime.hour;
		playMessage.content = content;
		playMessage.shortMsg = shortMsg;
		playMessage.isShown = false;
		msgList.Add(playMessage);
		newMsg++;
		if (newMsg > 99)
		{
			newMsg = 99;
		}
		if (msgList.Count > 99)
		{
			msgList.RemoveAt(0);
		}
	}

	public void Delete(PlayMessage msg)
	{
		msgList.Remove(msg);
	}

	public void Delete(int index)
	{
		msgList.RemoveAt(index);
	}

	public int GetCount()
	{
		return msgList.Count;
	}

	public int GetNewCount()
	{
		return newMsg;
	}

	public void ResetNewCount()
	{
		newMsg = 0;
	}

	public PlayMessage GetLastMessage()
	{
		if (msgList.Count == 0)
		{
			return null;
		}
		return msgList[msgList.Count - 1];
	}

	public PlayMessage[] GetMessageAll()
	{
		return msgList.ToArray();
	}

	public PlayMessage[] GetMessageNoShown()
	{
		int num = 0;
		foreach (PlayMessage msg in msgList)
		{
			if (!msg.isShown)
			{
				num++;
			}
		}
		PlayMessage[] array = new PlayMessage[num];
		int num2 = 0;
		foreach (PlayMessage msg2 in msgList)
		{
			if (!msg2.isShown)
			{
				array[num2] = msg2;
				num2++;
			}
		}
		return array;
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "MessageManager", msgList.Count);
		DataRegistry.Set(parent, "newMsg", newMsg);
		int num = 0;
		foreach (PlayMessage msg in msgList)
		{
			DataRegistry.KeyData parent2 = DataRegistry.Set(parent, num.ToString(), string.Empty);
			DataRegistry.Set(parent2, "type", msg.type);
			DataRegistry.Set(parent2, "level", (int)msg.level);
			DataRegistry.Set(parent2, "day", msg.day);
			DataRegistry.Set(parent2, "hour", msg.hour);
			DataRegistry.Set(parent2, "shortMsg", msg.shortMsg);
			DataRegistry.Set(parent2, "content", msg.content);
			DataRegistry.Set(parent2, "isShown", msg.isShown);
			num++;
		}
	}

	public void Load()
	{
		msgList.Clear();
		newMsg = 0;
		int keyvalue = 0;
		DataRegistry.KeyData current = null;
		if (!DataRegistry.Get(null, "MessageManager", ref keyvalue, ref current))
		{
			return;
		}
		DataRegistry.KeyData current2 = null;
		DataRegistry.Get(current, "newMsg", ref newMsg, ref current2);
		for (int i = 0; i < keyvalue; i++)
		{
			string keyvalue2 = string.Empty;
			if (DataRegistry.Get(current, i.ToString(), ref keyvalue2, ref current2))
			{
				DataRegistry.KeyData current3 = null;
				PlayMessage playMessage = new PlayMessage();
				DataRegistry.Get(current2, "type", ref playMessage.type, ref current3);
				int keyvalue3 = 0;
				DataRegistry.Get(current2, "level", ref keyvalue3, ref current3);
				playMessage.level = (PlayMessage.MessagLevel)keyvalue3;
				DataRegistry.Get(current2, "day", ref playMessage.day, ref current3);
				DataRegistry.Get(current2, "hour", ref playMessage.hour, ref current3);
				DataRegistry.Get(current2, "content", ref playMessage.content, ref current3);
				DataRegistry.Get(current2, "shortMsg", ref playMessage.shortMsg, ref current3);
				DataRegistry.Get(current2, "isShown", ref playMessage.isShown, ref current3);
				msgList.Add(playMessage);
			}
		}
	}
}
