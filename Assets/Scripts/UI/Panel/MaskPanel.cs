using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskPanel : UIBase
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

    Image mask;
    private void Awake()
    {
        mask = GetComponent<Image>();
        mask.color = new Color(0, 0, 0, 0);
        gameObject.SetActive(false);
    }

    public void Transition(float timeCount, string panelName)
    {
        StartCoroutine(Mask(timeCount, panelName));
    }
    public void Transition(float timeCount, int dialogID)
    {
        StartCoroutine(Mask(timeCount, dialogID));
    }
    public void Transition(float timeCount, DialogData data)
    {
        StartCoroutine(Mask(timeCount, data));
    }
    IEnumerator Mask(float timeCount, int dialogID)
    {
        float a = 0;

        float ratio = 255f / (timeCount / 2 / Time.deltaTime);
        while (a < 255)
        {
            a += ratio;
            mask.color = new Color(0, 0, 0, a / 255f);
            yield return null;
        }
        UIManager.Instance.OpenPanel(PanelName.BackGroundPanel);
        UIManager.Instance.GetPanel(PanelName.RecordsPanel).transform.SetAsFirstSibling();
        UIManager.Instance.ClosePanel(PanelName.SetPanel);
        UIManager.Instance.ClosePanel(PanelName.DataPanel);
        UIManager.Instance.ClosePanel(PanelName.MainPanel);
        DialogManager.Instance.RunDialog(dialogID);
        transform.SetAsLastSibling();
        while (mask.color.a > 0)
        {
            a -= ratio;
            mask.color = new Color(0, 0, 0, a / 255f);
            yield return null;
        }
        UIManager.Instance.ClosePanel(PanelName.MaskPanel);
    }
    IEnumerator Mask(float timeCount, DialogData data)
    {
        float a = 0;

        float ratio = 255f / (timeCount / 2 / Time.deltaTime);
        while (a < 255)
        {
            a += ratio;
            mask.color = new Color(0, 0, 0, a / 255f);
            yield return null;
        }
        if (data != null)
        {
            UIManager.Instance.OpenPanel(PanelName.BackGroundPanel);
            UIManager.Instance.OpenPanel(PanelName.RecordsPanel);
            UIManager.Instance.GetPanel(PanelName.RecordsPanel).transform.SetAsFirstSibling();
            DialogManager.Instance.SetDialog(data);
            UIManager.Instance.OpenPanel(PanelName.DialogPanel);
            DialogManager.Instance.RunDialog(data.DialogID);
            UIManager.Instance.ClosePanel(PanelName.SetPanel);
            UIManager.Instance.ClosePanel(PanelName.DataPanel);
            UIManager.Instance.ClosePanel(PanelName.MainPanel);
            transform.SetAsLastSibling();
        }
        transform.SetAsLastSibling();
        while (mask.color.a > 0)
        {
            a -= ratio;
            mask.color = new Color(0, 0, 0, a / 255f);
            yield return null;
        }
        DialogManager.first = true;
        UIManager.Instance.ClosePanel(PanelName.MaskPanel);
    }

    IEnumerator Mask(float timeCount, string panelName)
    {
        float a = 0;

        float ratio = 255f / (timeCount / 2 / Time.deltaTime);
        while (a < 255)
        {
            a += ratio;
            mask.color = new Color(0, 0, 0, a / 255f);
            yield return null;
        }
        foreach (string item in UIManager.Instance.panelDict.Keys)
        {
            if (item!=PanelName.MaskPanel)
            {
                UIManager.Instance.ClosePanel(item);
            }
        }
        UIManager.Instance.OpenPanel(panelName);
        transform.SetAsLastSibling();
        while (mask.color.a > 0)
        {
            a -= ratio;
            mask.color = new Color(0, 0, 0, a / 255f);
            yield return null;
        }
        DialogManager.first = true;
        UIManager.Instance.ClosePanel(PanelName.MaskPanel);
    }

}
