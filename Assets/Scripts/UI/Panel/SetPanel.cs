using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : UIBase
{
    public override void OnEnter()
    {
        gameObject.SetActive(true);
        if (UIManager.Instance.GetPanel(PanelName.MainPanel) != null && UIManager.Instance.GetPanel(PanelName.MainPanel).gameObject.activeSelf)
        {
            GetUI("Exit_UI").SetActive(false);
            GetUI("Load_UI").SetActive(false);
            GetUI("Save_UI").SetActive(false);
        }
        else
        {
            GetUI("Exit_UI").SetActive(true);
            GetUI("Load_UI").SetActive(true);
            GetUI("Save_UI").SetActive(true);
        }
    }
    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }
    public override void OnExit()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        OnInitial();
        AddListeners();
    }

    void OnInitial()
    {
        GetUI("MasterVolume_UI").GetComponent<Slider>().value = AudioManager.Instance.MasterVolume;
        GetUI("MusicVolume_UI").GetComponent<Slider>().value = AudioManager.Instance.BgmVolume;
        GetUI("AudioVolume_UI").GetComponent<Slider>().value = AudioManager.Instance.AudioVolume;
    }

    void AddListeners()
    {
        AddSliderListen("MasterVolume_UI", MasterVolume);
        AddSliderListen("MusicVolume_UI", MusicVolume);
        AddSliderListen("AudioVolume_UI", AudioVolume);
        AddButtonListen("Close_UI", ClosePanel);
        AddButtonListen("Save_UI", () =>
        {
            DataPanel.type = PanelType.Save;
            UIManager.Instance.OpenPanel(PanelName.DataPanel);
        });
        AddButtonListen("Load_UI", () =>
        {
            DataPanel.type = PanelType.Load;
            UIManager.Instance.OpenPanel(PanelName.DataPanel);
        });
        AddButtonListen("Exit_UI", () =>
        {
            UIManager.Instance.CloseAllPanel();
            UIManager.Instance.OpenPanel(PanelName.MainPanel);
        });
    }

    void MasterVolume(float value)
    {
        AudioManager.Instance.MasterVolume = value;
    }

    void MusicVolume(float value)
    {
        AudioManager.Instance.BgmVolume = value;
    }

    void AudioVolume(float value)
    {
        AudioManager.Instance.AudioVolume = value;
    }

    void ClosePanel()
    {
        //todo 保存设置
        SetData data = new SetData();
        data.AudioVolume = AudioManager.Instance.AudioVolume;
        data.BgmVolume = AudioManager.Instance.BgmVolume;
        data.MasterVolume = AudioManager.Instance.MasterVolume;
        ResourcesManager.SaveSetData(data);
        UIManager.Instance.ClosePanel(this);
        if (UIManager.Instance.GetPanel(PanelName.MainPanel) == null)
            UIManager.Instance.OpenPanel(PanelName.DialogPanel);
        //UIManager.Instance.OpenPanel(PanelName.DialogPanel);
    }
}
