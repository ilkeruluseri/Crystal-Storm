using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class WallScript : MonoBehaviour
{
    [SerializeField] private GameObject preview;
    [SerializeField] private BuildManager buildManager;
    private GridSystem grid;
    [SerializeField] private GameObject testingObject;
    private Testing testing;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] cornerPieces;

    int left;
    int right;
    int top;
    int bottom;
    int wallX;
    int wallY;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = buildManager.GetWallSprite();
        testingObject = GameObject.Find("Testing");
        testing = testingObject.GetComponent<Testing>();
        grid = testing.GetGrid();

        // L-piece variables
        grid.GetXY(transform.position, out int x, out int y);
        wallX = x;
        wallY = y;
        left = grid.GetValue(wallX - 1, wallY);
        right = grid.GetValue(wallX + 1, wallY);
        top = grid.GetValue(wallX, wallY + 1);
        bottom = grid.GetValue(wallX, wallY - 1);
        //Debug.Log("Left: " + left + " Right: " + right + " Top: " + top + " Bottom: " + bottom);
    }
    private void Update()
    {
        left = grid.GetValue(wallX - 1, wallY);
        right = grid.GetValue(wallX + 1, wallY);
        top = grid.GetValue(wallX, wallY + 1);
        bottom = grid.GetValue(wallX, wallY - 1);
        // L-piece logic
        //Debug.Log("Left: " + left + " Right: " + right + " Top: " + top + " Bottom: " + bottom);
        if (left == 1 && bottom == 1)
        {
            spriteRenderer.sprite = cornerPieces[0];
        }
        else if ((bottom == 1 && right == 1))
        {
            spriteRenderer.sprite = cornerPieces[1];
        }
        else
        {
            spriteRenderer.sprite = cornerPieces[2];
        }
    }


}
*/