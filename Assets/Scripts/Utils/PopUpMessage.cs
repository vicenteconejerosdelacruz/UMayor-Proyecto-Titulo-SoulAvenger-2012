using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopUpMessage : TMonoBehaviour
{
	public static bool PopupVisible = false;
	
	public enum TPOPUP
	{
		BTN_OK,
		BTN_RETRY_CANCEL,
		BTN_OK_CANCEL
	}
	
	public static GameObject MsgBoxOk(string prefab,string message,PopupButtonMessageDelegate okDelegate)
	{
		return MsgBox(prefab,message,okDelegate,null,null,TPOPUP.BTN_OK);
	}
	
	public static GameObject MsgBoxRetryCancel(string prefab,string message,PopupButtonMessageDelegate retryDelegate,PopupButtonMessageDelegate cancelDelegate)
	{
		return MsgBox(prefab,message,null,retryDelegate,cancelDelegate,TPOPUP.BTN_RETRY_CANCEL);
	}	
	
	public static GameObject MsgBoxOkCancel(string prefab,string message,PopupButtonMessageDelegate okDelegate,PopupButtonMessageDelegate cancelDelegate)
	{
		return MsgBox(prefab,message,okDelegate,null,cancelDelegate,TPOPUP.BTN_OK_CANCEL);
	}		
	
	public static GameObject MsgBox(
		 string prefab
		,string message
		,PopupButtonMessageDelegate okDelegate
		,PopupButtonMessageDelegate retryDelegate
		,PopupButtonMessageDelegate cancelDelegate
		,TPOPUP popupType)
	{
		GameObject go = GameObject.Instantiate(Resources.Load(prefab) as GameObject) as GameObject;
		PopUpMessage popUp = go.GetComponent<PopUpMessage>();
			
		popUp.onOk 		= okDelegate;
		popUp.onCancel 	= cancelDelegate;
		popUp.onRetry 	= retryDelegate;
		
		popUp.textMesh.text = message;
		popUp.textMesh.maxChars = message.Length;
		popUp.textMesh.Commit();
		
		popUp.popupType = popupType;
		popUp.InitializeButtons();
		
		PopupVisible = true;
		return go;
	}
	
	public void OnBtnOk()
	{
		PopupVisible = false;
		if(onOk!=null){onOk();}
		Destroy(this.gameObject);
	}
	
	public void OnBtnCancel()
	{
		PopupVisible = false;
		if(onCancel!=null){onCancel();}
		Destroy(this.gameObject);
	}
	
	public void OnBtnRetry()
	{
		PopupVisible = false;
		if(onRetry!=null){onRetry();}
		Destroy(this.gameObject);
	}

	public void InitializeButtons ()
	{
		switch(popupType)
		{
		case TPOPUP.BTN_OK:
			Destroy(PopupRetryCancelButtons);
			Destroy(PopupOkCancelButtons);
			break;
		case TPOPUP.BTN_RETRY_CANCEL:
			Destroy(PopupOkButtons);
			Destroy(PopupOkCancelButtons);
			break;
		case TPOPUP.BTN_OK_CANCEL:
			Destroy(PopupRetryCancelButtons);
			Destroy(PopupOkButtons);
			break;
		}
	}	
	
	public tk2dTextMesh	textMesh;
	public GameObject	PopupOkButtons;
	public GameObject	PopupRetryCancelButtons;
	public GameObject	PopupOkCancelButtons;
	
	public delegate void PopupButtonMessageDelegate();
	
	public PopupButtonMessageDelegate onOk;
	public PopupButtonMessageDelegate onCancel;
	public PopupButtonMessageDelegate onRetry;
	
	[HideInInspector]
	public TPOPUP popupType = TPOPUP.BTN_OK;
}
