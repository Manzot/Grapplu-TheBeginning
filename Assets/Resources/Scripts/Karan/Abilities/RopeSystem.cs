using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RopeSystem : MonoBehaviour
{

    public SpringJoint2D joint;
    public PlayerController player;
    public bool isRopeAttached = false;
    private Vector2 playerPosition;

    private List<Vector2> ropePositions = new List<Vector2>();
    public GameObject hookShoot;


    GameObject hookPrefab;
    public Hook hook;

    private float climbSpeed = 5f;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        joint = GetComponent<SpringJoint2D>();
        joint.enabled = false;
        playerPosition = transform.position;

    }
    public void Start()
    {
        hookShoot = GameObject.Find("Shoot");
        hookPrefab = Resources.Load<GameObject>("Prefabs/Karan/RopeHook");
        hook = GameObject.Instantiate<GameObject>(hookPrefab, hookShoot.transform.position, Quaternion.identity).GetComponent<Hook>();
        hook.Initialise();
        hook.gameObject.SetActive(false);
    }
    void Update()
    {
        HandleInput(player.angleDirection);
        HandleRopeLength();
        JointAttached();

        if (hook.gameObject.activeSelf)
        {
            RaycastHit2D hit = Physics2D.Raycast(hook.transform.position, player.angleDirection, 0.2f);
            if (hit)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grappleable"))
                {
                    hook.hookRb.velocity = Vector2.zero;
                    //hook.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, .5f, 0);
                    hook.hookRb.isKinematic = true;
                    isRopeAttached = true;
                }
            }
            if ((hook.transform.position - player.transform.position).sqrMagnitude >= 80)
            {
                hook.gameObject.SetActive(false);
                isRopeAttached = false;
            }
        }
    }

    private void HandleInput(Vector2 aimDirection)
    {
        if (Input.GetMouseButtonDown(0) && !isRopeAttached)
        {
            hook.transform.position = hookShoot.transform.position;
            hook.gameObject.SetActive(true);
            hook.ThrowHook(player.angleDirection);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            hook.gameObject.SetActive(false);
            hook.hookRb.isKinematic = false;
            isRopeAttached = false;
        }
    }

    void JointAttached()
    {
        if (isRopeAttached && hook)
        {
            player.isSwinging = true;
            player.ropeHook = hook.gameObject.transform.position;
            joint.enabled = true;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = hook.transform.position;
            joint.distance = Vector2.Distance(player.transform.position, hook.transform.position);
        }
        else
        {
            player.SetCrosshairPoint(player.aimAngle);
            player.isSwinging = false;
            joint.enabled = false;
        }

    }
    private void HandleRopeLength()
    {
        if (Input.GetAxis("Vertical") >= 1f && isRopeAttached)
        {
            joint.distance -= Time.deltaTime * climbSpeed;
        }
        else if (Input.GetAxis("Vertical") < 0f && isRopeAttached)
        {
            joint.distance += Time.deltaTime * climbSpeed;
        }
    }
}

