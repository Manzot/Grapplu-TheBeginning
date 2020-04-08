using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSlowMo
{
    //public float slowdownFactor = 0.05f;
    // public float slowdownLength = 3f;

    // public bool timeSlow;

    public float customUnscaledDeltaTime = Time.unscaledDeltaTime * 1;
    public float customFixedUnscaledDeltaTime = Time.fixedUnscaledDeltaTime * 1;
    public float customFixedDeltaTime = Time.fixedDeltaTime * 1;
    public float customTimeScale = 1;
    private float fixTime = 0.02f;// Time.fixedDeltaTime;
    

    public float TimeScaleReset(float timeScale)
    {
        if (timeScale < 1 || timeScale > 1)
        {
            customTimeScale = 1;
        }
        return customTimeScale;
    }
    public float TimeReset(float _slowdownLength)
    {

        Time.timeScale += (1f / _slowdownLength) * Time.unscaledDeltaTime;

        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);

        if (Time.timeScale == 1.0f)
        {
            Time.fixedDeltaTime = fixTime;
        }
        return Time.timeScale;
    }

    public void SlowMotion(float _slowdownFactor)
    {
        Time.timeScale = _slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void InstantResetTime()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = fixTime;
    }

}
