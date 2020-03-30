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

    Transform hookShootPos;
    GameObject hookPrefab;
    [HideInInspector]
    public Hook hook;
    GameObject ropeLinePrefab;
    LineRenderer ropeLine;

    float climbSpeed = 2f;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        joint = GetComponent<SpringJoint2D>();
        joint.enabled = false;
        playerPosition = transform.position;
    }
    public void Start()
    {
        hookShootPos = transform.Find("Shoot").transform;
        hookPrefab = Resources.Load<GameObject>("Prefabs/Karan/RopeHook");
        ropeLinePrefab = Resources.Load<GameObject>("Prefabs/RopeLine");
        hook = GameObject.Instantiate<GameObject>(hookPrefab, hookShootPos.transform.position, Quaternion.identity).GetComponent<Hook>();
        ropeLine = GameObject.Instantiate<GameObject>(ropeLinePrefab, hookShootPos.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
        hook.Initialise();
        //hook.transform.SetParent(player.transform);
        //ropeLine.transform.SetParent(player.transform);
        hook.gameObject.SetActive(false);
        ropeLine.gameObject.SetActive(false);
    }
    void Update()
    {        
        HandleInput(player.angleDirection);

        if(isRopeAttached)
        HandleRopeLength();

        JointAttached();

        if (hook.gameObject.activeSelf)
        {
            GrappleCollisionCheck();
            ropeLine.SetPosition(0, hookShootPos.transform.position);
            ropeLine.SetPosition(1, hook.transform.position);
        }
    }

    private void HandleInput(Vector2 aimDirection)
    {
        if (Input.GetMouseButtonDown(0) && !isRopeAttached)
        {
            hook.transform.position = hookShootPos.transform.position;
            hook.gameObject.SetActive(true);
            ropeLine.gameObject.SetActive(true);
            hook.ThrowHook(player.angleDirection);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            hook.gameObject.SetActive(false);
            ropeLine.gameObject.SetActive(false);
            hook.hookRb.isKinematic = false;
            isRopeAttached = false;
        }
    }

    void GrappleCollisionCheck()
    {
        var grappleCheck = Physics2D.OverlapCircle(new Vector2(hook.transform.position.x - .1f, hook.transform.position.y - .1f), .05f, LayerMask.GetMask("Grappleable"));
        if (grappleCheck)
        {
            hook.hookRb.velocity = Vector2.zero;
            hook.hookRb.isKinematic = true;
            isRopeAttached = true;
        }
        if ((hook.transform.position - player.transform.position).sqrMagnitude >= 80)
        {
            hook.gameObject.SetActive(false);
            ropeLine.gameObject.SetActive(false);
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
/*            Debug.Log(joint.distance);*/
            
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
        if (Input.GetAxis("Vertical") >= 1f/* && isRopeAttached*/)
        {
            joint.distance -= Time.deltaTime * climbSpeed;
        }
        else if (Input.GetAxis("Vertical") < 0f/* && isRopeAttached*/)
        {
            joint.distance += Time.deltaTime * climbSpeed;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector2(hook.transform.position.x - hookShootPos.transform.position.x - .5f, hook.transform.position.y - hookShootPos.transform.position.y - .5f), .05f);
    }
}

