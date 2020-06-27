using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundPanel : UIBase 
{
    public override void OnEnter()
    {
        gameObject.SetActive(true);
        DialogManager.Instance.BackGround = GetComponent<UnityEngine.UI.Image>();
        transform.SetAsFirstSibling();
    }
    public override void OnExit()
    {
        gameObject.SetActive(false);
    }
}
