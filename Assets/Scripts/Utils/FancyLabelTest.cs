using UnityEngine;
using System.Collections;

public class FancyLabelTest : GuiUtils {
	
	public Rect r1 = new Rect();
	public Rect r2 = new Rect();
	
	public string t1 = "";
	public string t2 = "";
	
	void OnGUI()
	{
		string[]		fonts = { "Description","ButtonFont","DescriptionMidle", "ButtonFontMidle","ButtonFontBigXL"};
		
		GuiUtils.showLabelFormat(r1,t1,fonts);
		/*
	    //string t1 = "[ffffffff][ha c]Fireball\n";
		string s = "";
		
		s = t1.Replace("\\n","\n");		
	    FormattedLabel fl1 = new FormattedLabel(r1.width, s);
		
		s = t1.Replace("\\n","\n");
	    FormattedLabel fl2 = new FormattedLabel(r2.width, s);		
		
		GUILayout.BeginArea(r1);
	    fl1.Draw();
		GUILayout.EndArea();
		
		GUILayout.BeginArea(r2);
		fl2.Draw();
		GUILayout.EndArea();
		*/
	}
}
