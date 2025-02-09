using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    
    [SerializeField] private Vector2Int size; // number of cells
    [SerializeField] private float rotation = 0;
    [SerializeField] private int resource0cost;
    [SerializeField] private int resource1cost;
    [SerializeField] private int resource2cost;

    [SerializeField] private int health = 10;
    private float cellSize;
    private GridSystem grid;

    void Awake()
    {
        Testing testing = FindObjectOfType<Testing>();
        grid = testing.GetGrid();
        cellSize = grid.GetCellSize();
    }

    public Vector2Int GetSize()
    {
        return size;
    }

    public float GetRotation()
    {
        return rotation;
    }

    public void SetRotation(float val)
    {
        rotation = val;
    }

    public int[] GetCosts()
    {
        return new int[] { resource0cost, resource1cost, resource2cost };
    }

    public void TakeDamage(int value)
    {
        health -= value;
        if (health <= 0)
        {
            DestroyBuilding();
        }
    }

    public void AddHealth(int value)
    {
        health += value;
    }

    public int GetHealth()
    {
        return health;
    }

    public void DestroyBuilding()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                // Go to top left cell
                float startX = transform.position.x - (cellSize / 2) * size.x;
                float startY = transform.position.y + (cellSize / 2) * size.y - cellSize;

                // Reset cell to 0
                Vector3 currentCell = new Vector3(startX + cellSize * i, startY - cellSize * j);
                grid.SetValue(currentCell, 0);
            }
        }
        //gameObject.SetActive(false); Enemy script destroys it, doing it here causes error
    }

    public void DelayedDestroy()
    {
        Delay(0.2f);
        Destroy(gameObject);
    }

    private IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
