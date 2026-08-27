using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DataRegistry
{
	public class ParamData
	{
		public string dataname = string.Empty;

		public string datavalue = string.Empty;
	}

	public class KeyData
	{
		public string keyname = string.Empty;

		public string keyvalue = string.Empty;

		public List<KeyData> child = new List<KeyData>();
	}

	private const bool isEncrypt = true;

	private const string slotName = "DataSlot";

	private const string lineSeparater = "\n";

	private const string arraySeparater = "|";

	private const string valueSeparater = "/";

	private static bool usePlayerPrefs = true;

	private static string SavePath(int slot)
	{
		return Path.Combine(Application.persistentDataPath, "DataSlot" + slot + ".sav");
	}

	private static int currentSlot = 0;

	private static KeyData dataList = new KeyData();

	private static byte[] Skey = Encoding.ASCII.GetBytes(BuildSkeySource());

	private static string BuildSkeySource()
	{
		string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
		if (string.IsNullOrEmpty(deviceUniqueIdentifier) || deviceUniqueIdentifier == SystemInfo.unsupportedIdentifier)
		{
			deviceUniqueIdentifier = "castlemaster";
		}
		deviceUniqueIdentifier = deviceUniqueIdentifier.PadLeft(8, 'x');
		return deviceUniqueIdentifier.Substring(deviceUniqueIdentifier.Length - 8, 8);
	}

	public static void ClearAll()
	{
		dataList = new KeyData();
		GC.Collect();
	}

	public static void SetSlot(int slot)
	{
		currentSlot = slot;
		ClearAll();
		Load();
	}

	public static void DeleteSlot(int slot)
	{
		string path = SavePath(slot);
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		if (slot == currentSlot)
		{
			ClearAll();
		}
	}

	public static KeyData Set(KeyData parent, string keyname, string keyvalue)
	{
		if (parent == null)
		{
			parent = dataList;
		}
		foreach (KeyData item in parent.child)
		{
			if (keyname.Equals(item.keyname))
			{
				item.keyvalue = keyvalue;
				return item;
			}
		}
		KeyData keyData = new KeyData();
		keyData.keyname = keyname;
		keyData.keyvalue = keyvalue;
		parent.child.Add(keyData);
		return keyData;
	}

	public static KeyData Set(KeyData parent, string keyname, int keyvalue)
	{
		return Set(parent, keyname, keyvalue.ToString());
	}

	public static KeyData Set(KeyData parent, string keyname, float keyvalue)
	{
		return Set(parent, keyname, keyvalue.ToString());
	}

	public static KeyData Set(KeyData parent, string keyname, bool keyvalue)
	{
		return Set(parent, keyname, (!keyvalue) ? "0" : "1");
	}

	public static KeyData Set(KeyData parent, string keyname, int[] keyvalue)
	{
		string text = string.Empty;
		for (int i = 0; i < keyvalue.Length; i++)
		{
			text = text + keyvalue[i] + ((i + 1 != keyvalue.Length) ? "|" : string.Empty);
		}
		return Set(parent, keyname, text);
	}

	public static KeyData Set(KeyData parent, string keyname, float[] keyvalue)
	{
		string text = string.Empty;
		for (int i = 0; i < keyvalue.Length; i++)
		{
			text = text + keyvalue[i] + ((i + 1 != keyvalue.Length) ? "|" : string.Empty);
		}
		return Set(parent, keyname, text);
	}

	public static KeyData Set(KeyData parent, string keyname, bool[] keyvalue)
	{
		string text = string.Empty;
		for (int i = 0; i < keyvalue.Length; i++)
		{
			text = text + ((!keyvalue[i]) ? "0" : "1") + ((i + 1 != keyvalue.Length) ? "|" : string.Empty);
		}
		return Set(parent, keyname, text);
	}

	public static bool Get(KeyData parent, string keyname, ref string keyvalue, ref KeyData current)
	{
		if (parent == null)
		{
			parent = dataList;
		}
		foreach (KeyData item in parent.child)
		{
			if (keyname.Equals(item.keyname))
			{
				keyvalue = item.keyvalue;
				current = item;
				return true;
			}
		}
		return false;
	}

	public static bool Get(KeyData parent, string keyname, ref int keyvalue, ref KeyData current)
	{
		string keyvalue2 = string.Empty;
		if (!Get(parent, keyname, ref keyvalue2, ref current))
		{
			return false;
		}
		int result = 0;
		bool flag = int.TryParse(keyvalue2, out result);
		if (flag)
		{
			keyvalue = result;
		}
		return flag;
	}

	public static bool Get(KeyData parent, string keyname, ref float keyvalue, ref KeyData current)
	{
		string keyvalue2 = string.Empty;
		if (!Get(parent, keyname, ref keyvalue2, ref current))
		{
			return false;
		}
		float result = 0f;
		bool flag = float.TryParse(keyvalue2, out result);
		if (flag)
		{
			keyvalue = result;
		}
		return flag;
	}

	public static bool Get(KeyData parent, string keyname, ref bool keyvalue, ref KeyData current)
	{
		string keyvalue2 = string.Empty;
		bool flag = Get(parent, keyname, ref keyvalue2, ref current);
		if (flag)
		{
			keyvalue = keyvalue2.Equals("1");
		}
		return flag;
	}

	public static bool Get(KeyData parent, string keyname, ref int[] keyvalue, ref KeyData current)
	{
		string keyvalue2 = string.Empty;
		bool flag = Get(parent, keyname, ref keyvalue2, ref current);
		if (!flag)
		{
			return false;
		}
		string[] array = keyvalue2.Split("|".ToCharArray());
		int num = 0;
		string[] array2 = array;
		foreach (string s in array2)
		{
			int result = 0;
			if (int.TryParse(s, out result))
			{
				keyvalue[num] = result;
			}
			num++;
		}
		return flag;
	}

	public static bool Get(KeyData parent, string keyname, ref float[] keyvalue, ref KeyData current)
	{
		string keyvalue2 = string.Empty;
		bool flag = Get(parent, keyname, ref keyvalue2, ref current);
		if (!flag)
		{
			return false;
		}
		string[] array = keyvalue2.Split("|".ToCharArray());
		int num = 0;
		string[] array2 = array;
		foreach (string s in array2)
		{
			float result = 0f;
			if (float.TryParse(s, out result))
			{
				keyvalue[num] = result;
			}
			num++;
		}
		return flag;
	}

	public static bool Get(KeyData parent, string keyname, ref bool[] keyvalue, ref KeyData current)
	{
		string keyvalue2 = string.Empty;
		bool flag = Get(parent, keyname, ref keyvalue2, ref current);
		if (!flag)
		{
			return false;
		}
		string[] array = keyvalue2.Split("|".ToCharArray());
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			keyvalue[num] = text.Equals("1");
			num++;
		}
		return flag;
	}

	private static void WriteKeyDataToStream(KeyData data, StreamWriter stream)
	{
		stream.WriteLine(data.keyname + "/" + data.child.Count + "/" + data.keyvalue);
		foreach (KeyData item in data.child)
		{
			WriteKeyDataToStream(item, stream);
		}
	}

	public static void Save()
	{
		if (usePlayerPrefs)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			WriteKeyDataToStream(dataList, streamWriter);
			streamWriter.Flush();
			memoryStream.Seek(0L, SeekOrigin.Begin);
			byte[] array = new byte[memoryStream.Length];
			memoryStream.Read(array, 0, (int)memoryStream.Length);
			string @string = Encoding.UTF8.GetString(array);
			@string = Encrypt(@string);
			File.WriteAllText(SavePath(currentSlot), @string);
		}
	}

	private static void ReadKeyDataFromStream(KeyData parent, StreamReader stream)
	{
		string text = stream.ReadLine();
		string[] array = text.Split('/');
		if (array.Length >= 3)
		{
			parent.keyname = array[0];
			parent.keyvalue = array[2];
			int num = int.Parse(array[1]);
			for (int i = 0; i < num; i++)
			{
				KeyData keyData = new KeyData();
				parent.child.Add(keyData);
				ReadKeyDataFromStream(keyData, stream);
			}
		}
	}

	public static void Load()
	{
		if (!usePlayerPrefs)
		{
			return;
		}
		string path = SavePath(currentSlot);
		string @string = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
		if (@string.Length != 0)
		{
			@string = Decrypt(@string);
			dataList = new KeyData();
			if (@string.Length != 0)
			{
				MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(@string));
				StreamReader stream2 = new StreamReader(stream);
				ReadKeyDataFromStream(dataList, stream2);
			}
		}
	}

	private static string Encrypt(string p_data)
	{
		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		dESCryptoServiceProvider.Key = Skey;
		dESCryptoServiceProvider.IV = Skey;
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
		byte[] bytes = Encoding.UTF8.GetBytes(p_data.ToCharArray());
		cryptoStream.Write(bytes, 0, bytes.Length);
		cryptoStream.FlushFinalBlock();
		return Convert.ToBase64String(memoryStream.ToArray());
	}

	private static string Decrypt(string p_data)
	{
		try
		{
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			dESCryptoServiceProvider.Key = Skey;
			dESCryptoServiceProvider.IV = Skey;
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
			byte[] array = Convert.FromBase64String(p_data);
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.FlushFinalBlock();
			return Encoding.UTF8.GetString(memoryStream.GetBuffer());
		}
		catch
		{
			return string.Empty;
		}
	}
}
