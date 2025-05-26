using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject whiteTile;
    [SerializeField] GameObject blackTile;
    [SerializeField] GameObject WhitePieceGO;
    [SerializeField] GameObject BlackPieceGO;
    [SerializeField] GameObject chestPrefab;
    [SerializeField] GameObject healthPickupPrefab;
    [SerializeField] GameObject attackParticlePrefab;
    [SerializeField] GameObject restartButton;

    [Header("Int and Float")]
    [SerializeField] int tileCountX = 5;
    [SerializeField] int tileCountZ = 5;
    [SerializeField] float tileSize = 1f;
    [SerializeField] int piecesAmount = 3;
    [SerializeField] float baseHeight = 1.5f;
    [SerializeField] int minChests = 1;
    [SerializeField] int maxChests = 3;
    [SerializeField] float maxCooldown = 2f;
    [SerializeField] float minCooldown = 0.5f;


    [Header("Transform")]
    [SerializeField] Transform boardParent;
    [SerializeField] Transform cursorPrefab;

    [Header("Text")]
    [SerializeField] TMP_Text currentPlayer;
    [SerializeField] TMP_Text victoryText;


    [Header("Cursor Materials")]
    [SerializeField] Material lightBlueMaterial;
    [SerializeField] Material pinkMaterial;

    private Cursor cursor;
    public Board board;
    private Piece selectedPiece = null;
    private PieceOwner currentTurn = PieceOwner.Player1;

    private Vector2Int moveForward = new Vector2Int(0, 1);
    private Vector2Int moveBackward = new Vector2Int(0, -1);
    private Vector2Int moveRight = new Vector2Int(1, 0);
    private Vector2Int moveLeft = new Vector2Int(-1, 0);

    public List<Piece> LightBluePieces = new List<Piece>();
    public List<Piece> PinkPieces = new List<Piece>();
    public List<Chest> chests = new List<Chest>();
    private List<HealthPickup> healthPickups = new List<HealthPickup>();

    private bool gameEnded = false;

    void Start()
    {
        board = new Board(tileCountX, tileCountZ, whiteTile, blackTile, boardParent, null);

        cursor = new Cursor(Instantiate(cursorPrefab), tileCountX, tileCountZ);
        cursor.Startingposition();

        SetCursorMaterial(currentTurn);

        InstantiatePieces(4, BlackPieceGO, PinkPieces, PieceOwner.Player2, baseHeight);
        InstantiatePieces(0, WhitePieceGO, LightBluePieces, PieceOwner.Player1, baseHeight);

        SpawnChests();

        GameEvents.TurnChange.AddListener(OnTurnChange);
        currentPlayer.text = currentTurn.ToString();

        TurnTime turnTime = FindFirstObjectByType<TurnTime>();
        if (turnTime != null)
            turnTime.SetBarForPlayer(currentTurn);

        if (victoryText != null)
            victoryText.gameObject.SetActive(false);
        if (restartButton != null)
            restartButton.SetActive(false);
    }
    void Update()
    {
        if (gameEnded) return;

        if (!IsMyTurn()) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            TryMove(moveForward);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            TryMove(moveBackward);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            TryMove(moveRight);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            TryMove(moveLeft);

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            ChoosePiece();
        if (Input.GetKeyDown(KeyCode.Space))
            TryAttack();

        UpdateCooldowns();
    }

    void SetCursorMaterial(PieceOwner owner)
    {
        var renderer = cursor.GetTransform().GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = (owner == PieceOwner.Player1) ? lightBlueMaterial : pinkMaterial;
        }
    }

    public void ChoosePiece()
    {
        if (selectedPiece != null)
        {
            selectedPiece = null;
            Debug.Log("Pieza deseleccionada.");
            return;
        }

        Vector2Int cursorPos = cursor.GetPosition();
        Tile tile = board.At(cursorPos);

        if (tile.linkedEntity is Piece piece && piece.owner == currentTurn)
        {
            selectedPiece = piece;
            Debug.Log("Hay una pieza en esta casilla.");
        }
        else if (tile.linkedEntity is HealthPickup pickup && selectedPiece != null)
        {
            selectedPiece.ChangePieceLife(+1f);
            pickup.Collect();
            healthPickups.Remove(pickup);
            tile.linkedEntity = null;
            Debug.Log("Pick-up de salud recogido.");
        }
        else
        {
            selectedPiece = null;
            Debug.Log("No hay pieza en esta casilla.");
        }

    }

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

    void TryMove(Vector2Int direction)
    {
        if (selectedPiece != null)
        {
            MoveSelectedPiece(direction);
        }
        else
        {
            cursor.Move(direction);
            Debug.Log($"Posición del cursor: ({cursor.GetPosition().x}, {cursor.GetPosition().y})");
        }
    }

    void MoveSelectedPiece(Vector2Int direction)
    {
        Vector2Int currentPos = selectedPiece.position;
        Vector2Int newPos = currentPos + direction;

        if (!board.IsOOB(newPos))
        {
            Tile currentTile = board.At(currentPos);
            Tile targetTile = board.At(newPos);

            if (targetTile.linkedEntity == null || targetTile.linkedEntity is HealthPickup)
            {
                if (targetTile.linkedEntity is HealthPickup pickup)
                {
                    selectedPiece.ChangePieceLife(+1f);
                    pickup.Collect();
                    healthPickups.Remove(pickup);
                    targetTile.linkedEntity = null;
                }

                currentTile.linkedEntity = null;
                targetTile.linkedEntity = selectedPiece;

                selectedPiece.position = newPos;
                selectedPiece.pieceGO.transform.position = new Vector3(newPos.x, 0, newPos.y);

                cursor.SetPosition(newPos);

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
        currentPlayer.text = currentTurn.ToString();
        selectedPiece = null;
        Debug.Log($"Turno cambiado. Ahora juega: {currentTurn}");

        SetCursorMaterial(currentTurn);

        SpawnChests();

        TurnTime turnTime = FindFirstObjectByType<TurnTime>();
        if (turnTime != null)
        {
            turnTime.ResetTime();
            turnTime.SetBarForPlayer(currentTurn);
        }
    }


    bool IsMyTurn()
    {
        return true;
    }

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
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
        };

        foreach (var dir in directions)
        {
            Vector2Int targetPos = selectedPiece.position + dir;
            if (board.IsOOB(targetPos)) continue;

            Tile targetTile = board.At(targetPos);

            // SOLO SE ATACA SI LA ENTIDAD TIENE IOnAttack
            if (targetTile.linkedEntity is IOnAttack attackable)
            {
                attackable.OnAttacked(1f);
                PlayAttackParticle(targetTile.tileGO.transform.position, selectedPiece.owner);
                Debug.Log($"¡Entidad atacada en ({targetPos.x}, {targetPos.y})!");
            }
            // SI ES ENEMIGO OTRA LOGICA
            else if (targetTile.linkedEntity is Piece piece && piece.owner != selectedPiece.owner)
            {
                piece.ChangePieceLife(-0.5f);
                PlayAttackParticle(targetTile.tileGO.transform.position, selectedPiece.owner);
                Debug.Log($"¡Pieza atacada en ({targetPos.x}, {targetPos.y})!");
            }
        }

        selectedPiece.coolDown = GetCooldownForPlayer(selectedPiece.owner);
        SetPieceCooldownVisual(selectedPiece, true);

    }

    void UpdateCooldowns()
    {
        foreach (var piece in LightBluePieces)
            UpdatePieceCooldown(piece);
        foreach (var piece in PinkPieces)
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

    void SpawnChests()
    {
        int chestCount = Mathf.Min(Random.Range(minChests, maxChests + 1), maxChests - chests.Count);
        int tries = 0;
        while (chests.Count < maxChests && chestCount > 0 && tries < 100)
        {
            Vector2Int pos = new Vector2Int(Random.Range(0, tileCountX), Random.Range(0, tileCountZ));
            Tile tile = board.At(pos);
            if (tile.linkedEntity == null)
            {
                Chest chest = new Chest(pos, chestPrefab, this);
                chests.Add(chest);
                tile.linkedEntity = chest;
                chestCount--;
            }
            tries++;
        }
    }

    //COOLDOWN DINAMICO

    int GetPlayerPieceCount(PieceOwner owner)
    {
        if (owner == PieceOwner.Player1)
            return LightBluePieces.Count;
        else if (owner == PieceOwner.Player2)
            return PinkPieces.Count;
        return 0;
    }

    float GetCooldownForPlayer(PieceOwner owner)
    {
        int myCount = GetPlayerPieceCount(owner);
        int enemyCount = GetPlayerPieceCount(owner == PieceOwner.Player1 ? PieceOwner.Player2 : PieceOwner.Player1);

        if (myCount < enemyCount)
        {
            // Diferencia máxima posible
            int maxDiff = Mathf.Max(GetPlayerPieceCount(PieceOwner.Player1), GetPlayerPieceCount(PieceOwner.Player2)) - 1;
            int diff = enemyCount - myCount;
            // Interpolación lineal: 2.0 (sin desventaja) a 0.5 (máxima desventaja)
            float cooldown = Mathf.Lerp(2.0f, 0.5f, maxDiff == 0 ? 0 : (float)diff / maxDiff);
            return Mathf.Clamp(cooldown, 0.5f, 2.0f);
        }
        else
        {
            return 2.0f;
        }
    }

    public void SpawnHealthPickup(Vector2Int pos)
    {
        Tile tile = board.At(pos);
        HealthPickup pickup = new HealthPickup(pos, healthPickupPrefab);
        healthPickups.Add(pickup);
        tile.linkedEntity = pickup;
    }

    //PARTICULAS
    void PlayAttackParticle(Vector3 position, PieceOwner owner)
    {
        if (attackParticlePrefab == null) return;

        GameObject particle = Instantiate(attackParticlePrefab, position + Vector3.up * 0.5f, Quaternion.identity);

        // CAMBIO DE COLOR
        var main = particle.GetComponent<ParticleSystem>().main;
        if (owner == PieceOwner.Player1)
            main.startColor = lightBlueMaterial.color;
        else
            main.startColor = pinkMaterial.color;

        Destroy(particle, 2f);
    }

    //SE DETECTA EL FINAL DE LA PARTIDA
    public void CheckForGameEnd()
    {
        int player1Count = GetPlayerPieceCount(PieceOwner.Player1);
        int player2Count = GetPlayerPieceCount(PieceOwner.Player2);

        if (player1Count == 0)
        {
            ShowVictory(PieceOwner.Player2);
        }
        else if (player2Count == 0)
        {
            ShowVictory(PieceOwner.Player1);
        }
    }

    void ShowVictory(PieceOwner winner)
    {
        if (victoryText != null)
        {
            victoryText.text = $"{winner} won!";
            victoryText.gameObject.SetActive(true);
        }
        if (restartButton != null)
            restartButton.SetActive(true);

        Time.timeScale = 0f; 
        gameEnded = true;   
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        gameEnded = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }



}
