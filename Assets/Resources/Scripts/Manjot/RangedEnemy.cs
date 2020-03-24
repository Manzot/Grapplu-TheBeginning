using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyUnit
{
    const float ATTACK_TIMER = 3f;

    GameObject fireball;
    float throwAttackTimer = 6f; 
    Transform throwPoint;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
        fireball = Resources.Load<GameObject>("Prefabs/Manjot/FireBall");
        throwPoint = transform.GetChild(1);
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
            if (throwAttackTimer <= 0)
            {
                canAttack = true;
                CreateThrowable();
                throwAttackTimer = ATTACK_TIMER;
            }
            else
                canAttack = false;

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
        throwAttackTimer -= Time.deltaTime;
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
        GameObject.Instantiate(fireball, throwPoint.position, Quaternion.identity, throwPoint);
    }
}
