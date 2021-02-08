using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSystem : MonoBehaviour
{
    private Tile[,] _tileArray;
    private Tilemap _tilemap;

    // Start is called before the first frame update
    void Start()
    {
        _tileArray = new Tile[TileConstants.TileMapWidth, TileConstants.TileMapHeight];
        _tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaceShip(FriendlyBoat boat, Vector2Int[] occupyingTiles)
    {
        // 1. Check if you can place
        bool canPlace = true;

        foreach (Vector2Int tile in occupyingTiles)
        {
            if (!ValidateTileCoordinate(tile) || _tileArray[tile.x, tile.y].status != TileStatus.Empty)
            {
                canPlace = false;
            }
        }

        // 2. Place it
        if (canPlace)
        {

        }
    }

    private bool ValidateTileCoordinate(Vector2Int tileCoordinate)
    {
        return tileCoordinate.x >= 0 && tileCoordinate.x < TileConstants.TileMapWidth
            && tileCoordinate.y >= 0 && tileCoordinate.y < TileConstants.TileMapHeight;
    }

    //private Vector3 TranslateTileToCoordinates(Vector2Int tileCoordinate)
    //{

    //}
}
