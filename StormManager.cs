using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StormManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private GameObject fadeEffect;
    [SerializeField] GameObject thunder;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] ParticleSystem rain;
    [SerializeField] UIManager uiManager;
    [SerializeField] Player player;
    [SerializeField] Inventory inventory;
    [SerializeField] SceneChanger sceneChanger;
    [SerializeField] BaseBuilding baseBuilding;


    [Header("Wave Config")]

    [SerializeField] private float waveInterval = 40f;
    [SerializeField] private int waveCount;
    [SerializeField] private int[] RoundsPerEachWave;
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private int[] enemyCountPerRound;
    [SerializeField] private List<int> enemySpawnIndices; // Which enemy to spawn by Index

    Wave[] waves;
    private float roundInterval = 5f; // Random between 10-20 seconds

    private bool stormActive = false;
    private Color stormColor;
    private Color dayLightColor;
    private int index;
    private bool finalWaveSpawned = false;
    bool alreadyFinisihed = false;
    bool playedSound = false;
    bool crystalsCollected = false;

    AudioManager audioManager;

    private List<GameObject> aliveEnemies = new List<GameObject>();
    private int totalEnemyHealth;
    private int currentStormHealth;

    private int stormCount = 0;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        stormColor = new Color(0.164f, 0.2015f, 0.3584f);
        dayLightColor = new Color(0.8207f, 0.8207f, 0.6775f);
        globalLight.color = dayLightColor;
        waves = new Wave[waveCount];

    }
    private void Update()
    {
        //Subside Storm when waves are over
        if (aliveEnemies.Count == 0  && finalWaveSpawned && !alreadyFinisihed)
        {
            Color targetColor = new Color(0.24f, 0.28f, 0.44f);
            float colorTransitionSpeed = 0.5f; 
            // Smoothly transition globalLight color towards the target color
            globalLight.color = Color.Lerp(globalLight.color, targetColor, colorTransitionSpeed * Time.deltaTime);

            var main = rain.main;
            var emission = rain.emission;
            if (main.simulationSpeed > 1)
            {
                float newSpeed = main.simulationSpeed - 0.1f * Time.deltaTime;
                if (newSpeed <= 1) { newSpeed = 1; Debug.Log("Speed settled"); }
                main.simulationSpeed = newSpeed;
                //Debug.Log("Speed: " + newSpeed);
            }

            var rateOverTime = emission.rateOverTime;
            if (rateOverTime.constant > 40)
            {
                float newRate = rateOverTime.constant - 5 * Time.deltaTime;
                //Debug.Log("Rate: " + newRate);
                if (newRate <= 40) { newRate = 40; Debug.Log("Rate settled"); }

                rateOverTime.constant = newRate;
                emission.rateOverTime = rateOverTime;
            }
            if (rateOverTime.constant <= 40 && main.simulationSpeed <= 1)
            {
                alreadyFinisihed = true;
                Debug.Log("Finished?: " + alreadyFinisihed);
            }
        }
        
        if(aliveEnemies.Count == 0 && finalWaveSpawned && !playedSound)
        {
            StartCoroutine(audioManager.FadeMusicOut("stormMusic", 5f));
            audioManager.Play("victory");
            playedSound = true;
        }
    }

    public bool IsStormActive()
    {
        return stormActive;
    }

    public void ActivateStorm()
    {
        CreateWaves();
        StartCoroutine(StormActivator(true));
        StartCoroutine(SummonWaves());
    }

    public void DeactivateStorm()
    {
        StartCoroutine(StormActivator(false));
    }

    public void ToggleStorm()
    {
        if (stormActive)
        {
            DeactivateStorm();
        }
        else
        {
            ActivateStorm();
        }
    }

    public IEnumerator CallThunder()
    {
        thunder.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        thunder.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        thunder.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        thunder.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        thunder.SetActive(true);
        yield return new WaitForSeconds(0.12f);
        thunder.SetActive(false);
    }

    private IEnumerator StormActivator(bool activate)
    {
        fadeEffect.SetActive(true);
        yield return new WaitForSeconds(2f);
        player.SetPlayerEnergy(player.MAX_ENERGY);
        stormActive = activate;
        finalWaveSpawned = false;
        uiManager.ToggleBaseBar();
        player.SetPlayerHealth(100);
        if (activate)
        {
            StartCoroutine(audioManager.FadeMusicOut("dayMusic", 1f));
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(audioManager.FadeMusicIn("stormMusic", 1.5f));
            //Debug.Log(totalEnemyHealth);
            currentStormHealth = totalEnemyHealth;
            uiManager.SetBuildButton(false);
            globalLight.color = stormColor;
            rain.gameObject.SetActive(true);
            audioManager.Play("rain");
            //StartCoroutine(uiManager.ActivateStormBar(totalEnemyHealth));
            alreadyFinisihed = false;
            playedSound = false;
            var main = rain.main;
            var emission = rain.emission;
            var rateOverTime = emission.rateOverTime;
            main.simulationSpeed = 1f;
            rateOverTime.constant = 60f;
            emission.rateOverTime = rateOverTime;
            inventory.ResetCrystals();
            stormCount++;
            if(uiManager.buildMenu.activeSelf) { uiManager.ToggleBuildMenu(); }
            baseBuilding.SetBaseHealth(100);
        }
        else
        {
            StartCoroutine(audioManager.FadeMusicOut("stormMusic", 1.5f));
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(audioManager.FadeMusicIn("dayMusic", 1.5f));
            uiManager.SetBuildButton(true);
            globalLight.color = dayLightColor;
            rain.gameObject.SetActive(false);
            audioManager.Stop("rain");
            //uiManager.DeactivateStormBar();
            if (stormCount == 3)
            {
                sceneChanger.LoadWin();
            }
        }
        
    }

    private IEnumerator SummonWaves()
    {
        yield return new WaitForSeconds(5f);
        for(int i = 0; i < waves.Length- 2 + stormCount; i++)
        {
            StartCoroutine(SummonWave(waves[i]));
            //Debug.Log("Summoned Wave: " + (i + 1) + " with " + waves[i].GetRoundCount() + " rounds");
            yield return new WaitForSeconds(waveInterval);
        }
        crystalsCollected = inventory.CrystalsCollected();
        Debug.Log("Crystals Collected? " + crystalsCollected);
        while (!crystalsCollected)
        {
            crystalsCollected = inventory.CrystalsCollected();
            if (crystalsCollected) { finalWaveSpawned = true; break; }
            Debug.Log("Repeat Last Wave");
            StartCoroutine(SummonWave(waves[waves.Length - 1 - 2 + stormCount]));
            AddStormHealth(waves[waves.Length - 1].GetTotalWaveHealth());
            yield return new WaitForSeconds(waveInterval / (stormCount + 1));
            crystalsCollected = inventory.CrystalsCollected();
        }
        finalWaveSpawned = true;
    }
    private IEnumerator SummonWave(Wave wave)
    {
        //Debug.Log("Wave Round Count: " + wave.GetRoundCount());
        for (int i = 0; i < wave.GetRoundCount(); i++)
        {
            //Debug.Log("Spawning Round " + i);
            Round currentRound = wave.GetRound(i);
            List<Spawner> enemiesToSpawn = currentRound.GetSpawnerList();
            //Debug.Log("EnemiesToSpawn Length: " + enemiesToSpawn);
            //Spawn round
            foreach(Spawner enemyToSpawn in enemiesToSpawn)
            {
                //Debug.Log("Spawning Enemy");
                GameObject newEnemy = Instantiate(enemyToSpawn.getEnemy(),
                    enemyToSpawn.getTransform().position, Quaternion.identity);
            }
            yield return new WaitForSeconds(wave.GetRoundInterval());
        }
    }

    private void CreateWaves()
    {
        int currentSpawnIndex = 0;
        int currentRoundIndex = 0;
        totalEnemyHealth = 0;
        // Wave iteration
        for (int i = 0; i < waveCount; i++)
        {
            int roundsForThisWave = RoundsPerEachWave[i]; // Rounds per each wave's length should be equal to waveCount
            Round[] roundArray = new Round[roundsForThisWave]; 

            // Round iteration - Rounds per wave
            for (int j = 0; j < roundsForThisWave; j++)
            {
                int enemyCountForThisRound = enemyCountPerRound[currentRoundIndex + j];
                Spawner[] spawners = new Spawner[enemyCountForThisRound];
                for (int k = 0; k < enemyCountForThisRound; k++)
                {
                    // Create spawners
                    Spawner newSpawner = new Spawner(spawnLocations[currentSpawnIndex + k],
                        enemyPrefabs[enemySpawnIndices[currentSpawnIndex + k]]);
                    spawners[k] = newSpawner;
                    totalEnemyHealth += newSpawner.getEnemy().GetComponent<Enemy>().GetHealth();
                }
                currentSpawnIndex += enemyCountForThisRound;
                Round newRound = new Round(spawners);
                roundArray[j] = newRound;
            }

            currentRoundIndex += roundsForThisWave;
            Wave newWave = new Wave(roundArray, roundInterval);
            waves[i] = newWave;
        }
       
    }

    public void RemoveFromAliveList(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);
    }
    public void AddtoAliveList(GameObject enemy)
    {
        aliveEnemies.Add(enemy);
    }
    public List<GameObject> GetAliveList()
    {
        return aliveEnemies;
    }
    public bool HasFinalWaveSpawned()
    {
        return finalWaveSpawned;
    }
    public void DecreaseStormHealth(int value)
    {
        currentStormHealth -= value;
        //uiManager.SetStormBar(currentStormHealth);
    }
    public void AddStormHealth(int value)
    {
        currentStormHealth += value;
        //uiManager.SetStormBar(currentStormHealth);
    }
    public void SetStormHealth(int value)
    {
        currentStormHealth = value;
        //uiManager.SetStormBar(currentStormHealth);
    }
    public int CalculateAliveEnemiesHealth()
    {
        int returnHealth = 0;
        foreach(GameObject enemy in aliveEnemies)
        {
            returnHealth += enemy.GetComponent<Enemy>().MAX_HEALTH;
        }
        return returnHealth;
    }
}
