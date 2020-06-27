using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogPanel : UIBase
{
    GameObject normal;
    GameObject auto;
    private void Start()
    {
        //isAuto = GetUI("IsAuto_UI").GetComponent<Toggle>();
        AddButtonListen("DialogBG_UI", () => { OnClick(null); });
        AddPointClick("Root_UI", OnClick);
        AddButtonListen("Set_UI", () => {
            StopSkip();
            UIManager.Instance.ClosePanel(PanelName.DialogPanel);
            UIManager.Instance.OpenPanel(PanelName.SetPanel);
        });
        AddButtonListen("Record_UI", () => { StopSkip(); UIManager.Instance.OpenPanel(PanelName.RecordsPanel); });
        //AddToggleListen("IsAuto_UI", IsNormal);
        AddButtonListen("Normal_UI", () => IsNormal(false));
        AddButtonListen("Auto_UI", () => IsNormal(true));
        AddButtonListen("Skip_UI", SkipDialog);
        ActionManager.Instance.RegistAction(ActionType.StopSkip, (object[] objs) =>
         {
             StopSkip();
         });
        normal = GetUI("Normal_UI");
        auto = GetUI("Auto_UI");
        auto.SetActive(false);
        if (first)
        {
            first = false;
            Initial();
        }
        transform.SetSiblingIndex(2);
    }
    static bool first = true;
    void Initial()
    {
        DialogManager.Instance.CharacterName = GetUI("CharacterName_UI").GetComponent<Text>();
        DialogManager.Instance.DialogText = GetUI("DialogText_UI").GetComponent<Text>();
        DialogManager.Instance.CharacterPanel = GetUI("Character_UI");
        DialogManager.Instance.DialogPanel = GetUI("DialogBG_UI");
    }

    public override void OnEnter()
    {
        if (first)
        {
            first = false;
            Initial();
        }
        gameObject.SetActive(true);
        //if (UIManager.Instance.GetPanel(PanelName.SetPanel)!=null&&!UIManager.Instance.GetPanel(PanelName.SetPanel).gameObject.activeSelf&&
        //    UIManager.Instance.GetPanel(PanelName.SelectPanel) != null && !UIManager.Instance.GetPanel(PanelName.SelectPanel).gameObject.activeSelf)
        //    transform.SetAsLastSibling();
        //transform.SetSiblingIndex(2);
    }
    public override void OnExit()
    {
        gameObject.SetActive(false);
    }

    void OnClick(BaseEventData data)
    {
        if (DialogManager.Instance.gameMode == GameMode.Skip)
        {
            StopSkip();
            return;
        }
        if (DialogManager.Instance.gameMode == GameMode.Auto) IsNormal(true);
        else DialogManager.Instance.RunDialog();
    }

    //Timer temp;
    void IsNormal(bool value)
    {
        if (skip)
        {
            skip = false;
            GetUI("SkipText_UI").GetComponent<Text>().text = "跳过";
        }

        if (value)
        {
            normal.SetActive(true);
            auto.SetActive(false);
            DialogManager.Instance.gameMode = GameMode.Normal;
        }
        else
        {
            normal.SetActive(false);
            auto.SetActive(true);
            DialogManager.Instance.gameMode = GameMode.Auto;
            TimerManager.Instance.TimeAction(1, () => DialogManager.Instance.RunDialog());
        }
    }

    bool skip;
    void SkipDialog()
    {
        if (!skip)
        {
            StartSkip();
        }
        else
        {
            StopSkip();
            IsNormal(true);
        }
    }

    void StartSkip()
    {
        StopSkip();
        skip = true;
        GetUI("SkipText_UI").GetComponent<Text>().text = "停止";
        //to do 没有阅读记录就停止
        DialogManager.Instance.gameMode = GameMode.Skip;
        DialogManager.Instance.RunDialog();
    }
    void StopSkip()
    {
        skip = false;
        GetUI("SkipText_UI").GetComponent<Text>().text = "跳过";
        IsNormal(true);
        if (DialogManager.Instance.gameMode == GameMode.Skip)
            DialogManager.Instance.StopDialog();
    }
}
