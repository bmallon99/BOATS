using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy1Behaviour : BoatBehavior
{
    public int damage;
    private TileSystem _tileGrid;
    private TileOccupier _boatTileInfo;

    // Start is called before the first frame update
    public void Start()
    {
        _boatTileInfo = GetComponent<TileOccupier>();
        _tileGrid = FindObjectOfType<TileSystem>();
    }

    public override void Turn()
    {
        if (!Move())
        {
            Attack();
        }
    }

    // returns the next position location 
    private Vector2Int FindNextPosition(Vector2Int currPosition)
    {
        switch (_boatTileInfo.rotation)
        {
            case OccupierRotation.Zero:
                return new Vector2Int(currPosition.x - 1, currPosition.y);
            case OccupierRotation.Clockwise90:
                return new Vector2Int(currPosition.x, currPosition.y + 1);
            case OccupierRotation.Clockwise180:
                return new Vector2Int(currPosition.x + 1, currPosition.y);
            case OccupierRotation.Clockwise270:
                return new Vector2Int(currPosition.x, currPosition.y - 1);
            default:
                throw new System.Exception("Unassigned OccupierRotation");
        }
    }

    protected override bool Move()
    {
        Vector2Int[] occupyingTiles = _boatTileInfo.GetTilesOccupied(BoatPosition);
        Vector2Int nextPosition = FindNextPosition(BoatPosition);
        Vector2Int[] nextOccupyingTiles = _boatTileInfo.GetTilesOccupied(nextPosition);
        if (_tileGrid.TryMove(_boatTileInfo, occupyingTiles, nextOccupyingTiles))
        {
            Vector3 boatTransformPosition = _tileGrid.TileToWorldPoint(_boatTileInfo.GetFocusCoordinate(nextPosition));
            transform.position = boatTransformPosition;
            MyHealthText.transform.position = AdjustDamageBarPosition(boatTransformPosition);
            BoatPosition = nextPosition;
            return true;
        }
        return false;
    }

    protected override bool Attack() {
        Vector2Int nextPosition = FindNextPosition(BoatPosition);
        if (_tileGrid.ApplyDamage(_boatTileInfo.type, nextPosition, damage))
        {
            // FIREEEE
            return true;
        }
        return false;
    }
}
