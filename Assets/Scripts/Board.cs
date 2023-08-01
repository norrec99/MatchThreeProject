using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int borderSize;
    [SerializeField] private GameObject tilePrefab;

    private Tile[,] allTiles;
    private Camera mainCamera;

    private void Awake() 
    {
        mainCamera = Camera.main;    
    }

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new Tile[width,height];
        SetupTiles();
        SetupCamera();
    }

    private void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
                tile.name = $"Tile {i}, {j}";
                tile.transform.SetParent(transform);

                allTiles[i,j] = tile.GetComponent<Tile>();

            }
        }
    }

    private void SetupCamera()
    {
        mainCamera.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2f + (float) borderSize;
        float horizontalSize = ((float)width / 2f + (float) borderSize) + aspectRatio;
        mainCamera.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
    }


}
