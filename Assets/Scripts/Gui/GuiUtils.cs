using UnityEngine;
using System.Collections;

[System.Serializable]
public class GuiUtilButton
{
	public	Texture2D	enabled;
	public	Texture2D	disabled;
	public	Rect		rect;
	
	public GuiUtilButton(){}
		
	public GuiUtilButton(GuiUtilButton other)
	{
		enabled = other.enabled;
		disabled = other.disabled;
		rect = new Rect(other.rect);
	}
};

public class GuiDragToAreaButton
{
	public delegate void	OnDragDelegate(GuiDragToAreaButton btn,bool enabled);
	public delegate void	OnDropOnAreaDelegate(int index);
	
	//texture
	public	Texture2D		button;
	
	//texture
	public	Texture2D		disabled_button;	
	
	//original rect
	public	Rect			originalRect;
	
	//rect when selected: only width and height are important
	public	Rect			selectedRect;
	
	//rect used for drawing
	public	Rect			currentRect;
	
	public	EventType		mouseState = EventType.MouseUp;
	
	public void	drawButton(Vector2 nMousePos,Rect[] dropRects,OnDragDelegate onDrag,OnDropOnAreaDelegate onDrop , bool enabled)
	{
		currentRect.x = originalRect.x;
		currentRect.y = originalRect.y;
		currentRect.width = originalRect.width;
		currentRect.height = originalRect.height;
		
		if(Event.current.type == EventType.MouseDown)
		{
			Rect area = originalRect;
			
			if(	nMousePos.x > area.x				&&	nMousePos.y > area.y &&
				nMousePos.x < (area.x + area.width)	&&	nMousePos.y < (area.y + area.height)
			   )
			{
				if(enabled)
				{
					mouseState = EventType.MouseDown;
				}
				onDrag(this,enabled);
			}
		}
		else if(Event.current.type == EventType.MouseUp)
		{
			if(mouseState == EventType.MouseDown)
			{
				int index = -1;
			
				for(int i = 0 ; i < dropRects.Length ; i++)
				{
					Rect area = dropRects[i];
					
					if(	nMousePos.x > area.x				&&	nMousePos.y > area.y &&
						nMousePos.x < (area.x + area.width)	&&	nMousePos.y < (area.y + area.height)
			   		)
					{
						index = i;
						break;
					}
				}
				
				mouseState = EventType.MouseUp;
				onDrop(index);
			}
			
		}
		
		if(mouseState == EventType.MouseDown)
		{
			currentRect.x=nMousePos.x - selectedRect.width*0.5f;
			currentRect.y=nMousePos.y - selectedRect.height*0.5f;
			currentRect.width = selectedRect.width;
			currentRect.height = selectedRect.height;
		}
		
		if(enabled)
		{
			GuiUtils.showImage(button,currentRect);
		}
		else
		{
			GuiUtils.showImage(disabled_button,currentRect);
		}
	}
};

public class GuiUtils : TMonoBehaviour {
	
	public delegate void ButtonDelegate(Object o);
	
	public enum AspectRatio
	{
		AspectRatio3by2,
		AspectRatioFree
	};
	
	public static AspectRatio aspectRatio = AspectRatio.AspectRatio3by2;
	
	public static Rect normalizeRectToScreenRect(Rect rect)
	{
		float w = (float)Screen.width;
		float h = (float)Screen.height;
		
		Rect posSz = new Rect(rect);
				
		/*
		posSz.x = Mathf.Max(0.0f,posSz.x);
		posSz.x = Mathf.Min(1.0f,posSz.x);
		
		posSz.y = Mathf.Max(0.0f,posSz.y);
		posSz.y = Mathf.Min(1.0f,posSz.y);
		
		posSz.width = Mathf.Max(0.0f,posSz.width);
		posSz.width = Mathf.Min(1.0f,posSz.width);
		
		posSz.height = Mathf.Max(0.0f,posSz.height);
		posSz.height = Mathf.Min(1.0f,posSz.height);
		*/
		
		float wp3by2 = (2*w - 3*h)*0.25f;
		float rw3by2 = w - 2*wp3by2;		
		
		switch(aspectRatio)
		{
		case AspectRatio.AspectRatio3by2:
			{
				posSz.x = posSz.x*rw3by2 + wp3by2;
			}
			break;
		default:
			{
				posSz.x = posSz.x*w;
			}
			break;
		}
		
		posSz.width = posSz.width*rw3by2;
		
		posSz.y*=h;
		
		posSz.height*=h;
		
		return posSz;
	}
	
	public static Vector2 getNormalizedGuiMouseCoords()
	{
		float w = (float)Screen.width;
		float h = (float)Screen.height;
		float wp = (2*w - 3*h)*0.25f;
		float rw = w - 2*wp;
		
		float x = (Input.mousePosition.x - wp);
		float y = (Input.mousePosition.y);
		
		x = Mathf.Max(0.0f,x);
		x = Mathf.Min(rw,x);
		x/=rw;
		
		y = Mathf.Max(0.0f,y);
		y = Mathf.Min(h,y);
		y/=h;
		y = 1 - y;
		
		return new Vector2(x,y);
	}
	
	public static void showImage(Texture2D texture)
	{
		showImage(texture,new Rect(0,0,1,1));
	}
	
	public static void showImage(Texture2D texture,Rect rect)
	{
		if(texture)
		{
			Rect posSz = normalizeRectToScreenRect(rect);		
			GUI.DrawTexture(posSz,texture,ScaleMode.StretchToFill);
		}
	}
	
	public static void showRotatedImage(Texture2D texture, Rect rect,float angle)
	{
		if(texture)
		{
			//get the final rect coords
			Rect realRect = GuiUtils.normalizeRectToScreenRect(rect);
			
			//get its middle point
			Vector2 midPoint = new Vector2(realRect.x + realRect.width*0.5f,realRect.y + realRect.height*0.5f);
			
			//rotate around that point
			GUIUtility.RotateAroundPivot(angle,midPoint);
			
			//draw
			GUI.DrawTexture(realRect,texture,ScaleMode.StretchToFill);
			
			//restore rotation state
			GUIUtility.RotateAroundPivot(-angle,midPoint);
		}
		
	}
	
	public static void showButton(Rect buttonRect,string text,GUIStyle	style,bool isEnabled,ButtonDelegate cb,Object param)
	{
		Rect posSz = normalizeRectToScreenRect(buttonRect);
		
		if(isEnabled)
			GUI.enabled = true;
		else
			GUI.enabled = false;
		
		if(GUI.Button(posSz,new GUIContent(text,""),style))
		{
			cb(param);
		}
	}
	
	public static void showButton(Rect buttonRect,string text,GUIStyle	style,ButtonDelegate cb,Object param)
	{
		showButton(buttonRect,text,style,true,cb,param);		
	}
	
	public static void showButton(Rect buttonRect,string text,GUIStyle	style,bool isEnabled,ButtonDelegate cb)
	{
		showButton(buttonRect,text,style,isEnabled,cb,null);
	}
	
	public static void showButton(Rect buttonRect,string text,GUIStyle	style,ButtonDelegate cb)
	{
		showButton(buttonRect,text,style,true,cb);
	}
	
	public static void showButton(Rect buttonRect,Texture2D texture,GUIStyle style,ButtonDelegate cb)
	{
		Rect posSz = normalizeRectToScreenRect(buttonRect);
		
		if(GUI.Button(posSz,new GUIContent(texture,""),style))
		{
			cb(null);
		}
	}

	public static void showButton(GuiUtilButton hb,bool enabled,ButtonDelegate cb)
	{
		GUIStyle style = new GUIStyle();
		
		if(enabled)
		{
			style.normal.background = hb.enabled;
			showButton(hb.rect,"",style,true,cb);
		}
		else
		{
			style.normal.background = hb.disabled;
			showButton(hb.rect,"",style,true,cb);
		}
	}
	
	public static void showButton(GuiUtilButton hb,bool enabled,string name,ButtonDelegate cb,GUIStyle style)
	{
		if(enabled)
		{
			style.normal.background = hb.enabled;
			showButton(hb.rect,name,style,true,cb);
		}
		else
		{
			style.normal.background = hb.disabled;
			showButton(hb.rect,name,style,true,delegate(Object o){});
		}
	}	

	public static void showLabel(Rect rect,string text)
	{
		Rect posSz = normalizeRectToScreenRect(rect);
		GUI.Label(posSz,text);
	}

	public static void showLabel(Rect rect,string text,GUIStyle style)
	{
		Rect posSz = normalizeRectToScreenRect(rect);
		GUI.Label(posSz,text,style);
	}
	
	public static void showLabel(Rect rect,string text,GUIStyle style,ButtonDelegate cb)
	{
		Rect posSz = normalizeRectToScreenRect(rect);
		GUI.Label(posSz,text,style);
		cb(null);
	}
	
	public static void showLabelFormat(ref FormattedLabel fl,Rect rect, string text, GUIStyle style, string[] fonts)
	{
		Rect	textRect	=	normalizeRectToScreenRect(rect);
		float	textWidth	=	textRect.width;
		
		if(fl==null)
		{
	    	fl	= new FormattedLabel(textWidth, text.Replace("\\n","\n"),fonts);
		}
	
		try
		{
			GUILayout.BeginArea(textRect);
				fl.guiStyle = style;
	    		fl.Draw();
			GUILayout.EndArea();
		}
		catch(System.ArgumentException e)
		{
			if(e!=null){}
		}		
	}
	
	public static void showLabelFormat(Rect rect, string text, GUIStyle style, string[] fonts)
	{
		FormattedLabel fl = null;
		showLabelFormat(ref fl,rect,text,style,fonts);
	}
	
	public static void showLabelFormat(ref FormattedLabel fl,Rect rect, string text,string[] fonts)
	{
		try
		{
			Rect	textRect	=	normalizeRectToScreenRect(rect);
			float	textWidth	=	textRect.width;
			
			if(fl==null)
			{
				fl	= new FormattedLabel(textWidth, text.Replace("\\n","\n"),fonts);
			}
			
			GUILayout.BeginArea(textRect);
		    	fl.Draw();
			GUILayout.EndArea();
		}	
		catch(System.ArgumentException e)
		{
			if(e!=null){}
		}		
	}
	
	public static void showLabelFormat(Rect rect, string text,string[] fonts)
	{
		FormattedLabel fl = null;
		showLabelFormat(ref fl,rect,text,fonts);
	}
	
	public static Font styleInResolution(GUIStyle style,Font fontNormal,Font fontMidle,Font fontBig,Font fontXXL)
	{
		
		if(Screen.height<=320)
		{
			style.font = fontNormal;
		}
		else if(Screen.height<=480)
		{
			style.font = fontMidle;
		}
		else if(Screen.height<=1024)
		{
			style.font = fontBig;
		}
		else
		{
			style.font = fontXXL;
		}
		return style.font;
	}
	
	public static Font styleInResolution(GUIStyle style,Font fontNormal,Font fontMidle,Font fontBig,Font fontXXL,float fixedSmall)
	{
		
		if(Screen.height<=320)
		{
			style.font = fontNormal;
			style.fixedHeight = 12 - (fixedSmall/2);
		}
		else if(Screen.height<=480)
		{
			style.font = fontMidle;
			style.fixedHeight = 18 - fixedSmall;
		}
		else if(Screen.height<=1024)
		{
			style.font = fontBig;
			style.fixedHeight = 24 - fixedSmall;
		}
		else
		{
			style.font = fontXXL;
			style.fixedHeight = 30 - fixedSmall;
		}
		return style.font;
	}

	
	public static string textFont(string normal,string midle,string big,string xxl)
	{
		
		if(Screen.height<=320)
		{
			return normal;
		}
		else if(Screen.height<=480)
		{
			return midle;
		}
		else if(Screen.height<=1024)
		{
			return big;
		}
		else
		{
			return xxl;
		}
	}
	
}
