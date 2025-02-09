using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaScript : MonoBehaviour
{
    [SerializeField] ParticleSystem electricBolt;
    [SerializeField] private LayerMask enemyMask;
    private AudioManager audioManager;

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 80f;
    [SerializeField] private int damage = 5;
    [SerializeField] private float bps = 2f; // bullets per second

    private Transform[] targets;
    private float timeUntilFire = 0;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        timeUntilFire += Time.deltaTime;
        if (timeUntilFire > (1f / bps))
        {
            Attack();
            timeUntilFire = 0;
        }
    }

    private void FindTargets()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange,
            Vector2.zero, 0f, enemyMask); // Vector2.zero since no direction is needed

        targets = new Transform[hits.Length];

        for (int i = 0; i < hits.Length; i++)
        {
            targets[i] = hits[i].transform;
        }
    }

    private void Attack()
    {
        FindTargets();  // Call to get the updated targets within range

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                // Play the electric bolt effect
                electricBolt.Play();
                audioManager.Play("teslaShock");

                // Deal damage to the target
                Enemy enemy = targets[i].GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeHit(damage);
                }
            }
        }
    }
}
