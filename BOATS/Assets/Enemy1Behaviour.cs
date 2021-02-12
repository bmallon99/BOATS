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
        Vector2Int[] occupyingTiles = _boatTileInfo.GetTilesOccupied(currLocation);
        Vector2Int newLocation;
        TileOccupier blocker;
        switch (_boatTileInfo.rotation)
        {
            case OccupierRotation.Zero:
                newLocation = new Vector2Int(currLocation.x - 1, currLocation.y);
                blocker = _tileGrid.CheckHit(newLocation);
                if (blocker)
                {
                    blocker.TakeDamage(damage);
                }
                else
                {
                    Vector3 boatPosition = _tileGrid.TileToWorldPoint(_boatTileInfo.GetFocusCoordinate(newLocation));
                    Vector2Int[] newOccupyingTiles = _boatTileInfo.GetTilesOccupied(newLocation);
                    transform.position = boatPosition;
                    _tileGrid.UpdateShip(_boatTileInfo, occupyingTiles, newOccupyingTiles);
                }
                break;

            case OccupierRotation.Clockwise90:
                newLocation = new Vector2Int(currLocation.x, currLocation.y+1);
                blocker = _tileGrid.CheckHit(newLocation);
                if (blocker)
                {
                    blocker.TakeDamage(damage);
                }
                else
                {
                    Vector3 boatPosition = _tileGrid.TileToWorldPoint(_boatTileInfo.GetFocusCoordinate(newLocation));
                    Vector2Int[] newOccupyingTiles = _boatTileInfo.GetTilesOccupied(newLocation);
                    transform.position = boatPosition;
                    _tileGrid.UpdateShip(_boatTileInfo, occupyingTiles, newOccupyingTiles);
                }
                break;

            case OccupierRotation.Clockwise180:
                newLocation = new Vector2Int(currLocation.x-1, currLocation.y);
                blocker = _tileGrid.CheckHit(newLocation);
                if (blocker)
                {
                    blocker.TakeDamage(damage);
                }
                else
                {
                    Vector3 boatPosition = _tileGrid.TileToWorldPoint(_boatTileInfo.GetFocusCoordinate(newLocation));
                    Vector2Int[] newOccupyingTiles = _boatTileInfo.GetTilesOccupied(newLocation);
                    transform.position = boatPosition;
                    _tileGrid.UpdateShip(_boatTileInfo, occupyingTiles, newOccupyingTiles);
                }
                break;

            case OccupierRotation.Clockwise270:
                newLocation = new Vector2Int(currLocation.x, currLocation.y - 1);
                blocker = _tileGrid.CheckHit(newLocation);
                if (blocker)
                {
                    blocker.TakeDamage(damage);
                }
                else
                {
                    Vector3 boatPosition = _tileGrid.TileToWorldPoint(_boatTileInfo.GetFocusCoordinate(newLocation));
                    Vector2Int[] newOccupyingTiles = _boatTileInfo.GetTilesOccupied(newLocation);
                    transform.position = boatPosition;
                    _tileGrid.UpdateShip(_boatTileInfo, occupyingTiles, newOccupyingTiles);
                }
                break;

            default:
                throw new System.Exception("Unassigned OccupierRotation");
        }
    }
}
