using UnityEngine;

public class Tile
{
    public int x;
    public int z;
    public object linkedEntity = null; // Puede ser Piece, Chest, HealthPickup, etc.
    public GameObject tileGO;

    public Tile(int x, int z, GameObject prefab, Transform parent)
    {
        this.x = x;
        this.z = z;
        tileGO = Object.Instantiate(prefab, parent);
        tileGO.transform.position = new Vector3(x * 1, 0, z * 1);
        tileGO.name = $"Tile_{x}_{z}";
    }
}
