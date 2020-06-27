using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Record : MonoBehaviour 
{
    private DialogClass dialog;
    public DialogClass Dialog
    {
        set
        {
            dialog = value;
            SetRecord();
        }
    }

    void SetRecord()
    {
        Text[] trans= transform.GetComponentsInChildren<Text>();
        foreach (Text item in trans)
        {
            if (item.name== "Name")
            {
                item.text = dialog.Character;
            }
            if(item.name== "Connect")
            {
                item.text = dialog.Content;
            }
        }
    }
}
