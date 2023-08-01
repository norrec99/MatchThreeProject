using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject tilePrefab;

    private Tile[,] allTiles;

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new Tile[width,height];
        SetupBoard();
    }

    private void SetupBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
                tile.name = $"Tile {i}, {j}";

                allTiles[i,j] = tile.GetComponent<Tile>();

            }
        }
    }


}
