using System.Collections.Generic;

public static class DictHandel
{
    /// <summary>
    /// 得到字典的值
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    /// <typeparam name="Tvalue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static Tvalue GetValue<Tkey,Tvalue> (this Dictionary<Tkey,Tvalue> dict,Tkey key)
    {
        Tvalue tempValue;
        dict.TryGetValue(key, out tempValue);
        return tempValue;
    }
}
