using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyUnit
{
    const float ATTACK_DISTANCE = 8f;
    const float NEAR_TARGET = .2f;
    const float ATTACK_SPEED_MULTIPLIER = 3f;

    Collider2D coli;
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
        coli = GetComponent<Collider2D>();
    }

    public override void Refresh()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if ((transform.position - target.position).sqrMagnitude < 0.5f)
                TakeDamage(10);
        }
        if (!Death())
        {
            Timers();
            AnimationCaller();

            if (!isHurt && !isStunned)
            {
                if (!targetFound)
                {
                    RandomMove();
                    DirectionFacingWhenMoving();
                }
                else
                {
                    AttackMove();
                    LookingAtTarget();
                }
            }
            else if (isHurt)
            {
                Hurt();
            }
        }
        else
        {
            rb.gravityScale = 1f;
            coli.isTrigger = false;
        }

    }
    public override void PhysicsRefresh()
    {
        base.PhysicsRefresh();
    }

    void Timers()
    {
        moveTimeCounter -= Time.deltaTime;
        if(targetFound)
            attackCooldownTimer -= Time.deltaTime;

        if(attackCooldownTimer <= 0)
        {
            canAttack = true;
        }
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


        if (isHurt)
            anim.SetBool("isHurt", true);
        else
            anim.SetBool("isHurt", false);

    }

    void AttackMove()
    {
        Vector3 distanceToPlayer = (target.position - transform.position);

        if(canAttack)
        {
            if (distanceToPlayer.sqrMagnitude > NEAR_TARGET && distanceToPlayer.sqrMagnitude < ATTACK_DISTANCE && attackCooldownTimer <= 0)
            {
                rb.velocity = distanceToPlayer.normalized * speed * ATTACK_SPEED_MULTIPLIER * Time.deltaTime;
            }
            else if (distanceToPlayer.sqrMagnitude < NEAR_TARGET)
            {
                canAttack = false;
                attackCooldownTimer = attackCooldown;
            }
            else
            {
                rb.velocity = distanceToPlayer.normalized * speed * Time.deltaTime;
            }
        }
        else
        {
            if (distanceToPlayer.sqrMagnitude > ATTACK_DISTANCE)
                RandomMove();
            else
                rb.velocity = distanceToPlayer.normalized * -speed * Time.deltaTime;
        }

    }
}
