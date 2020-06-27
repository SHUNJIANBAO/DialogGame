using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour 
{
    static Text text;
    private void Awake()
    {
        text = GetComponent<Text>();
    }
    public static void ChangeText(string ts)
    {
        text.text += ts;
    }
}
