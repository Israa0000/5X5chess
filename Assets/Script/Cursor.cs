using Unity.VisualScripting;
using UnityEngine;

public class Cursor
{
    Vector2Int moveFoward;
    Vector2Int moveBackward;
    Vector2Int moveRight;
    Vector2Int moveLeft;
    
    public Cursor(Vector2Int actualPosition, Vector2Int moveFoward, Vector2Int moveBackward, Vector2Int moveRight, Vector2Int moveLeft)
    {
        this.moveBackward = new Vector2Int(0,1);
        new Vector3(moveFoward.x, 0, moveFoward.y);

        if (Input.GetKeyDown(KeyCode.W)) {
            actualPosition= actualPosition + moveFoward;
        }

    }
}
