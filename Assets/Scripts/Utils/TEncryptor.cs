using UnityEngine;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

public class TEncryptor
{	
	/// <summary>
	/// Encrypt and encode with base64 a string
	/// </summary>
	/// <param name="input"> The string to encrypt and encode</param>
	/// <param name="key"> The key for encryption and encode. The minimun lenght of the key is 15 characters. This key should be the same for decrypt (symmetric encryption)</param>
    /// <returns>An encrypted and encoded.</returns>
	public static string encryptAndEncodeToBase64(string input, string key)
    {
		return encodeBase64(encrypt(input,key));
	}
	/// <summary>
	/// Decrypt and decode an encrypted and encoded base64 string
	/// </summary>
	/// <param name="input"> The string to decrypt and decode</param>
	/// <param name="key"> The key for decryption and decode. The minimun lenght of the key is 15 characters. This key should be the same for encrypt (symmetric encryption)</param>
    /// <returns>An decrypted and decoded string.</returns>
	public static string decryptAndDecodeFromBase64(string input, string key)
    {
		return decrypt(decodeBase64(input),key);
	}
	/// <summary>
	/// Encrypt a string to byte array
	/// </summary>
	/// <param name="input"> The string to encrypt</param>
	/// <param name="key"> The key for encryption. The minimun lenght of the key is 15 characters. This key should be the same for decrypt (symmetric encryption)</param>
    /// <returns>An encrypted array of bytes.</returns>
	public static byte[] encrypt(string input, string key)
    {
       byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
       TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
       tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
       tripleDES.Mode = CipherMode.ECB;
       tripleDES.Padding = PaddingMode.PKCS7;
       ICryptoTransform cTransform = tripleDES.CreateEncryptor();
       byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
       tripleDES.Clear();
	   return resultArray;
       //return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
	/// <summary>
	/// Encode a byte array to base64
	/// </summary>
	/// <param name="input"> The string to encode</param>
    /// <returns>an encoded base64 string.</returns>
	public static string encodeBase64(byte[] input)
	{
		return Convert.ToBase64String(input, 0, input.Length);
	}
	/// <summary>
	/// Decode a byte array encoded in base64 and return his byte array
	/// </summary>
	/// <param name="input"> The string from a byte array encoded to base64</param>
    /// <returns>A Decoded byte array.</returns>
	public static byte[] decodeBase64(string input)
	{
		return Convert.FromBase64String(input);
	}
	/// <summary>
	/// Decrypt a byte array to string
	/// </summary>
	/// <param name="input"> The byte array to decrypt</param>
	/// <param name="key"> The key for decryption. The minimun lenght of the key is 15 characters. This key should be the same from encrypt (symmetric encryption)</param>
    /// <returns>A decrypted string.</returns>
	public static string decrypt(byte[] input, string key)
	{
		TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
		tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
		tripleDES.Mode = CipherMode.ECB;
		tripleDES.Padding = PaddingMode.PKCS7;
		ICryptoTransform cTransform = tripleDES.CreateDecryptor();
		byte[] resultArray = cTransform.TransformFinalBlock(input, 0, input.Length);
		tripleDES.Clear();
		return UTF8Encoding.UTF8.GetString(resultArray);
	}
}
