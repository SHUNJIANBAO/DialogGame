using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadManager : MonoBehaviour 
{
    private static DownloadManager _instance;
    public static DownloadManager Instance
    {
        get
        {
            if (_instance==null)
            {
                _instance = new GameObject("DownloadManager").AddComponent<DownloadManager>();
            }
            return _instance;
        }
    }

    Queue<WWWBase> tasks;
    private void Awake()
    {
        tasks = new Queue<WWWBase>();
    }

    public void AddTask(string path,string targetPath)
    {
        WWWItem item = new WWWItem(path, targetPath);
        tasks.Enqueue(item);
        if (tasks.Count==1)
        {
            StartCoroutine(Download());
        }
    }

    IEnumerator Download()
    {
        while (tasks.Count>0)
        {
            WWWBase www = tasks.Dequeue();
            yield return www.Download();
        }
    }

}
