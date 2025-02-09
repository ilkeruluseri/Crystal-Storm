using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPreviewScript : MonoBehaviour
{

    [SerializeField] private Sprite[] wallSprites;
    [SerializeField] private BuildManager buildManager;

    private int rotation = 0;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        buildManager.SetWallSprite(wallSprites[0]);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Rotate90();
        }
    }

    private void Rotate90()
    {
        rotation += 1;
        if (rotation == 5)
        {
            rotation = 0;
        }
        spriteRenderer.sprite = wallSprites[rotation];
        buildManager.SetWallSprite(spriteRenderer.sprite);
    }
}
