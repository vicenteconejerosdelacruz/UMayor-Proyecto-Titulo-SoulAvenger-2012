using UnityEngine;
using System.Collections;

// A command is any modification handled by FormattedLabel,
// such as the background color, the font, size etc.
// Each sequence apply all it's commands before being drawn.
public interface ICommand
{
	void Apply( GUIStyle style );
}

// Change the color of the background
public class BackColor : ICommand
{
	private Texture2D bgColor;
	public BackColor( Texture2D _tex ){ bgColor = _tex; }
	public BackColor( string _HexColor )
	{ 
		Color c;
        HexUtil.HexToColor(_HexColor, out c);
        bgColor = new Texture2D(1, 1);
        bgColor.SetPixel(0, 0, c);
        bgColor.wrapMode = TextureWrapMode.Repeat;
        bgColor.Apply();
	}
	
	public void Apply( GUIStyle style ){ style.normal.background = bgColor; }	
}

// Change the color of the text
public class FontColor : ICommand
{
	private Color fontColor;
	public FontColor( Color color ) {  fontColor = color; }
	
	public void Apply( GUIStyle style ){ style.normal.textColor = fontColor; }	
}

// The type Font is already taken, let's not confuse everything
public class CustomFont : ICommand
{
	private Font font;
	public CustomFont( Font _font ) { font = _font; }
	
	public void Apply( GUIStyle style ){ style.font = font; }	
}


// Change the size of the font
public class FontSize : ICommand
{
	private int size;
	public FontSize( int fontSize ) { size = fontSize; }
	
	public void Apply( GUIStyle style ){ style.fontSize = size; }	
}

// Add a space
public class AddSpace : ICommand
{
	private int space;
	public AddSpace( int _space ) { space = _space; }
	
	public void Apply( GUIStyle style ){ GUILayout.Space(space); }	
}