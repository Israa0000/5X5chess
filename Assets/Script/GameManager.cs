using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject whiteTile;
    [SerializeField] GameObject blackTile;
    [SerializeField] GameObject WhitePieceGO;
    [SerializeField] GameObject BlackPieceGO;

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
    Piece linkedEntity = null;

    public int piecesAmount = 3;

    private Vector2Int moveForward = new Vector2Int(0, 1);
    private Vector2Int moveBackward = new Vector2Int(0, -1);
    private Vector2Int moveRight = new Vector2Int(1, 0);
    private Vector2Int moveLeft = new Vector2Int(-1, 0);

    List<Piece> WhitePieces = new List<Piece>();
    List<Piece> BlackPieces = new List<Piece>();

    void Start()
    {
        board = new Board(tileCountX, tileCountZ, whiteTile, blackTile, boardParent);
        cursor = new Cursor(Instantiate(cursorPrefab), tileCountX, tileCountZ);
        cursor.Startingposition();

        for (int i = 0; i < piecesAmount; i++)
        {
            Vector2Int pos = new Vector2Int(1 + i, 4);
            Piece newPiece = new Piece(owner, pos, BlackPieceGO);
            BlackPieces.Add(newPiece);
            board.At(pos).linkedEntity = newPiece; // at tiene que devolver la casilla en la posicion que se pasa como paramentro 
            // tiene que estar puesta en el la clase tablero
        }

        for (int i = 0; i < piecesAmount; i++)
        {
            Vector2Int pos = new Vector2Int(1 + i, 0);
            Piece newPiece = new Piece(owner, pos, WhitePieceGO);
            WhitePieces.Add(newPiece);
        }
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChoosePiece();
        }
    }

    public void ChoosePiece()
    {
        board.At(x, y).LinkedEntity

    }
        /*Vector2Int cursorPos = cursor.GetPosition();
        foreach (var piece in WhitePieces)
        {
            if (piece.position == cursorPos)
            {
                selectedPiece = piece;
                Debug.Log($"Pieza blanca seleccionada en {cursorPos}");
                return;
            }
        }

        foreach (var piece in BlackPieces)
        {
            if (piece.position == cursorPos)
            {
                selectedPiece = piece;
                Debug.Log($"Pieza negra seleccionada en {cursorPos}");
                return;
            }
        }

        Debug.Log("No hay pieza en esta posición.");
        selectedPiece = null;*/
}

