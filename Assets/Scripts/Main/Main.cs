using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour 
{
    private static Main _instance;
    public static Main Instance;
    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
        StartCoroutine(ResourcesManager.Load());
        StartCoroutine(ResourcesManager.LoadDialogJson());
        ActionRegist.Regist();
    }

}
