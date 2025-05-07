using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject whiteTile;
    [SerializeField] GameObject blackTile;
    [SerializeField] GameObject piecePrefab;

    [Header("Int and Float")]
    [SerializeField] int tileCountX = 5;
    [SerializeField] int tileCountZ = 5;
    [SerializeField] float tileSize = 1f;

    [Header("Transform")]
    [SerializeField] Transform boardParent;
    [SerializeField] Transform cursorPrefab;

    Vector2Int position;
    PieceOwner owner;

    Cursor cursor;
    Board board;
    Piece piece;

    private Vector2Int moveForward = new Vector2Int(0, 1);
    private Vector2Int moveBackward = new Vector2Int(0, -1);
    private Vector2Int moveRight = new Vector2Int(1, 0);
    private Vector2Int moveLeft = new Vector2Int(-1, 0);


    void Start()
    {
        board = new Board(tileCountX, tileCountZ, whiteTile, blackTile, boardParent);
        cursor = new Cursor(Instantiate(cursorPrefab), tileCountX, tileCountZ);
        cursor.Startingposition();
        piece = new Piece(owner,position, piecePrefab);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            cursor.Move(moveForward);
            Debug.Log($"Posición del cursor: ({cursor.GetPosition().x}, {cursor.GetPosition().y})");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            cursor.Move(moveBackward);
            Debug.Log($"Posición del cursor: ({cursor.GetPosition().x}, {cursor.GetPosition().y})");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            cursor.Move(moveRight);
            Debug.Log($"Posición del cursor: ({cursor.GetPosition().x}, {cursor.GetPosition().y})");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            cursor.Move(moveLeft);
            Debug.Log($"Posición del cursor: ({cursor.GetPosition().x}, {cursor.GetPosition().y})");
        }
    }


}

