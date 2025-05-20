using UnityEngine;

public class Piece
{
    public PieceOwner owner;
    public Vector2Int position;
    public int life = 3;
    public GameObject pieceGO;
    public float coolDown = 2;

    public Piece(PieceOwner owner, Vector2Int position, GameObject piecePrefab)
    {
        this.owner = owner;
        this.position = position;

        // Instanciar
        pieceGO = Object.Instantiate(piecePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        pieceGO.name = $"Piece_{owner}_{position.x}_{position.y}";
    }

}
