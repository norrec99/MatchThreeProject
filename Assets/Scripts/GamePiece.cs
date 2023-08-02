using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    [SerializeField] private int xIndex;
    [SerializeField] private int yIndex;

    private Board m_board;
    bool m_isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update() 
    {
        // if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     Move((int)transform.position.x + 1, (int)transform.position.y, 0.5f);
        // }
        // if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     Move((int)transform.position.x - 1, (int)transform.position.y, 0.5f);
        // }
    }
    public void Init(Board board)
    {
        m_board = board;
    }

    public void SetCoord(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
    public void Move(int destX, int destY, float timeToMove)
    {
        if (!m_isMoving)
        {
            StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0f), timeToMove));
        }
    }

    private IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
    {
        Vector3 startPosition = transform.position;
        bool reachedDestination = false;
        float elapsedTime = 0f;
        m_isMoving = true;
        while (!reachedDestination)
        {
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;
                if (m_board != null)
                {
                    m_board.PlaceGamePiece(this, (int)destination.x, (int)destination.y);
                }
                break;
            }
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / timeToMove;

            transform.position = Vector3.Lerp(startPosition, destination, t);

            yield return null;
        }
        m_isMoving = false;
    }
}
