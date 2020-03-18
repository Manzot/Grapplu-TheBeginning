﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rb;

    public float speed;
    public float jumpForce;
    public int hitPoints;
    public int damage;
    
    public virtual void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public virtual void PostInitialize()
    {

    }
    public virtual void Refresh()
    {

    }
    public virtual void PhysicsRefresh()
    {

    }
}
