using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemData : MonoBehaviour
{
    //Text id;
    //Text connect;
    //Text dataTime;
    Button btn;
    private int id;
    public DialogData data;
    Dictionary<string, GameObject> texts = new Dictionary<string, GameObject>();
    public void Awake()
    {
        btn = GetComponent<Button>();
        Transform[] childs = GetComponentsInChildren<Transform>();
        foreach (Transform item in childs)
        {
            if (item.name.EndsWith("_T")) texts.Add(item.name, item.gameObject);
        }
        Button tempBtn = texts["Del_T"].GetComponent<Button>();
        tempBtn.onClick.AddListener(() =>
        {
            SetData(id.ToString(), "存档为空", null);
            data = null;
            ResourcesManager.SaveData.Remove(id);
            ResourcesManager.SaveDialogData(null);
            btn.onClick.RemoveAllListeners();
            tempBtn.gameObject.SetActive(false);
        });
    }

    public void CheckType(PanelType type)
    {
        btn.onClick.RemoveAllListeners();
        switch (type)
        {
            case PanelType.Save:
                texts["Del_T"].SetActive(false);
                btn.onClick.AddListener(() =>
                {

                    if (data == null)
                    {

                        SaveData();
                    }
                    else
                    {
                        UIManager.Instance.OpenPanel(PanelName.CheckSavePanel);
                        CheckSavePanel.DialogDataID = id - 1;
                    };
                });
                break;
            case PanelType.Load:
                if (GetText("Time_T").text != string.Empty)
                {
                    btn.onClick.AddListener(LoadData);
                    texts["Del_T"].SetActive(true);
                }
                else
                {
                    texts["Del_T"].SetActive(false);
                }
                break;
        }
    }
    public Text GetText(string uiName)
    {
        return texts[uiName].GetComponent<Text>();
    }
    public void SetData(string id, string connect, string dataTime)
    {
        this.id = int.Parse(id);
        //GetText("ID_T").text = id;
        GetText("Content _T").text = connect;
        GetText("Time_T").text = dataTime;
    }
    public void SetData(int id, DialogData data)
    {
        this.data = data;
        SetData(id.ToString(), data.Title, data.TimeData);
        //GetText("Content _T").text = data.Title;
        //GetText("Time_T").text = data.TimeData;
    }
    /// <summary>
    /// 保存进度
    /// </summary>
    public void SaveData()
    {
        data = new DialogData();
        data.ID = id;
        data.PlayerName = DialogManager.Instance.PlayerName;
        data.DialogID = DialogManager.CurrentDialog.ID;
        data.Title = DialogManager.CurrentDialog.Content;
        data.BackGround = DialogManager.Instance.BackGround.sprite.name;
        if (AudioManager.Instance.bgmSource != null && AudioManager.Instance.bgmSource.clip != null)
            data.Music = AudioManager.Instance.bgmSource.clip.name;
        else data.Music = "";
        DateTime time = DateTime.Now;
        data.TimeData = time.ToString("yyyy年MM月dd日HH时mm分");
        ResourcesManager.SaveDialogData(data);
        SetData(data.ID, data);
    }

    public void LoadData()
    {
        CheckLoadPanel.data = data;
        UIManager.Instance.OpenPanel(PanelName.CheckLoadPanel);
        //Debug.Log("LoadData");
    }
}
