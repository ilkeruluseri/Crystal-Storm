using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    private GameObject crossbow;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 30f;
    [SerializeField] private int bulletDamage = 3;

    private Transform target;
    private Vector3 crossbowPos;

    private void Start()
    {
        crossbow = GameObject.FindGameObjectWithTag("Crossbow");
        crossbowPos = crossbow.GetComponent<Transform>().position;
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    private void FixedUpdate()
    {
        if (!target) return;

        Vector2 direction = (target.position - transform.position).normalized;

        rb.velocity = direction * bulletSpeed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle - 135f));
    }

    private void Update()
    {
        if (Vector2.Distance(crossbowPos, gameObject.transform.position) >= 200f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Damage enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeHit(bulletDamage);
        }
        Destroy(gameObject);
    }

    public void SetDamage(int _damage)
    {
        bulletDamage = _damage;
    }

}
