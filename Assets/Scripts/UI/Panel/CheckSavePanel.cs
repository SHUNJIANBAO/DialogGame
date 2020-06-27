using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSavePanel : UIBase 
{
    public static int DialogDataID;
    private void Start()
    {
        AddButtonListen("Yes_UI", () => {
            ActionManager.Instance.Invoke(ActionType.ui_CheckSave, DialogDataID);
            UIManager.Instance.ClosePanel(this);
            //Debug.Log("yes close");
        });
        AddButtonListen("No_UI", () => {
            UIManager.Instance.ClosePanel(this);
            //Debug.Log("no close");
        });
    }
    public override void OnEnter()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
    public override void OnExit()
    {
        gameObject.SetActive(false);
    }
}
