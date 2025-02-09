using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEnemy : MonoBehaviour
{
    [SerializeField] Enemy enemy;

    private void Update()
    {
        if (enemy.objectsInRange.Count > 0)
        {
            enemy.isAttacking = true;
        }
        else
        {
            enemy.isAttacking = false;
        }

        enemy.animator.SetBool("isAttacking", enemy.isAttacking);
    }

    private void Attack()
    {
        if (enemy.objectsInRange.Count > 0)
        {
            List<GameObject> objectsToRemove = new List<GameObject>();

            // Attack all objects in range
            foreach (GameObject obj in enemy.objectsInRange)
            {
                if (obj != null)
                {
                    if (obj.GetComponent<BuildingScript>() == null && obj.GetComponent<Player>() != null) // Must be player
                    {
                        Player player = obj.GetComponent<Player>();
                        player.SetPlayerHealth(player.GetPlayerHealth() - enemy.attackDamage);
                        continue;
                    }
                    BuildingScript building = obj.GetComponent<BuildingScript>();
                    if (building != null)
                    {
                        building.TakeDamage(enemy.attackDamage);

                        //Debug.Log("Checking if building is gone");
                        if (building.GetHealth() <= 0)
                        {
                            //Debug.Log("Removing object");
                            objectsToRemove.Add(obj);
                        }
                    }
                    BaseBuilding baseBuilding = obj.GetComponent<BaseBuilding>();
                    if (baseBuilding != null)
                    {
                        baseBuilding.SetBaseHealth(baseBuilding.GetBaseHealth() - enemy.attackDamage);
                        // Base dying is handled in BaseBuilding script
                    }
                }
            }
            foreach (GameObject obj in objectsToRemove)
            {
                //Debug.Log("Got here");
                enemy.objectsInRange.Remove(obj);
                obj.GetComponent<BuildingScript>().DelayedDestroy();
            }
            enemy.ActivateParticle(true);
            //Debug.Log("Finished attack");
        }
    }
}
