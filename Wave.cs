using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    private List<Round> rounds = new List<Round>();
    private float roundInterval;
    private int totalWaveHealth;

    public Wave(Round[] roundArray, float roundInterval)
    {
        this.roundInterval = roundInterval;
        totalWaveHealth = 0;
        for (int i = 0; i < roundArray.Length; i++)
        {
            rounds.Add(roundArray[i]);
            totalWaveHealth += roundArray[i].GetTotalRoundHealth();
        }
        //Debug.Log("Wave created with roundArray length: " + roundArray.Length);
    }

    public Round GetRound(int i)
    {
        return rounds[i];
    }
    public float GetRoundInterval()
    {
        return roundInterval;
    }
    public int GetTotalWaveHealth()
    {
        return totalWaveHealth;
    }
    public int GetRoundCount()
    {
        return rounds.Count;
    }
}

public class Round
{
    private List<Spawner> spawnerList = new List<Spawner>();
    private int totalRoundHealth;

    public Round(Spawner[] spawners)
    {
        totalRoundHealth = 0;
        for (int i = 0; i<spawners.Length; i++)
        {
            if (spawners[i] == null)
            {
                Debug.Log("Couldn't add: " + i + "th spawner.");
            }
            spawnerList.Add(spawners[i]);
            totalRoundHealth += spawners[i].getEnemy().GetComponent<Enemy>().GetHealth();
        }
        //Debug.Log("Round created with spawnerArray length: " + spawners.Length);
        //for (int i = 0; i < transformArray.Length; i++)
        //{
        //    GameObject enemyPrefab = enemyPrefabArray[enemySpawnIndexArray[i]];
        //    Spawner newSpawner = new Spawner(transformArray[i], enemyPrefab);
        //    spawnerList.Add(newSpawner);
        //    totalRoundHealth += enemyPrefab.GetComponent<Enemy>().GetHealth();
        //}
    }
    public List<Spawner> GetSpawnerList()
    {
        return spawnerList;
    }
    public int GetTotalRoundHealth()
    {
        return totalRoundHealth;
    }



}

public class Spawner
{
    private Transform transform;
    private GameObject enemyPrefab;

    public Spawner(Transform transform, GameObject enemyPrefab)
    {

        this.transform = transform;
        this.enemyPrefab = enemyPrefab;

        //Debug.Log("Spawner created with position: " + transform.position + "enemy: "+enemyPrefab.ToString());
    }

    public Transform getTransform()
    {
        return transform;
    }
    public GameObject getEnemy()
    {
        return enemyPrefab;
    }
}
