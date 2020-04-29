using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum Abilities { Grappler, Rewind, SlowMotion }

public class PlayerController : MonoBehaviour, IDamage
{
    const float SLOMO_FACTOR = 0.3f;
    const float ATTACK_RANGE = .6f;
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

    UnityEngine.UI.Image healthBar;

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
    [HideInInspector]
    public Animator animator;

    public Vector2 angleDirection;
    public Vector2 savePoint;
    public Vector3 deathLoc;

    public GameObject crosshair;
    [HideInInspector]
    public TimeSlowMo timeSlowMo;

    Transform punchesPos;

    public float health = 100f;
    private const float MAX_HEALTH = 100;

    public const float slowMoCooldown = 10f;
    public const float rewindCooldown = 6f;
    float slowMoTimer;
    bool isCoolingDown = false;
    private float rewindTimer;
    public int currentSceneIndex;



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
        healthBar = GameObject.FindGameObjectWithTag("PlayerHealthUI").GetComponent<UnityEngine.UI.Image>();
        healthBar.fillAmount = health / MAX_HEALTH;

    }
    public void PostInitialize()
    {
        pointsInTime = new List<PointInTime>();
        punchesPos = transform.Find("Punches").transform;
    }


    public void Refresh()
    {

        if (Input.GetKey(KeyCode.Q))
        {
            rb.gravityScale = 0;
            rb.isKinematic = true;
            rb.velocity = new Vector2(horizontal, Input.GetAxis("Vertical") * 2);
        }
        else
        {
            rb.gravityScale = 3;
            rb.isKinematic = false;

        }

        //Debug.Log(health);
        if (!Dead())
        {
            Jump();
            SetCrossairPoint(CrossairDirection());

            Timers();
            if (Input.GetButtonDown("TimeSlow"))
            {

                if (!timeSlow)
                {

                    if (slowMoTimer <= 0)
                    {
                        SoundManager.Instance.Play("PlayerTimeSlow");
                        TimeSlowAbility();
                        slowMoTimer = slowMoCooldown;
                    }

                }
            }
            TimeSlowReset();
        }
        Attack();

        if (Input.GetButtonDown("Attack"))
        {
            isAttacking = true;
        }
        if (!isCoolingDown)
        {
            if (Input.GetButtonDown("TimeRewind"))
            {

                if (!timeSlow)
                {
                    if (rewindTimer <= 0)
                    {
                        StartRewind();
                        rewindTimer = rewindCooldown;
                    }
                }
            }

            if (Input.GetButtonUp("TimeRewind"))
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
            GravityCheck();

            if (!isAttacking)
                Movement();

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

    private void Timers()
    {
        slowMoTimer -= Time.deltaTime;
        rewindTimer -= Time.deltaTime;
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
        ThrowAttacks throwObject = go.GetComponent<ThrowAttacks>();
        // throwObject.rb.velocity = Vector2.zero;
        throwObject.rb.AddForce((throwObject.transform.position - transform.position) * throwObject.throwSpeed / 1.5f, ForceMode2D.Impulse);
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
        //Debug.DrawLine(transform.position, playerToHookDirection, Color.red, 0f);

        if (Input.GetAxis("Horizontal") < 0)
        {
            // sprite.flipX = true;
            perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
            var leftPerpPos = (Vector2)transform.position + perpendicularDirection;
            //  Debug.DrawLine(transform.position, leftPerpPos, Color.green, 0f);
        }
        else
        {
            // sprite.flipX = false;
            perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
            var rightPerpPos = (Vector2)transform.position - perpendicularDirection;
            // Debug.DrawLine(transform.position, rightPerpPos, Color.green, 0f);
        }

        return perpendicularDirection;
    }

    /* Applying the force towards the perpendicular direction */
    public void SwingDirectionForce()
    {
        if (rb.velocity.x < 0f || rb.velocity.x > 0f)
        {
           // if (isSwinging)
            {
                Vector2 perpendicularDirection = CalculatePerpendicularDirection();

                var force = perpendicularDirection * swingForce;
                rb.AddForce(force);
            }
        }
    }

    /*Movement of the player left and right also checking the direction mouse cursor and rotating the player towards it*/
    public void Movement()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        horizontal = Input.GetAxis("Horizontal");
        float direction = CrossairDirection();
        float angle = CrossairDirection() * Mathf.Rad2Deg;

        {
            if (!isSwinging || Grounded())
            {
                if (horizontal > 0)
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                else if (horizontal < 0)
                    transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else if(!RopeSystem.isClimbing)
            {
                if (rb.velocity.x > 0)
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                else if (rb.velocity.x < 0)
                    transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
        }

        if (!isSwinging && !RopeSystem.isThrowingHook)
        {
            if (!timeSlow)
            {
                rb.velocity = new Vector2(horizontal * speed   /*timeSlowMo.customFixedUnscaledDeltaTime*/, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(horizontal * speed   /*timeSlowMo.customFixedUnscaledDeltaTime*/, rb.velocity.y); ;
            }
        }
        else if (!isSwinging)
        {
            if (Mathf.Abs(angle) < 90)
            {
                //  sprite.flipX = false;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (Mathf.Abs(angle) > 90)
            {
                // sprite.flipX = true;
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
        }

    }
    /* Player Jump */
    public void Jump()
    {
        if (Grounded() && !RopeSystem.isRopeAttached)
        {
            animator.SetBool("isJumping", false);
            if (Input.GetButtonDown("Jump"))
            {
                if (!isJumping)
                {
                    if (!timeSlow)
                        rb.AddForce(new Vector2(rb.velocity.x, jumpForce /** timeSlowMo.customFixedUnscaledDeltaTime*/), ForceMode2D.Impulse);
                    else
                        rb.AddForce(new Vector2(rb.velocity.x, jumpForce /** timeSlowMo.customFixedUnscaledDeltaTime*/), ForceMode2D.Impulse);

                    isJumping = true;
                    SoundManager.Instance.Play("PlayerJump");
                    TimerDelg.Instance.Add(() => { isJumping = false; }, .5f);
                }
            }
        }
        else
        {
            animator.SetBool("isJumping", true);
        }
    }
    /* Checking for ground collision */
    public bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(feet.position, 0.3f, LayerMask.GetMask("Ground", "IObject", "Platform"));
    }
    /* getting a direction and angle between User mouse and User */
    float CrossairDirection()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        Vector3 faceDirection = worldMousePosition - transform.position;

        aimAngle = Mathf.Atan2(faceDirection.y, faceDirection.x);

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
    /* Attacking Animation of the player */
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
    /* Player Damage and triggering the hurt animation */
    public void TakeDamage(int damage)
    {
        if (!isHurt)
        {
            isHurt = true;
            SoundManager.Instance.Play("PlayerHurt");
            health -= damage;
            sprite.material.color = Color.red;
            //animator.SetTrigger("isHurt");
            TimerDelg.Instance.Add(() => { sprite.material.color = Color.white; }, 0.5f);
            TimerDelg.Instance.Add(() => { isHurt = false; }, 0.8f);
        }
        healthBar.fillAmount = health / MAX_HEALTH;
    }

    /* Checking if the player is dead or not, Playing the animation if dead and Setting the timeScale to normal if player dies during slowMo*/
    public bool Dead()
    {
        if (health <= 0)
        {
            //animator.SetTrigger("isDead");
            isSwinging = false;
            GetComponent<RopeSystem>().RopeDisattached();
            animator.SetBool("isJumping", false);
            animator.SetBool("is_dead", true);
            rb.velocity = new Vector2(rb.velocity.x / 4, rb.velocity.y);
            if (timeSlow)
            {
                timeSlowMo.InstantResetTime();
            }
            //rb.velocity = Vector2.zero;
            SoundManager.Instance.Play("PlayerDeath");
            isAlive = false;
            TimerDelg.Instance.Add(() => this.gameObject.SetActive(false), 1.5f);

            PlayerManager.Instance.IsDead();
            return true;
        }
        else
        {
            animator.SetBool("is_dead", false);
        }
        return false;
    }
    /* Using Gravity Check */
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
    /* Recording the position in List<PointsInTime> where PointsInTime is the class, to rewind the player for 3 seconds */
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
    /* Rewind The player */
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

    /* Damaging the Enemies using Colliders */
    public void DamageEnemies(int _damage)
    {
        Collider2D bossCol = Physics2D.OverlapCapsule(punchesPos.position, Vector2.one / 1.5f, CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Boss"));
        Collider2D enemyCol = Physics2D.OverlapCapsule(punchesPos.position, Vector2.one / 1.5f, CapsuleDirection2D.Horizontal, 0, LayerMask.GetMask("Enemy", "FlyingEnemy"));
        Collider2D deflectCol = Physics2D.OverlapCircle(punchesPos.position, 0.35f, LayerMask.GetMask("Throwable"));


        if (bossCol)
        {
            bossCol.gameObject.GetComponent<BossUnit>().TakeDamage(damage);
        }
        if (enemyCol)
        {
            enemyCol.gameObject.GetComponent<EnemyUnit>().TakeDamage(damage);
        }
        if (deflectCol)
        {
            if (timeSlow)
            {
                DeflectBullet(deflectCol.gameObject);
            }
        }
    }

    public void PlaySound(string soundName)
    {
        if (soundName == "jump")
            SoundManager.Instance.Play("PlayerJump");
    }


}




