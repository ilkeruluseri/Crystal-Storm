using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private BarScript energyBar;
    [SerializeField] private BarScript healthBar;
    AudioManager audioManager;
    [SerializeField] ParticleSystem dustParticles;
    [SerializeField] TrailRenderer runningTrail;
    [SerializeField] GameObject sword;
    [SerializeField] Transform swordRotationPoint;
    [SerializeField] StormManager stormManager;
    [SerializeField] SceneChanger sceneChanger;

    [Header("Sound")]
    [SerializeField] private TileBase[] grassTiles;
    [SerializeField] private TileBase[] floorTiles;
    [SerializeField] private List<AudioClip> grassSounds;
    [SerializeField] private List<AudioClip> floorSounds;

    enum FootstepMaterial
    {
        Grass, Floor, Empty
    }

    [Header("Attributes")]
    public float moveSpeed = 50f;
    [SerializeField] float walkSpeed = 30f;
    [SerializeField] float runSpeed = 80f;
    [SerializeField] int health;
    [SerializeField] public readonly int MAX_HEALTH = 100;
    [SerializeField] int energy;
    [SerializeField] public readonly int MAX_ENERGY = 200;

    Vector2 movement;
    private AudioSource footstepSource;

    // Timer variables for speed boost
    private float moveTimer = 0f;
    private const float moveTimeThreshold = 3f;

    private bool swordAttacking = false;

    void Start()
    {
        health = MAX_HEALTH;
        energy = MAX_ENERGY;
        energyBar.SetMaxValue(MAX_ENERGY);
        energyBar.SetValue(energy);
        healthBar.SetMaxValue(MAX_HEALTH);
        healthBar.SetValue(health);
        audioManager = FindObjectOfType<AudioManager>();
        footstepSource = GetComponent<AudioSource>();
    }

    bool lookingRight = false;
    void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Animation
        if (movement != Vector2.zero)
        {
            animator.SetBool("isMoving", true);
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveTimeThreshold && moveSpeed != runSpeed)
            {
                moveSpeed = runSpeed;
                dustParticles.Play();
                runningTrail.emitting = true;
                animator.speed = 1.5f;
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
            moveTimer = 0f;
            runningTrail.emitting = false;
            moveSpeed = walkSpeed;
            animator.speed = 1f;
        }

        // Flip the character based on horizontal movement
        if (movement.x > 0 && !lookingRight)
        {
            // Moving right, face right
            dustParticles.Play();
            lookingRight = true;
        }
        else if (movement.x < 0 && lookingRight)
        {
            // Moving left, face left
            dustParticles.Play();
            lookingRight = false;

        }

        if (lookingRight)
        {
            //transform.eulerAngles = new Vector3(0f, 180f, 0f);
            spriteRenderer.flipX = true;
        }
        else
        {
            transform.eulerAngles = new Vector3(0f, 0, 0f);
            spriteRenderer.flipX = false;
        }

        if (stormManager.IsStormActive() && Input.GetMouseButtonDown(0) && !swordAttacking)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                if (!hit.collider.TryGetComponent<ResourceScript>(out ResourceScript x))
                {
                    SwordAttack();
                }
            }
            else
            {
                SwordAttack();
            }
        }

    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        Vector3 newPos = new Vector3(rb.position.x, rb.position.y);
        gameObject.GetComponent<Transform>().SetPositionAndRotation(newPos, Quaternion.identity);
    }

    public void PlayFootStep()
    {
        AudioClip clip = null;
        FootstepMaterial surface = DetectSurface();
        switch (surface)
        {
            case FootstepMaterial.Grass:
                clip = grassSounds[Random.Range(0, grassSounds.Count)];
                break;
            case FootstepMaterial.Floor:
                clip = floorSounds[Random.Range(0, floorSounds.Count)];
                break;
            default:
                break;
        }

        if (surface != FootstepMaterial.Empty)
        {
            footstepSource.clip = clip;
            footstepSource.volume = Random.Range(0.3f, 0.35f);
            footstepSource.pitch = Random.Range(0.9f, 1.1f);

            footstepSource.Play();
        }

    }

    private bool ExistsInArray<T>(T[] array, T itemToLookFor)
    {
        if (array == null)
        {
            return false;
        }
        foreach (T item in array)
        {
            if (item.Equals(itemToLookFor))
            {
                return true;
            }
        }
        return false;
    }

    private FootstepMaterial DetectSurface()
    {
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
        TileBase tileUnderPlayer = tilemap.GetTile(cellPosition);
        if (tileUnderPlayer != null)
        {
            if (ExistsInArray<TileBase>(grassTiles, tileUnderPlayer))
            {
                return FootstepMaterial.Grass;
            }
            else if (ExistsInArray<TileBase>(floorTiles, tileUnderPlayer))
            {
                return FootstepMaterial.Floor;
            }
            else
            {
                return FootstepMaterial.Empty;
            }

        }
        else
        {
            Debug.Log("No tile found under the player.");
            return FootstepMaterial.Empty;
        }
    }

    public void SetPlayerEnergy(int value)
    {
        energy = value;
        energyBar.SetValue(value);
    }

    public int GetPlayerEnergy()
    {
        return energy;
    }

    public void SetPlayerHealth(int value)
    {
        health = value;
        healthBar.SetValue(value);
        if (health > MAX_HEALTH) { health = MAX_HEALTH; }
        if (health <= 0)
        {
            sceneChanger.LoadLose();
            //Kill player, you died screen, restart day?
        }
    }

    public int GetPlayerHealth()
    {
        return health;
    }

    private void SwordAttack()
    {
        swordRotationPoint.transform.eulerAngles = new Vector3(0, 0, 135);
        Vector3 spawnPos;
        Quaternion spawnRotation;
        if (lookingRight)
        {
            // Player is facing right, adjust position and rotation
            spawnPos = swordRotationPoint.transform.position + new Vector3(6f, 6f); // Adjusted for right-facing direction
            spawnRotation = Quaternion.Euler(0f, 180f, 90f); // Reverse the angle to match the right-facing direction
        }
        else
        {
            // Player is facing left, default values
            spawnPos = swordRotationPoint.transform.position + new Vector3(-6f, 6f);
            spawnRotation = Quaternion.Euler(0f, 0f, 90f);
        }
        GameObject instantiatedSword = Instantiate(sword, spawnPos, spawnRotation, swordRotationPoint);
        StartCoroutine(SmoothRotateSword(instantiatedSword, 90, 0.2f));
    }
    private IEnumerator SmoothRotateSword(GameObject swordObject, float angle, float duration)
    {
        swordAttacking = true;

        // Store the initial rotation of the swordRotationPoint
        Quaternion startRotation = swordRotationPoint.transform.localRotation;
        // Calculate the target rotation
        Quaternion endRotation;
        if (lookingRight)
        {
            endRotation = startRotation * Quaternion.Euler(0, 0, -angle);
        }
        else
        {
            endRotation = startRotation * Quaternion.Euler(0, 0, angle);
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Interpolate between start and end rotation
            swordRotationPoint.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        // Ensure the final rotation is exactly the target rotation
        swordRotationPoint.transform.localRotation = endRotation;

        Destroy(swordObject);

        swordAttacking = false;
    }
}
