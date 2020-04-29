using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RopeSystem : MonoBehaviour
{
    const float ROPE_LENGTH = 35;

    public SpringJoint2D joint;
    public PlayerController player;
    public static bool isRopeAttached = false;
    private Vector2 playerPosition;

    Transform hookShootPos;
    GameObject hookPrefab;
    [HideInInspector]
    public Hook hook;
    GameObject ropeLinePrefab;
    LineRenderer ropeLine;
    Rigidbody2D rb;

    public static bool isClimbing;
    public static bool isThrowingHook;

    float climbSpeed = 4.5f;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        joint = GetComponent<SpringJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        joint.enabled = false;
        playerPosition = transform.position;
    }
    public void Start()
    {
        hookShootPos = transform.Find("Shoot").transform;

        hook = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Karan/RopeHook"), hookShootPos.transform.position, Quaternion.identity).GetComponent<Hook>();
        ropeLine = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Karan/RopeLine"), hookShootPos.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
        hook.Initialise();
        //hook.transform.SetParent(player.transform);
        //ropeLine.transform.SetParent(player.transform);
        hook.gameObject.SetActive(false);
        ropeLine.gameObject.SetActive(false);
    }
    void Update()
    {

        if (player.health > 0)
        {
            HandleInput(player.angleDirection);

            JointAttached();

            if (hook.gameObject.activeSelf)
            {
                GrappleCollisionCheck();
                if (isRopeAttached)
                {
                    HandleRopeLength();
                    player.animator.SetBool("is_grappling", true);

                    player.animator.SetBool("throwRope", false);

                    if (joint.distance >= 5.8f)
                        joint.distance = 5.8f;
                }
                else
                {
                    player.animator.SetBool("is_grappling", false);
                }

                ropeLine.SetPosition(0, hookShootPos.transform.position);
                ropeLine.SetPosition(1, hook.transform.position);

                isThrowingHook = true;

            }
            else
                isThrowingHook = false;
        }
        
    }

    private void HandleInput(Vector2 aimDirection)
    {
        if (Input.GetButtonDown("Grapple") && !isRopeAttached)
        {
            hook.transform.position = hookShootPos.transform.position;
            hook.gameObject.SetActive(true);
            ropeLine.gameObject.SetActive(true);
            player.animator.SetBool("throwRope", true);
            hook.ThrowHook(player.angleDirection);
        }
        else if (Input.GetButtonUp("Grapple") || player.health <= 0)
        {
            hook.gameObject.SetActive(false);
            ropeLine.gameObject.SetActive(false);
            hook.hookRb.isKinematic = false;
            player.animator.SetBool("is_grappling", false);
            player.animator.SetBool("throwRope", false);
            hook.transform.SetParent(null);
            if (isRopeAttached && !player.Grounded())
                rb.AddForce(new Vector2(player.horizontal, 2f) * 3f, ForceMode2D.Impulse);
            if (isRopeAttached)
                rb.AddForce(new Vector2(player.horizontal, 1f) * 1f, ForceMode2D.Impulse);

            isRopeAttached = false;

        }
    }

    void GrappleCollisionCheck()
    {
        var grappleCheck = Physics2D.OverlapCircle(new Vector2(hook.transform.position.x - .1f, hook.transform.position.y - .1f), .05f, LayerMask.GetMask("Grappleable", "Ground", "Platform"));
        if (grappleCheck)
        {
            hook.hookRb.velocity = Vector2.zero;
            hook.hookRb.isKinematic = true;
            isRopeAttached = true;
            hook.transform.SetParent(grappleCheck.transform);
        }
        if ((hook.transform.position - player.transform.position).sqrMagnitude >= ROPE_LENGTH)
        {
            hook.gameObject.SetActive(false);
            ropeLine.gameObject.SetActive(false);
            isRopeAttached = false;
            player.animator.SetBool("throwRope", false);
        }
    }

    void JointAttached()
    {
        if (isRopeAttached && hook)
        {
            if (joint.gameObject.activeSelf)
            {
                player.isSwinging = true;
                player.ropeHook = hook.gameObject.transform.position;
                joint.enabled = true;

                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = hook.transform.position;
            }
        }
        else
        {
            player.SetCrossairPoint(player.aimAngle);
            player.isSwinging = false;
            joint.enabled = false;
        }

    }
    private void HandleRopeLength()
    {

        if (Input.GetKey(KeyCode.W)/* && isRopeAttached*/)
        {
            if (joint.distance > 1.5f)
            {
                joint.distance -= climbSpeed * Time.deltaTime;
                isClimbing = true;
            }
        }
        else if (Input.GetKey(KeyCode.S )/* && isRopeAttached*/)
        {
            if (joint.distance <= 5.8f - 0.5f && !player.Grounded())
            {
                joint.distance += climbSpeed * Time.deltaTime;
                isClimbing = true;
            }
        }
        else
        {
            isClimbing = false;
        }
    }

    public void RopeDisattached()
    {
        hook.gameObject.SetActive(false);
        ropeLine.gameObject.SetActive(false);
        hook.hookRb.isKinematic = false;
        player.animator.SetBool("is_grappling", false);
        player.animator.SetBool("throwRope", false);
        hook.transform.SetParent(null);
        if (isRopeAttached && !player.Grounded())
            rb.AddForce(new Vector2(player.horizontal, 2f) * 3f, ForceMode2D.Impulse);
        if (isRopeAttached)
            rb.AddForce(new Vector2(player.horizontal, 1f) * 1f, ForceMode2D.Impulse);

        isRopeAttached = false;
        joint.enabled = false;

        player.animator.SetBool("is_dead", true);
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(new Vector2(hook.transform.position.x - hookShootPos.transform.position.x - .5f, hook.transform.position.y - hookShootPos.transform.position.y - .5f), .05f);
    //}
}

