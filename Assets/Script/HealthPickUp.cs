using UnityEngine;

public class HealthPickup : IOnAttack
{
    public Vector2Int position;
    public GameObject pickupGO;

    public HealthPickup(Vector2Int pos, GameObject prefab)
    {
        position = pos;
        pickupGO = Object.Instantiate(prefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
    }

    public void Collect()
    {
        Object.Destroy(pickupGO);
    }

    public void OnAttacked(float amount)
    {
        Collect();
    }
}
