using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordsPanel : UIBase 
{
    Queue<GameObject> records = new Queue<GameObject>();
    RectTransform view;
    static bool first=true;
    private void Awake()
    {
        ActionManager.Instance.RegistAction(ActionType.ui_AddRecord, (object[] objs) => {
            DialogClass dialog = (DialogClass)objs[0];
            AddRecords(dialog);
            RemoveRecords();
            if (!gameObject.activeSelf) { gameObject.SetActive(true); transform.SetAsFirstSibling(); }
            
            if (view == null) view = GetUI("Root_UI").GetComponent<RectTransform>();
            if (first && view != null && (view.rect.height > 380 || records.Count >= 13))
            {
                first = false;
                view.pivot = new Vector2(0.5f, 0);
                gameObject.SetActive(false);
            }
            ResourcesManager.SaveRecords();
        });
        gameObject.SetActive(false);
    }
    private void Start()
    {
        AddListeners();
        if(view==null) view = GetUI("Root_UI").GetComponent<RectTransform>();
    }
    void AddListeners()
    {
        AddButtonListen("Close_UI", () => UIManager.Instance.ClosePanel(PanelName.RecordsPanel));
    }

    void AddRecords(DialogClass dialog)
    {
        GameObject go = UIManager.Instance.CreateUI("Record", GetUI("Root_UI").transform);
        records.Enqueue(go);
        go.GetComponent<Record>().Dialog = dialog;
    }

    void RemoveRecords()
    {
        if (records.Count>100)
        {
            GameObject temp = records.Dequeue();
            Destroy(temp);
        }
    }
    public override void OnEnter()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        transform.SetAsLastSibling();

    }
    public override void OnExit()
    {
        //if (!UIManager.Instance.GetPanel(PanelName.BackGroundPanel).gameObject.activeSelf)
            gameObject.SetActive(false);
        //else transform.SetAsFirstSibling();
    }
}
