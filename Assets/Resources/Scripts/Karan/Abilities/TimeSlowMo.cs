using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSlowMo {
    //public float slowdownFactor = 0.05f;
    // public float slowdownLength = 3f;

    // public bool timeSlow;
    float fixTime = Time.fixedDeltaTime;

    public float TimeReset(float _slowdownLength)
    {
        
            Time.timeScale += (1f / _slowdownLength) * Time.unscaledDeltaTime;
           
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            if (Time.timeScale == 1.0f)
            {
                Time.fixedDeltaTime = fixTime;
               // timeSlow = false;
            /*pc.jumpForce = pc.oldJumpForce*Time.unscaledDeltaTime;*/
        }
        return Time.timeScale;
    }

    public void SlowMotion(float _slowdownFactor)
    {
        Time.timeScale = _slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

}
