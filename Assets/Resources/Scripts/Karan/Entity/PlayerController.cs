using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum Abilities { Grappler, Rewind, SlowMotion }

public class PlayerController : MonoBehaviour, IDamage
{
    const float SLOMO_FACTOR = 0.3f;
    const float ATTACK_RANGE = .4f;
    const float maxGravity = -12f;

    public Vector2 ropeHook;
    Collider2D groundCheckColi;
    public float speed;
    public int damage = 10;
    public float swingForce;
    public float jumpForce;
    public float oldJumpForce;
    private float jumpInput;
    public float horizontal;
    public float aimAngle;
    float timeSlowCooldown = 10f;

    bool timeSlow;

    bool isRewinding;
    List<PointInTime> pointsInTime;

    //int jumpCount = 1;

    bool isJumping;
    bool isAttacking;
    public bool groundCheck;
    public bool isSwinging;
    public bool isAlive;
    bool isHurt;


    public Transform feet;
    public Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;

    public Vector2 angleDirection;
    public Vector3 deathLoc;

    public GameObject crosshair;
    TimeSlowMo timeSlowMo;

    Transform punchesPos;

    //Hook hook;

    public float health = 100f;
    private const float MAX_HEALTH = 100;


    public void Initialize()
    {
        if (health <= 0)
        {
            health = MAX_HEALTH;
        }


        isAlive = true;

        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        timeSlowMo = new TimeSlowMo();


    }
    public void PostInitialize()
    {
        pointsInTime = new List<PointInTime>();
        punchesPos = transform.Find("Punches").transform;
    }
    public void Refresh()
    {
        //Debug.Log(health);
        if (!Dead())
        {
            Jump();

            SetCrossairPoint(CrossairDirection());

            if (Input.GetKeyDown(KeyCode.T))
            {

                if (!timeSlow)
                {
                    SoundManager.Instance.Play("PlayerTimeSlow");
                    TimeSlowAbility();
                }

            }

            TimeSlowReset();
            Attack();

            if (Input.GetButtonDown("Fire1"))
            {
                isAttacking = true;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!timeSlow)
                    StartRewind();
            }
            if (Input.GetKeyUp(KeyCode.R))
            {
                StopRewind();
                SoundManager.Instance.StopPlaying("PlayerTimeRewind");
            }
        }




    }
    public void PhysicsRefresh()
    {
        if (!Dead())
        {
            Movement();
            GravityCheck();

            if (isSwinging)
                SwingDirectionForce();

            if (isRewinding)
            {


                Rewind();
            }
            else
            {
                Record();
            }
        }

    }

    private void TimeSlowAbility()
    {
        if (!timeSlow)
        {

            timeSlowMo.SlowMotion(SLOMO_FACTOR);

            timeSlow = true;

        }
    }

    private void TimeSlowReset()
    {
        if (timeSlow)
        {
            if (timeSlowMo.TimeReset(timeSlowCooldown) >= 1)
            {
                timeSlow = false;
            }
        }
    }

    /*Deflecting a gameobject */
    private void DeflectBullet(GameObject go)
    {
        Rigidbody2D rbGO = go.GetComponent<Rigidbody2D>();
        rbGO.velocity = -1 * rbGO.velocity;
        Vector2 dir = (go.transform.position - transform.position).normalized;
        var angle2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.AngleAxis(angle2, Vector3.forward);
    }

    /*Get a normalized direction vector from the player to the hook point 
     * and  Inverse the direction to get a perpendicular direction based on the HorizontalInput */
    private Vector2 CalculatePerpendicularDirection()
    {
        Vector2 perpendicularDirection;

        var playerToHookDirection = (ropeHook - (Vector2)transform.position).normalized;
        Debug.DrawLine(transform.position, playerToHookDirection, Color.red, 0f);

        if (Input.GetAxis("Horizontal") < 0)
        {
            sprite.flipX = true;
            perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
            var leftPerpPos = (Vector2)transform.position + perpendicularDirection;
            Debug.DrawLine(transform.position, leftPerpPos, Color.green, 0f);
        }
        else
        {
            sprite.flipX = false;
            perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
            var rightPerpPos = (Vector2)transform.position - perpendicularDirection;
            Debug.DrawLine(transform.position, rightPerpPos, Color.green, 0f);
        }

        return perpendicularDirection;
    }
    /* Applying the force towards the perpendicular direction */
    public void SwingDirectionForce()
    {
        if (rb.velocity.x < 0f || rb.velocity.x > 0f)
        {
            if (isSwinging)
            {
                Vector2 perpendicularDirection = CalculatePerpendicularDirection();

                var force = perpendicularDirection * swingForce;
                rb.AddForce(force);
            }

        }
    }

    /*Movement of the player*/
    public void Movement()
    {
        horizontal = Input.GetAxis("Horizontal");
        float direction = CrossairDirection();
        float angle = CrossairDirection() * Mathf.Rad2Deg;
        /*Debug.Log(angle);*/
        if (!RopeSystem.isRopeAttached)
        {
            if (angle > 91)
            {
                sprite.flipX = true;

                if (angle > 91 && (rb.velocity.x < 0))
                {
                    sprite.flipX = true;
                    // transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

                }
                else if (angle > 91 && (rb.velocity.x > 0))
                {
                    sprite.flipX = false;

                }
            }
            else
            {
                sprite.flipX = false;

                if (angle < 91 && (rb.velocity.x > 0))
                {
                    sprite.flipX = false;

                    // transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                }
                else if (angle < 91 && (rb.velocity.x < 0))
                {
                    sprite.flipX = true;

                }

            }
        }

        if (!isSwinging)
        {
            if (Input.GetKey(KeyCode.A))
            {
                // horizontal = -1;
                rb.velocity = new Vector2(horizontal * speed * timeSlowMo.customFixedUnscaledDeltaTime, rb.velocity.y);


            }
            else if (Input.GetKey(KeyCode.D))
            {
                // horizontal = 1;
                rb.velocity = new Vector2(horizontal * speed * timeSlowMo.customFixedUnscaledDeltaTime, rb.velocity.y);


            }
        }
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));



    }
    public void Jump()
    {
        if (Grounded())
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                SoundManager.Instance.Play("PlayerJump");

                if (!timeSlow)
                    rb.AddForce(new Vector2(rb.velocity.x, jumpForce * timeSlowMo.customFixedUnscaledDeltaTime), ForceMode2D.Impulse);
                else

                    rb.AddForce(new Vector2(rb.velocity.x, jumpForce * timeSlowMo.customFixedUnscaledDeltaTime), ForceMode2D.Impulse);

                isJumping = true;
                TimerDelg.Instance.Add(() => { isJumping = false; }, .5f);
            }
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isJumping", true);
        }
    }
    /* Checking for ground collision */
    public bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(feet.position, 0.1f, LayerMask.GetMask("Ground", "IObject"));
    }

    float CrossairDirection()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        Vector3 faceDirection = worldMousePosition - transform.position;

        aimAngle = Mathf.Atan2(faceDirection.y, faceDirection.x);

        /*if (aimAngle < 0)
        {
            return aimAngle = Mathf.PI * 2 + aimAngle;
        }*/

        angleDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        return aimAngle;
    }
    public void SetCrossairPoint(float aimAngle)
    {

        float x = transform.position.x + 2f * Mathf.Cos(aimAngle);
        float y = transform.position.y + 2f * Mathf.Sin(aimAngle);

        Vector3 crosshairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crosshairPosition;
    }



    public void Attack()
    {
        if (isAttacking)
        {
            SoundManager.Instance.Play("PlayerAttack");
            DamageEnemies(damage);
            animator.SetBool("isAttacking", true);
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }

    }

    public void DisableBools()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (!isHurt)
        {
            isHurt = true;
            SoundManager.Instance.Play("PlayerHurt");
            health -= damage;
            animator.SetTrigger("isHurt");
            TimerDelg.Instance.Add(() => { isHurt = false; }, 1f);
        }
    }

    public bool Dead()
    {
        if (health <= 0)
        {
            rb.velocity = Vector2.zero;
            animator.SetTrigger("isDead");
            SoundManager.Instance.Play("PlayerDeath");
            isAlive = false;
            deathLoc = this.transform.position;
            TimerDelg.Instance.Add(() => this.gameObject.SetActive(false), 2);
            PlayerManager.Instance.IsDead();
            return true;
        }
        return false;
    }

    void GravityCheck()
    {
        if (!Grounded())
        {
            if (rb.velocity.y < maxGravity)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxGravity);
            }
        }
    }

    private void StartRewind()
    {
        this.GetComponent<SpriteRenderer>().material.color = Color.blue;
        isRewinding = true;


        rb.isKinematic = true;
    }

    private void StopRewind()
    {
        this.GetComponent<SpriteRenderer>().material.color = Color.white;
        isRewinding = false;
        rb.isKinematic = false;
    }
    private void Record()
    {

        if (pointsInTime.Count > Mathf.Round(3f / timeSlowMo.customFixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }
        else
        {
            pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
        }
    }
    private void Rewind()
    {
        SoundManager.Instance.Play("PlayerTimeRewind");
        if (pointsInTime.Count > 0)
        {

            PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
            transform.rotation = pointInTime.rotation;

            pointsInTime.RemoveAt(0);
        }
        else
        {
            StopRewind();
        }
    }

    public void DamageEnemies(int _damage)
    {
        RaycastHit2D hit = Physics2D.Raycast(punchesPos.position, transform.right, ATTACK_RANGE);
        Collider2D bossCol = Physics2D.OverlapCapsule(punchesPos.position, Vector2.one / 1.5f, CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Boss"));
        Collider2D enemyCol = Physics2D.OverlapCapsule(punchesPos.position, Vector2.one / 1.5f, CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Enemy"));
        if (bossCol)
        {
            bossCol.gameObject.GetComponent<BossUnit>().TakeDamage(damage);
        }
        if (enemyCol)
        {
            enemyCol.gameObject.GetComponent<EnemyUnit>().TakeDamage(damage);
        }
        if (hit.collider)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Throwable"))
            {
                if (timeSlow)
                    DeflectBullet(hit.collider.gameObject);
            }
        }
    }
}



