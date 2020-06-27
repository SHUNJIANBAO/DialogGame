using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLoadPanel : UIBase 
{
    public override void OnEnter()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
    public override void OnExit()
    {
        gameObject.SetActive(false);
    }

    public static DialogData data=new DialogData();
    private void Start()
    {
        AddButtonListen("Yes_UI", () => {
            ActionManager.Instance.Invoke(ActionType.ui_CheckLoad, data);
            //todo 跳转
            UIManager.Instance.ClosePanel(this);
            //Debug.Log("yes close");
        });
        AddButtonListen("No_UI", () => {
            UIManager.Instance.ClosePanel(this);
            //Debug.Log("no close");
        });
    }

}
