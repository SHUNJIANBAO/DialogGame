using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NamePanel : UIBase
{
    private void Start()
    {
        AddButtonListen("Button_UI", OnClick);
    }
    public override void OnEnter()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
    public override void OnExit()
    {
        gameObject.SetActive(false);
    }

    static int shortCount=0;
    static int longCount = 0;
    public void OnClick()
    {
        //Debug.Log(GameObject.Find("Name_UI").GetComponent<UnityEngine.UI.Text>().text);
        string tempName = GetUI("Name_UI").GetComponent<UnityEngine.UI.Text>().text;
        if (string.IsNullOrEmpty(tempName))
        {
            shortCount++;
            if (ActionManager.Instance.HaveAction(ActionType.SendMessage))
            {
                if (shortCount < 10)
                {
                    ActionManager.Instance.Invoke(ActionType.SendMessage, "这也太短了吧");
                }
                else if (shortCount < 20)
                {
                    ActionManager.Instance.Invoke(ActionType.SendMessage, "你故意的吗？");
                }
                else
                {
                    ActionManager.Instance.Invoke(ActionType.SendMessage, "M***** F*** 告辞");
                    ActionManager.Instance.RemoveAction(ActionType.SendMessage);
                }
            }
            return;
        }
        int count = Encoding.Default.GetByteCount(tempName);
        if (count > 8)
        {
            longCount++;
            if (ActionManager.Instance.HaveAction(ActionType.SendMessage))
            {
                if (longCount < 10)
                {
                    ActionManager.Instance.Invoke(ActionType.SendMessage, "这个长度有点不妙");
                }
                else if (longCount < 20)
                {
                    ActionManager.Instance.Invoke(ActionType.SendMessage, "这么长塞不下啦");
                }
                else
                {
                    ActionManager.Instance.Invoke(ActionType.SendMessage, "累觉不爱，告辞！");
                    ActionManager.Instance.RemoveAction(ActionType.SendMessage);
                }
            }
            return;
        }

        DialogManager.Instance.PlayerName = tempName;
        DialogManager.Instance.SetPlayerName(tempName);
        DialogManager.Instance.RunDialog();
        UIManager.Instance.ClosePanel(this);
        UIManager.Instance.RemovePanel(this);
    }
}
