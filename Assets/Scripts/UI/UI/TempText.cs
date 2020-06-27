using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempText : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Text>().text = "\u3000\u3000" + "每一个喜爱游戏的人都曾梦想过自己能够做出一款游戏，但最后大多因为诸如不懂得编程，或者不会画画，等种种原因而选择了放弃。有时理想总是很丰满，而现实却总是不尽如人意。但其实理想本身并没有错，也许你只是需要一点助力，而现在正是绝佳的机会。\n"
+ "\u3000\u3000" + "如果你喜欢我们的游戏，擅长绘画，通宵乐理或者有写作的热情，我们便随时欢迎你的加入。虽然我们的团队也刚刚起步，但也正是如此，我们才能和诸君共同进步，成为彼此的助力。\n"
+ "\u3000\u3000" + "QQ群：580716393";
    }
}
