using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRegist
{
    public static void Regist()
    {
        //注册黑屏过渡事件
        ActionManager.Instance.RegistAction(ActionType.Transition, (object[] obj) =>
        {
            try
            {
                Transition(float.Parse(obj[0].ToString()), int.Parse(obj[1].ToString()));
            }
            catch (System.Exception)
            {
                Debug.Log("count" + obj.Length + "值" + obj[0].ToString() + "   " + obj[1].ToString());
            }
        }
        );

        //注册语句判断事件
        //ActionManager.Instance.RegistAction(ActionType.If, (object[] objs) =>
        //{
        //    DialogClass dialog = (DialogClass)objs[0];
        //});

    }

    private static void Transition(float timeCount, int dialogID)
    {
        UIManager.Instance.OpenPanel(PanelName.MaskPanel);
        UIManager.Instance.GetPanel(PanelName.MaskPanel).GetComponent<MaskPanel>().Transition(timeCount, dialogID);
    }

}
