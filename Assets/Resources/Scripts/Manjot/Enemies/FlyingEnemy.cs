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
    Vector3 startPos;
    
    //Collider2D boundary;

    public override void Initialize()
    {
        base.Initialize();
        eType = EnemyType.Flying;
    }

    public override void PostInitialize()
    {
        base.PostInitialize();
        //coli = GetComponent<Collider2D>();
        startPos = transform.position;
        //boundary = transform.parent.GetComponent<Collider2D>();
    }

    public override void Refresh()
    {
        base.Refresh();

        if (!Death())
        {
            Timers();
            AnimationCaller();
            OutofBoundary();
            CheckGroundCollision();

            if (!isHurt)
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
            if (!isHurt)
            {
                if (!targetFound)
                {
                    RandomMove();
                }
                else
                {
                    if ((transform.position - target.position).sqrMagnitude <= TARGET_IN_RANGE)
                        AttackMove();
                    else RandomMove();
                }
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
            DamageTarget(0.2f, damage);
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
        Vector3 directionToPlayer = (target.position - transform.position);
        

        if (canAttack)
        {
            if (directionToPlayer.sqrMagnitude > NEAR_TARGET && directionToPlayer.sqrMagnitude < ATTACK_DISTANCE && attackCooldownTimer <= 0)
            {
                rb.velocity = directionToPlayer.normalized * speed * ATTACK_SPEED_MULTIPLIER * Time.fixedDeltaTime;
            }
            else if (directionToPlayer.sqrMagnitude < NEAR_TARGET)
            {
                rb.velocity = -directionToPlayer.normalized * speed * ATTACK_SPEED_MULTIPLIER * Time.fixedDeltaTime;
                canAttack = false;
                attackCooldownTimer = attackCooldown;
            }
            else
            {
                if(!movingUp)
                    rb.velocity = directionToPlayer.normalized * speed * Time.fixedDeltaTime;
            }
        }
        else
        {
            if(!movingUp)
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
        //if (transform.position.x < boundary.bounds.min.x || transform.position.x > boundary.bounds.max.x ||
        //    transform.position.y < boundary.bounds.min.y || transform.position.y > boundary.bounds.max.y)
        if((transform.position - startPos).sqrMagnitude >= 200 && !canAttack)
        {
            CorrectingDirection((startPos - transform.position).normalized * speed * Time.fixedDeltaTime);
        }
    }

    void CorrectingDirection(Vector2 dir)
    {
        movingUp = true;
        rb.velocity = dir;
        TimerDelg.Instance.Add(() => { movingUp = false;  }, 2f);
    }

    

    private void CheckGroundCollision()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position, 0.4f, LayerMask.GetMask("Ground", "NonGrappableWalls"));
        
        if(coll)
        {
            coli.isTrigger = false;
            if (!canAttack)
            {
                moveTimeCounter = 0;
                moveRight = !moveRight;
            }
        }
        else
        {
            coli.isTrigger = true;
        }
    }
    
}
