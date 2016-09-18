using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


public class MunerisMenuItem : MonoBehaviour
{
	private static string sdkPath = "__MunerisAndroidSDKRoot";

	[MenuItem( "Muneris/Debug Build Android App...", false, 0 )]
	static void buildApp()
	{
		buildApp(BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler, false);
	}
	
	[MenuItem( "Muneris/Debug Build and Install Android App...", false, 1 )]
	static void buildAppInstall()
	{
		buildApp(BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler, true);
	}
	
	[MenuItem( "Muneris/Release Build Android App...", false, 12 )]
	static void buildAppDebug()
	{
		//buildApp(BuildOptions.None, false);
		buildApp(BuildOptions.None, false);
	}
	
	[MenuItem( "Muneris/Release Build and Install Android App...", false, 13 )]
	static void buildAppInstallDebug()
	{
		buildApp(BuildOptions.None, true);
	}
	
	[MenuItem( "Muneris/Select Android SDK...", false, 24 )]
	static void setSDKPath()
	{
		string path = EditorPrefs.GetString(sdkPath, "");
		EditorPrefs.SetString(sdkPath, EditorUtility.OpenFolderPanel("Android SDK Directory", path, ""));
	}
	
	/*[MenuItem( "Muneris/Export Muneris Package...", false, 35 )]
	static void exportMuneris()
	{
		string path = EditorUtility.SaveFilePanel("Select Package Name", "", "Muneris.unitypackage", "unitypackage");
		if ( path == "" )
			return;
		
		AssetDatabase.ExportPackage(new string[] {"Assets/Plugins/Android", "Assets/Editor/Muneris", "Assets/Plugins/Muneris"}, path);
	}*/
	
	static void runCommand(String app, String param)
	{	
		UnityEngine.Debug.Log("Running command " + app + " " + param);
		
		ProcessStartInfo info1 = new ProcessStartInfo();
		info1.CreateNoWindow = true;
		info1.UseShellExecute = true;
		info1.ErrorDialog = true;
		info1.FileName = app;
		info1.WindowStyle = ProcessWindowStyle.Hidden;
		info1.Arguments = param;
		info1.WorkingDirectory = Directory.GetCurrentDirectory() + "/Temp/StagingArea";
		
		try
		{
			using (Process proc = Process.Start(info1))
			{
				 proc.StartInfo.RedirectStandardOutput = true;

            	proc.StartInfo.RedirectStandardInput = true;

            	proc.StartInfo.RedirectStandardError = true;
				
				proc.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
					UnityEngine.Debug.Log(e.Data);
					
				};
				
				proc.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
					UnityEngine.Debug.LogError(e.Data);				
				};
				
				
				proc.WaitForExit();
			}
		}
		catch ( Exception err )
		{
			UnityEngine.Debug.LogError(err.Message);	
		}
	}
	
	static public void CopyFolder( string sourceFolder, string destFolder )
    {
		UnityEngine.Debug.Log("Copying folder: " + sourceFolder);	
		
        if (!Directory.Exists( destFolder ))
            Directory.CreateDirectory( destFolder );
        string[] files = Directory.GetFiles( sourceFolder );
        foreach (string file in files)
        {
            string name = Path.GetFileName( file );
            string dest = Path.Combine( destFolder, name );
			if(Path.GetFileName(name).IndexOf('.') != 0 && Path.GetExtension(name).ToLower() != ".meta")
            	File.Copy( file, dest );
        }
        string[] folders = Directory.GetDirectories( sourceFolder );
        foreach (string folder in folders)
        {
            string name = Path.GetFileName( folder );
            string dest = Path.Combine( destFolder, name );
            CopyFolder( folder, dest );
        }
    }
	
	static void buildApp(BuildOptions options, bool install)
	{
		bool isDebug = ((options & BuildOptions.Development) != 0);
		
		// Sanity check to make sure key is provided
		UnityEngine.Debug.Log("keyaliasPass:"+PlayerSettings.Android.keyaliasPass);
		UnityEngine.Debug.Log("keyaliasName:"+PlayerSettings.Android.keyaliasName);
		UnityEngine.Debug.Log("keystoreName:"+PlayerSettings.Android.keystoreName);
		UnityEngine.Debug.Log("keystorePass:"+PlayerSettings.Android.keystorePass);
		
		/*
		if ( !isDebug )
		{
			if ( PlayerSettings.Android.keyaliasPass == "" ||
				 PlayerSettings.Android.keyaliasName == "" ||
				 PlayerSettings.Android.keystoreName == "" ||
				 PlayerSettings.Android.keystorePass == "" )
			{
				EditorUtility.DisplayDialog("Error", "No key signing information provided", "ok");
				return;
			}
		}
		*/
		// Make sure we have the SDK path
		
		string sdk = EditorPrefs.GetString(sdkPath, "");
		if ( sdk == "" )
		{
			setSDKPath();
			
			sdk = EditorPrefs.GetString(sdkPath, "");
			if ( sdk == "" )
				return;
		}
		
		UnityEngine.Debug.Log("Using Android SDK: " + sdk);
		string progressTitle = "Android Build...";
		
		// Ask user to choose a file to build to
		
		string path = EditorUtility.SaveFilePanel("Select APK File", "", EditorPrefs.GetString("__MunerisBuildFile", "Build.apk"), "apk");
		if ( path.Length == 0 )
			return;
		
		EditorPrefs.SetString("__MunerisBuildFile", Path.GetFileName(path));
		
		// Create the list of active scenes to build
		
		List<String> tmp = new List<String>();
		foreach ( EditorBuildSettingsScene s in EditorBuildSettings.scenes )
		{
			if ( s.enabled )
				tmp.Add(s.path);
		}
		
		// Do a build to get the StagingArea directory setup
		
		string err = BuildPipeline.BuildPlayer(tmp.ToArray(), path, BuildTarget.Android, options);
		if ( err != "" )
		{
			EditorUtility.ClearProgressBar();
			EditorUtility.DisplayDialog("Error", err, "ok");
			return;
		}
		
		EditorUtility.DisplayProgressBar(progressTitle, "Removing Vendor Directory", 0.0f);
		
		if(Directory.Exists("Temp/StagingArea/src"))
			Directory.Delete("Temp/StagingArea/src", true);
		if(Directory.Exists("Temp/vendors"))
			Directory.Delete("Temp/vendors", true);
		
		EditorUtility.DisplayProgressBar(progressTitle, "Copying Vendor Directory", 0.0f);
		
		CopyFolder("Assets/Plugins/Android/src" , "Temp/StagingArea/src");
		
		//CopyFolder("Assets/Plugins/Android/vendors/facebook/3.1.1/facebook-sdk/src" , "Temp/StagingArea/src");
		
		CopyFolder("Assets/Plugins/Android/vendors" , "Temp/vendors");
		
		
		
		
		// Copy files into libs directory
		
		string[] libFiles = Directory.GetFiles("Temp/StagingArea/plugins");
		int x = 0;
		
		foreach ( string s in libFiles )
		{
			string dest = Path.Combine("Temp/StagingArea/libs", Path.GetFileName(s));
			
			string debug = "Moving " + s + " to " + dest;
			UnityEngine.Debug.Log(debug);
			
			EditorUtility.DisplayProgressBar(progressTitle, debug, (x++)/(float)libFiles.Length);
			File.Move(s, dest);
		}
		
		try
		{
			File.Move("Temp/StagingArea/bin/classes.jar", "Temp/StagingArea/libs/classes.jar");
		}
		catch ( Exception )
		{
			// ...
		}
		
		// Create the src directory
		
		//Directory.CreateDirectory("Temp/StagingArea/src");
		
		// Delete the bin directory to ensure a clean build
		
		//Directory.Delete("Temp/StagingArea/gen", true);
		//Directory.Delete("Temp/StagingArea/bin", true);
		
		// Update android project settings
		
		EditorUtility.DisplayProgressBar(progressTitle, "Updating project settings.", 0.0f);
		
		//their code
		//runCommand(Path.Combine(sdk, "tools/android"), "update project -p . -t android-14 --library ../vendors/mopub/1.7.0.0/mopub-android-sdk --library ../vendors/facebook/3.1.1/facebook-sdk");
		//my code
		runCommand(Path.Combine(sdk, "tools/android"), "update project -p . -t android-14 --library ../vendors/mopub/1.7.0.0/mopub-android-sdk");
		runCommand(Path.Combine(sdk, "tools/android"), "update project -p . -t android-14 --library ../vendors/facebook/3.1.1/facebook-sdk");
		
		runCommand(Path.Combine(sdk, "tools/android"), "update project -p ../vendors/mopub/1.7.0.0/mopub-android-sdk -t android-14");
		runCommand(Path.Combine(sdk, "tools/android"), "update project -p ../vendors/facebook/3.1.1/facebook-sdk -t android-14");
		
		EditorUtility.DisplayProgressBar(progressTitle, "Cleaning Android project.", 0.0f);
		
		
		
		runCommand("ant", "clean");


		// Perform the build
		
		EditorUtility.DisplayProgressBar(progressTitle, "Performing build...", 0.25f);
		
		if(isDebug){
			runCommand("ant", "debug");
		}else{
			runCommand("ant", "release");
		}
		
		
		// Create debug keystore if necessary
		
		if ( isDebug )
		{
			//EditorUtility.DisplayProgressBar(progressTitle, "Creating debug keystore...", 0.45f);
			//runCommand("keytool", "-genkey -keyalg RSA -sigalg MD5withRSA -alias androiddebugkey -keystore debug.keystore -keypass android -storepass android -dname \"CN=Muneris Debug, OU=Debug, O=Outblaze, L=HK, S=HK, C=HK\"");
		}
		
		// Sign the APK
		//string keystoreName = PlayerSettings.Android.keystoreName;
		//string keystorePass = PlayerSettings.Android.keystorePass;
		//string keyaliasPass = PlayerSettings.Android.keyaliasPass;
		//string keyaliasName = PlayerSettings.Android.keyaliasName;
		
		string keystoreName = "tksoulavenger.keystore";
		string keystorePass = "lemuria79";
		string keyaliasPass = "lemuria79";
		string keyaliasName = "tksoulavenger";	
//		string keystoreName = "DarkRedemption.keystore";
//		string keystorePass = "bdpubapps2012";
//		string keyaliasPass = "bdpubapps2012";
//		string keyaliasName = "darkredemption";
		
		string signcmd = "-sigalg MD5withRSA -digestalg SHA1"
			+ " -keystore " + (isDebug ? "debug.keystore" : keystoreName) + " -storepass " + (isDebug ? "android" : keystorePass)
			+ " -keypass " + (isDebug ? "android" : keyaliasPass)
			+ (isDebug ? " bin/StagingArea-debug.apk " : " bin/StagingArea-release-unsigned.apk ") 
			+ (isDebug ? "androiddebugkey" : keyaliasName);
		
		if(!isDebug){
			EditorUtility.DisplayProgressBar(progressTitle, "Signing APK...", 0.50f);
			runCommand("jarsigner", signcmd); 
		}
		
		// Copy the final jar to destination
		
		try
		{
			File.Delete(Path.Combine("Temp/StagingArea/bin", path));
			if(isDebug){
				File.Copy("Temp/StagingArea/bin/StagingArea-debug.apk", path);
			}else{
				File.Copy("Temp/StagingArea/bin/StagingArea-release-unsigned.apk", path);
			}
		}
		catch ( Exception e)
		{
			UnityEngine.Debug.Log("Error copying file. " + e.Message);
			EditorUtility.ClearProgressBar();
			EditorUtility.DisplayDialog("Error", "Could not move APK to destination path. Please try again.", "ok");
			return;
		}
		
		// Install if necessary
			
		if ( install )
		{
			EditorUtility.DisplayProgressBar(progressTitle, "Installing APK on device...", 0.75f);
			
			runCommand(Path.Combine(sdk, "platform-tools/adb"), "uninstall " +  PlayerSettings.bundleIdentifier);
			
			runCommand(Path.Combine(sdk, "platform-tools/adb"), "install " + path);
                        
			runCommand(Path.Combine(sdk, "platform-tools/adb"), "push " + path.Substring(0,path.Length-4)+"_gameData.apk /mnt/sdcard/Android/obb/" + PlayerSettings.bundleIdentifier + "/main."+PlayerSettings.Android.bundleVersionCode +"."+PlayerSettings.bundleIdentifier+".obb");
			
			runCommand(Path.Combine(sdk, "platform-tools/adb"), "shell am start -n " + PlayerSettings.bundleIdentifier + "/muneris.android.unity.Activity");
		}
		
		// All done...
		EditorUtility.ClearProgressBar();
		EditorUtility.DisplayDialog("Done", path, "ok");
		
	}
	
}
