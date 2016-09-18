using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
/// <summary>
/// Getting Started:
/// Put all of the C# files into your scripts folder.
/// Then put the following code into a script attached to a gameObject:
/// void OnGUI()
/// {
///     string textToFormat = "[ffffffff][ha c]Fireball";
///     FormattedLabel fLabel = new FormattedLabel(screen.Width, textToFormat);
///     fLabel.draw();
/// }
/// Create a series of labels to accomodate the various formatting commands contained
/// within the text.  Commands are enclosed within brackets: [].  The following
/// case-insensitive commands are available:
///   * Background color (BC, BackColor): <RRGGBBAA> or '?' to reset to default
///   * Color (C, Color): <RRGGBBAA> or '?' to reset to default
///   * Font name (F, Font): <font name>
///   * Font attribute (FA, FontAttribute): U (underline on), -U (underline off), S (strikethrough on) or -S (strikethrough off)
///   * Font size (FS, FontSize): <size>
///   * Hyperlink (H, HyperLink): H (hyperlink start) <hyperlink tag>, -H (hyperlink end)
///   * Horizontal alignment (HA, HAlign):  L (left), R (right), or C (center)
///   * Space (S, Space): <pixels>
///   * Vertical alignment (VA, VAlign):  B (bottom) or '?' to reset to Unity default
///   Based on http://forum.unity3d.com/threads/9549-FancyLabel-Multicolor-and-Multifont-label
///   Hexadecimal color picker:  http://www.colorpicker.com/
///   Creator:  
///   Stephane Bessette: 
///   http://forum.unity3d.com/members/28757-Stephane.Bessette
///   
///   Contributers:
///   Shane Mummaw: 
///   mailto:skmummaw@gmail.com / mailto:admin@zerokelvingames.com 
///   http://www.zerokelvingames.com
/// </summary>
public partial class FormattedLabel : IHyperlinkCallback
{
	public bool isFancy{ get; private set; }
    private const string HYPERLINK_TAG = "Hyperlink_";
    private List<string> _lines;
	private List<FormattedLine> _fLines;
    //private bool _fontUnderline = false;
    //private bool _fontStrikethrough = false;
    private Texture2D _backgroundColor;
    private IHyperlinkCallback _hyperlinkCallback;
    private string _lastTooltip = "";
    //private string _createHyperlinkId = "";
    private string _hoveredHyperlinkId = "";
    private bool _activatedHyperlink = false;
    private enum VerticalAlignment { Default, Bottom };
    //private VerticalAlignment _verticalAlignment = VerticalAlignment.Default;
    private float _lineHeight;
    private Color _defaultColor;
	private string rawText;
	public GUIStyle guiStyle;
	public float rawWidth;
	private System.Collections.Hashtable fontTable;
	
	// Those properties can be used to update the FormattedLabel
	public string text
	{
		get{ return rawText; }
		set{ rawText = value; Cook(); }
	}
	public float width
	{
		get{ return rawWidth; }
		set{ rawWidth = value; Cook(); }
	}
    

    /// <summary>
    /// Format the commands wihtin the specified text such that they
    /// are wrapped to the specified width.
    /// </summary>
    /// <param name="width">The width at which to wrap text to new lines</param>
    /// <param name="text">The text to parse</param>
    /// <param name="fonts">The name of the fonts you'll be using. This reduce the number of Resource.Load.</param>
    public FormattedLabel(float _width, string _text, params string[] fonts)
    {
		rawWidth = _width;
		rawText = _text;
		guiStyle = new GUIStyle();

		
		fontTable = new Hashtable();
		foreach( string f in fonts )
		{
			fontTable.Add( f, (Font)Resources.Load("Fonts/" + f) );
		}
		Cook();
    }
	
	// Use that function when you want to change text AND width but rebuild
	// everything only once.
	public void Cook(float _width, string _text)
    {
		rawWidth = _width;
		rawText = _text;
		Cook();
    }
	
	// Must be called once the 
	private void Cook()
	{
		if( isFancy = text.IndexOf('[') >= 0 )
		{
			ProfilerFree.StartProfile( "Cook" );
			TextFormatter textFormatter = new TextFormatter(rawWidth, rawText, fontTable);
			ProfilerFree.EndProfile( "Cook" );
			
			_lines = textFormatter.getLines();
			_fLines = textFormatter.getFLines();
			
			_lines.DebugLines();
			_fLines.DebugLines();
		}
	}
	
    /// <summary>
    /// Draw the formatted text onto the screen
    /// </summary>
    public void Draw()
    {
		try
		{
			if( !isFancy ){
				GUILayout.Label(rawText);
				
				return;
			}
			
			
	       /*GUIStyle guiStyle = new GUIStyle();
	        guiStyle.normal.textColor = GUI.skin.GetStyle("Label").normal.textColor;
	        guiStyle.font = GUI.skin.font;
	
	        guiStyle.border = new RectOffset(0, 0, 0, 0);
	        guiStyle.contentOffset = new Vector2(0.0f, 0.0f);
	        guiStyle.margin = new RectOffset(0, 0, 0, 0);
	        guiStyle.padding = new RectOffset(0, 0, 0, 0);
	
	        _defaultColor = guiStyle.normal.textColor;
	        _defaultBackgroundColor = GUI.skin.GetStyle("Label").normal.background;*/
	
	        GUILayout.BeginVertical();
			{
		        GUILayout.BeginHorizontal();
				{
					foreach( FormattedLine L in _fLines )
		       		{
						L.Draw( guiStyle );
				
				        GUILayout.EndHorizontal();
				        GUILayout.BeginHorizontal();
				    }
				}
		        GUILayout.EndHorizontal();
			}
	        GUILayout.EndVertical();
			
	
	        handleHyperlink();
		}
		catch(ArgumentException e)
		{
			if(e!=null){}
		}
    }
}
