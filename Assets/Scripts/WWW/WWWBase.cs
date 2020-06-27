using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWBase
{
    private string url;
    public string URL
    {
        get { return url; }
        set { url = value; }
    }
    private string targetUrl;
    public string TargetURL
    {
        get { return targetUrl; }
        set { targetUrl = value; }
    }
    public virtual void BeginDownload()
    {

    }
    public virtual void FinishDownload(WWW www)
    {

    }
    public virtual void DownloadError(string error)
    {

    }
    public IEnumerator Download()
    {
        BeginDownload();
        WWW www = new WWW(URL);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            FinishDownload(www);
        }
        else
        {
            //Test.ChangeText("本地失败" + URL+"\n");
            DownloadError(www.error);
        }
    }
}
