#if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
	#define WRITE_SAVEGAMES
#endif

#define USE_BINARY_FILES

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class TapjoyDates : MonoBehaviour 
{
	public static string[] fileList = 
	{
#if USE_BINARY_FILES
		 "globaldata.sav"
#else
		 "globaldata.xml"
#endif
	};

	const string encriptionKey = "PyGJNoynLhzpzrEi"; //<random enough
	
	public static string getDirectoryPath()
	{
		string	path = "";
#if UNITY_ANDROID || UNITY_IPHONE
		path = Application.persistentDataPath + "/Data/";
#else
		path = Application.dataPath + "/Data/";
#endif
		return path;
	}
	
	public static string getSaveFileSlotPath()
	{
		return getSaveFileSlotPath(0);
	}
	
	public static string getSaveFileSlotPath(int index)
	{
		return getDirectoryPath() + fileList[index];
	}
	
	public static bool saveFileExist()
	{
		return saveFileExist(0);
	}
	
	public static bool saveFileExist(int index)
	{
	#if !WRITE_SAVEGAMES
		return false;
	#else
		return File.Exists(getSaveFileSlotPath(index));
	#endif
	}
	
	public static void createDirectoryIfNotExist()
	{
	#if WRITE_SAVEGAMES		
		if(!Directory.Exists(getDirectoryPath()))
		{
			Directory.CreateDirectory(getDirectoryPath());
		}		
	#endif
	}
	
	public static string dumpXml(XmlDocument doc)
	{
        StringWriter	sw = new StringWriter();
        XmlTextWriter	tx = new XmlTextWriter(sw);
		
        doc.WriteTo(tx);
		
        return sw.ToString();		
	}
	
	public static void eraseSaveFile()
	{
		eraseSaveFile(0);
	}
	
	public static void eraseSaveFile(int slot)
	{
	#if WRITE_SAVEGAMES
		if(saveFileExist(slot))
		{
			string path = getSaveFileSlotPath(slot);
			
			File.Delete(path);
		}
	#endif
	}
	
	public static XmlDocument getXmlDocFromFile(string path)
	{
		XmlDocument xmlDoc = new XmlDocument();
		
		try
		{
			//load
			#if !USE_BINARY_FILES	
			xmlDoc.Load(path);
			#else
			StreamReader streamReader = new StreamReader(path);
			string decodedString = streamReader.ReadToEnd();
			streamReader.Close();
			
			xmlDoc.LoadXml(TEncryptor.decryptAndDecodeFromBase64(decodedString,encriptionKey));
			#endif
		}
		catch(Exception e)
		{
			if(e!=null){} //keep it here!
			return null;
		}

		return xmlDoc;
	}
		
	public static void CheckPlayingDates()
	{
		#if WRITE_SAVEGAMES		
		createDirectoryIfNotExist();
	
		string path = getSaveFileSlotPath();
		
		//if save file doesn't exist it means is the first time he plays the game or he delete the savefile and we are screw
		if(!saveFileExist())
		{
			XmlDocument doc = new XmlDocument();
			
			XmlNode data = doc.AppendChild(doc.CreateElement("Data"));
			
			DateTime now = DateTime.Now;
			now = now.AddSeconds(-now.Second).AddMinutes(-now.Minute).AddHours(-now.Hour);
			
			data.AppendChild(doc.CreateElement("Day0")).InnerXml = now.ToString();
			data.AppendChild(doc.CreateElement("Day1")).InnerXml = "-1";
			data.AppendChild(doc.CreateElement("Day2")).InnerXml = "-1";
			data.AppendChild(doc.CreateElement("Day3")).InnerXml = "-1";
			
			//save
			#if !USE_BINARY_FILES		
			doc.Save(path);
			#else
			File.WriteAllText(path,TEncryptor.encryptAndEncodeToBase64(dumpXml(doc),encriptionKey));
			#endif			
		}
		else
		{
			XmlDocument doc = getXmlDocFromFile(path);
			if(doc==null)
				return;
		
			DateTime Day0 = DateTime.Parse(doc.GetElementsByTagName("Day0").Item(0).InnerXml);
			
			bool day1isValid = doc.GetElementsByTagName("Day1").Item(0).InnerXml!="-1";
			bool day2isValid = doc.GetElementsByTagName("Day2").Item(0).InnerXml!="-1";
			bool day3isValid = doc.GetElementsByTagName("Day3").Item(0).InnerXml!="-1";
			
			DateTime now = DateTime.Now;
			now = now.AddSeconds(-now.Second).AddMinutes(-now.Minute).AddHours(-now.Hour);			
			
			TimeSpan span = now - Day0;
			
			//Play again on the next day
			if(!day1isValid && span.Days==1)
			{
				doc.GetElementsByTagName("Day1").Item(0).InnerXml = now.ToString();
				#if UNITY_ANDROID
				Game.CompleteTapjoyAction("1a3e7829-9cbc-498c-87cf-70e30e26799d");
				#elif UNITY_IPHONE
				Game.CompleteTapjoyAction("7c3f6acd-30a4-4999-a245-75bba0d4f13a");
				#endif
			}
			//Play again on the 3rd day
			if(!day2isValid && span.Days==2)
			{
				doc.GetElementsByTagName("Day2").Item(0).InnerXml = now.ToString();
				#if UNITY_ANDROID
				Game.CompleteTapjoyAction("e2e6690c-b2c5-4843-95be-661749d6cb4b");
				#elif UNITY_IPHONE
				Game.CompleteTapjoyAction("a200b23f-1313-4136-b363-3ed2c670695e");
				#endif				
			}
			//cPlay again on the 4th day
			if(!day3isValid && span.Days==3)
			{
				doc.GetElementsByTagName("Day3").Item(0).InnerXml = now.ToString();
				#if UNITY_ANDROID
				Game.CompleteTapjoyAction("8a4d018b-526e-4147-ba84-9a881d4bccd2");
				#elif UNITY_IPHONE
				Game.CompleteTapjoyAction("1d4d18fb-5700-42f4-93eb-d424de380823");
				#endif				
			}
			
			//save
			#if !USE_BINARY_FILES		
			doc.Save(path);
			#else
			File.WriteAllText(path,TEncryptor.encryptAndEncodeToBase64(dumpXml(doc),encriptionKey));
			#endif				
		}
		#endif
	}
}
