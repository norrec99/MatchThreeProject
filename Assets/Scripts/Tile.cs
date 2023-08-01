using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int xIndex;
    [SerializeField] private int yIndex;

    private Board board;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(int x, int y, Board gameBoard)
    {
        xIndex = x;
        yIndex = y;
        board = gameBoard;
    }
}
