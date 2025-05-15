using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
    [SerializeField] int piecesAmount = 3;

    [Header("Transform")]
    [SerializeField] Transform boardParent;
    [SerializeField] Transform cursorPrefab;

    Vector2Int position;
    PieceOwner owner;

    Cursor cursor;
    Board board;
    Piece piece;
    Piece linkedEntity = null;
    Piece selectedPiece = null;

    private PieceOwner currentTurn = PieceOwner.Player1;

    private Vector2Int moveForward = new Vector2Int(0, 1);
    private Vector2Int moveBackward = new Vector2Int(0, -1);
    private Vector2Int moveRight = new Vector2Int(1, 0);
    private Vector2Int moveLeft = new Vector2Int(-1, 0);

    private Vector2Int pos;

    List<Piece> WhitePieces = new List<Piece>();
    List<Piece> BlackPieces = new List<Piece>();

    void Start()
    {
        board = new Board(tileCountX, tileCountZ, whiteTile, blackTile, boardParent, linkedEntity);
        cursor = new Cursor(Instantiate(cursorPrefab), tileCountX, tileCountZ);
        cursor.Startingposition();

        InstantiatePieces(4, BlackPieceGO, BlackPieces, PieceOwner.Player2);
        InstantiatePieces(0, WhitePieceGO, WhitePieces, PieceOwner.Player1);

        GameEvents.TurnChange.AddListener(OnTurnChange);
    }
    void Update()
    {
        if (!IsMyTurn()) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryMove(moveForward);
            Debug.Log($"Posición del cursor: ({cursor.GetPosition().x}, {cursor.GetPosition().y})");
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            TryMove(moveBackward);
            Debug.Log($"Posición del cursor: ({cursor.GetPosition().x}, {cursor.GetPosition().y})");
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMove(moveRight);
            Debug.Log($"Posición del cursor: ({cursor.GetPosition().x}, {cursor.GetPosition().y})");
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMove(moveLeft);
            Debug.Log($"Posición del cursor: ({cursor.GetPosition().x}, {cursor.GetPosition().y})");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChoosePiece();
        }
    }

    //FUNCIONES 

        //Move() esta en cursor
        //Startingposition() en cursor 
        //GetPosition() esta en cursor
        //At() esta en board
        //IsOOB() esta en board

    //ELEGIR PIEZA
    public void ChoosePiece()
    {
        if (selectedPiece != null)
        {
            // Si ya hay una pieza seleccionada, deja de estar seleccionada
            selectedPiece = null;
            Debug.Log("Pieza deseleccionada.");
            return;
        }
        
        Vector2Int cursorPos = cursor.GetPosition();
        Tile tile = board.At(cursorPos);

        if (tile.linkedEntity != null && tile.linkedEntity.owner == currentTurn)
        {
            selectedPiece = tile.linkedEntity;
            Debug.Log("Hay una pieza en esta casilla.");
        }
        else
        {
            selectedPiece = null;
            Debug.Log("No hay pieza en esta casilla.");
        }
    }

    //INSTANCIAR PIEZAS
    void InstantiatePieces(int startRow, GameObject piecePrefab, List<Piece> pieceList, PieceOwner owner)
    {
        for (int i = 0; i < piecesAmount; i++)
        {
            Vector2Int pos = new Vector2Int(1 + i, startRow);
            Piece newPiece = new Piece(owner, pos, piecePrefab);
            pieceList.Add(newPiece);
            board.At(pos).linkedEntity = newPiece;
        }
    }

    //COMPROBAR SI SE PUEDE MOVER LA PIEZA
    void TryMove(Vector2Int direction)
    {
        if (selectedPiece != null)
        {
            moveSelectedPiece(direction);
        }
        else
        {
            cursor.Move(direction);
            Debug.Log($"Posición del cursor: ({cursor.GetPosition().x}, {cursor.GetPosition().y})");
        }
    }

    //MOVER PIEZA SELECCIONADA
    void moveSelectedPiece(Vector2Int direction)
    {
        Vector2Int currentPos = selectedPiece.position;
        Vector2Int newPos = currentPos + direction;

        // direccion dentro del tablero
        if (!board.IsOOB(pos))// Out Of Bounds
        {
            Tile currentTile = board.At(currentPos);
            Tile targetTile = board.At(newPos);

            if (targetTile.linkedEntity == null)
            {
                // Actualiza la referencia en las casillas
                currentTile.linkedEntity = null;
                targetTile.linkedEntity = selectedPiece;

                // Actualiza la posición de la pieza
                selectedPiece.position = newPos;
                selectedPiece.pieceGO.transform.position = new Vector3(newPos.x, 0, newPos.y);

                // Actualiza posiscion de cursor
                cursor.Move(newPos - cursor.GetPosition());

                Debug.Log($"Pieza movida a ({newPos.x}, {newPos.y})");
            }
            else
            {
                Debug.Log("La casilla destino está ocupada.");
            }
        }
        else
        {
            Debug.Log("Movimiento fuera del tablero.");
        }
    }

    //CAMBIO DE TURNO
    void OnTurnChange()
    {
        currentTurn = (currentTurn == PieceOwner.Player1) ? PieceOwner.Player2 : PieceOwner.Player1;
        selectedPiece = null;
        Debug.Log($"Turno cambiado. Ahora juega: {currentTurn}");

        TurnTime turnTime = FindObjectOfType<TurnTime>();
        if (turnTime != null)
            turnTime.ResetTime();
    }

    bool IsMyTurn()
    {
        return true;
    }
}

