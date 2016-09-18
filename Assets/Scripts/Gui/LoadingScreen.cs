using UnityEngine;
using System.Collections;

public class LoadingScreen :GuiUtils
{
	public TranslatedText[] tips;
	
	static public string[] fonts = new string[]{"ButtonFontSmall", "ButtonFontBig","FontSize24","FontSize42"};
	
	public Rect animText;
	public Rect tipTextRect;
	
	public string fontLoading;

	private string fontInResolution;
	
	//private float progress;
	
	private int randomTip;
	
	private FormattedLabel	loadingMessageLabel	= null;
	private FormattedLabel	tipMessageLabel		= null;
	
	public override void TStart ()
	{
		base.TStart ();
		randomTip = Random.Range(0,tips.Length);
		loadingMessageLabel = null;
		tipMessageLabel = null;
	}
	
	private float timer = 0.0f;
	
	public TranslatedText loadingText;
	
	public void OnGUI()
	{
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		fontInResolution = textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]");
		
		timer+=Time.deltaTime;
		
		fontLoading = "[c FFFFFFFF]"+loadingText.text;
		
		if(timer<1.0f)
		{
			loadingMessageLabel = null;
			fontLoading+= ".";
		}
		else
		if(timer<2.0f)
		{
			loadingMessageLabel = null;
			fontLoading+= "..";
		}
		else
		if(timer<3.0f)
		{
			loadingMessageLabel = null;
			fontLoading+= "...";
		}		
		else
		{
			loadingMessageLabel = null;
			timer = 0.0f;
		}
		
		fontLoading+= "[c FFFFFFFF]";
		
		
		/*
		progress +=0.1f;
		
		if(progress<0.5f)
		{
			fontLoading = "[c FFFFFFFF]LOADING[c FFFFFFFF]";	
		}
		else if(progress<4f)
		{
			fontLoading = "[c FFFFFFFF]LOADING.[c FFFFFFFF]";	
		}
		else if(progress<8f)
		{
			fontLoading = "[c FFFFFFFF]LOADING..[c FFFFFFFF]";	
		}
		else
		{
			fontLoading = "[c FFFFFFFF]LOADING...[c FFFFFFFF]";	
		}
		 
		if(progress>15f)
		{
			progress = 0f;
		}
		*/
		showLabelFormat(ref loadingMessageLabel,animText,fontInResolution+fontLoading+fontInResolution,fonts);
		
		showLabelFormat(ref tipMessageLabel,tipTextRect,fontInResolution+"[c FFFFFFFF][HA C]"+tips[randomTip].text+"[c FFFFFFFF]"+fontInResolution,fonts);
	}
}
