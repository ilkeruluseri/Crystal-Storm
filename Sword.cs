using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] int damage = 3;
    AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.Play("swordSwipe");
        //transform.Rotate(0f, 0f, 45f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy hitEnemy = collision.gameObject.GetComponent<Enemy>();
        if (hitEnemy != null)
        {
            audioManager.Play("swordHit");
            hitEnemy.TakeHit(damage);
        }
    }
}
