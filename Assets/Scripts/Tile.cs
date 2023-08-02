using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int xIndex;
    [SerializeField] private int yIndex;

    private Board m_board;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;
        m_board = board;
    }

    private void OnMouseDown() 
    {
        if (m_board != null)
        {
            m_board.ClickTile(this);
        }
    }
    private void OnMouseEnter() 
    {
        if (m_board != null)
        {
            m_board.DragToTile(this);
        }
    }
    private void OnMouseUp() 
    {
        if (m_board != null)
        {
            m_board.ReleaseTile(    );
        }
    }
}
