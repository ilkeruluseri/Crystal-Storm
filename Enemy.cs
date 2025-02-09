using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 10;
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private Animator animator;

    [SerializeField] float desiredDistanceFromTarget = 6f;
    [SerializeField] ParticleSystem myParticleSystem;
    
    private Transform target;
    private List<GameObject> objectsInRange = new List<GameObject>();
    private bool isAttacking = false;
    private bool isMoving = false;

    private void Start()
    {
    }

    private void Update()
    {
        FindNearestBuilding();
        if (target != null)
        {
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
        if (objectsInRange.Count > 0)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isMoving", isMoving);
    }

    /* / Detect objects entering attack range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BuildingScript>() != null)
        {
            objectsInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BuildingScript>() != null)
        {
            //objectsInRange.Remove(other.gameObject);
        }
    }
    */

    public void AddObjectInRange(GameObject obj)
    {
        objectsInRange.Add(obj);
    }

    // Method to remove objects from range
    public void RemoveObjectInRange(GameObject obj)
    {
        objectsInRange.Remove(obj);
    }

    private void FindNearestBuilding()
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

        target = nearestBuilding;
    }

    private void MoveTowardsTarget()
    {
        isAttacking = false;
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    /*private IEnumerator PeriodicAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            
            if (objectsInRange.Count > 0)
            {
                isAttacking = true;

                List<GameObject> objectsToRemove = new List<GameObject>();

                // Attack all objects in range
                foreach (GameObject obj in objectsInRange)
                {
                    if (obj != null)
                    {
                        BuildingScript building = obj.GetComponent<BuildingScript>();
                        if (building != null)
                        {
                            building.TakeDamage(attackDamage);

                            //Debug.Log("Checking if building is gone");
                            if (building.GetHealth() <= 0)
                            {
                                //Debug.Log("Removing object");
                                objectsToRemove.Add(obj);
                            }
                        }
                    }
                }
                foreach (GameObject obj in objectsToRemove)
                {
                    //Debug.Log("Got here");
                    objectsInRange.Remove(obj);
                    obj.GetComponent<BuildingScript>().DelayedDestroy();
                }
                //Debug.Log("Finished attack");
                isAttacking = false;
            }
        }
    }
    */
    private void ActivateParticle()
    {
        myParticleSystem.Play();
    }

    private void Attack()
    {
        if (objectsInRange.Count > 0)
        {
            List<GameObject> objectsToRemove = new List<GameObject>();

            // Attack all objects in range
            foreach (GameObject obj in objectsInRange)
            {
                if (obj != null)
                {
                    BuildingScript building = obj.GetComponent<BuildingScript>();
                    if (building != null)
                    {
                        building.TakeDamage(attackDamage);

                        //Debug.Log("Checking if building is gone");
                        if (building.GetHealth() <= 0)
                        {
                            //Debug.Log("Removing object");
                            objectsToRemove.Add(obj);
                        }
                    }
                }
            }
            foreach (GameObject obj in objectsToRemove)
            {
                //Debug.Log("Got here");
                objectsInRange.Remove(obj);
                obj.GetComponent<BuildingScript>().DelayedDestroy();
            }
            ActivateParticle();
            //Debug.Log("Finished attack");
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public void TakeHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
