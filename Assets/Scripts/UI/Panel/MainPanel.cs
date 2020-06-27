using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : UIBase 
{
    public override void OnEnter()
    {
        AudioManager.Instance.StopAudio();
        AudioManager.Instance.PlayBGM("DDD");
    }
    public override void OnExit()
    {
        UIManager.Instance.RemovePanel(this);
    }

    private void Start()
    {
        AddListeners();
    }

    void AddListeners()
    {
        AddButtonListen("Start_UI",NewGame);
        AddButtonListen("Set_UI", SetGame);
        AddButtonListen("Load_UI", () =>
        {
            DataPanel.type = PanelType.Load;
            UIManager.Instance.OpenPanel(PanelName.DataPanel);
        });
        AddButtonListen("Other_UI", () => { UIManager.Instance.OpenPanel(PanelName.OtherPanel); });
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    void NewGame()
    {
        UIManager.Instance.LoadPanel(PanelName.RecordsPanel);
        MaskPanel mask = UIManager.Instance.OpenPanel(PanelName.MaskPanel) as MaskPanel ;
        mask.Transition(3, 1);
    }

    void SetGame()
    {
        UIManager.Instance.OpenPanel(PanelName.SetPanel);
    }

}
