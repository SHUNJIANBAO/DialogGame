using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public enum GameMode
{
    Auto,
    Normal,
    Skip
}

public class DialogManager : MonoBehaviour
{
    #region 单例和初始化
    private static DialogManager _instance;
    public static DialogManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogManager>();
            }
            if (_instance == null)
            {
                _instance = new GameObject("DialogManager").AddComponent<DialogManager>();
            }
            return _instance;
        }
    }
    public void Awake()
    {
        dialogData = ResourcesManager.StoryData;
        GetPlayerName();
        BG = ResourcesManager.BG;
    }
    //public DialogManager()
    //{
    //    dialogData = ResourcesManager.StoryData;
    //    GetPlayerName();
    //    BG = ResourcesManager.BG;
    //}
    #endregion


    public Dictionary<string, Sprite> BG = new Dictionary<string, Sprite>();
    public Dictionary<int, DialogClass> dialogData = new Dictionary<int, DialogClass>();
    private List<int> playerNameIDs = new List<int>();
    private static int id = 1;
    private static int lastID = 1;
    private static bool isReading = false;  //是否在打字机过程中
    //当前的剧情进度
    private static DialogClass currentDialog;
    public static DialogClass CurrentDialog
    {
        get { return currentDialog; }
    }

    #region 属性
    public GameMode gameMode = GameMode.Normal;
    private string playerName = "XXX";
    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value; }
    }

    //背景图片
    private Image backGround;
    public Image BackGround
    {
        get
        {
            return backGround;
        }

        set
        {
            backGround = value;
        }
    }

    //角色名
    private Text characterName;
    public Text CharacterName
    {
        get
        {
            return characterName;
        }

        set
        {
            characterName = value;
        }
    }

    //角色名Panel
    private GameObject characterPanel;
    public GameObject CharacterPanel
    {
        get
        {
            return characterPanel;
        }

        set
        {
            characterPanel = value;
        }
    }

    //对话框文字
    private Text dialogText;
    public Text DialogText
    {
        get
        {
            return dialogText;
        }

        set
        {
            dialogText = value;
        }
    }

    //对话框Panel
    private GameObject dialogPanel;
    public GameObject DialogPanel
    {
        get
        {
            return dialogPanel;
        }

        set
        {
            dialogPanel = value;
        }
    }

    //选择Panel
    private SelectPanel select;
    public SelectPanel Select
    {
        get { return select; }
        set { select = value; }
    }

    //事件
    private Action action;
    #endregion


    /// <summary>
    /// 进行对话
    /// </summary>
    /// <param name="dialogID"></param>
    public static bool first = true;//读取存档时设为false，不触发本身的事件
    public void RunDialog(int dialogID = -7386)
    {
        //如果打字机没结束，就结束掉，并退出
        if (StopDialog())
        {
            return;
        }

        #region 事件
        if (first && currentDialog != null && dialogData[lastID].Action != string.Empty)
        {
            DialogClass tempDialog = dialogData[lastID];
            string[] tempStr = tempDialog.Action.Split(';');
            object[] value = new object[tempStr.Length - 1];
            for (int i = 1; i < tempStr.Length; i++)
            {
                value[i - 1] = tempStr[i];
            }
            bool temp = Enum.IsDefined(typeof(ActionType), tempStr[0].Trim());
            if (!temp) return;
            ActionType type = (ActionType)Enum.Parse(typeof(ActionType), tempStr[0].Trim());
            if (type == ActionType.Transition)
            {
                if (dialogID != -7386)
                {
                    ActionManager.Instance.Invoke(type, value[0], dialogID);
                    Debug.Log(dialogID);
                }
                else
                {
                    ActionManager.Instance.Invoke(type, value[0], id);
                }
                first = false;
                return;
            }
            if (type == ActionType.OpenName)
            {
                ActionManager.Instance.Invoke(ActionType.StopSkip);
                UIManager.Instance.OpenPanel(PanelName.NamePanel);
                lastID = id;
                return;
            }

            //else
            //{
            //    ActionManager.Instance.Invoke(type, value);
            //}
        }
        first = true;
        #endregion
        if (dialogID == -7386)
        {
            if (!dialogData.ContainsKey(id))
            {
                //todo 返回主界面
                UIManager.Instance.OpenPanel(PanelName.MaskPanel).GetComponent<MaskPanel>().Transition(3, PanelName.MainPanel);
                //UIManager.Instance.ClosePanel(dialogPanel);
                return;
            }
            currentDialog = dialogData[id];
        }
        else if (!dialogData.ContainsKey(dialogID))
        {
            //to do
            //关闭对话框

            UIManager.Instance.GetPanel(PanelName.MaskPanel).GetComponent<MaskPanel>().Transition(3, PanelName.MainPanel);
            return;
        }
        else
        {
            currentDialog = dialogData[dialogID];
        }
        //to do

        #region 音乐
        if (currentDialog.Music != string.Empty)
        {
            AudioManager.Instance.PlayBGM(currentDialog.Music);
        }
        #endregion
        //to do 
        #region 背景图片
        if (currentDialog.BackGround != string.Empty)
        {
            backGround.sprite = BG[currentDialog.BackGround.Trim()];
        }
        #endregion

        #region 对话类型
        switch (currentDialog.Type)
        {
            case 0://旁白
                   //创建没有头像的对话框
                ActionManager.Instance.Invoke(ActionType.ui_AddRecord, currentDialog);
                if (currentDialog.Audio != string.Empty)
                {
                    AudioManager.Instance.PlayAudio(currentDialog.Audio);
                }
                UIManager.Instance.OpenPanel(PanelName.DialogPanel);
                characterPanel.SetActive(false);
                //先关闭正在打字的协程
                StopCoroutine("Typer");
                //如果内容为空，关闭对话框
                if (string.IsNullOrEmpty(currentDialog.Content))
                {
                    //UIManager.Instance.ClosePanel(PanelName.DialogPanel);
                    dialogPanel.SetActive(false);
                }
                else
                {
                    if (!dialogPanel.activeSelf)
                    {
                        dialogPanel.SetActive(true);
                    }
                    //打字机效果
                    StartCoroutine("Typer", currentDialog.Content);
                }
                if (!ResourcesManager.Records.Contains(currentDialog.ID))
                    ResourcesManager.Records.Add(currentDialog.ID);
                //dialogText.text = currentDialog.Content;
                break;
            case 1://对话
                   //创建有头像的对话框
                ActionManager.Instance.Invoke(ActionType.ui_AddRecord, currentDialog);
                if (currentDialog.Audio != string.Empty)
                {
                    AudioManager.Instance.PlayAudio(currentDialog.Audio);
                }
                if (currentDialog.Character != string.Empty)
                {
                    characterPanel.SetActive(true);
                    characterName.text = currentDialog.Character;
                }
                //打字机效果
                StopCoroutine("Typer");
                if (string.IsNullOrEmpty(currentDialog.Content))
                {
                    //UIManager.Instance.ClosePanel(PanelName.DialogPanel);
                    dialogPanel.SetActive(false);
                }
                else
                {
                    if (!dialogPanel.activeSelf)
                    {
                        dialogPanel.SetActive(true);
                    }
                    //打字机效果
                    StartCoroutine("Typer", currentDialog.Content);
                }
                //dialogText.text = currentDialog.Content;
                if (!ResourcesManager.Records.Contains(currentDialog.ID))
                    ResourcesManager.Records.Add(currentDialog.ID);
                //todo 头像图片
                if (currentDialog.Sprite != string.Empty)
                {
                }

                break;
            case 2://选择
                ActionManager.Instance.Invoke(ActionType.StopSkip);
                DialogClass temp = currentDialog;
                UIManager.Instance.OpenPanel(PanelName.SelectPanel);
                while (temp.Type == 2)
                {
                    //添加按钮，并把类和事件注册进去
                    //to do
                    UnityAction action = null;
                    //int tempID = int.Parse( temp.NextID);
                    DialogClass tempClass = temp;
                    action += () => ActionManager.Instance.Invoke(ActionType.ui_AddRecord, tempClass);//记录对话信息
                    if (temp.Action != string.Empty)
                    {
                        string[] tempStr = temp.Action.Split(';');
                        object[] value = new object[tempStr.Length - 1];
                        for (int i = 1; i < tempStr.Length; i++)
                        {
                            value[i - 1] = tempStr[i];
                        }
                        bool tempB = Enum.IsDefined(typeof(ActionType), tempStr[0].Trim());
                        if (tempB)
                        {
                            ActionType type = (ActionType)Enum.Parse(typeof(ActionType), tempStr[0].Trim());
                            if (type == ActionType.Transition)
                            {
                                int tempID = int.Parse(temp.NextID);
                                action += () => ActionManager.Instance.Invoke(type, value[0], tempID);
                            }
                            //如果读过句字则显示 ，否则不创建
                            else if (type == ActionType.If)
                            {
                                //string[] nextIds = currentDialog.NextID.Split(';');
                                if (ResourcesManager.Records.Contains(int.Parse(value[0].ToString())))
                                {
                                    int tempID = int.Parse(currentDialog.NextID);
                                    action += () => RunDialog(tempID);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                int tempID = int.Parse(temp.NextID);
                                action += () => RunDialog(tempID);
                            }
                        }
                    }
                    else
                    {
                        int tempID = int.Parse(temp.NextID);
                        action += () => RunDialog(tempID);
                    }
                    //action += () =>
                    //{
                    //    if (!ResourcesManager.Records.Contains(temp.ID))
                    //        ResourcesManager.Records.Add(temp.ID);
                    //};//将对话记录保存下来
                    //if (gameMode == GameMode.Skip)
                    //    action += () => ActionManager.Instance.Invoke(ActionType.StopSkip);//选择后停止跳过模式
                    if (currentDialog.Audio.Trim() != string.Empty)
                    {
                        string audio = currentDialog.Audio.Trim();
                        action += () => AudioManager.Instance.PlayAudio(audio);
                    }

                    select.AddListener(temp.Content, action);
                    Debug.Log("addListener");
                    temp = dialogData[temp.ID + 1];
                }
                break;
        }
        #endregion
        lastID = currentDialog.ID;

        if (currentDialog.Action != null && currentDialog.Action != string.Empty)
        {
            string[] tempStr = currentDialog.Action.Split(';');
            object[] value = new object[tempStr.Length - 1];
            for (int i = 1; i < tempStr.Length; i++)
            {
                value[i - 1] = tempStr[i];
            }
            bool temp = Enum.IsDefined(typeof(ActionType), tempStr[0].Trim());
            if (!temp) return;
            ActionType type = (ActionType)Enum.Parse(typeof(ActionType), tempStr[0].Trim());

            if (type == ActionType.If)
            {
                string[] nextIds = currentDialog.NextID.Split(';');
                if (ResourcesManager.Records.Contains(int.Parse(value[0].ToString())))
                {
                    id = int.Parse(nextIds[1].ToString());

                }
                else
                {
                    id = int.Parse(nextIds[0].ToString());
                }

                return;
            }

        }
        id = int.Parse(currentDialog.NextID);
    }
    /// <summary>
    /// 停止对话
    /// </summary>
    /// <returns></returns>
    public bool StopDialog()
    {
        if (gameMode != GameMode.Normal)
        {
            return false;
        }
        if (isReading)
        {
            StopCoroutine("Typer");
            dialogText.text = currentDialog.Content;
            isReading = false;
            return true;
        }
        return false;
    }
    //打字机
    IEnumerator Typer(string value)
    {
        dialogText.text = "";

        switch (gameMode)
        {
            case GameMode.Auto:
                if (!string.IsNullOrEmpty(dialogData[lastID].Action))
                {
                    string[] temp = dialogData[lastID].Action.Split(';');
                    Debug.Log(temp[0]);
                    ActionType type = (ActionType)Enum.Parse(typeof(ActionType), temp[0].Trim());
                    if (type == ActionType.Transition)
                    {
                        yield return new WaitForSeconds(1.5f);
                    }
                }
                for (int i = 0; i < value.Length; i++)
                {
                    dialogText.text += value[i];
                    yield return new WaitForSeconds(0.05f);
                }
                if (gameMode == GameMode.Auto)
                {
                    float timeCount = value.Length * 0.1f;
                    timeCount = Mathf.Clamp(timeCount, 1f, 3f);
                    yield return new WaitForSeconds(timeCount);
                    if (gameMode == GameMode.Auto)
                        RunDialog();
                }
                break;
            case GameMode.Normal:
                for (int i = 0; i < value.Length; i++)
                {
                    dialogText.text += value[i];
                    isReading = true;
                    yield return new WaitForSeconds(0.05f);
                }
                isReading = false;
                break;
            case GameMode.Skip:
                if (!ResourcesManager.Records.Contains(currentDialog.ID))
                {
                    //todo 停止跳过模式
                    for (int i = 0; i < value.Length; i++)
                    {
                        dialogText.text += value[i];
                        yield return new WaitForSeconds(0.001f);
                    }
                    ActionManager.Instance.Invoke(ActionType.StopSkip);
                    break;
                }
                //for (int i = 0; i < value.Length; i++)
                //{
                //dialogText.text += value[i];
                dialogText.text = value;
                yield return new WaitForSeconds(0.1f);
                //}
                //if (gameMode == GameMode.Skip)
                //{
                //    float timeCount = value.Length * 0.001f;
                //    timeCount = Mathf.Clamp(timeCount, 0, 0.1f);
                //    yield return new WaitForSeconds(timeCount);
                //    if (gameMode == GameMode.Skip)
                isReading = false;
                RunDialog();
                // }
                break;
        }

    }

    /// <summary>
    /// 读取配置时设置对话框
    /// </summary>
    /// <param name="data"></param>
    public void SetDialog(DialogData data)
    {
        if (dialogData.Count == 0)
            dialogData = ResourcesManager.StoryData;
        BackGround.sprite = BG[data.BackGround];
        first = false;//不触发对话的事件
        SetPlayerName(data.PlayerName);
        if (!string.IsNullOrEmpty(data.Music))
        {
            AudioManager.Instance.PlayBGM(data.Music);
        }
    }

    #region 主角名字相关
    /// <summary>
    /// 得到主角名字的所有ID
    /// </summary>
    public void GetPlayerName()
    {
        foreach (int item in dialogData.Keys)
        {
            if (dialogData[item].Character == "XXX")
            {
                playerNameIDs.Add(item);
            }
        }
    }
    /// <summary>
    /// 设置主角名字
    /// </summary>
    /// <param name="playerName"></param>
    public void SetPlayerName(string playerName)
    {
        PlayerName = playerName;
        if (playerNameIDs.Count == 0) GetPlayerName();
        foreach (int item in playerNameIDs)
        {
            dialogData[item].Character = playerName;
        }
        foreach (DialogClass item in dialogData.Values)
        {
            item.Content = item.Content.Replace("XXX", playerName);
        }
    }
    #endregion
}

