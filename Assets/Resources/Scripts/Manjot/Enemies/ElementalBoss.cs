using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBoss : BossUnit
{
    Vector2 dirTowardsTarget;
    float moveTimeCounter;
    float moveTime = 2;
    float RANDOM_Y = 0;
    bool moveRight;

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
        base.Refresh();
        dirTowardsTarget = (target.position - transform.position);

        if (!isAttacking)
            RandomMove();

        PunchAttack();
    }
    public override void PhysicsRefresh()
    {
        base.PhysicsRefresh();
    }

    public void MoveToPlayer()
    {
       

        if (!enraged)
            rb.velocity = new Vector2(dirTowardsTarget.normalized.x, 0) * speed * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);
        else
        {
            rb.velocity = new Vector2(dirTowardsTarget.normalized.x, 0) * speed * 2f * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);
        }
    }

    public void PunchAttack()
    {
        if(dirTowardsTarget.sqrMagnitude <= 3f)
        {
            isAttacking = true;
            rb.velocity = Vector2.zero;
            anim.SetBool("punchAttack", true);
            TimerDelg.Instance.Add(() => { anim.SetBool("punchAttack", false); isAttacking = false; }, 1.2f);

        }
    }

    public void RandomMove()
    {
        
        if (moveTimeCounter <= 0)
        {
            RANDOM_Y = Random.Range(-1f, 1f);
            int rnd = Random.Range(0, 2);

            if (rnd == 0) moveRight = true;
            else moveRight = false;

            moveTimeCounter = moveTime;
        }
        else
        {
                if (moveRight) rb.velocity = new Vector2(1, RANDOM_Y) * speed * Time.fixedDeltaTime; // Move Right
                else rb.velocity = new Vector2(-1, RANDOM_Y) * speed * Time.fixedDeltaTime; // Move Left
            
        }

    }

    public void Timers()
    {
        moveTimeCounter -= Time.deltaTime;
    }
}
