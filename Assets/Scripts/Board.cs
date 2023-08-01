using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int borderSize;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject[] defaultGamePieces;

    private Tile[,] allTiles;
    private GamePiece[,] allDefaultGamePieces;
    private Camera mainCamera;

    private void Awake() 
    {
        mainCamera = Camera.main;    
    }

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new Tile[width,height];
        allDefaultGamePieces = new GamePiece[width,height];
        SetupTiles();
        SetupCamera();
        FillRandom();
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

                allTiles[i,j].Init(i, j, this);

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

    private GameObject GetRandomDefaultGamePiece()
    {
        int randomIdx = Random.Range(0, defaultGamePieces.Length);

        if (defaultGamePieces[randomIdx] == null)
        {
            Debug.LogWarning($"BOARD: {randomIdx} does not contain a valid GamePiece prefab!");
        }

        return defaultGamePieces[randomIdx];
    }

    private void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning($"BOARD: Invalid GamePiece!");
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;
        gamePiece.SetCoord(x,y);
    }

    private void FillRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject randomPiece = Instantiate(GetRandomDefaultGamePiece(), new Vector3(i, j, 0), Quaternion.identity);

                if (randomPiece != null)
                {
                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j);
                }
            }
        }
    }


}
