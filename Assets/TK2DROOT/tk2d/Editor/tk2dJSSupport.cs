using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class tk2dJsSupport
{
	[MenuItem(tk2dMenu.root + "Set Up for JavaScript", false, 5)]
	public static void SetUpForJs()
	{
		if (Application.isPlaying)
		{
			EditorUtility.DisplayDialog("Set Up for JavaScript", "Unable to Set Up for JavaScript when in Play mode. Stop and try again.", "Ok");
			return;
		}
		
		string rootDir = "Plugins";
		string baseDir = rootDir + "/tk2d";
		string fontsBase = baseDir + "/Fonts";
		string guiBase = baseDir + "/Gui";
		string spritesBase = baseDir + "/Sprites";
		string camerasBase = baseDir + "/Camera";
		
		RequireDirectory(rootDir);
		RequireDirectory(baseDir);
		RequireDirectory(fontsBase);
		RequireDirectory(guiBase);
		RequireDirectory(spritesBase);
		RequireDirectory(camerasBase);

		AssetDatabase.Refresh();

		MoveFile("tk2dCamera.cs", camerasBase);
		MoveFile("tk2dCameraAnchor.cs", camerasBase);

		MoveFile("tk2dSpriteCollectionData.cs", spritesBase);
		MoveFile("tk2dSpriteCollection.cs", spritesBase);
		MoveFile("tk2dSpriteAnimation.cs", spritesBase);
		MoveFile("tk2dPixelPerfectHelper.cs", spritesBase);
		MoveFile("tk2dBaseSprite.cs", spritesBase);
		MoveFile("tk2dSprite.cs", spritesBase);
		MoveFile("tk2dSlicedSprite.cs", spritesBase);
		MoveFile("tk2dAnimatedSprite.cs", spritesBase);
		MoveFile("tk2dStaticSpriteBatcher.cs", spritesBase);
		
		MoveFile("tk2dFontData.cs", fontsBase);
		MoveFile("tk2dFont.cs", fontsBase);
		MoveFile("tk2dTextMesh.cs", fontsBase);

		MoveFile("tk2dButton.cs", guiBase);
	}
	
	static void RequireDirectory(string name)
	{
		string path = Application.dataPath + "/" + name;
		if (!System.IO.Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
	}
	
	static string FindPath(string foundPath, System.IO.DirectoryInfo root, string file)
	{
		System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;
		
		try
		{
			files = root.GetFiles(file);
			subDirs = root.GetDirectories();
		}
		catch
		{
			return foundPath;
		}
		
		foreach (var f in files)
		{
			return f.FullName;
		}
		
		foreach (var sd in subDirs)
		{
			foundPath = FindPath(foundPath, sd, file);
			if (foundPath != "")
				return foundPath;
		}
		
		return foundPath;
	}
	
	static void MoveFile(string file, string dest)
	{
		System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.dataPath);
		string sourcePath = FindPath("", di, file);
		if (sourcePath != "")
		{
			sourcePath = sourcePath.Replace("\\", "/").Substring(Application.dataPath.Length + 1);
			string destPath = dest + "/" + file;
			AssetDatabase.MoveAsset("Assets/" + sourcePath, "Assets/" + destPath);
		}
	}
}
