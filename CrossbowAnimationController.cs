using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] Crossbow crossbow;
    AudioManager audioManager;
    private bool isAttacking;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

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

    public void DrawBowSound()
    {
        audioManager.Play("drawBow");
    }

    public void PlayClick()
    {
        audioManager.Play("bowClick");
    }
}
