using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    public float speed;

    public float jumpForce;
    public Transform feet;
    Collider2D groundCheckColi;
    int jumpCount = 1;

    float horizontal;
    int flipX = 0;

    Animator anim;
    SpriteRenderer sprite;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("xFloat", Mathf.Abs(rb.velocity.x));
        horizontal = Input.GetAxis("Horizontal");
        if (horizontal > 0) flipX = 1;
        else if (horizontal < 0) flipX = -1;

        if (flipX > 0) sprite.flipX = false;
        else sprite.flipX = true;

        rb.velocity = new Vector2(horizontal * speed * Time.deltaTime, rb.velocity.y);

        if (Grounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime) , ForceMode2D.Impulse);
                jumpCount--;
            }
            if (jumpCount < 1)
                jumpCount = 1;
        }
        else
        {
            if (jumpCount > 0 && Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(new Vector2(rb.velocity.x, jumpForce / 1.4f * Time.fixedDeltaTime) , ForceMode2D.Impulse);
                jumpCount--;
            }
        }
    }

    public bool Grounded()
    {
        return groundCheckColi = Physics2D.OverlapCircle(feet.position, 0.1f, LayerMask.GetMask("Ground", "IObject"));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(feet.position, .1f);
    }
}
