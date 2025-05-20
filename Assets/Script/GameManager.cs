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
                ChangePieceLife(targetTile.linkedEntity, -1);
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


    //VIDA Y ALTURA
    // Cambia la vida de una pieza y actualiza su altura
    void ChangePieceLife(Piece piece, int delta)
    {
        piece.life += delta;
        UpdatePieceHeight(piece);

        if (piece.life <= 0)
        {
            RemovePiece(piece);
        }
    }

    // Actualiza la altura del modelo según la vida
    void UpdatePieceHeight(Piece piece)
    {
        if (piece.pieceGO != null)
        {
            Vector3 scale = piece.pieceGO.transform.localScale;
            scale.y = Mathf.Max(0.1f, piece.life); // Evita altura 0 o negativa
            piece.pieceGO.transform.localScale = scale;
        }
    }

    // Elimina la pieza del tablero, lista y escena
    void RemovePiece(Piece piece)
    {
        board.At(piece.position).linkedEntity = null;

        if (piece.owner == PieceOwner.Player1)
            WhitePieces.Remove(piece);
        else if (piece.owner == PieceOwner.Player2)
            BlackPieces.Remove(piece);

        if (piece.pieceGO != null)
            Destroy(piece.pieceGO);

        if (selectedPiece == piece)
            selectedPiece = null;

        Debug.Log("¡Pieza destruida!");
    }
}

