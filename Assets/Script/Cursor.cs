using Unity.VisualScripting;
using UnityEngine;

public class Cursor
{
    private Vector2Int position;  // Posición del cursor
    private Transform cursorTransform;  // Transform del cursor

    private int boardWidth = 4;
    private int boardHeight = 4;

    public Cursor(Transform cursorObject, int x, int y)
    {
        this.cursorTransform = cursorObject;
        this.boardWidth = x;
        this.boardHeight = y;
        this.position = new Vector2Int(0, 0);
    }

    
    public void Startingposition()
    {
        cursorTransform.position = new Vector3(position.x, 0, position.y);
    }

    public void Move(Vector2Int direction)
    {
        position += direction;

        //Eje X
        if (position.x < 0)
            position.x = boardWidth - 1;
        else if (position.x >= boardWidth)
            position.x = 0;

        //Eje y
        if (position.y < 0)
            position.y = boardHeight - 1;
        else if (position.y >= boardHeight)
            position.y = 0;

        cursorTransform.position = new Vector3(position.x, 0, position.y);
    }

    public Vector2Int GetPosition()
    {
        return position;
    }
}