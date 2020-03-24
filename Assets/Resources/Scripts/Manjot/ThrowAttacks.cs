using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAttacks : MonoBehaviour
{
    Transform target;
    Rigidbody2D rb;
    Animator anim;
    public float throwSpeed;
    public float angle2;
    GameObject fireBallFX;
    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        fireBallFX = transform.GetChild(0).gameObject;
        Destroy(gameObject, 5f);
    }

    public void Throw()
    {
        anim.SetTrigger("throw");
        fireBallFX.gameObject.SetActive(true);
        Vector2 dir2 = (target.position - transform.position).normalized;
        var angle2 = Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle2, transform.forward);
        rb.AddForce(dir2 * throwSpeed, ForceMode2D.Impulse);
    }
}
