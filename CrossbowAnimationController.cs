using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] Crossbow crossbow;
    private bool isAttacking;

    private void Update()
    {
        animator.SetBool("isAttacking", isAttacking);
    }

    public void Shoot()
    {
        crossbow.ShootCrossBow();
    }

    public void setAttacking(bool value)
    {
        isAttacking = value;
    }
}
