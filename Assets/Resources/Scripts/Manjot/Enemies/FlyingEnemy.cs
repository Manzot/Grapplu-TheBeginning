using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyUnit
{
    const float ATTACK_DISTANCE = 8f;
    const float NEAR_TARGET = .1f;
    const float ATTACK_SPEED_MULTIPLIER = 2f;

    float RANDOM_Y = 0;
    bool movingUp;

    Collider2D coli;
    Collider2D boundary;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
        coli = GetComponent<Collider2D>();
        boundary = transform.parent.GetComponent<Collider2D>();
    }

    public override void Refresh()
    {
        base.Refresh();
        if (Input.GetKeyDown(KeyCode.K))
        {
            if ((transform.position - target.position).sqrMagnitude < 0.5f)
                TakeDamage(10);
        }
        if (!Death())
        {
            Timers();
            AnimationCaller();
            OutofBoundary();

            if (!isHurt && !isStunned)
            {
                if (!targetFound)
                {
                    FindTarget();
                    FindTarget(new Vector3(transform.right.x, -1f, 0), 6f);
                    DirectionFacingWhenMoving();
                }
                else
                {
                    LookingAtTarget();
                }
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

        if (!Death())
        {
            if (!isHurt && !isStunned)
            {
                if (!targetFound)
                {
                    RandomMove();
                }
                else
                {
                    AttackMove();
                }
            }
            else if (isHurt)
            {
                Hurt();
            }
        }
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
        

        if (canAttack)
        {
            if (distanceToPlayer.sqrMagnitude > NEAR_TARGET && distanceToPlayer.sqrMagnitude < ATTACK_DISTANCE && attackCooldownTimer <= 0)
            {
                rb.velocity = distanceToPlayer.normalized * speed * ATTACK_SPEED_MULTIPLIER * Time.fixedDeltaTime;
            }
            else if (distanceToPlayer.sqrMagnitude < NEAR_TARGET)
            {
                rb.velocity = -distanceToPlayer.normalized * speed * ATTACK_SPEED_MULTIPLIER * Time.fixedDeltaTime;
                canAttack = false;
                attackCooldownTimer = attackCooldown;
            }
            else
            {
                rb.velocity = distanceToPlayer.normalized * speed * Time.fixedDeltaTime;
            }
        }
        else
        {
            if(transform.position.y < target.position.y - NEAR_TARGET)
            {
                CorrectingDirection(new Vector2(0, 1) * speed * Time.fixedDeltaTime);
            }
            else
                RandomMove();
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
            if (!movingUp)
            {
                if (moveRight) rb.velocity = new Vector2(1, RANDOM_Y) * speed * Time.fixedDeltaTime; // Move Right
                else rb.velocity = new Vector2(-1, RANDOM_Y) * speed * Time.fixedDeltaTime; // Move Left
            }
        }

    }

    void OutofBoundary()
    {
        if (transform.position.x < boundary.bounds.min.x || transform.position.x > boundary.bounds.max.x ||
            transform.position.y < boundary.bounds.min.y || transform.position.y > boundary.bounds.max.y)
        {
            CorrectingDirection(rb.velocity * -1);
        }
    }

    void CorrectingDirection(Vector2 dir)
    {
        movingUp = true;
        rb.velocity = dir;
        TimerDelg.Instance.Add(() => { movingUp = false; moveTimeCounter = moveTime; }, 2f);
    }
}
