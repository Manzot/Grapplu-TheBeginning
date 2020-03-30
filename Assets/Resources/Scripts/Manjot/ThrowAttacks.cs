using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAttacks : ThrowAbles
{
    GameObject fireBallFX;
    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        fireBallFX = transform.GetChild(0).gameObject;
        Vector2 dir = (target.position - transform.position).normalized;
        var angle2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle2, Vector3.forward);
        Destroy(gameObject, 5f);
    }

    public void Throw()
    {
        anim.SetTrigger("throw");
        fireBallFX.gameObject.SetActive(true);
        transform.parent = null;
       
        rb.AddForce(transform.right * throwSpeed, ForceMode2D.Impulse);

        //if (fireBall.hit)
        //{
        //    fireBall.blast();
        //}
    }
}
