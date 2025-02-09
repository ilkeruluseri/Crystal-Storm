using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeCollider : MonoBehaviour
{
    private Enemy enemyParent;

    private void Start()
    {
        // Get the Enemy component from the parent
        enemyParent = GetComponentInParent<Enemy>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enemyParent != null && (other.GetComponent<BuildingScript>() != null || other.GetComponent<Player>() != null ||
            other.GetComponent<BaseBuilding>() != null))
        {
            enemyParent.AddObjectInRange(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (enemyParent != null && (other.GetComponent<BuildingScript>() != null || other.GetComponent<Player>() != null ||
            other.GetComponent<BaseBuilding>() != null))
        {
            enemyParent.RemoveObjectInRange(other.gameObject);
        }
    }
}
