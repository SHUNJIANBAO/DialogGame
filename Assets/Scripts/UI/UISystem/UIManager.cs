using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CanvasTag
{
    MainCanvas
}
public struct PanelName
{
    public const string BackGroundPanel = "BackGroundPanel";
    public const string DialogPanel = "DialogPanel";
    public const string SelectPanel = "SelectPanel";
    public const string MaskPanel = "MaskPanel";
    public const string MainPanel = "MainPanel";
    public const string SetPanel = "SetPanel";
    public const string DataPanel = "DataPanel";
    public const string CheckSavePanel = "CheckSavePanel";
    public const string CheckLoadPanel = "CheckLoadPanel";
    public const string RecordsPanel = "RecordsPanel";
    public const string NamePanel = "NamePanel";
    public const string MessagePanel = "MessagePanel";
    public const string OtherPanel = "OtherPanel";
}

public class UIManager : MonoBehaviour
{
    #region 单例
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
            }
            if (_instance == null)
            {
                _instance = new GameObject("UIManager").AddComponent<UIManager>();
            }
            return _instance;
        }
    }
    #endregion

    //private UIManager()
    //{
    //    mainCanvas = GameObject.FindGameObjectWithTag(CanvasTag.MainCanvas.ToString()).transform;
    //    if (mainCanvas == null) mainCanvas = GameObject.Find("Canvas").transform;
    //    if (mainCanvas == null) Debug.LogError("没有找到Canvas画布");
    //}
    private void Awake()
    {
        mainCanvas = GameObject.FindGameObjectWithTag(CanvasTag.MainCanvas.ToString()).transform;
        if (mainCanvas == null) mainCanvas = GameObject.Find("Canvas").transform;
        if (mainCanvas == null) Debug.LogError("没有找到Canvas画布");
    }

    private static string panelPath = "UI/Panel/";
    private static string uiPath = "UI/UI/";
    public Dictionary<string, UIBase> panelDict;
    public Dictionary<string, Dictionary<string, GameObject>> uiDict;

    private Transform mainCanvas;
    //todo
    #region Panel和UI的加载与销毁
    /// <summary>
    /// 打开Panel
    /// </summary>
    /// <param name="panelName"></param>
    public UIBase OpenPanel(string name)
    {
        if (panelDict == null) panelDict = new Dictionary<string, UIBase>();
        if (!panelDict.ContainsKey(name)) LoadPanel(name);
        UIBase panel = panelDict.GetValue(name);
        if (panel == null) Debug.LogError("Panel没有UIBase脚本:" + name);
        panel.OnEnter();
        return panel;
    }

    /// <summary>
    /// 关闭Panel
    /// </summary>
    /// <param name="panelName"></param>
    public void ClosePanel(string panelName)
    {
        if (panelDict.ContainsKey(panelName))
        {
            panelDict[panelName].OnExit();
        }
    }
    public void ClosePanel(UIBase panelObj)
    {
        if (panelDict.ContainsKey(panelObj.name))
        {
            ClosePanel(panelObj.name);
        }
        else
        {
            Debug.LogError("要关闭的Panel:" + panelObj.name + "不存在");
        }
    }

    public void CloseAllPanel()
    {
        foreach (var key in panelDict.Keys)
        {
            panelDict[key].OnExit();
        }
    }

    /// <summary>
    /// 移除并销毁Panel
    /// </summary>
    /// <param name="panelName"></param>
    public void RemovePanel(string panelName)
    {
        if (panelDict.ContainsKey(panelName))
        {
            UIBase temp = panelDict[panelName];
            panelDict.Remove(panelName);
            GameObject.Destroy(temp.gameObject);
        }
    }
    public void RemovePanel(UIBase panelBase)
    {
        RemovePanel(panelBase.name);
    }

    public GameObject CreateUI(string uiName, Transform parent)
    {
        GameObject obj = Resources.Load<GameObject>(uiPath + uiName);
        if (obj == null)
        {
            Debug.Log(uiPath + uiName);
        }
        GameObject go = GameObject.Instantiate(obj, parent);
        go.name = uiName;
        UIBehaviour uiBe = go.GetComponent<UIBehaviour>();
        //if (uiBe == null&&go.name.EndsWith("_UI")) uiBe=go.AddComponent<UIBehaviour>();
        if (uiBe == null && go.name.EndsWith("_UI")) uiBe = go.AddComponent<UIBehaviour>();
        return go;
    }

    /// <summary>
    /// 将panel异步加载到内存中
    /// </summary>
    /// <param name="panelName">panel名字</param>
    public IEnumerator LoadPanelSync(string panelName, Transform canvas = null)
    {
        if (panelDict == null) panelDict = new Dictionary<string, UIBase>();
        if (!panelDict.ContainsKey(panelName))
        {
            ResourceRequest requst = Resources.LoadAsync<GameObject>(panelPath + panelName);
            yield return requst;
            while (!requst.isDone)
            {
                yield return null;
            }
            GameObject go = requst.asset as GameObject;
            if (go == null)
            {
                Debug.LogError(panelPath + panelName + "——没有找到Panel");
            }
            GameObject panel;
            if (canvas == null)
                panel = GameObject.Instantiate(go, mainCanvas, false);
            else panel = GameObject.Instantiate(go, canvas, false);
            panel.name = panelName;
            UIBase uiBase = panel.GetComponent<UIBase>();
            uiBase.RegistPanelAndUi();
            panelDict.Add(panelName, uiBase);
            go.SetActive(false);
            yield return null;
        };
    }


    /// <summary>
    /// 将panel加载到内存中
    /// </summary>
    /// <param name="panelName">panel名字</param>
    public GameObject LoadPanel(string panelName, Transform canvas = null)
    {
        if (panelDict == null) panelDict = new Dictionary<string, UIBase>();
        if (panelDict.ContainsKey(panelName))
        {
            UIBase temp = panelDict.GetValue(panelName);
            return temp.gameObject;
        };
        GameObject go = Resources.Load<GameObject>(panelPath + panelName);
        if (go == null)
        {
            Debug.LogError(panelPath + panelName + "——没有找到Panel");
        }
        GameObject panel;
        if (canvas == null)
            panel = GameObject.Instantiate(go, mainCanvas, false);
        else panel = GameObject.Instantiate(go, canvas, false);
        panel.name = panelName;
        UIBase uiBase = panel.GetComponent<UIBase>();
        uiBase.RegistPanelAndUi();
        panelDict.Add(panelName, uiBase);
        return go;
    }

    #endregion
    //todo 移除
    #region Panel和UI的注册与移除
    /// <summary>
    /// 注册UI
    /// </summary>
    /// <param name="panelName"></param>
    /// <param name="uiName"></param>
    /// <param name="uiObj"></param>
    public void RegistUI(string panelName, string uiName, GameObject uiObj)
    {
        if (uiDict == null) uiDict = new Dictionary<string, Dictionary<string, GameObject>>();
        if (!uiDict.ContainsKey(panelName))
        {
            uiDict[panelName] = new Dictionary<string, GameObject>();
        }
        if (uiDict[panelName].ContainsKey(uiName))
        {
            return;
        }
        uiDict[panelName].Add(uiName, uiObj);
    }

    public void UnRegistUI(string panelName, string uiName)
    {
        if (!uiDict.ContainsKey(panelName))
        {
            return;
        }
        if (uiDict[panelName].ContainsKey(uiName))
        {
            uiDict[panelName].Remove(uiName);
        }
    }

    public void UnRegistPanel(string panelName)
    {
        if (uiDict.ContainsKey(panelName))
        {
            uiDict.Remove(panelName);
        }
        if (panelDict.ContainsKey(panelName))
        {
            uiDict.Remove(panelName);
        }
    }
    #endregion

    #region Panel和UI的获取
    /// <summary>
    /// 得到UI
    /// </summary>
    /// <param name="panelName"></param>
    /// <param name="uiName"></param>
    /// <returns></returns>
    public GameObject GetUI(string panelName, string uiName)
    {
        if (uiDict == null) uiDict = new Dictionary<string, Dictionary<string, GameObject>>();
        if (!uiDict.ContainsKey(panelName))
        {
            Debug.LogError("Panel不存在:" + panelName);
            return null;
        }
        if (!uiDict[panelName].ContainsKey(uiName))
        {
            panelDict[panelName].RegistPanelAndUi();
        }
        if (!uiDict[panelName].ContainsKey(uiName))
        {
            Debug.LogError("UI不存在或名称不合法");
            return null;
        }
        return uiDict[panelName][uiName];
    }

    /// <summary>
    /// 得到Panel
    /// </summary>
    /// <param name="panelName"></param>
    /// <returns></returns>
    public UIBase GetPanel(string panelName)
    {
        if (panelDict.ContainsKey(panelName))
            return panelDict[panelName];
        return null;
    }

    /// <summary>
    /// 得到所有Panel
    /// </summary>
    /// <returns></returns>
    public RectTransform[] GetAllPanel()
    {
        List<RectTransform> list = new List<RectTransform>();
        foreach (var panel in panelDict)
        {
            GameObject panelgo = GetPanel(panel.Key).gameObject;
            if (panelgo == null)
            {
                continue;
            }
            RectTransform tran = panelgo.transform as RectTransform;
            if (!tran) continue;
            list.Add(tran);
        }
        return list.ToArray();
    }
    #endregion
}
