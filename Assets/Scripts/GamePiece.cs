using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    [SerializeField] private Sprite[] sprites;
    private Board m_board;
    private bool m_isMoving = false;

    public enum MatchValue
    {
        Blue,
        Green,
        Pink,
        Purple,
        Red,
        Yellow
    }
    public MatchValue matchValue;

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
            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            transform.position = Vector3.Lerp(startPosition, destination, t);

            yield return null;
        }
        m_isMoving = false;
    }

    public Sprite[] GetSprites()
    {
        return sprites;
    }

}
