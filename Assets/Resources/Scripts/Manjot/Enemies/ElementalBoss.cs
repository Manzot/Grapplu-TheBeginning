using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBoss : BossUnit
{
    const float PUNCH_ATK_TIME = 1.2f;
    const float BIGLASER_ATK_TIME = 3f;
    const float SMALLLASERS_ATK_TIME = 3f;
    const float SPIKY_ATK_TIME = 2f;

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

        if (!Dead())
        {
            Timers();
            LookingAtTarget();
            Debug.Log(isAttacking);
            //PunchAttack();
            //if(Input.GetKey(KeyCode.Alpha1))
                PunchAttack();
        }

    }
    public override void PhysicsRefresh()
    {
        base.PhysicsRefresh();

        if (!Dead())
        {
            if (!isAttacking)
            {
               // RandomMove();
            }
        }
    }

    public void MoveToPlayer()
    {
      //  anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));
        if (!enraged)
            rb.velocity = new Vector2(dirTowardsTarget.normalized.x, dirTowardsTarget.normalized.y / 50) * speed * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);
        else
        {
            rb.velocity = new Vector2(dirTowardsTarget.normalized.x, dirTowardsTarget.normalized.y / 50) * speed * 2f * Time.fixedDeltaTime + new Vector2(0, rb.velocity.y);
        }
    }

    public void PunchAttack()
    {
        isAttacking = true;

        if (dirTowardsTarget.sqrMagnitude <= 3f)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("punchAttack", true);
            TimerDelg.Instance.Add(() => { anim.SetBool("punchAttack", false); isAttacking = false; }, PUNCH_ATK_TIME);
        }
        else
            MoveToPlayer();
    }

    public void BigLaserAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("bigLaserAttack");
        TimerDelg.Instance.Add(() => { isAttacking = false; moveTimeCounter = 0; }, BIGLASER_ATK_TIME);

    }

    public void SmallLasersAttacks()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        RandomMove();
        anim.SetBool("smallaserAttack", true);
        TimerDelg.Instance.Add(() => { isAttacking = false; moveTimeCounter = 0; anim.SetBool("smallaserAttack", false); }, SMALLLASERS_ATK_TIME);
    }

    public void SpikyAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("spikyAttack");
        TimerDelg.Instance.Add(() => { isAttacking = false; moveTimeCounter = 0; }, SPIKY_ATK_TIME);
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
       // else
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
