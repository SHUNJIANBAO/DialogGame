using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerManager : MonoBehaviour
{
    #region 单例
    private static TimerManager _instance;
    public static TimerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TimerManager>();
            }
            if (_instance == null)
            {
                _instance = new GameObject("TimeManager").AddComponent<TimerManager>();
            }
            return _instance;
        }
    }
    #endregion

    private static List<Timer> timerActions = new List<Timer>();

    private static List<Timer> tempList = new List<Timer>();

    private void Update()
    {
        UpdateTimer();
        RemoveCompleteTimer();
    }

    /// <summary>
    /// 调用计时器
    /// </summary>
    private void UpdateTimer()
    {
        for (int i = 0; i < timerActions.Count; i++)
        {
            Timer timer = timerActions[i];
            if (timer.isComplete)
            {
                tempList.Add(timer);
            }
            else
            {
                timer.Update(Time.unscaledDeltaTime);
            }
        }
    }

    /// <summary>
    /// 移除已经结束的事件
    /// </summary>
    private void RemoveCompleteTimer()
    {
        for (int i = 0; i < tempList.Count; i++)
        {
            timerActions.Remove(tempList[i]);
        }
        tempList.Clear();
    }

    /// <summary>
    /// 计时调用方法
    /// </summary>
    /// <param name="time">总时长</param>
    /// <param name="action">计时结束后调用的方法</param>
    public Timer TimeAction(float totalTime, Action action)
    {
        Timer temp = new Timer(totalTime);
        temp.completeAction = action;
        timerActions.Add(temp);
        return temp;
    }

    /// <summary>
    /// 计时多次调用方法
    /// </summary>
    /// <param name="totalTime">总时长</param>
    /// <param name="intervalTime">间隔时长</param>
    /// <param name="action">间隔调用的方法</param>
    /// <param name="loop">是否循环（总时长无效）</param>
    public Timer TimeAction(float totalTime, float intervalTime, Action intervalAction,Action completeAction=null, bool loop = false)
    {
        Timer temp = new Timer(totalTime, intervalTime, loop);
        temp.intervalAction = intervalAction;
        temp.completeAction = completeAction;
        timerActions.Add(temp);
        return temp;
    }


    /// <summary>
    /// 清除所有计时事件
    /// </summary>
    public void StopAction()
    {
        timerActions.Clear();
    }

    public void StopAction(Timer temp)
    {
        if (timerActions.Contains(temp))
        {
            timerActions.Remove(temp);
        }
    }
}
