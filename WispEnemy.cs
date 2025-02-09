using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispEnemy : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] GameObject wispPrefab;
    [SerializeField] float spawnInterval = 10f;
    private StormManager stormManager;

    private float initialMoveSpeed;
    
    private void Start()
    {
        enemy.ActivateParticle(false);
        initialMoveSpeed = enemy.moveSpeed;
        StartCoroutine(SpawnFriends(spawnInterval));
        stormManager = FindObjectOfType<StormManager>();
        if (gameObject.name != "Wisp(Clone)")
        {
            //Debug.Log("Am friend");
            stormManager.AddStormHealth(enemy.MAX_HEALTH);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.GetComponent<BuildingScript>() == null && obj.GetComponent<Player>() != null) // Must be player
        {
            Player player = obj.GetComponent<Player>();
            player.SetPlayerHealth(player.GetPlayerHealth() - enemy.attackDamage);
            enemy.TakeHit(enemy.GetHealth());
        }
        BuildingScript building = obj.GetComponent<BuildingScript>();
        if (building != null)
        {
            building.TakeDamage(enemy.attackDamage);
            enemy.TakeHit(enemy.GetHealth());
        }
        BaseBuilding baseBuilding = obj.GetComponent<BaseBuilding>();
        if (baseBuilding != null)
        {
            baseBuilding.SetBaseHealth(baseBuilding.GetBaseHealth() - enemy.attackDamage);
            // Base dying is handled in BaseBuilding script
            enemy.TakeHit(enemy.GetHealth());
        }

        
    }

    private IEnumerator SpawnFriends(float interval)
    {
        yield return new WaitForSeconds(interval);
        //Debug.Log("Friend!");
        Vector3 pos = transform.position + new Vector3(5f, 5f); 
        Instantiate(wispPrefab, pos, Quaternion.identity);
    }
}
