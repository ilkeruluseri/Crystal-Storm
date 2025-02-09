using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Attributes")]
    public float moveSpeed = 5f;
    [SerializeField] int health;
    [SerializeField] const int MAX_HEALTH = 4;

    Vector2 movement;

    void Start()
    {
    }

    void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Animation
        if (movement != Vector2.zero)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        // Flip the character based on horizontal movement
        if (movement.x > 0)
        {
            // Moving right, face right
            spriteRenderer.flipX = true;
        }
        else if (movement.x < 0)
        {
            // Moving left, face left
            spriteRenderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        Vector3 newPos = new Vector3(rb.position.x, rb.position.y);
        gameObject.GetComponent<Transform>().SetPositionAndRotation(newPos, Quaternion.identity);
    }
}
