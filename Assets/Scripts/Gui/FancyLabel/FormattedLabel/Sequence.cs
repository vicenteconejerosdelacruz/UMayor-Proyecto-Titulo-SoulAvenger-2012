using UnityEngine;
using System.Collections.Generic;

// A sequence represente at least one character with specific parameters
// empty sequences are merged during the formatting
public class Sequence
{
	// List of commands to be applied BEFORE the drawing
	public List<ICommand> commands;
	
	public Sequence()
	{ 
		commands  = new List<ICommand>(); 
		txt = "";
		alignBottom = false;
		underline = false;
		strikeThrough = false;
		hyperlinkId = "";
	}
	
	public string txt = "";
	public bool alignBottom = false;
	public bool underline = false;
	public bool strikeThrough = false;
	public string hyperlinkId = "";
	
	// empty strings or single spaces aren't drawn.
	public bool isValid{ get{ return txt.Length > 0 && !txt.Equals(" "); } }
	
	public void Add( ICommand command ){ commands.Add( command ); }
	
	// Called by FormattedLine's loop
	// This is basically drawText()
	public void Draw( GUIStyle style, float height )
	{	
		foreach( ICommand C in commands )
			C.Apply( style );
		
        Rect lastRect;
        float fillerHeight;
		
		// Draw the label with a blank space if needs be.
        if( alignBottom )
        {
            fillerHeight = height
                         - style.CalcSize(new GUIContent(txt)).y
                         + (style.fontSize - 16) / 4f; // Where does that 16 comes from ???
            GUILayout.BeginVertical();
			// Draw a blank space the size of fillerHeight
            GUILayout.Label(" ", GUILayout.MinHeight(fillerHeight), GUILayout.MaxHeight(fillerHeight));
            GUILayout.Label(new GUIContent(txt, hyperlinkId), style);
            lastRect = GUILayoutUtility.GetLastRect();
            GUILayout.EndVertical();
        }
        else
        {
            fillerHeight = 0.0f;
            GUILayout.Label(new GUIContent(txt, hyperlinkId), style);
            lastRect = GUILayoutUtility.GetLastRect();
        }
	
		// Handle underlines and strikethroughs
        if (Event.current.type == EventType.Repaint)
        {
            // GetLastRect() is only valid during a repaint event
            if (underline)
            {
                Vector2 from = new Vector2(lastRect.x, lastRect.yMin - fillerHeight + height);
                Vector2 to = new Vector2(from.x + lastRect.width, from.y);
				//Debug.Log( "UNDERLINE for " + txt + " == From : " + from  + ". to : " + to );
                GuiHelper.DrawLine(from, to, style.normal.textColor);
            }
            if (strikeThrough)
            {
                Vector2 from = new Vector2(lastRect.x,
                                           lastRect.yMin - fillerHeight + height - height / 2f);
                Vector2 to = new Vector2(from.x + lastRect.width, from.y);
				//Debug.Log( "STRIKETHRGOUH for " + txt + " == From : " + from  + ". to : " + to );
                GuiHelper.DrawLine(from, to, style.normal.textColor);
            }
        }		
	}
	
	// Used to append two sequences
	public static Sequence operator +( Sequence a, Sequence b)
	{
		//Debug.Log( "Copy requested between " + a + " and " + b );
		Sequence result = new Sequence();
		result.txt = a.txt + b.txt;
		result.alignBottom = a.alignBottom;
		result.underline = a.underline || b.underline;
		result.strikeThrough = a.strikeThrough || b.strikeThrough;
		result.hyperlinkId = a.hyperlinkId + b.hyperlinkId;
		
		foreach( ICommand C in a.commands ) result.commands.Add( C );
		foreach( ICommand C in b.commands ) result.commands.Add( C );
		
		return result;
	}
		
	public override string ToString()
	{
		string str = ":";
		foreach( ICommand C in commands )
			str += C.ToString() + ",";
		
		str += underline ? "UNDERLINED, " : "";
		str += strikeThrough ? "STRICKTHROUGH, " : "";
		str += hyperlinkId.Equals("") ? "" : "HyperLinkID, ";
				
		return txt + str.Substring(0, str.Length-1);
	}
}	
