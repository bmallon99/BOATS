using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender4Behaviour : BoatBehavior
{
    TileSystem _tileGrid;
    TileOccupier _boatTileInfo;
    // Start is called before the first frame update
    public void Start()
    {
        _boatTileInfo = GetComponent<TileOccupier>();
        _tileGrid = FindObjectOfType<TileSystem>();
    }

    public override void Turn()
    {
        Attack();
    }

    protected override bool Move() { return false; }

    protected override bool Attack()
    {
        bool attackSuccessful = false;
        Vector2Int[,] firingRange = GetFiringRange(_tileGrid.WorldToTilePoint(transform.position));

        for (int gun = 0; gun < _boatTileInfo.tilesWide + _boatTileInfo.tilesTall; gun++)
        {
            for (int i = 0; i < range; i++)
            {
                if (_tileGrid.ApplyDamage(_boatTileInfo.type,
                                          firingRange[gun, 0],
                                          firingRange[gun, i],
                                          damage))
                {
                    attackSuccessful = true;
                    break;
                }
            }
        }
        return attackSuccessful;
    }

    public override Vector2Int[,] GetFiringRange(Vector2Int position)
    {

        Vector2Int[,] firingRange = new Vector2Int[_boatTileInfo.tilesWide + _boatTileInfo.tilesTall, range];

        //////////////////////////////////////
        // Side Range
        for (int gun = 0; gun < _boatTileInfo.tilesWide; gun++)
        {
            Vector2Int baseLocation = position;
            for (int i = 0; i < range; i++)
            {
                switch (_boatTileInfo.rotation)
                {
                    case OccupierRotation.Zero:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x += gun;
                        firingRange[gun, i].y += i + _boatTileInfo.tilesTall;
                        break;

                    case OccupierRotation.Clockwise90:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x += i + _boatTileInfo.tilesTall;
                        firingRange[gun, i].y -= gun + 1;
                        break;

                    case OccupierRotation.Clockwise180:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x -= gun + 1;
                        firingRange[gun, i].y -= i + 3;
                        break;

                    case OccupierRotation.Clockwise270:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x -= i + 3;
                        firingRange[gun, i].y += gun;
                        break;

                    default:
                        throw new System.Exception("Unassigned OccupierRotation");
                }
            }
        }

        //////////////////////////////////////
        // Rear Range
        for (int gun = 3; gun < _boatTileInfo.tilesTall+3; gun++)
        {
            int currGun = gun - 3;
            Vector2Int baseLocation = position;
            for (int i = 0; i < range; i++)
            {
                switch (_boatTileInfo.rotation)
                {
                    case OccupierRotation.Clockwise270:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x += currGun - 2;
                        firingRange[gun, i].y += i + _boatTileInfo.tilesWide;
                        break;

                    case OccupierRotation.Zero:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x += i + _boatTileInfo.tilesWide;
                        firingRange[gun, i].y -= currGun - 1;
                        break;

                    case OccupierRotation.Clockwise90:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x += currGun;
                        firingRange[gun, i].y -= i + _boatTileInfo.tilesWide + 1;
                        break;

                    case OccupierRotation.Clockwise180:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x -= i + _boatTileInfo.tilesWide + 1;
                        firingRange[gun, i].y += currGun - 2;
                        break;

                    default:
                        throw new System.Exception("Unassigned OccupierRotation");
                }
            }
        }
        return firingRange;
    }
}
