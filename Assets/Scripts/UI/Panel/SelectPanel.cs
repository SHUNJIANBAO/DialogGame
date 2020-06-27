using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectPanel : UIBase
{
    List<UIBehaviour> selects = new List<UIBehaviour>();
    Transform root;
    private void Awake()
    {
        DialogManager.Instance.Select = this;
    }

    public void AddListener(string connect, params UnityAction[] action)
    {
        foreach (UIBehaviour item in selects)
        {
            if (item.gameObject.activeSelf == false)
            {
                item.gameObject.SetActive(true);
                item.GetComponentInChildren<Text>().text = connect;
                for (int i = 0; i < action.Length; i++)
                {
                    item.AddButtonListen(action[i]);
                }
                item.AddButtonListen(() => UIManager.Instance.ClosePanel(this));
                return;
            }
        }
        UIBehaviour select = UIManager.Instance.CreateUI("Select_UI", root).GetComponent<UIBehaviour>();
        select.GetComponentInChildren<Text>().text = connect;
        for (int i = 0; i < action.Length; i++)
        {
            select.AddButtonListen(action[i]);
        }
        select.AddButtonListen(() => UIManager.Instance.ClosePanel(this));
        selects.Add(select);
    }

    void ClearSelect()
    {
        foreach (var item in selects)
        {
            item.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            item.gameObject.SetActive(false);
        }
    }

    public override void OnEnter()
    {
        if (root == null)
        {
            root = GetUI("Mask_UI").transform;
            for (int i = 0; i < 3; i++)
            {
                UIBehaviour select = UIManager.Instance.CreateUI("Select_UI", root).GetComponent<UIBehaviour>();
                selects.Add(select);
                select.gameObject.SetActive(false);
            }
        }
        gameObject.SetActive(true);
        if (UIManager.Instance.GetPanel(PanelName.SetPanel) != null && !UIManager.Instance.GetPanel(PanelName.SetPanel).gameObject.activeSelf)
            transform.SetAsLastSibling();
    }
    public override void OnExit()
    {
        ClearSelect();
        gameObject.SetActive(false);
    }
}
