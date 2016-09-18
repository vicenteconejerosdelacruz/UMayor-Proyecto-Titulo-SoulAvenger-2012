using UnityEngine;
using System.Collections;
using System.Text;
using System.Xml;
using System.IO;

public class CreditsUI : GuiUtils
{
	public tk2dTextMesh titleCredits;
	public tk2dTextMesh textCredits;
	public float speed;
	private float totalTimer;
	
	private string patch;
	
	private string position;
	private string namePerson;
	
	private int countLine=0;
	
	public float duration = 16f;
	
	
	public	Texture2D[]	skipIcon	= null;
	public	int			frameRate	= 0;
	public	Rect		skipRect	= new Rect(0,0,0,0);
	private	bool		canSkip		= false;
	private	float		alpha		= 0.0f;
	//private	Animation	anim		= null;
	private float		animTime	= 0.0f;
	private float		oneOverFPS	= 0.0f;

	
	public override void TStart()
	{
		TextAsset ta = Resources.Load("Credits/Credits") as TextAsset;
		
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(ta.text);
		
		XmlNodeList offices = xmlDoc.GetElementsByTagName("Office");
		
		foreach (XmlElement pos in offices)
		{
			position = pos.GetAttribute("position");
			
			XmlNodeList persons = pos.GetElementsByTagName("Person");
			
			titleCredits.text += position+"\n\n";
			titleCredits.color = Color.red;

			foreach(XmlElement person in persons)
			{
				namePerson = person.GetAttribute("name");
				textCredits.text += "\n"+namePerson;
				
				countLine++;

				if(countLine>=1)
				{
					titleCredits.text += "\n";
				}
			}
			textCredits.text += "\n\n";
		}
		titleCredits.Commit();
		textCredits.Commit();
		
        //anim = GetComponent<Animation>();
		oneOverFPS = 1.0f/(float)frameRate;
	}
		
	public override void TUpdate()
	{
		float objToMove = speed * Time.deltaTime;
		
		titleCredits.transform.Translate(Vector3.up*objToMove,Space.World);
		textCredits.transform.Translate(Vector3.up*objToMove,Space.World);
		
		if(!isRunning(duration,Time.deltaTime))
		{
			Application.LoadLevel("MainMenu");
		}
		
				if(canSkip)
		{
			animTime += Time.deltaTime;
		}
		
		
		if(Input.GetMouseButtonDown(0))
		{
			if(!canSkip)
			{
				enableSkip();
				showSkipIcon();
			}
			else
			{
				disableSkip();
				//anim.Stop();
				Application.LoadLevel("MainMenu");
			}
		}
	}
	
	
	public bool isRunning(float duration, float deltaTime)
	{
		int 	lines     = countCharacter(textCredits.text,'\n');
		float   extraTime;
		float   totalTime = 0f;
		
		totalTimer+= deltaTime;
		
		extraTime = (float)lines * 0.54f;
		
		totalTime = duration + extraTime;
		
		if(totalTimer<totalTime)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
		public void enableSkip()
	{
		canSkip = true;
	}
	
	public void disableSkip()
	{
		canSkip = false;
	}
	
	void showSkipIcon()
	{
		animTime = 0.0f;
		iTween.Stop(this.gameObject);
		iTween.ValueTo(this.gameObject,
			iTween.Hash("from",0.0f,
						"to",1.0f,
						"time",1.0f,
						"onupdate","updateiconalpha",
						"oncomplete","hideSkipIcon"
		));
	}
	
	public void hideSkipIcon()
	{
		iTween.ValueTo(this.gameObject,
		iTween.Hash("from",1.0f,
					"to",0.0f,
					"time",1.0f,
					"delay",4.0f,
					"onupdate","updateiconalpha",
					"oncomplete","disableSkip"
	));	
	}
			
	public void updateiconalpha(float value)
	{
		alpha = value;
	}
	
	public int countCharacter(string text, char charFind)
	{
		int countChars = 0;
		
		string[] arreglo = textCredits.text.Split(charFind);
		
		for(int i= 0; i<arreglo.Length; i++)
		{
			countChars++;
		}
		
		return countChars;
	}
	
	void OnGUI()
	{
		if(canSkip)
		{
			int currentFrame = ((int)(animTime/oneOverFPS+1))%skipIcon.Length;
				
			GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
			
			GUI.color = new Color(1,1,1,alpha);
			
			GuiUtils.showImage(skipIcon[currentFrame],skipRect);
			
			GUI.color = Color.white;
			
			GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		}
	}
}
