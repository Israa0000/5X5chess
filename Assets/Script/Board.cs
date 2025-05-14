using JetBrains.Annotations;
using UnityEngine;

public class Board
{
    public int tileCountX;
    public int tileCountZ;
    public Tile[,] tiles;


    public Board(int tileCountX, int tileCountZ, GameObject whiteTile, GameObject blackTile, Transform boardParent, Piece LinkedEntity)
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

        if (tiles[1, 1].linkedEntity == null)
            Debug.Log("vacía");
        else
            Debug.Log("ocupada");

    }
    public void At()
    {

    }
}
