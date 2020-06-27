using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WWWItem :WWWBase
{
    public WWWItem(string url,string target)
    {
        URL = url;
        TargetURL = target;
    }
    public override void FinishDownload(WWW www)
    {
        File.WriteAllText(TargetURL,www.text);
        ResourcesManager.LoadDialogJson();
    }
    public override void DownloadError(string error)
    {
        Test.ChangeText(error);
        Test.ChangeText("Error:"+TargetURL);
    }
}
