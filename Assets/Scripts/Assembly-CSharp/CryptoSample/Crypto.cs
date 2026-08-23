using System;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSample
{
	public static class Crypto
	{
		public static string Encode(string strKey, string strText)
		{
			string text = null;
			try
			{
				if (string.IsNullOrEmpty(strText))
				{
					return string.Empty;
				}
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				byte[] key = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(strKey));
				byte[] bytes = Encoding.UTF8.GetBytes(strText);
				mD5CryptoServiceProvider = null;
				TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
				tripleDESCryptoServiceProvider.Key = key;
				tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
				text = Convert.ToBase64String(tripleDESCryptoServiceProvider.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length));
				tripleDESCryptoServiceProvider = null;
			}
			catch
			{
				text = string.Empty;
			}
			return text;
		}

		public static string Decode(string strKey, string strText)
		{
			string text = null;
			try
			{
				if (string.IsNullOrEmpty(strText))
				{
					return string.Empty;
				}
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				byte[] key = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(strKey));
				byte[] array = Convert.FromBase64String(strText);
				mD5CryptoServiceProvider = null;
				TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
				tripleDESCryptoServiceProvider.Key = key;
				tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
				text = Encoding.UTF8.GetString(tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(array, 0, array.Length));
				tripleDESCryptoServiceProvider = null;
			}
			catch
			{
				text = string.Empty;
			}
			return text;
		}
	}
}
