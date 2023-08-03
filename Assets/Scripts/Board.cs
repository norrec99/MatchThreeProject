using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int borderSize;
    [SerializeField] private float swapTime;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject[] defaultGamePieces;

    private Camera mainCamera;
    private Tile[,] m_allTiles;
    private GamePiece[,] m_allDefaultGamePieces;
    private Tile m_clickedTile;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_allTiles = new Tile[width, height];
        m_allDefaultGamePieces = new GamePiece[width, height];
        SetupTiles();
        SetupCamera();
        FillBoard(10, 1f);
        HighlightMatches();
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

                m_allTiles[i, j] = tile.GetComponent<Tile>();

                m_allTiles[i, j].Init(i, j, this);

            }
        }
    }

    private void SetupCamera()
    {
        mainCamera.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2f + (float)borderSize;
        float horizontalSize = ((float)width / 2f + (float)borderSize) + aspectRatio;
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

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning($"BOARD: Invalid GamePiece!");
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;
        if (IsWithinBounds(x, y))
        {
            m_allDefaultGamePieces[x, y] = gamePiece;
        }
        gamePiece.SetCoord(x, y);
    }

    private bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    private void FillBoard(int falseYOffset = 0, float moveTime = 0.1f)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (m_allDefaultGamePieces[i,j] == null)
                {
                    GamePiece piece = FillRandomAt(i, j, falseYOffset, moveTime);
                }
            }
        }
    }

    private GamePiece FillRandomAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        GameObject randomPiece = Instantiate(GetRandomDefaultGamePiece(), new Vector3(x, y, 0), Quaternion.identity);

        if (randomPiece != null)
        {
            randomPiece.GetComponent<GamePiece>().Init(this);
            PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), x, y);

            if (falseYOffset != 0)
            {
                randomPiece.transform.position = new Vector3(x, y + falseYOffset, 0);
                randomPiece.GetComponent<GamePiece>().Move(x, y, moveTime);
            }

            randomPiece.transform.SetParent(transform);
            return randomPiece.GetComponent<GamePiece>();
        }
        return null;
    }
    public void ClickTile(Tile tile)
    {
        if (m_clickedTile == null)
        {
            m_clickedTile = tile;
            Debug.Log("clicked tile: " + tile.name);
            List<GamePiece> clickedPieceMatches = FindMatchesAt(tile.xIndex, tile.yIndex);
            
            BlastTiles(m_clickedTile);
        }
    }
    public void ReleaseTile()
    {
        m_clickedTile = null;
    }
    private void BlastTiles(Tile clickedTile)
    {
        StartCoroutine(BlastTilesRoutine(clickedTile));
    }
    private IEnumerator BlastTilesRoutine(Tile clickedTile)
    {
        GamePiece clickedPiece = m_allDefaultGamePieces[clickedTile.xIndex, clickedTile.yIndex];
        if (clickedPiece != null)
        {
            List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);

            if (clickedPieceMatches.Count == 0)
            {
                //Maybe do some anim
            }
            else
            {
                yield return new WaitForSeconds(swapTime);
                ClearPieceAt(clickedPieceMatches);
                CollapseColumn(clickedPieceMatches);
                yield return StartCoroutine(RefillRoutine());
            }

        }
    }

    private List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 2)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;

        if (IsWithinBounds(startX, startY))
        {
            startPiece = m_allDefaultGamePieces[startX, startY];
        }
        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }

        int nextX;
        int nextY;

        int maxValue = (width > height) ? width : height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            GamePiece nextPiece = m_allDefaultGamePieces[nextX, nextY];
            if (nextPiece == null)
            {
                break;
            }
            else
            {
                if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }

        }


        if (matches.Count >= minLength)
        {
            return matches;
        }

        return null;
    }
    private List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 2)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 1);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 1);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }
        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

        return combinedMatches.Count >= minLength ? combinedMatches : null;
    }
    private List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 2)
    {
        List<GamePiece> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 1);
        List<GamePiece> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 1);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }
        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return combinedMatches.Count >= minLength ? combinedMatches : null;
    }
    private List<GamePiece> FindMatchesAt(int x, int y, int minLength = 2)
    {
        List<GamePiece> horizMatches = FindHorizontalMatches(x, y, minLength);
        List<GamePiece> vertMatches = FindVerticalMatches(x, y, minLength);

        if (horizMatches == null)
        {
            horizMatches = new List<GamePiece>();
        }
        if (vertMatches == null)
        {
            vertMatches = new List<GamePiece>();
        }

        var combinedMatches = horizMatches.Union(vertMatches).ToList();
        return combinedMatches;
    }

    private void HighlightMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                HighlightMatchesAt(i, j);

            }
        }
    }

    private void HighlightMatchesAt(int x, int y)
    {
        HighlightTileOff(x, y);

        var combinedMatches = FindMatchesAt(x, y);

        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece piece in combinedMatches)
            {
                switch (combinedMatches.Count)
                {
                    case 2:
                        piece.GetComponent<SpriteRenderer>().sprite = piece.GetSprites()[0];
                        break;
                    case 3:
                        piece.GetComponent<SpriteRenderer>().sprite = piece.GetSprites()[1];
                        break;
                    case 4:
                        piece.GetComponent<SpriteRenderer>().sprite = piece.GetSprites()[2];
                        break;
                    case 5:
                        piece.GetComponent<SpriteRenderer>().sprite = piece.GetSprites()[3];
                        break;
                    case 6:
                        piece.GetComponent<SpriteRenderer>().sprite = piece.GetSprites()[4];
                        break;
                }
                // HighlightTileOn(piece.xIndex, piece.yIndex);
            }
        }
    }

    private void HighlightTileOn(int x, int y)
    {
        SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
    }

    private void HighlightTileOff(int x, int y)
    {
        SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a);
    }
    private void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = m_allDefaultGamePieces[x, y];

        if (pieceToClear != null)
        {
            m_allDefaultGamePieces[x, y] = null;
            Destroy(pieceToClear.gameObject);
        }

        HighlightTileOff(x, y);
    }
    private void ClearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPieceAt(i, j);
            }
        }
    }
    private void ClearPieceAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                ClearPieceAt(piece.xIndex, piece.yIndex);
            }
        }
    }
    private List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        for (int i = 0; i < height - 1; i++)
        {
            if (m_allDefaultGamePieces[column, i] == null)
            {
                for (int j = i + 1; j < height; j++)
                {
                    if (m_allDefaultGamePieces[column, j] != null)
                    {
                        m_allDefaultGamePieces[column, j].Move(column, i, collapseTime);
                        m_allDefaultGamePieces[column, i] = m_allDefaultGamePieces[column, j];
                        m_allDefaultGamePieces[column, i].SetCoord(column, i);

                        if (!movingPieces.Contains(m_allDefaultGamePieces[column, i]))
                        {
                            movingPieces.Add(m_allDefaultGamePieces[column, i]);
                        }

                        m_allDefaultGamePieces[column, j] = null;
                        break;
                    }
                }
            }
        }
        return movingPieces;
    }
    private List<GamePiece> CollapseColumn(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<int> columnsToCollapse = GetColumns(gamePieces);

        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }

        return movingPieces;

    }
    private List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in gamePieces)
        {
            if (!columns.Contains(piece.xIndex))
            {
                columns.Add(piece.xIndex);
            }
        }
        return columns;
    }
    private IEnumerator RefillRoutine()
    {
        yield return new WaitForSeconds(swapTime);
        FillBoard(10, 0.2f);
        HighlightMatches();
        yield return null;
    }
}
