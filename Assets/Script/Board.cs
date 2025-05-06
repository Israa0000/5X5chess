using UnityEngine;

public class Board
{
    public int tileCountX;
    public int tileCountZ;
    public Tile[,] tiles;

    public Board(int tileCountX, int tileCountZ, GameObject whiteTile, GameObject blackTile, Transform boardParent)
    {
        this.tileCountX = tileCountX;
        this.tileCountZ = tileCountZ;

        tiles = new Tile[tileCountX, tileCountZ];

        for (int x = 0; x < tileCountX; x++)
        {
            for (int z = 0; z < tileCountZ; z++)
            {
                GameObject prefab = (x + z) % 2 == 0 ? whiteTile : blackTile;
                tiles[x, z] = new Tile(x, z, prefab, boardParent);
            }
        }
    }
}
