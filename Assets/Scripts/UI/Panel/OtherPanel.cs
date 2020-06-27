using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPanel : UIBase 
{
    private void Start()
    {
        AddButtonListen("Close_UI", () => { UIManager.Instance.ClosePanel(PanelName.OtherPanel); });
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
