using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LogoPanel : MonoBehaviour 
{
    Image mask;
    
    private void Awake()
    {
        mask = GetComponent<Image>();
    }
    private void Start()
    {
        StartCoroutine(Mask());
    }

    IEnumerator Mask()
    {
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            mask.color = new Color(a, a, a, 1);
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        UIManager.Instance.OpenPanel(PanelName.MainPanel);
        transform.SetAsLastSibling();
        while (a > 0)
        {
            a -= Time.deltaTime;
            mask.color = new Color(a , a , a , 1 );
            yield return null;
        }
        a = 1f;
        while (a>0)
        {
            a -= Time.deltaTime ;
            mask.color = new Color(0, 0, 0, a);
            yield return null;
        }
        Destroy(gameObject);
    }

}
