using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    private List<int> resources = new List<int>();
    [SerializeField] GameObject[] resourcePrefabs;
    [SerializeField] TextMeshProUGUI[] resourceTexts;
    [SerializeField] AudioManager audioManager;
    [SerializeField] Player player;
    [SerializeField] StormManager stormManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] List<Transform> crystalSpots;
    List<GameObject> currentCrystals = new List<GameObject>();

    private void Start()
    {
        resources.Add(0);
        resources.Add(0);
        resources.Add(0);
        foreach(Transform spot in crystalSpots)
        {
            GameObject newCrystal  = Instantiate(resourcePrefabs[2], spot.position, Quaternion.identity);
            currentCrystals.Add(newCrystal);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null && player.GetPlayerEnergy() > 0)
            {
                ResourceScript resourceScriptAlpha = hit.collider.GetComponent<ResourceScript>();
                if (resourceScriptAlpha != null && !resourceScriptAlpha.isCollecting)
                {
                    if (hit.collider.CompareTag("Resource0"))
                    {
                        ResourceScript resourceScript = resourcePrefabs[0].GetComponent<ResourceScript>();
                        audioManager.Play("treeCollect");
                        AddResource(0, resourceScript.GetIncrease());
                    }
                    else if (hit.collider.CompareTag("Resource1"))
                    {
                        audioManager.Play("rockCollect");
                        AddResource(1, resourcePrefabs[1].GetComponent<ResourceScript>().GetIncrease());
                    }
                    else if (hit.collider.CompareTag("Resource2"))
                    {
                        if (stormManager.IsStormActive())
                        {
                            audioManager.Play("crystalCollect");
                            AddResource(2, resourcePrefabs[2].GetComponent<ResourceScript>().GetIncrease());
                        }
                        else
                        {
                            uiManager.ToggleCrystalMessage();
                            return;
                        }
                        
                    }
                    StartCoroutine(hit.collider.GetComponent<ResourceScript>().CollectAnimation());
                    player.SetPlayerEnergy(player.GetPlayerEnergy() - 1);
                }
            }
        }

        for (int i = 0; i < resourceTexts.Length; i++)
        {
            resourceTexts[i].text = resources[i].ToString();
        }
    }

    public void AddResource(int resourceType, int amount)
    {
        resources[resourceType] += amount;
    }
    public void DecreaseResource(int resourceType, int amount)
    {
        resources[resourceType] -= amount;
    }

    public int GetResource(int resourceType)
    {
        return resources[resourceType];
    }

    public bool CrystalsCollected()
    {
        //Debug.Log("Inventory checking crystals: " + (currentCrystals.Count <= 0));
        return currentCrystals.Count <= 3;
    }

    public void RemoveFromCrystals(GameObject crystal)
    {
        currentCrystals.Remove(crystal);
    }

    public void ResetCrystals()
    {
        ResourceScript[] aliveCrystals = FindObjectsByType<ResourceScript>(FindObjectsSortMode.None);
        foreach (var x in aliveCrystals)
        {
            if (x.gameObject.CompareTag("Resource2"))
            {
                currentCrystals.Remove(x.gameObject);
                Destroy(x.gameObject);
            }
        }
        foreach (Transform spot in crystalSpots)
        {
            GameObject newCrystal =  Instantiate(resourcePrefabs[2], spot.position, Quaternion.identity);
            currentCrystals.Add(newCrystal);
        }
    }

}
