using UnityEngine;
using System.Collections.Generic;
using System.Text;

public static class StringCompare
{
	public static bool IsEqual( this string a, string b )
	{
        return string.Equals(a, b, System.StringComparison.OrdinalIgnoreCase);
	}
}

public partial class FormattedLabel : IHyperlinkCallback
{
	/// <summary>
    /// Internal class to parse a text entry into an easier to process
    /// text (assumedly the parsed text is faster to process).  The
    /// available commands are also standardized to their short forms
    /// and in uppercase.
    /// </summary>
    private class TextFormatter
    {
        private const string _lineHeightCommand = "[LH &]";
		private const string HYPERLINK_TAG = "Hyperlink_";
        private float _width;
        private List<string> _lines;
		private List<FormattedLine> _fLines;
		private Sequence _sequence;
		private Sequence _nextSequence;
		private FormattedLine _fLine;
        private GUIStyle _guiStyle;
        private StringBuilder _line;
        private float _lineLength;
        private float _lineHeight;
        private bool invalidCommand = false;
		private System.Collections.Hashtable fontTable;

        public TextFormatter(float width, string text, System.Collections.Hashtable fonts )
        {
            _width = width;
			_lines = new List<string>();
			_fLines = new List<FormattedLine>();
			fontTable = fonts;
            format(text);
        }

        public List<string> getLines()
        {
            return _lines;
        }
        public List<FormattedLine> getFLines()
        {
            return _fLines;
        }
		
        /// <summary>
        /// Format the raw text into an easier (aka faster) format to parse.
        /// * Process \n such that they are removed and 'new lines' created
        /// * Break down the text into lines that fit the requested width
        /// </summary>
        /// <param name="text">The raw text to parse</param>
        private void format(string text)
        {
            //Debug.Log("Formatting: " + text);
            _guiStyle = new GUIStyle();
            int endIndex;
            _line = new StringBuilder();
			_fLine = new FormattedLine();
            addLineHeight(false);
            _lineLength = 0;
            _lineHeight = 0.0f;
            StringBuilder word = new StringBuilder();
			_sequence = new Sequence();
			_nextSequence = new Sequence();

            for (int letterIndex = 0; letterIndex < text.Length; letterIndex++)
            {
                int currentLetterIndex = letterIndex;

                if (text[letterIndex] == '\\'
                    && text.Length > letterIndex + 1
                    && text[letterIndex + 1] == '\\')
                {
                    // Escaped '\'
                    word.Append("\\");
                    letterIndex++; // Skip the second '\'
                }
                else if (text[letterIndex] == '\n')
                {
                    // New line
                    addWordToLine(word.ToString());
                    createNewLine();
                    word.Length = 0;
                }
                else if (text[letterIndex] == ' '
                    && word.Length != 0)
                {
                    // Reached a word boundary
                    addWordToLine(word.ToString());
                    word.Length = 0;
                    word.Append(' ');
                }
                else if (text[letterIndex] == '['
                    && text.Length > letterIndex + 1
                    && text[letterIndex + 1] == '[')
                {
                    // Escaped '['
                    word.Append("[[");
                    letterIndex++; // Skip the second '['
                }
                else if (text[letterIndex] == '['
                    && text.Length > letterIndex + 1
                    && (endIndex = text.IndexOf(']', letterIndex)) != -1)
                {
                    // Command
                    addWordToLine(word.ToString());
                    word.Length = 0;
                    string command = text.Substring(letterIndex + 1, endIndex - letterIndex - 1);
                    letterIndex += command.Length + 1;
                    string[] commandList = command.Split(' ');
                    for (int commandIndex = 0; commandIndex < commandList.Length; commandIndex++)
                    {
                        command = commandList[commandIndex].ToUpper();
						if ( command.IsEqual("NL") || command.IsEqual("NEWLINE") )
						{
							createNewLine();
						}
                        else if ( command.IsEqual("BC") || command.IsEqual("BACKCOLOR") )
                        //if (command == "BC" || command == "BACKCOLOR")
                        {
                            // Background Color
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                Color color;
                                if (commandList[commandIndex] == "?" )
								{
									_nextSequence.Add( new BackColor(_guiStyle.normal.background) );
                                    addCommandToLine("BC " + commandList[commandIndex]);
								}
								else if( HexUtil.HexToColor(commandList[commandIndex], out color))
                                {
									_nextSequence.Add( new BackColor(commandList[commandIndex]) );
                                    addCommandToLine("BC " + commandList[commandIndex]);
                                }
                                else
                                {
                                    Debug.LogError("The 'BackColor' command requires a color parameter of RRGGBBAA or '?'.");
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'BackColor' command requires a color parameter of RRGGBBAA or '?'.");
                            }
                        }
                        else if (command.IsEqual("C") || command.IsEqual("COLOR"))
                        {
                            // Color
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                Color color;
                                if (commandList[commandIndex] == "?"
                                    || HexUtil.HexToColor(commandList[commandIndex], out color))
                                {
									_nextSequence.Add( new FontColor(color) );
                                    addCommandToLine("C " + commandList[commandIndex]);
                                }
                                else
                                {
                                    Debug.LogError("The 'color' command requires a color parameter of RRGGBBAA or '?'.");
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'color' command requires a color parameter of RRGGBBAA:\n" + text);
                            }
                        }
                        else if (command.IsEqual("F") || command.IsEqual("FONT"))
                        {
                            // Font
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                Font font = (Font)fontTable[ commandList[commandIndex] ];
                                if (font == null)
                                {
                                    Debug.LogError("The font '" + commandList[commandIndex] + "' does not exist within Assets/Resources/Fonts/");
                                }
                                else
                                {
                                    _guiStyle.font = font; // Update the font to properly measure text
                                    addCommandToLine("F " + commandList[commandIndex]);
									_nextSequence.Add( new CustomFont(font) );
                                }
                                if (commandList.Length > commandIndex + 1)
                                {
                                    commandIndex++;
                                    int fontSize;
                                    if (System.Int32.TryParse(commandList[commandIndex], out fontSize))
                                    {
                                        addCommandToLine("FS " + commandList[commandIndex]);
										_nextSequence.Add( new FontSize(fontSize) );
                                        _guiStyle.fontSize = fontSize; // Update the size to properly measure text
                                    }
                                    else
                                    {
                                        Debug.LogError("The font size '" + commandList[commandIndex] + "' is not a valid integer");
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'font' command requires a font name parameter and an optional font size parameter.");
                            }
                        }
                        else if (command.IsEqual("FA") || command.IsEqual("FONTATTRIBUTE"))
                        {
                            // Font Attribute
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                string attribute = commandList[commandIndex].ToUpper();
                                switch (attribute)
                                {
                                    case "U":
                                    case "UNDERLINE":
                                        attribute = "U";
										_nextSequence.underline = true;
                                        break;
                                    case "-U":
                                    case "-UNDERLINE":
                                        attribute = "-U";
										_nextSequence.underline = false;
                                        break;
                                    case "S":
                                    case "STRIKETHROUGH":
                                        attribute = "S";
										Debug.Log( "strike ? " + _nextSequence.txt );
										_nextSequence.strikeThrough = true;
                                        break;
                                    case "-S":
                                    case "-STRIKETHROUGH":
                                        attribute = "-S";
										_nextSequence.strikeThrough = false;
                                        break;
                                    default:
                                        attribute = "";
                                        Debug.LogError("The 'font attribute' command requires a font parameter of U (underline on), -U (underline off), S (strikethrough on) or -S (strikethrough off).");
                                        break;
                                }
                                if (attribute.Length != 0)
                                {
                                    addCommandToLine("FA " + attribute);
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'font attribute' command requires a font parameter of U (underline on), -U (underline off), S (strikethrough on) or -S (strikethrough off).");
                            }
                        }
                        else if (command.IsEqual("FS") || command.IsEqual("FONTSIZE"))
                        {
                            // Font Size
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                int fontSize;
                                if (System.Int32.TryParse(commandList[commandIndex], out fontSize))
                                {
                                    addCommandToLine("FS " + commandList[commandIndex]);
									_nextSequence.Add( new FontSize(fontSize) );
                                    _guiStyle.fontSize = fontSize; // Update the size to properly measure text
                                }
                                else
                                {
                                    Debug.LogError("The font size '" + commandList[commandIndex] + "' is not a valid integer");
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'font size' command requires a font size parameter.");
                            }
                        }
                        else if (command.IsEqual("H") || command.IsEqual("HYPERLINK"))
                        {
                            // Hyperlink on
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                addCommandToLine("H " + commandList[commandIndex]);
								_nextSequence.hyperlinkId = HYPERLINK_TAG + commandList[commandIndex];
                            }
                            else
                            {
                                Debug.LogError("The 'hyperlink' command requires an hyperlink id parameter.");
                            }
                        }
                        else if (command.IsEqual("-H") || command.IsEqual("-HYPERLINK"))
                        {
                            // Hyperlink off
                            addCommandToLine("-H");
							_nextSequence.hyperlinkId = "";
                        }
                        else if (command.IsEqual("HA") || command.IsEqual("HALIGN"))
                        {
                            // Horizontal line alignment
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                string alignment = commandList[commandIndex].ToUpper();
                                switch (alignment)
                                {
                                    case "L":
                                    case "LEFT":
                                        alignment = "L";
										_fLine.alignement = TextAlignment.Left;
                                        break;
                                    case "R":
                                    case "RIGHT":
                                        alignment = "R";
										_fLine.alignement = TextAlignment.Right;
                                        break;
                                    case "C":
                                    case "CENTER":
                                        alignment = "C";
										_fLine.alignement = TextAlignment.Center;
                                        break;
                                    default:
                                        alignment = "";
                                        Debug.LogError("The 'HAlign' command requires an alignment parameter of L (left), R (right), or C (center).");
                                        break;
                                }
                                if (alignment.Length != 0)
                                {
                                    addCommandToLine("HA " + alignment);
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'HAlign' command requires an alignment parameter of L (left), R (right), or C (center).");
                            }
                        }
                        else if (command.IsEqual("S") || command.IsEqual("SPACE"))
                        {
                            // Space (pixels)
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                int spaceSize;
                                if (System.Int32.TryParse(commandList[commandIndex], out spaceSize))
                                {
                                    addCommandToLine("S " + commandList[commandIndex]);
									_nextSequence.Add( new AddSpace(spaceSize) );
                                    _lineLength += spaceSize;
                                }
                                else
                                {
                                    Debug.LogError("The space size '" + commandList[commandIndex] + "' is not a valid integer");
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'space' command requires a pixel count parameter.");
                            }
                        }
                        else if (command.IsEqual("VA") || command.IsEqual("VALIGN"))
                        {
                            // Vertical alignment
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                string alignment = commandList[commandIndex].ToUpper();
                                switch (alignment)
                                {
                                    case "?":
                                        alignment = "?";
										_nextSequence.alignBottom = false;
                                        break;
                                    case "B":
                                    case "BOTTOM":
                                        alignment = "B";
										_nextSequence.alignBottom = true;
                                        break;
                                    default:
                                        alignment = "";
                                        Debug.LogError("The 'VAlign' command requires an alignment parameter of ? (default) or B (bottom).");
                                        break;
                                }
                                if (alignment.Length != 0)
                                {
                                    addCommandToLine("VA " + alignment);
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'VAlign' command requires an alignment parameter of ? (default) or B (bottom).");
                            }
                        }
                        else
                        {
                            //Pass through any invalid commands or let words with brackets with out using double bracket
                            //and decide what to do with it later
                            invalidCommand = true;
                        }
                    }
                    if (invalidCommand)
                    {
                        addCommandToLine(string.Format("{0}", text.Substring(currentLetterIndex + 1, endIndex - currentLetterIndex - 1)));
                        //Debug.Log(string.Format("Invalid Command: {0}", commandList[commandIndex]));
                        invalidCommand = false;
                    }
					addSequenceToLine();
                }
                else
                {
                    // Add letter to the word
                    word.Append(text[letterIndex]);
                }
            }
            addWordToLine(word.ToString());
            addLineToLines();
        }    

        private bool addWordToLine(string word)
        {
			//Debug.Log( "addWordToLine in : " + word );
            bool createdNewLine = false;
            if (word.Length != 0)
            {
                Vector2 wordSize = _guiStyle.CalcSize(new GUIContent(word));
                float wordLength = (word.Equals(" "))
                                 ? getSpaceWidth()
                                 : wordSize.x;
               
                if (wordLength > _width)
                {
                    //Check for super long word and break it down
                    string newWord = string.Empty;
                    float newLineLength = _lineLength;
                    Vector2 charSize = Vector2.zero;

                    for (int i = 1; i < word.Length; i++)
                    {
                        charSize = _guiStyle.CalcSize(new GUIContent(word[i].ToString()));
                        float charLength = charSize.x;

                        if (newLineLength + charLength < _width)
                        {
                            newLineLength += charLength;
                            newWord += word[i];
                        }
                        else
                        {
                            _line.Append(newWord);
							_sequence.txt += newWord;
							//Debug.Log( "New word added to : " + _sequence );
                            newWord = word[i].ToString();
                            newLineLength = charLength;
                            createNewLine();                            
                        }
                    }
                    
                    word = newWord;
                    wordSize = _guiStyle.CalcSize(new GUIContent(word));
                    wordLength = (word == " ")
                                 ? getSpaceWidth()
                                 : wordSize.x;

                    createdNewLine = true;                         
                }
                else if (_lineLength + wordLength > _width)
                {
                    
                    // Word does not fit on current line
                    //Debug.Log("--- new line ---");
                    createNewLine();
                    createdNewLine = true;
                    word = word.TrimStart(' '); // Remove leading spaces since we created a new line
                    wordLength = _guiStyle.CalcSize(new GUIContent(word)).x;
                }

                if (word != string.Empty)
                {
                    _line.Append(word);
					_sequence.txt += word;
					//Debug.Log( "New word added to : " + _sequence );
                    _lineLength += wordLength;
                    _lineHeight = Mathf.Max(_lineHeight, wordSize.y);
                    //Debug.Log("Appended '" + word + "', length: " + wordLength + ", line: " + _lineLength + " x " + _lineHeight);
                }
            }
            return createdNewLine;
        }

        private float getSpaceWidth()
        {
            // Apparently we cannot measure a space directly, so let's deduce it
            float starWidth = _guiStyle.CalcSize(new GUIContent("**")).x;
            float wordWidth = _guiStyle.CalcSize(new GUIContent("* *")).x;
            float spaceWidth = wordWidth - starWidth;
            return spaceWidth;
        }

        private void addCommandToLine(string command)
        {
            bool mustPrefixCommand = command.StartsWith("HA ");

            command = "[" + command + "]";
            if (mustPrefixCommand)
            {
                //Debug.Log("Prepended command '" + command + "'");
                _line.Insert(0, command);
            }
            else
            {
                int trailingSpaceCount = _line.Length - _line.ToString().TrimEnd(' ').Length;
                if (trailingSpaceCount != 0)
                {
                    string line = _line.ToString().TrimEnd(' ');
                    _line.Length = 0;
                    _line.Append(line);

                    // Convert the spaces into a 'space' command
                    float spaceWidth = getSpaceWidth() * trailingSpaceCount;
                    command = "[S " + spaceWidth + "]" + command;
					_nextSequence.Add( new AddSpace(Mathf.RoundToInt(spaceWidth)) );
					//addSequenceToLine();
					
                    // Ensure to account for the size of these 'spaces'
                    _lineLength += spaceWidth;

                }
                _line.Append(command);
                //Debug.Log("Appended command '" + command + "'");
            }
        }

        private void addLineToLines()
        {
            if (_line.ToString() == _lineHeightCommand)
            {
                // Empty lines do not properly display; add a space
                _line.Append(" ");
            }
            addLineHeight(true);
            _lines.Add(_line.ToString());
			//Debug.Log( "Add line : " + _fLine );
			_fLines.Add( _fLine );
            //Debug.Log("  Parsed line: " + _line.ToString());
        }
		
		private void addSequenceToLine()
		{
			//Debug.Log( "Add sequence : " + _sequence );
			
			if( _sequence.isValid )
			{
				_fLine.Add( _sequence );
				_sequence = _nextSequence;
				_nextSequence = new Sequence();
			}
			else{
				_sequence = _sequence + _nextSequence;
				_nextSequence = new Sequence();
			}
		}

        private void createNewLine()
        {
            addLineToLines();
			
            _line.Length = 0;
            addLineHeight(false);
            _lineLength = 0; 
			
			//Debug.Log( "Create new line " + _fLine );
			addSequenceToLine();
			bool usePrevAlignment = false;
			TextAlignment prevAlignment = TextAlignment.Center;
			if(_fLine!=null)
			{
				prevAlignment = _fLine.alignement;
				usePrevAlignment = true;
			}
			_fLine = new FormattedLine();
			if(usePrevAlignment)
			{
				_fLine.alignement = prevAlignment;
			}
        }

        private void addLineHeight(bool realHeight)
        {
            if (realHeight)
            {
                string realLineHeightCommand = _lineHeightCommand.Replace("&", _lineHeight.ToString());
                _line.Replace(_lineHeightCommand, realLineHeightCommand);
				_fLine.lineHeight = _lineHeight;
                _lineHeight = 0.0f;
            }
            else
            {
                _line.Append(_lineHeightCommand);
            }
        }

        
    }
}
