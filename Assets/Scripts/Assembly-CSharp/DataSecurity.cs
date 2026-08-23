using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class DataSecurity
{
	public const string defaultKey = "surkwjch";

	public static string Encrypt(string p_data, string strkey)
	{
		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		byte[] iV = (dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(strkey.Substring(strkey.Length - 8, 8)));
		dESCryptoServiceProvider.IV = iV;
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
		byte[] bytes2 = Encoding.UTF8.GetBytes(p_data.ToCharArray());
		cryptoStream.Write(bytes2, 0, bytes2.Length);
		cryptoStream.FlushFinalBlock();
		return Convert.ToBase64String(memoryStream.ToArray());
	}

	public static string Decrypt(string p_data, string strkey, out bool succeed)
	{
		succeed = false;
		try
		{
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			byte[] iV = (dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(strkey.Substring(strkey.Length - 8, 8)));
			dESCryptoServiceProvider.IV = iV;
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
			byte[] array = Convert.FromBase64String(p_data);
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.FlushFinalBlock();
			succeed = true;
			return Encoding.UTF8.GetString(memoryStream.GetBuffer());
		}
		catch
		{
			return string.Empty;
		}
	}
}
