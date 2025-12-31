using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public Rigidbody2D hookRb;
    TimeSlowMo timeSlowMo;
    public float moveSpeed;
    
    public void Initialise()
    {
        timeSlowMo = new TimeSlowMo();
        hookRb = GetComponent<Rigidbody2D>();

    }
    public void ThrowHook(Vector2 dir)
    {
       hookRb.AddForce(dir * moveSpeed , ForceMode2D.Impulse);
    }
}
