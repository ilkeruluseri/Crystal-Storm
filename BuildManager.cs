using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class BuildManager : MonoBehaviour
{
    [SerializeField] private GameObject[] buildingPrefabs;
    [SerializeField] private GameObject[] buildingPreviews;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Testing testing;

    private GameObject preview;
    private int selectedBuilding;
    private int sizeX;
    private int sizeY;
    private bool selected = false;
    private bool deleteMode = false;
    private GridSystem grid;

    private Sprite currentWallSprite;

    private void Start()
    {
        grid = testing.GetGrid();
        //Debug.Log(grid.ToString());
    }

    private void Update()
    {
        // Esc to quit build mode
        if (Input.GetKeyDown(KeyCode.Escape) && selected)
        {
            Destroy(preview);
            selected = false;
        }
        if (selected)
        {
            float cellSize = grid.GetCellSize();
            Vector3 selectedCell = UtilsClass.GetMouseWorldPosition();
            grid.GetXY(selectedCell, out int x, out int y);
            Vector3 newPos = grid.GetWorldPosition(x, y) + new Vector3(sizeX * cellSize / 2, (sizeY * cellSize / 2) - ((sizeY - 1) * 10), 0);
            preview.transform.position = newPos;
            if (testing.CanPlace())
            {
                Color green = Color.green;
                preview.GetComponent<SpriteRenderer>().color = new Color(green.r, green.g, green.b, 0.5f);
            }
            else
            {
                Color red = Color.red;
                preview.GetComponent<SpriteRenderer>().color = new Color(red.r, red.g, red.b, 0.5f);
            }
        }
    }

    public void SetSelectedBuilding(int value)
    {
        //If there was a previous building selection
        if (preview != null)
        {
            Destroy(preview);
        }
        //Price Check
        int[] costs = buildingPrefabs[value - 1].GetComponent<BuildingScript>().GetCosts();
        if (!(inventory.GetResource(0) >= costs[0] &&
              inventory.GetResource(1) >= costs[1] &&
              inventory.GetResource(2) >= costs[2]))
        {
            selected = false;
            return;
        }
        SetSelectedTrue();
        selectedBuilding = value;
        preview = Instantiate(buildingPreviews[selectedBuilding - 1]);
        deleteMode = false;
    }
    public void SetSizeX(int x)
    {
        sizeX = x;
    }
    public void SetSizeY(int y)
    {
        sizeY = y;
    }

    public int GetSelectedBuilding()
    {
        return selectedBuilding;
    }

    public void SetSelectedTrue()
    {
        selected = true;
    }
    public void SetSelectedFalse()
    {
        Destroy(preview);
        selected = false;
    }
    public bool IsSelected()
    {
        return selected;
    }
    public Vector2Int GetSize()
    {
        return new Vector2Int(sizeX, sizeY);
    }

    public void ToggleDeleteMode()
    {
        selected = false;
        deleteMode = !deleteMode;
    }

    public bool IsRemoving()
    {
        return deleteMode;
    }

    public void SetWallSprite(Sprite newSprite)
    {
        currentWallSprite = newSprite;
    }

    public Sprite GetWallSprite()
    {
        return currentWallSprite;
    }
}
