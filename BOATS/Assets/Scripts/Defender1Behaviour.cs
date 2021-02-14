using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Defender1Behaviour : BoatBehavior
{
    public int range;
    public int damage;
    TileSystem _tileGrid;
    TileOccupier _boatTileInfo;
    // Start is called before the first frame update
    void Start()
    {
        _boatTileInfo = GetComponent<TileOccupier>();
        _tileGrid = FindObjectOfType<TileSystem>();
        InitHealthBar();
    }

    public override void Turn()
    {
        Attack();
    }

    protected override bool Move() { return false; }

    protected override bool Attack()
    {
        bool attackSuccessful = false;
        for (int gun = 0; gun < _boatTileInfo.tilesWide; gun++)
        {
            Vector2Int[] firingRange = new Vector2Int[range];
            Vector2Int baseLocation = (Vector2Int)_tileGrid.WorldToTilePoint(transform.position);
            for (int i=0; i<range; i++)
            {
                switch (_boatTileInfo.rotation)
                {
                    case OccupierRotation.Zero:
                        firingRange[i] = baseLocation;
                        firingRange[i].x += gun;
                        firingRange[i].y += i + 1;
                        break;

                    case OccupierRotation.Clockwise90:
                        firingRange[i] = baseLocation;
                        firingRange[i].x += i + 1;
                        firingRange[i].y -= gun + 1;
                        break;

                    case OccupierRotation.Clockwise180:
                        firingRange[i] = baseLocation;
                        firingRange[i].x -= gun + 1;
                        firingRange[i].y -= i + 2;
                        break;

                    case OccupierRotation.Clockwise270:
                        firingRange[i] = baseLocation;
                        firingRange[i].x -= i + 2;
                        firingRange[i].y += gun;
                        break;

                    default:
                        throw new System.Exception("Unassigned OccupierRotation");
                }
            }
            foreach (Vector2Int location in firingRange)
            {
                if (_tileGrid.ApplyDamage(_boatTileInfo.type, location, damage))
                {
                    attackSuccessful = true;
                    break;
                }

                //TileOccupier hit = _tileGrid.CheckHit(location);
                //if (hit && hit.type==TileOccupierType.Enemy)
                //{
                //    _tileGrid.Fire(firingRange[0], location);
                //    hit.TakeDamage(damage);
                //    break;
                //}
            }
        }
        return attackSuccessful;
    }
}
