﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSystem : MonoBehaviour
{
    public GameObject prefab;
    private TileOccupier[,] _tileArray;
    private Tilemap _tilemap;

    // Start is called before the first frame update
    void Start()
    {
        _tileArray = new TileOccupier[TileConstants.TileMapWidth, TileConstants.TileMapHeight];
        _tilemap = GetComponent<Tilemap>();

        PlaceShip(prefab, new Vector2Int(2, 2));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool TilesAreEmpty(Vector2Int[] locations)
    {
        foreach (Vector2Int location in locations)
        {
            if (_tileArray[location.x, location.y] != null)
            {
                return false;
            }
        }

        return true;
    }

    public bool PlaceShip(GameObject boat, Vector2Int location)
    {
        if (IsValidTileCoordinate(location))
        {
            TileOccupier occupier = boat.GetComponent<TileOccupier>();
            Vector3 boatPosition = TranslateTileToCoordinates(occupier.GetFocusCoordinate(location));

            Vector2Int[] occupyingTiles = occupier.GetTilesOccupied(location);

            if (TilesAreEmpty(occupyingTiles))
            {
                Instantiate(boat, boatPosition, Quaternion.Euler(Vector3.back * (int)occupier.rotation));

                foreach (Vector2Int tile in occupier.GetTilesOccupied(location))
                {
                    _tileArray[tile.x, tile.y] = occupier;
                }

                return true;
            }
        }

        return false;
    }

    private bool IsValidTileCoordinate(Vector2Int tileCoordinate)
    {
        return tileCoordinate.x >= 0 && tileCoordinate.x < TileConstants.TileMapWidth
            && tileCoordinate.y >= 0 && tileCoordinate.y < TileConstants.TileMapHeight;
    }

    private Vector3 TranslateTileToCoordinates(Vector2Int tileCoordinate)
    {
        Vector3 worldCoordinates = _tilemap.layoutGrid.CellToWorld(new Vector3Int(tileCoordinate.x, tileCoordinate.y, 0));

        // Adjust so it's visible (in front of the tilemap)
        worldCoordinates.z = -1;
        return worldCoordinates;
    }
}
