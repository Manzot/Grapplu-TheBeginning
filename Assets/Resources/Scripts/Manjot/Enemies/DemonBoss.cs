using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBoss : BossUnit
{
    const float maxGravity = -12f;
    Vector2 dirTowardsTarget;
    bool isSpawning = true;
    float attackCooldownTimer;
    bool isAttacking;

    public override void Initialize()
    {
        base.Initialize();
        rb.velocity = Vector2.zero;
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
        attackCooldownTimer = attackCooldown;
        TimerDelg.Instance.Add(() => { isSpawning = false; }, 3f);
    }

    public override void Refresh()
    {
        base.Refresh();
        if (!Dead())
        {
            Timers();

            if (!isSpawning)
                AttackMove();
            else
            {
                if (rb.velocity.y < maxGravity)
                {
                    rb.velocity = new Vector2(rb.velocity.x, maxGravity);
                }
            }

            anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));
            LookingAtTarget();

        }
    }

    public override void PhysicsRefresh()
    {
        base.PhysicsRefresh();
        
    }

    public void MoveToPlayer()
    {
        dirTowardsTarget = (target.position - transform.position).normalized;
        rb.velocity = new Vector2(dirTowardsTarget.x, 0) * speed * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);
    }

    void AttackMove()
    {
        int rndAttack = Random.Range(0, 4);

        switch (rndAttack)
        {
            case 0:
                SlashAttack1();
                break;
            case 1:
                SlashAttack2();
                break;
            case 2:
                Kick();
                break;
            case 3:
                JumpAttack();
                break;
        }

        if (!isAttacking)
        {
            MoveToPlayer();
        }
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
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("slash_1");
    }
    public void SlashAttack2()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("slash_2");
    }
    public void Kick()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("kick_1");
    }
    public void JumpAttack()
    {
        //rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("jump_attack");
    }
    public void BoolDisabler()
    {
        isAttacking = false;
    }

    void Timers()
    {
        attackCooldownTimer -= Time.deltaTime;
    }
}
