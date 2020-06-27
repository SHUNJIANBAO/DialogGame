using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIBase : MonoBehaviour
{
    public virtual void OnEnter()
    {
    }
    public virtual void OnExit()
    {
    }

    public void RegistPanelAndUi()
    {
        Transform[] allChild = GetComponentsInChildren<Transform>();
        foreach (var child in allChild)
        {
            if (child.name.EndsWith("_UI"))
            {
                if (child.GetComponent<UIBehaviour>() == null)
                    child.gameObject.AddComponent<UIBehaviour>();
                UIManager.Instance.RegistUI(name, child.name, child.gameObject);
            }
        }
    }

    protected GameObject GetUI(string uiName)
    {
        return UIManager.Instance.GetUI(name, uiName);
    }

    UIBehaviour GetBehaviour(string uiName)
    {
        return UIManager.Instance.GetUI(name,uiName).GetComponent<UIBehaviour>();
    }

    #region 添加Start中的自定义事件
    protected void AddStartAction(string uiName, Action<UIBehaviour> action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);

        tempB.UiStart += action;
    }
    protected void AddStartAction(GameObject go, Action<UIBehaviour> action)
    {
        go.GetComponent<UIBehaviour>().UiStart += action;
    }
    #endregion

    #region 添加Update中的自定义事件
    protected void AddUpdateAction(string uiName, Action<UIBehaviour> action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);
        tempB.UiUpdate += action;
    }

    protected void AddUpdateAction(GameObject go, Action<UIBehaviour> action)
    {
        go.GetComponent<UIBehaviour>().UiUpdate += action;
    }
    #endregion

    #region 添加Toggle点击事件
    protected void AddToggleListen(string uiName, UnityAction<bool> action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);
        if (tempB != null)
        {
            tempB.AddToggleListen(action);
        }
    }
    #endregion

    #region 添加Button点击事件
    protected void AddButtonListen(string uiName, UnityAction action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);
        if (tempB != null)
        {
            tempB.AddButtonListen(action);
        }
        else
        {
            Debug.Log(uiName + "不存在");
            foreach (var k in UIManager.Instance.uiDict["MenuPanel"])
            {
                Debug.Log(k);
            }
        }
    }
    #endregion

    #region 添加Slider滑动事件
    protected void AddSliderListen(string uiName, UnityAction<float> action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);
        if (tempB != null)
        {
            tempB.AddSliderListen(action);
        }
    }
    #endregion

    #region 添加点击事件
    public void AddPointClick(string uiName, UnityAction<BaseEventData> action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);
        if (tempB != null)
        {
            tempB.AddPointClick(action);
        }
    }
    #endregion

    #region 添加点击按下事件
    public void AddPointDown(string uiName, UnityAction<BaseEventData> action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);
        if (tempB != null)
        {
            tempB.AddPointClickDown(action);
        }
    }
    #endregion

    #region 添加点击抬起事件
    public void AddPointUP(string uiName, UnityAction<BaseEventData> action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);
        if (tempB != null)
        {
            tempB.AddPointClickUP(action);
        }
    }
    #endregion

    #region 添加拖拽事件
    public void AddDrag(string uiName, UnityAction<BaseEventData> action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);
        if (tempB != null)
        {
            tempB.AddDrag(action);
        }
    }
    #endregion

    #region 添加拖拽开始事件
    public void AddBeginDrag(string uiName, UnityAction<BaseEventData> action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);
        if (tempB != null)
        {
            tempB.AddOnBeginDrag(action);
        }
    }
    #endregion

    #region 添加拖拽结束事件
    public void AddEndDrag(string uiName, UnityAction<BaseEventData> action)
    {
        UIBehaviour tempB = GetBehaviour(uiName);
        if (tempB != null)
        {
            tempB.AddOnEndDrag(action);
        }
    }
    #endregion


    private void OnDestroy()
    {
        if (FindObjectOfType<UIManager>()!=null)
        {
            UIManager.Instance.UnRegistPanel(this.name);
        }
    }
}
