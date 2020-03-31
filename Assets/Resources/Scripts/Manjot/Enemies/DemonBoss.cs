using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBoss : BossUnit
{
    Vector2 dirTowardsTarget;
    bool isSpawning = true;

    public override void Initialize()
    {
        base.Initialize();
        rb.velocity = Vector2.zero;
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
        TimerDelg.Instance.Add(() => { isSpawning = false; }, 3f);
    }

    public override void Refresh()
    {
        base.Refresh();
        if (!Dead())
        {
            anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));
            LookingAtTarget();
        }
    }

    public override void PhysicsRefresh()
    {
        base.PhysicsRefresh();

        if(!isSpawning)
            MoveToPlayer();
        else
        {
            if (rb.velocity.y < -10f)
            {
                rb.velocity = new Vector2(rb.velocity.x, -10f);
            }
        }
    }

    public void MoveToPlayer()
    {
        dirTowardsTarget = (target.position - transform.position).normalized;
        rb.velocity = new Vector2(dirTowardsTarget.x, rb.velocity.y) * speed * Time.fixedDeltaTime;
    }

    public void LookingAtTarget()
    {
        if (target.position.x < transform.position.x)
            transform.rotation = Quaternion.Euler(new Vector2(0, 180));
        else
            transform.rotation = Quaternion.Euler(new Vector2(0, 0));
    }


    public void SlashAttack1()
    {
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("slash_1");
    }
    public void SlashAttack2()
    {
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("slash_2");
    }
    public void Kick()
    {
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("kick_1");
    }
    public void JumpAttack()
    {
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("jump_attack");
    }
    public void BoolDisabler(string boolName)
    {
    }
}
