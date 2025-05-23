using UnityEngine;

public class Chest : IOnAttack
{
    public Vector2Int position;
    public GameObject chestGO;
    public GameManager gameManager;

    public Chest(Vector2Int pos, GameObject prefab, GameManager gm)
    {
        position = pos;
        gameManager = gm;
        chestGO = Object.Instantiate(prefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
    }

    public void OnAttacked(float amount)
    {
        Tile tile = gameManager.board.At(position);
        tile.linkedEntity = null;
        gameManager.chests.Remove(this);

        gameManager.SpawnHealthPickup(position);

        Object.Destroy(chestGO);
    }
}
