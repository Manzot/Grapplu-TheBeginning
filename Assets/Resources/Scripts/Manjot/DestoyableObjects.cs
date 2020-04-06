using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyableObjects : MonoBehaviour
{
    Rigidbody2D rb;
    public float fallAfter;
    public float destroyAfter;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Hook"))
        {
            TimerDelg.Instance.Add(()=> {
                rb.isKinematic = false;
                Destroy(gameObject, destroyAfter);
            }, fallAfter);
            
        }
    }
}
