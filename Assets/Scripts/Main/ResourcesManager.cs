using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using System.Text;
using System.Reflection;

public static class ResourcesManager
{
    //private static readonly string dialogJsonPath = Application.persistentDataPath + "/DialogJson.txt";
    //private static readonly string tempdialogJsonPath = Application.streamingAssetsPath + "/DialogJson.txt";
    private static readonly string storyPath = "Story/DialogJson";
    private static readonly string dialogDataPath = Application.persistentDataPath + "/DialogData.txt";
    private static readonly string recordsPath = Application.persistentDataPath + "/RecordData.txt";
    private static readonly string spritePath = "Sprites/BackGround";
    private static readonly string setDataPath = Application.persistentDataPath + "/SetData.txt";


    #region 属性
    //剧情
    private static Dictionary<int, DialogClass> storyData = new Dictionary<int, DialogClass>();
    public static Dictionary<int, DialogClass> StoryData
    {
        get
        {
            return storyData;
        }
    }
    //背景
    private static Dictionary<string, Sprite> bg = new Dictionary<string, Sprite>();
    public static Dictionary<string, Sprite> BG
    {
        get
        {
            return bg;
        }
    }
    //游戏进度
    private static Dictionary<int, DialogData> saveData = new Dictionary<int, DialogData>();
    public static Dictionary<int, DialogData> SaveData
    {
        get { return saveData; }
    }
    //游戏历史记录
    private static List<int> records = new List<int>();
    public static List<int> Records
    {
        get { return records; }
        private set { records = value; }
    }
    #endregion

    public static IEnumerator Load()
    {
#if !UNITY_EDITOR
        //if (!File.Exists(dialogJsonPath))
        //{
        //    DownloadManager.Instance.AddTask(tempdialogJsonPath, dialogJsonPath);
        //}
        //else
        //{
            LoadDialogJson();//加载剧情信息
        //}
#endif

#if UNITY_EDITOR
        //Excel to Json
        //DownloadManager.Instance.AddTask(tempdialogJsonPath, dialogJsonPath);
        //LoadDialogJson();//加载剧情信息
#endif
        LoadPanel();//加载界面
        LoadSprites();//加载图片
        LoadSetData();//读取游戏的设置
        LoadDialogData();//读取保存的进度
        LoadRecords();//读取游戏的历史记录
        TimerManager.Instance.TimeAction(30, 20f, () => SaveRecords(), null, true);//每20秒保存一次游戏历史记录
        yield return null;
    }


    /// <summary>
    /// 保存游戏进度
    /// </summary>
    /// <param name="dialogData"></param>
    public static void SaveDialogData(DialogData dialogData)
    {
        if (dialogData != null)
        {
            //如果存在相同的ID，移除已有的数据
            if (saveData.ContainsKey(dialogData.ID))
            {
                saveData.Remove(dialogData.ID);
            }
            saveData.Add(dialogData.ID, dialogData);
        }
        StreamWriter sw = new StreamWriter(dialogDataPath);
        sw.Write("[");
        bool first = true;
        foreach (var dict in saveData.Keys)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sw.Write(",");
            }
            string jsonStr = JsonMapper.ToJson(saveData[dict]);
            sw.Write(jsonStr);
        }
        sw.Write("]");
        sw.Close();
    }

    public static DialogData GetDidalogData(int id)
    {
        if (!saveData.ContainsKey(id)) Debug.LogError("不存在id为" + id + "的数据");
        return saveData[id];
    }

    public static void LoadDialogData()
    {
        if (!File.Exists(dialogDataPath))
        {
            return;
        }
        StreamReader sr = new StreamReader(dialogDataPath);
        string jsonStr = sr.ReadToEnd();
        sr.Close();
        JsonData jsonData = JsonMapper.ToObject(jsonStr);
        foreach (JsonData json in jsonData)
        {
            DialogData data = new DialogData();
            data.ID = int.Parse(json["ID"].ToString());
            data.DialogID = int.Parse(json["DialogID"].ToString());
            data.Title = json["Title"].ToString();
            data.TimeData = json["TimeData"].ToString();
            data.BackGround = json["BackGround"].ToString();
            data.PlayerName = json["PlayerName"].ToString();
            data.Music = json["Music"].ToString();
            saveData.Add(data.ID, data);
        }
    }

    public static void SaveSetData(SetData setData)
    {
        string jsonStr = JsonMapper.ToJson(setData);
        StreamWriter sw = new StreamWriter(setDataPath);
        sw.Write(jsonStr);
        sw.Close();
    }

    public static SetData LoadSetData()
    {
        if (!File.Exists(setDataPath))
        {
            return null;
        }
        StreamReader sr = new StreamReader(setDataPath);
        string jsonStr = sr.ReadToEnd();
        sr.Close();
        SetData data = JsonMapper.ToObject<SetData>(jsonStr);
        if (data != null)
        {
            AudioManager.Instance.AudioVolume = (float)data.AudioVolume;
            AudioManager.Instance.MasterVolume = (float)data.MasterVolume;
            AudioManager.Instance.BgmVolume = (float)data.BgmVolume;
        }
        return data;
    }

    private static void LoadPanel()
    {
        FieldInfo[] fileInfos = typeof(PanelName).GetFields();
        foreach (var item in fileInfos)
        {
            string value = "";
            value = item.GetValue(value) as string;
            UIManager.Instance.LoadPanelSync(value);
        }
        UIManager.Instance.OpenPanel(PanelName.MessagePanel);
        //UIManager.Instance.OpenPanel("BackGroundPanel").gameObject.SetActive(false);
        //UIManager.Instance.OpenPanel("DialogPanel").gameObject.SetActive(false);
        //UIManager.Instance.OpenPanel("SelectPanel").gameObject.SetActive(false);
        //UIManager.Instance.OpenPanel("SetPanel").gameObject.SetActive(false);
        //UIManager.Instance.OpenPanel("DataPanel").gameObject.SetActive(false);
        //UIManager.Instance.OpenPanel("CheckSavePanel").gameObject.SetActive(false);
        //UIManager.Instance.OpenPanel("CheckLoadPanel").gameObject.SetActive(false);
        //UIManager.Instance.OpenPanel("RecordsPanel").gameObject.SetActive(false);
        //UIManager.Instance.LoadPanel(PanelName.MessagePanel);
        //UIManager.Instance.LoadPanel(PanelName.MainPanel);
    }



    /// <summary>
    /// 解析对话Json
    /// </summary>
    public static IEnumerator LoadDialogJson()
    {
        //FileStream file = new FileStream(dialogJsonPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        ResourceRequest result = Resources.LoadAsync(storyPath);
        yield return result;
        while (!result.isDone)
        {
            yield return null;
        }
        TextAsset file = result.asset as TextAsset;
        yield return file;
        string jsonStr = file.text;
        //StreamReader sr = new StreamReader(file);
        //string jsonStr = sr.ReadToEnd();
        //sr.Close();
        //file.Close();
        JsonData jsonData = JsonMapper.ToObject(jsonStr);

        foreach (JsonData data in jsonData)
        {
            DialogClass temp = new DialogClass();

            if ((string)data["ID"] == "" || (string)data["ID"] == string.Empty)
            {
                continue;
            }


            temp.ID = int.Parse(data["ID"].ToString());
            try
            {
                temp.Type = int.Parse(data["Type"].ToString());
            }
            catch (System.Exception)
            {

                //throw;
                Debug.Log("ID:"+temp.ID+".Type:"+ data["Type"].ToString());
            }
            //temp.Type = int.Parse(data["Type"].ToString());
            temp.Character = (string)data["Character"];
            //temp.Sprite = (string)data["Sprite"];
            temp.Content = (string)data["Content"];
            temp.Audio = (string)data["Audio"];
            temp.Music = (string)data["Music"];
            temp.Action = (string)data["Action"];
            temp.BackGround = (string)data["BackGround"];
            if ((string)data["NextID"] == "" || (string)data["NextID"] == string.Empty)
            {
                temp.NextID = (temp.ID + 1).ToString();
            }
            else
            {
                temp.NextID = data["NextID"].ToString();
            }
            if (!storyData.ContainsKey(temp.ID))
            {
                storyData.Add(temp.ID, temp);
            }
        }
        Debug.Log("读取完成");
    }

    /// <summary>
    /// 读取背景图片
    /// </summary>
    private static void LoadSprites()
    {
        object[] tempBG = Resources.LoadAll(spritePath, typeof(Sprite));
        foreach (var item in tempBG)
        {
            string temp = item.ToString();
            string spName = temp.Substring(0, temp.IndexOf('('));
            //Debug.Log(spName);
            bg.Add(spName.Trim(), item as Sprite);
        }
    }

    /// <summary>
    /// 保存游戏历史记录（阅读后的）
    /// </summary>
    public static void SaveRecords()
    {
        StreamWriter sw = new StreamWriter(recordsPath);
        bool temp = true;
        for (int i = 0; i < records.Count; i++)
        {
            if (temp)
            {
                temp = false;
                sw.Write(records[i]);
            }
            else
            {
                sw.Write(";" + records[i]);
            }
        }
        sw.Close();
    }
    /// <summary>
    /// 读取历史记录
    /// </summary>
    public static void LoadRecords()
    {
        if (!File.Exists(recordsPath)) return;

        FileStream fileStream = new FileStream(recordsPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        StreamReader sr = new StreamReader(fileStream);
        string fileStr = sr.ReadToEnd();
        if (fileStr != string.Empty)
        {
            string[] tempArray = fileStr.Split(';');
            for (int i = 0; i < tempArray.Length; i++)
            {
                records.Add(int.Parse(tempArray[i]));
            }
        }
        sr.Close();
        fileStream.Close();
    }




}
