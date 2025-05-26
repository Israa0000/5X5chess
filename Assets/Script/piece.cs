using UnityEngine;

public class Piece
{
    public PieceOwner owner;
    public Vector2Int position;
    public float life = 1.5f;
    public GameObject pieceGO;
    public float coolDown = 2;
    Board board;

    public Piece(PieceOwner owner, Vector2Int position, GameObject piecePrefab, float baseHeight)
    {
        this.owner = owner;
        this.position = position;
        pieceGO = Object.Instantiate(piecePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        pieceGO.name = $"Piece_{owner}_{position.x}_{position.y}";
        Vector3 scale = pieceGO.transform.localScale;
        scale.y = baseHeight;
        pieceGO.transform.localScale = scale;
    }

    public void SetBoard(Board board)
    {
        this.board = board;
    }

    public void ChangePieceLife(float delta)
    {
        life += delta;
        UpdatePieceHeight();

        if (life <= 0)
        {
            RemovePiece();
        }
    }

    public void UpdatePieceHeight()
    {
        if (pieceGO != null)
        {
            Vector3 scale = pieceGO.transform.localScale;
            scale.y = Mathf.Max(0.1f, life);
            pieceGO.transform.localScale = scale;
        }
    }

    public void RemovePiece()
    {
        if (board != null)
            board.At(position).linkedEntity = null;

        if (pieceGO != null)
            Object.Destroy(pieceGO);

        var gm = GameObject.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            if (owner == PieceOwner.Player1)
                gm.LightBluePieces.Remove(this);
            else if (owner == PieceOwner.Player2)
                gm.PinkPieces.Remove(this);

            gm.CheckForGameEnd();
        }

        Debug.Log("¡Pieza destruida!");
    }



}
