using System.Collections.Generic;
using UnityEngine;

public class ClipManager 
{
    AudioClip[] objs;
    public ClipManager(string clipFilePath)
    {
        objs = Resources.LoadAll<AudioClip>(clipFilePath);
    }

    public AudioClip GetClip(string clipName)
    {
        foreach (AudioClip item in objs)
        {
            if (item.name==clipName)
            {
                return item;
            }
        }
        Debug.LogError("没有名称为：" + clipName + "的音频");
        return null;
    }
}
