using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerForBosses : MonoBehaviour
{
    int damage;

    private void Start()
    {
        damage = GetComponentInParent<BossUnit>().damage;
    }
    
}
