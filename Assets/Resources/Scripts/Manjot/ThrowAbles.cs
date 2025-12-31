using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAbles : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Animator anim;
    public float throwSpeed;
}
