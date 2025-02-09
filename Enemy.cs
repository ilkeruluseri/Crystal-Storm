using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [SerializeField] public int MAX_HEALTH = 10;
    [SerializeField] private int health = 10;
    [SerializeField] public float moveSpeed = 30f;
    [SerializeField] public int attackDamage = 5;
    [SerializeField] public Animator animator;
    

    [SerializeField] float desiredDistanceFromTarget = 1f;
    [SerializeField] ParticleSystem myParticleSystem;
    [SerializeField] string particleSFX;
    [SerializeField] ParticleSystem hitEffect;
    
    private Transform target;
    [HideInInspector] public List<GameObject> objectsInRange = new List<GameObject>();
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isMoving = false;
    private StormManager stormManager;
    private int aliveIndex;
    AudioManager audioManager;
    SpriteRenderer spriteRenderer;


    private void Start()
    {
        health = MAX_HEALTH;
        spriteRenderer = GetComponent<SpriteRenderer>();
        stormManager = (StormManager)FindObjectOfType(typeof(StormManager));
        audioManager = FindObjectOfType<AudioManager>();
        SpawnEffect();
        stormManager.AddtoAliveList(gameObject);
        audioManager.Play("thunder");
    }

    private void Update()
    {
        SetTarget();
        if (target != null)
        {
            CalculateDesiredDistance();
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget > desiredDistanceFromTarget)
            {
                isMoving = true;
                MoveTowardsTarget();
            }
            else
            {
                isMoving = false;
            }
        }
        
        animator.SetBool("isMoving", isMoving);
        Vector2 movement = Vector2.zero;
        if (target != null)
        {
            movement = (target.position - transform.position).normalized;
        }
        // Flip the character based on horizontal movement
        if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    public void AddObjectInRange(GameObject obj)
    {
        objectsInRange.Add(obj);
    }

    public void RemoveObjectInRange(GameObject obj)
    {
        objectsInRange.Remove(obj);
    }

    private Transform FindNearestBuilding()
    {
        BuildingScript[] buildings = FindObjectsByType<BuildingScript>(FindObjectsSortMode.None);
        float nearestDistance = Mathf.Infinity;
        Transform nearestBuilding = null;

        foreach(BuildingScript building in buildings)
        {
            float distanceToBuilding = Vector2.Distance(transform.position, building.transform.position);
            if (distanceToBuilding < nearestDistance)
            {
                nearestDistance = distanceToBuilding;
                nearestBuilding = building.transform;
            }
        }

        return nearestBuilding;
    }

    private void SetTarget()
    {
        Player player = FindObjectOfType<Player>();
        BaseBuilding baseBuilding = FindObjectOfType<BaseBuilding>();
        Transform nearestBuilding = FindNearestBuilding();
        float distanceToNearestBuilding;
        if (nearestBuilding == null)
        {
            distanceToNearestBuilding = Mathf.Infinity;
        } else
        {
            distanceToNearestBuilding = Vector2.Distance(transform.position, nearestBuilding.transform.position);
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        float distanceToBase = Vector2.Distance(transform.position, baseBuilding.transform.position);

        float closestDistance = Mathf.Min(distanceToPlayer, distanceToBase, distanceToNearestBuilding);
        if (closestDistance == distanceToBase)
        {
            target = baseBuilding.transform;
        }
        else if (closestDistance == distanceToNearestBuilding)
        {
            target = nearestBuilding.transform;
        }
        else if (closestDistance == distanceToPlayer)
        {
            target = player.transform;
        }

    }

    private void CalculateDesiredDistance()
    {
        if (target != null) { return; }
        if (target.TryGetComponent<BaseBuilding>(out BaseBuilding baseBuilding))
        {
            // Currently for square, alter for rectangular shape
            desiredDistanceFromTarget = 1f + baseBuilding.GetSize().x * 5f;
        }
        else if (target.TryGetComponent<BuildingScript>(out BuildingScript building))
        {
            desiredDistanceFromTarget = 1f + building.GetSize().x * 5f;
        }
        else if (target.TryGetComponent<Player>(out Player player))
        {
            desiredDistanceFromTarget = 1f;
        }
    }

    private void MoveTowardsTarget()
    {
        isAttacking = false;
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    public void ActivateParticle(bool withSound)
    {
        myParticleSystem.Play();
        if (withSound)
        {
            audioManager.Play(particleSFX);
        }
        
    }

    public int GetHealth()
    {
        return health;
    }

    public void TakeHit(int damage)
    {
        if (health <= 0) { return; }
        health -= damage;
        hitEffect.Play();
        if (health <= 0)
        {
            stormManager.RemoveFromAliveList(gameObject);
            Destroy(gameObject);
            stormManager.DecreaseStormHealth(MAX_HEALTH);
            
        }
    }

    private void SpawnEffect()
    {
        myParticleSystem.Play();
        StartCoroutine(stormManager.CallThunder());
    }

    public void SetIndex(int value)
    {
        aliveIndex = value;
    }

    public int GetIndex()
    {
        return aliveIndex;
    }
}
