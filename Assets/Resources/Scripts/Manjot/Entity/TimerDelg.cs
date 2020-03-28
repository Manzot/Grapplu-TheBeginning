using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerDelg
{
    #region singleton
    private static TimerDelg instance;
    private TimerDelg() { }
    public static TimerDelg Instance { get { return instance ?? (instance = new TimerDelg()); } }
    #endregion

    List<TimerFunction> timerFuncList;

    public void PostInitialize()
    {
        timerFuncList = new List<TimerFunction>();
    }
    public void Refresh()
    {
        //Debug.Log(timerFuncList.Count);
        for (int i = timerFuncList.Count -1 ; i >= 0; i--)
        {
            timerFuncList[i].timer -= Time.deltaTime;
            if (timerFuncList[i].timer <= 0)
            {
                timerFuncList[i].action.Invoke();
                timerFuncList.Remove(timerFuncList[i]);
            }
        }
    }

    public void Add(Action a, float _timer)
    {
        TimerFunction tf = new TimerFunction();
        tf.action = a;
        tf.timer = _timer;
        timerFuncList.Add(tf);
    }

}

public class TimerFunction
{
    public Action action;
    public float timer;
}

