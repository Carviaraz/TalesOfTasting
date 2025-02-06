using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected Transform shootPos;
    [SerializeField] protected ItemWeapon itemWeapon;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected void PlayShootAnimation()
    {
        //animator.SetTrigger("Shoot");
    }

    public virtual void UseWeapon()
    {

    }
}
