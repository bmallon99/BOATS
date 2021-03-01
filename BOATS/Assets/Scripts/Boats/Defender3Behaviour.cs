using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender3Behaviour : BoatBehavior
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
        Vector2Int[] startPositions = GetStartPositions(_tileGrid.WorldToTilePoint(transform.position));

        for (int gun = 0; gun < _boatTileInfo.tilesWide; gun++)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_tileGrid.ApplyDamage(_boatTileInfo.type,
                                          startPositions[gun],
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
        Vector2Int[,] firingRange = new Vector2Int[_boatTileInfo.tilesWide, 3];
        for (int gun = 0; gun < _boatTileInfo.tilesWide; gun++)
        {
            Vector2Int baseLocation = position;
            for (int i = 0; i < 3; i++)
            {
                switch (_boatTileInfo.rotation)
                {
                    case OccupierRotation.Zero:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x += gun;
                        firingRange[gun, i].y += (range-2) + i + 1;
                        break;

                    case OccupierRotation.Clockwise90:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x += (range - 2) + i + 1;
                        firingRange[gun, i].y -= gun + 1;
                        break;

                    case OccupierRotation.Clockwise180:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x -= gun + 1;
                        firingRange[gun, i].y -= (range - 2) + i + 2;
                        break;

                    case OccupierRotation.Clockwise270:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x -= (range - 2) + i + 2;
                        firingRange[gun, i].y += gun;
                        break;

                    default:
                        throw new System.Exception("Unassigned OccupierRotation");
                }
            }
        }
        return firingRange;
    }

    private Vector2Int[] GetStartPositions(Vector2Int position)
    {
        Vector2Int[] firingRange = new Vector2Int[3];
        for (int gun = 0; gun < _boatTileInfo.tilesWide; gun++)
        {
            Vector2Int baseLocation = position;
            switch (_boatTileInfo.rotation)
            {
                case OccupierRotation.Zero:
                    firingRange[gun] = baseLocation;
                    firingRange[gun].x += gun;
                    firingRange[gun].y += 1;
                    break;

                case OccupierRotation.Clockwise90:
                    firingRange[gun] = baseLocation;
                    firingRange[gun].x += 1;
                    firingRange[gun].y -= gun + 1;
                    break;

                case OccupierRotation.Clockwise180:
                    firingRange[gun] = baseLocation;
                    firingRange[gun].x -= gun + 1;
                    firingRange[gun].y -= 2;
                    break;

                case OccupierRotation.Clockwise270:
                    firingRange[gun] = baseLocation;
                    firingRange[gun].x -= 2;
                    firingRange[gun].y += gun;
                    break;

                default:
                    throw new System.Exception("Unassigned OccupierRotation");
            }
        }
        return firingRange;
    }
}
