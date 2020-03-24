using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyUnit
{
    GameObject fireball;
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
        fireball = Resources.Load<GameObject>("Prefabs/Manjot/FireBall");
    }

    public override void Refresh()
    {
        Timers();
        DirectionFacingWhenMoving();
        AnimationCaller();
        FindTarget();

        if (!targetFound)
        {
            RandomMove();
        }
        else
        {
            canAttack = true;
            rb.velocity = Vector2.zero;
            LookingAtTarget();
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

    void AnimationCaller()
    {
        anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));
        if (canAttack)
        {
            anim.SetBool("isAttacking", true);
        }
        else
            anim.SetBool("isAttacking", false);

    }
    void CreateThrowable()
    {
        GameObject.Instantiate(fireball, new Vector2(transform.position.x + 0.4f, transform.position.y), Quaternion.identity);
    }
}
