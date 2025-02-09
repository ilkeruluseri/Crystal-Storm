using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    private List<int> resources = new List<int>();
    [SerializeField] GameObject[] resourcePrefabs;
    [SerializeField] TextMeshProUGUI[] resourceTexts;

    private void Start()
    {
        resources.Add(1000);
        resources.Add(1000);
        resources.Add(1000);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Resource0"))
                {

                    AddResource(0, resourcePrefabs[0].GetComponent<ResourceScript>().GetIncrease());
                }
                else if (hit.collider.CompareTag("Resource1"))
                {
                    AddResource(1, resourcePrefabs[1].GetComponent<ResourceScript>().GetIncrease());
                }
                else if (hit.collider.CompareTag("Resource2"))
                {
                    AddResource(2, resourcePrefabs[2].GetComponent<ResourceScript>().GetIncrease());
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

}
