using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] float baseHeight = 1.5f;

    [Header("Transform")]
    [SerializeField] Transform boardParent;
    [SerializeField] Transform cursorPrefab;

    [Header("Text")]
    [SerializeField] TMP_Text currentPlayer;

    [Header("Cursors")]
    [SerializeField] Transform cursorPrefabPlayer1;
    [SerializeField] Transform cursorPrefabPlayer2;

    private Cursor cursorPlayer1;
    private Cursor cursorPlayer2;
    private Cursor activeCursor;

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

        // Instancia ambos cursores
        cursorPlayer1 = new Cursor(Instantiate(cursorPrefabPlayer1), tileCountX, tileCountZ);
        cursorPlayer2 = new Cursor(Instantiate(cursorPrefabPlayer2), tileCountX, tileCountZ);

        cursorPlayer1.Startingposition();
        cursorPlayer2.Startingposition();

        // Solo el cursor del jugador activo está visible
        cursorPlayer1.GetTransform().gameObject.SetActive(true);
        cursorPlayer2.GetTransform().gameObject.SetActive(false);

        activeCursor = cursorPlayer1;

        InstantiatePieces(4, BlackPieceGO, BlackPieces, PieceOwner.Player2, baseHeight);
        InstantiatePieces(0, WhitePieceGO, WhitePieces, PieceOwner.Player1, baseHeight);

        GameEvents.TurnChange.AddListener(OnTurnChange);

        currentPlayer.text = currentTurn.ToString();
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

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            ChoosePiece();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryAttack();
        }
        UpdateCooldowns();
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
            selectedPiece = null;
            Debug.Log("Pieza deseleccionada.");
            return;
        }

        Vector2Int cursorPos = activeCursor.GetPosition();
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
    void InstantiatePieces(int startRow, GameObject piecePrefab, List<Piece> pieceList, PieceOwner owner, float baseHeight)
    {
        for (int i = 0; i < piecesAmount; i++)
        {
            Vector2Int pos = new Vector2Int(startRow, 1 + i);
            Piece newPiece = new Piece(owner, pos, piecePrefab, baseHeight);
            newPiece.SetBoard(board);
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
            activeCursor.Move(direction);
            Debug.Log($"Posición del cursor: ({activeCursor.GetPosition().x}, {activeCursor.GetPosition().y})");
        }
    }

    //MOVER PIEZA SELECCIONADA
    void moveSelectedPiece(Vector2Int direction)
    {
        Vector2Int currentPos = selectedPiece.position;
        Vector2Int newPos = currentPos + direction;

        // dirección dentro del tablero
        if (!board.IsOOB(newPos)) // Corrige aquí también: usa newPos, no pos
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

                // Actualiza posición del cursor activo
                activeCursor.Move(newPos - activeCursor.GetPosition());

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
        // Guarda la posición del cursor saliente
        Vector2Int lastCursorPos = activeCursor.GetPosition();

        // Cambia el turno
        currentTurn = (currentTurn == PieceOwner.Player1) ? PieceOwner.Player2 : PieceOwner.Player1;
        currentPlayer.text = currentTurn.ToString();
        selectedPiece = null;
        Debug.Log($"Turno cambiado. Ahora juega: {currentTurn}");

        TurnTime turnTime = FindObjectOfType<TurnTime>();
        if (turnTime != null)
        {
            turnTime.ResetTime();
            turnTime.SetBarForPlayer(currentTurn);
        }

        // Cambia el cursor activo y copia la posición
        if (currentTurn == PieceOwner.Player1)
        {
            cursorPlayer1.GetTransform().gameObject.SetActive(true);
            cursorPlayer2.GetTransform().gameObject.SetActive(false);
            cursorPlayer1.SetPosition(lastCursorPos); // <-- Copia la posición
            activeCursor = cursorPlayer1;
        }
        else
        {
            cursorPlayer1.GetTransform().gameObject.SetActive(false);
            cursorPlayer2.GetTransform().gameObject.SetActive(true);
            cursorPlayer2.SetPosition(lastCursorPos); // <-- Copia la posición
            activeCursor = cursorPlayer2;
        }
    }

    bool IsMyTurn()
    {
        return true;
    }

    //ATAQUES
    void TryAttack()
    {
        if (selectedPiece == null)
        {
            Debug.Log("No hay pieza seleccionada para atacar.");
            return;
        }

        if (selectedPiece.coolDown > 0f)
        {
            Debug.Log("La pieza está en cooldown.");
            return;
        }

        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(0, 1),   // Arriba
        new Vector2Int(0, -1),  // Abajo
        new Vector2Int(1, 0),   // Derecha
        new Vector2Int(-1, 0)   // Izquierda
        };

        foreach (var dir in directions)
        {
            Vector2Int targetPos = selectedPiece.position + dir;
            if (board.IsOOB(targetPos)) continue;

            Tile targetTile = board.At(targetPos);
            if (targetTile.linkedEntity != null && targetTile.linkedEntity.owner != selectedPiece.owner)
            {
                targetTile.linkedEntity.ChangePieceLife(-0.5f);
                Debug.Log($"¡Atacado a ({targetPos.x}, {targetPos.y})!");
            }
        }

        // Inicia cooldown 
        selectedPiece.coolDown = 2f;

        SetPieceCooldownVisual(selectedPiece, true);
    }

    void UpdateCooldowns()
    {
        foreach (var piece in WhitePieces)
            UpdatePieceCooldown(piece);
        foreach (var piece in BlackPieces)
            UpdatePieceCooldown(piece);
    }

    void UpdatePieceCooldown(Piece piece)
    {
        if (piece.coolDown > 0f)
        {
            piece.coolDown -= Time.deltaTime;
            if (piece.coolDown <= 0f)
            {
                piece.coolDown = 0f;
                SetPieceCooldownVisual(piece, false);
            }
        }
    }

    void SetPieceCooldownVisual(Piece piece, bool inCooldown)
    {
        if (piece.pieceGO != null)
        {
            var renderer = piece.pieceGO.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = inCooldown ? Color.gray : Color.white;
            }
        }
    }
}

