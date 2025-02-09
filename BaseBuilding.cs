using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuilding : MonoBehaviour
{
    [SerializeField] private Vector2Int size; // number of cells

    [SerializeField] private int health = 100;
    [SerializeField] SceneChanger sceneChanger;
    [SerializeField] UIManager uiManager;

    private float cellSize;
    private GridSystem grid;

    void Start()
    {
        Testing testing = FindObjectOfType<Testing>();
        grid = testing.GetGrid();
        cellSize = grid.GetCellSize();

        int sizeX = size.x;
        int sizeY = size.y;

        float topLeftX = transform.position.x - (cellSize / 2) * size.x;
        float topLeftY = transform.position.y + (cellSize / 2) * size.y - cellSize;

        Vector3 selectedCell = new Vector3(topLeftX, topLeftY);
        int selectedBuilding = 42;
        for (int i = 0; i < sizeX; i++)
        {
            Vector3 currentCell = selectedCell + new Vector3(i * cellSize, 0, 0);
            grid.SetValue(currentCell, selectedBuilding);
            for (int j = 0; j < sizeY; j++)
            {
                grid.SetValue(currentCell + new Vector3(0, -j * cellSize, 0), selectedBuilding);
            }
        }
    }

    public int GetBaseHealth()
    {
        return health;
    }

    public void SetBaseHealth(int value)
    {
        health = value;
        uiManager.SetBaseBar(value);
        if (health <= 0)
        {
            sceneChanger.LoadLose();
            // Lose screen, maybe an explosion for base
        }
    }

    public Vector2Int GetSize()
    {
        return size;
    }
}
