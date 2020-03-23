using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyUnit
{
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
    }

    public override void Refresh()
    {
        Timers();
        RandomMove();
        FindTarget();
        if (targetFound)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public override void PhysicsRefresh()
    {
        base.PhysicsRefresh();
    }

    void Timers()
    {
        moveTimeCounter -= Time.deltaTime;
    }
}
