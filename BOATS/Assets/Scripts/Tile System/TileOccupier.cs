using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OccupierRotation:int
{
    Zero = 0,
    Clockwise90 = 90,
    Clockwise180 = 180,
    Clockwise270 = 270,
}

public class TileOccupier : MonoBehaviour
{
    // Set in Prefab Editor
    public TileOccupierType type;
    public int tilesWide;
    public int tilesTall;
    public int health;

    // Set during Runtime
    public OccupierRotation rotation;

    private TileSystem _tileSystem;


    void Start()
    {
        _tileSystem = FindObjectOfType<TileSystem>();
    }


    // MARK - Location stuff


    // In a non-rotated object, the location should be the bottom left;
    public Vector2Int GetFocusCoordinate(Vector2Int location)
    {
        switch (rotation)
        {
            case OccupierRotation.Zero:
                return location;

            case OccupierRotation.Clockwise90:
                return new Vector2Int(location.x, location.y + 1);

            case OccupierRotation.Clockwise180:
                return new Vector2Int(location.x + 1, location.y + 1);

            case OccupierRotation.Clockwise270:
                return new Vector2Int(location.x + 1, location.y);

            default:
                throw new System.Exception("Unassigned OccupierRotation");
        }
    }

    public Vector2Int[] GetTilesOccupied(Vector2Int location)
    {
        Vector2Int[] tiles = new Vector2Int[tilesWide * tilesTall];
        int idx = 0;

        for (int offsetX = 0; offsetX < tilesWide; offsetX++)
        {
            for (int offsetY = 0; offsetY < tilesTall; offsetY++)
            {
                switch (rotation)
                {
                    case OccupierRotation.Zero:
                        tiles[idx] = new Vector2Int(location.x + offsetX, location.y + offsetY);
                        break;

                    case OccupierRotation.Clockwise90:
                        tiles[idx] = new Vector2Int(location.x + offsetY, location.y - offsetX);
                        break;

                    case OccupierRotation.Clockwise180:
                        tiles[idx] = new Vector2Int(location.x - offsetX, location.y - offsetY);
                        break;

                    case OccupierRotation.Clockwise270:
                        tiles[idx] = new Vector2Int(location.x - offsetY, location.y + offsetX);
                        break;

                    default:
                        throw new System.Exception("Unassigned OccupierRotation");
                }
                idx += 1;
            }
        }

        return tiles;
    }


    // MARK - Health


    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            Vector2Int location = (Vector2Int)_tileSystem.WorldToTilePoint(transform.position);
            _tileSystem.Died(gameObject, location);
        }
    }
}
