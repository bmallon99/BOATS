using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Behaviour : EnemyBoatBehaviour
{
    public int damage;
    TileSystem _tileGrid;
    TileOccupier _boatTileInfo;
    // Start is called before the first frame update
    void Start()
    {
        _boatTileInfo = GetComponent<TileOccupier>();
        _tileGrid = FindObjectOfType<TileSystem>();
    }

    public override void Move()
    {
        Vector2Int currLocation = (Vector2Int)_tileGrid.WorldToTilePoint(transform.position);
        Vector2Int[] occupyingTiles;
        Vector2Int newLocation;
        switch (_boatTileInfo.rotation)
        { 
            case OccupierRotation.Zero:
                occupyingTiles = _boatTileInfo.GetTilesOccupied(currLocation);
                newLocation = new Vector2Int(currLocation.x - 1, currLocation.y);
                attackOrMove(currLocation, newLocation, occupyingTiles);
                break;

            case OccupierRotation.Clockwise90:
                // calculate backwards from position to find actual boat placement
                currLocation = new Vector2Int(currLocation.x, currLocation.y - 1);
                // calculate tiles for that placement
                occupyingTiles = _boatTileInfo.GetTilesOccupied(currLocation);
                // do movement update for that placement
                newLocation = new Vector2Int(currLocation.x, currLocation.y + 1);
                attackOrMove(currLocation, newLocation, occupyingTiles);

                break;

            case OccupierRotation.Clockwise180:
                currLocation = new Vector2Int(currLocation.x - 1, currLocation.y - 1);
                occupyingTiles = _boatTileInfo.GetTilesOccupied(currLocation);
                newLocation = new Vector2Int(currLocation.x+1, currLocation.y);
                attackOrMove(currLocation, newLocation, occupyingTiles);
                break;

            case OccupierRotation.Clockwise270:
                currLocation = new Vector2Int(currLocation.x - 1, currLocation.y);
                occupyingTiles = _boatTileInfo.GetTilesOccupied(currLocation);
                newLocation = new Vector2Int(currLocation.x, currLocation.y-1);
                attackOrMove(currLocation, newLocation, occupyingTiles);
                break;

            default:
                throw new System.Exception("Unassigned OccupierRotation");
        }
    }

    private void attackOrMove(Vector2Int currLocation, Vector2Int newLocation, Vector2Int[] occupyingTiles)
    {
        //check if something is in the way
        TileOccupier blocker = _tileGrid.CheckHit(newLocation);
        if (blocker)
        {
            // attack if supposed to
            if (blocker.type == TileOccupierType.Friendly || blocker.type == TileOccupierType.Base)
            {
                blocker.TakeDamage(damage);
            }
        }
        else
        {
            // move
            Vector3 boatPosition = _tileGrid.TileToWorldPoint(_boatTileInfo.GetFocusCoordinate(newLocation));
            Vector2Int[] newOccupyingTiles = _boatTileInfo.GetTilesOccupied(newLocation);
            transform.position = boatPosition;
            _tileGrid.UpdateShip(_boatTileInfo, occupyingTiles, newOccupyingTiles);
        }
    }
}
