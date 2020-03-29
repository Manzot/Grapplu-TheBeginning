using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public Rigidbody2D hookRb;
    public float moveSpeed = 300f;
    
    private float step =25f;
    
    public void Initialise()
    {
        hookRb = GetComponent<Rigidbody2D>();
    }
    public void ThrowHook(Vector2 dir)
    {
       hookRb.AddForce(dir * moveSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
   
        //if (rope.hook)
        //{
        //    UpdateRopePosition();
        //}
    }

    //private void UpdateRopePosition()
    //{
    //    ropeRenderer.SetPosition(0, rope.hookShoot.transform.position);
    //    if (canMove)
    //    {
    //        ropeRenderer.SetPosition(1, rope.hook.transform.position);

    //    }
    //}

   

}
