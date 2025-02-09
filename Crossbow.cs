using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Crossbow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private GameObject crossbow;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private CrossbowAnimationController animationController;
    private AudioManager audioManager;

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private int damage = 3;

    private Transform target;
    private float timeUntilFire;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
        }
        RotateTowardsTarget();
        if (!CheckTargetIsInRange())
        {
            target = null;
            animationController.setAttacking(false);
        }
        else
        {
            animationController.setAttacking(true);
            /*
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / (bps))
            {
                Shoot();
                timeUntilFire = 0;
            }
            */
        }
    }

    private void RotateTowardsTarget()
    {
        if (target != null)
        {
            float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x -
            transform.position.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle - 135f));
            turretRotationPoint.rotation = Quaternion.RotateTowards(
                turretRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Band-aid solution to weird offset issue
            
        }

    }

    private void Shoot()
    {
        if (target == null) { return; }
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
        BulletScript bulletScript = bulletObj.GetComponent<BulletScript>();
        bulletScript.SetTarget(target);
        bulletScript.SetDamage(damage);
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange,
            (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
    }

    private bool CheckTargetIsInRange()
    {
        if (target != null)
        {
            return Vector2.Distance(target.position, transform.position) <= targetingRange;
        }
        return false;
    }

    public void ShootCrossBow()
    {
        audioManager.Play("fireBow");
        Shoot();
    }

    /*
    private void OnDrawGizmosSelected() // To see range in editor
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
    */
}
