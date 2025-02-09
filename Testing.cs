using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.EventSystems;

public class Testing : MonoBehaviour
{
    private GridSystem grid;
    private float cellSize = 10f;
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private GameObject[] buildingPrefabs;
    [SerializeField] private Inventory inventory;

    void Awake()
    {
        grid = new GridSystem(30, 30, cellSize, new Vector3(-150, -150));
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && buildManager.IsSelected() && !EventSystem.current.IsPointerOverGameObject())
        {
            if (CanPlace())
            {
                Place();
            }
        } else if (Input.GetMouseButton(0) && buildManager.IsRemoving())
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            // Removal of building
            if (hit.collider != null && hit.collider.CompareTag("Building"))
            {
                GameObject clickedObject = hit.collider.gameObject;
                Vector3 pos = clickedObject.transform.position;
                Vector2Int size = clickedObject.GetComponent<BuildingScript>().GetSize();

                for (int i = 0; i < size.x; i++)
                {
                    for(int j = 0; j < size.y; j++)
                    {
                        // Go to top left cell
                        float startX = pos.x - (cellSize/2) * size.x;
                        float startY = pos.y + (cellSize / 2) * size.y - cellSize;

                        // Reset cell to 0
                        Vector3 currentCell = new Vector3(startX + cellSize * i, startY - cellSize * j);
                        grid.SetValue(currentCell, 0);
                    }
                }
                // Return cost
                int[] costs = clickedObject.GetComponent<BuildingScript>().GetCosts();
                inventory.AddResource(0, costs[0]);
                inventory.AddResource(1, costs[1]);
                inventory.AddResource(2, costs[2]);
                Destroy(clickedObject);
            }
        }
    }

    private void Place()
    {
        Vector3 selectedCell = UtilsClass.GetMouseWorldPosition();
        Vector2Int size = buildManager.GetSize();
        int sizeX = size.x;
        int sizeY = size.y;
        int selectedBuilding = buildManager.GetSelectedBuilding();

        int[] costs = buildingPrefabs[selectedBuilding - 1].GetComponent<BuildingScript>().GetCosts();
        // if it gets here, enough resources
        inventory.DecreaseResource(0, costs[0]);
        inventory.DecreaseResource(1, costs[1]);
        inventory.DecreaseResource(2, costs[2]);
        //Update grid  -- Assumed selected cell is on the top-left, rotation not yet implemented, only rectangular
        for (int i = 0; i < sizeX; i++)
        {
            Vector3 currentCell = selectedCell + new Vector3(i * cellSize, 0, 0);
            grid.SetValue(currentCell, selectedBuilding);
            for (int j = 0; j < sizeY; j++)
            {
                grid.SetValue(currentCell + new Vector3(0, -j * cellSize, 0), selectedBuilding);
            }
        }
        grid.GetXY(selectedCell, out int x, out int y);
        Vector3 newPos = grid.GetWorldPosition(x, y) + new Vector3(sizeX * cellSize / 2, (sizeY * cellSize / 2) - ((sizeY - 1) * 10), 0);
        Instantiate(buildingPrefabs[selectedBuilding - 1], newPos, Quaternion.identity);
    }

    public bool CanPlace()
    {
        bool result = true;
        Vector3 selectedCell = UtilsClass.GetMouseWorldPosition();
        Vector2Int size = buildManager.GetSize();
        int sizeX = size.x;
        int sizeY = size.y;
        for (int i = 0; i < sizeX; i++)
        {
            Vector3 currentCell = selectedCell + new Vector3(i * cellSize, 0, 0);
            if (grid.GetValue(currentCell) != 0)
            {
                result = false;
                break;
            }
            for (int j = 0; j < sizeY; j++)
            {
                if (grid.GetValue(currentCell + new Vector3(0, -j * cellSize, 0)) != 0)
                {
                    result = false;
                    break;
                }
            }
        }
        //Price Check again if player continues placing
        int selectedBuilding = buildManager.GetSelectedBuilding();
        int[] costs = buildingPrefabs[selectedBuilding - 1].GetComponent<BuildingScript>().GetCosts();
        if (!(inventory.GetResource(0) >= costs[0] &&
              inventory.GetResource(1) >= costs[1] &&
              inventory.GetResource(2) >= costs[2]))
        {
            return false;
        }

        return result;
    }

    public GridSystem GetGrid()
    {
        return grid;
    }

}
