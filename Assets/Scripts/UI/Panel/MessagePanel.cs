using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : UIBase 
{
    CanvasGroup message;
    Text text;
    private void Start()
    {
        message = GetComponent<CanvasGroup>();
        message.alpha = 0;
        text = GetUI("Text_UI").GetComponent<Text>();
        ActionManager.Instance.RegistAction(ActionType.SendMessage, (object[] objs) =>
         {
             transform.SetAsLastSibling();
             //发送的信息内容
             string mess=(string) objs[0];
             text.text = mess;
             StopCoroutine("Send");
             StartCoroutine("Send");
         });
    }

    IEnumerator Send()
    {
        while (message.alpha<1)
        {
            message.alpha += Time.unscaledDeltaTime*4;
            yield return null;
        }
        yield return new WaitForSeconds(2);
        while (message.alpha > 0)
        {
            message.alpha -= Time.unscaledDeltaTime*4;
            yield return null;
        }
    }
}
