using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PanelType
{
    Save,
    Load
}

public class DataPanel : UIBase
{
    public static PanelType type;
    List<ItemData> datas = new List<ItemData>();
    public delegate void Enter(PanelType type);
    public static Enter enter;
    List<GameObject> gos = new List<GameObject>();
    private void Start()
    {
        if (gos.Count==0)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject go = UIManager.Instance.CreateUI("ItemData_UI", GetUI("Root_UI").transform);
                gos.Add(go);
                ItemData item = go.GetComponent<ItemData>();
                datas.Add(item);
                enter += item.CheckType;
            }
        }
        AddListeners();
    }

    void AddListeners()
    {
        AddButtonListen("Close_UI", () => { UIManager.Instance.ClosePanel(this);UIManager.Instance.ClosePanel(PanelName.SetPanel);
            if (UIManager.Instance.GetPanel(PanelName.MainPanel) == null)
                UIManager.Instance.OpenPanel(PanelName.DialogPanel);
        });
        ActionManager.Instance.RegistAction(ActionType.ui_CheckSave, (object[] objs) =>
         {
             int id = int.Parse(objs[0].ToString());
             datas[id].SaveData();
         });
        ActionManager.Instance.RegistAction(ActionType.ui_CheckLoad, (object[] objs) =>
         {
             DialogData data = (DialogData)objs[0];
             MaskPanel mask = UIManager.Instance.OpenPanel(PanelName.MaskPanel) as MaskPanel;
             //Debug.Log(data.BackGround);
             //foreach (var item in DialogManager.Instance.BG.Keys)
             //{
             //    Debug.Log("dia  " + item);
             //}

             mask.Transition(3, data);
             //DialogManager.Instance.BackGround.sprite = DialogManager.Instance.BG[data.BackGround.Trim()];
         });
    }
    public override void OnEnter()
    {
        if (gos.Count == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject go = UIManager.Instance.CreateUI("ItemData_UI", GetUI("Root_UI").transform);
                gos.Add(go);
                ItemData item = go.GetComponent<ItemData>();
                datas.Add(item);
                enter += item.CheckType;
            }
        }

        switch (type)
        {
            case PanelType.Save:
                for (int i = 0; i < datas.Count; i++)
                {
                    if (ResourcesManager.SaveData.ContainsKey(i+1))
                    {
                        DialogData data = ResourcesManager.SaveData[i+1];
                        datas[i].SetData(data.ID, data);
                    }
                    else
                    {
                        datas[i].SetData((i + 1).ToString(), "存档为空", null);
                    }
                }
                break;
            case PanelType.Load:
                for (int i = 0; i < datas.Count; i++)
                {
                    if (ResourcesManager.SaveData.ContainsKey(i+1))
                    {
                        DialogData data = ResourcesManager.SaveData[i + 1];
                        datas[i].SetData(data.ID, data);
                    }
                    else
                    {
                        datas[i].SetData((i+1).ToString(), "存档为空", null);
                    }
                }
                break;
        }
        if (enter != null)
            enter(type);
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }
    public override void OnExit()
    {
        gameObject.SetActive(false);
    }

}
