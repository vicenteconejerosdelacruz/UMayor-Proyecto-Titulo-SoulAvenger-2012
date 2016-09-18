using UnityEngine;
using System;
using System.Collections.Generic;

// Each formatted lines is a list of sequences.
// Only the line's height and the alignment is shared
// for all the sequences of the line.
public class FormattedLine
{
	public TextAlignment alignement;
	public float lineHeight;
	private List<Sequence> sequences;
	
	public FormattedLine(){ sequences = new List<Sequence>(); }
	
	// Add a new sequence
	public void Add( Sequence newSequence ){ sequences.Add( newSequence ); }
	
	// Called by FormattedLabel
	public void Draw( GUIStyle style )
	{	
		try
		{
        	if (alignement == TextAlignment.Right || alignement == TextAlignment.Center)
	            GUILayout.FlexibleSpace();
			foreach( Sequence S in sequences )
				
				S.Draw( style, lineHeight );	
			
        	if (alignement == TextAlignment.Left || alignement == TextAlignment.Center)
	            GUILayout.FlexibleSpace();
		}
		catch(ArgumentException e)
		{
			if(e!=null){}
		}
	}
	
	// For debug purposes
	public override string ToString()
	{
		string str = "";
		foreach( Sequence S in sequences )
			str += S.ToString() + "<||>";
		
		return str;
	}
	
}

public static class LinesDebugger
{
	public static void DebugLines( this List<FormattedLine> lines )
	{
		string str = "FormattedLine Debug. Size " + lines.Count + "\n";
		foreach( FormattedLine F in lines )
			str += F.ToString() + "\n";
		
		//Debug.Log( str );
	}
	public static void DebugLines( this List<string> lines )
	{
		string str = "Strings Debug. Size " + lines.Count + "\n";
		foreach( string S in lines )
			str += S + "\n";
		
		//Debug.Log( str );
	}
}