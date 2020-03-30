using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public Rigidbody2D hookRb;
    public float moveSpeed = 300f;
    
    public void Initialise()
    {
        hookRb = GetComponent<Rigidbody2D>();
    }
    public void ThrowHook(Vector2 dir)
    {
       hookRb.AddForce(dir * moveSpeed * Time.fixedUnscaledDeltaTime, ForceMode2D.Impulse);
    }
}
