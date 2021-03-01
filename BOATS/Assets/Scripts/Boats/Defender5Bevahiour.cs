using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Defender5Bevahiour : BoatBehavior
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
        int[] fired = new int[range];

        // do all damage first, finding all targets hit
        for (int i = 0; i < range; i++)
        {
            if (_tileGrid.ApplyDamage(_boatTileInfo.type,
                                        firingRange[0, 0],
                                        firingRange[0, i],
                                        damage, false))
            {
                fired[i] = i;
                attackSuccessful = true;
            }
        }
        // do animation, but no damage
        if (attackSuccessful)
        {
            _tileGrid.ApplyDamage(_boatTileInfo.type,
                                firingRange[0, 0],
                                firingRange[0, fired.Max()],
                                0);
        }
        
        return attackSuccessful;
    }

    public override Vector2Int[,] GetFiringRange(Vector2Int position)
    {

        Vector2Int[,] firingRange = new Vector2Int[_boatTileInfo.tilesTall, range];

        //////////////////////////////////////
        // Front Range
        for (int gun = 0; gun < _boatTileInfo.tilesTall; gun++)
        {
            Vector2Int baseLocation = position;
            for (int i = 0; i < range; i++)
            {
                switch (_boatTileInfo.rotation)
                {
                    case OccupierRotation.Clockwise90:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x += gun;
                        firingRange[gun, i].y += i;
                        break;

                    case OccupierRotation.Clockwise180:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x += i;
                        firingRange[gun, i].y -= gun + 1;
                        break;

                    case OccupierRotation.Clockwise270:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x += gun - 1;
                        firingRange[gun, i].y -= i + 1;
                        break;

                    case OccupierRotation.Zero:
                        firingRange[gun, i] = baseLocation;
                        firingRange[gun, i].x -= i + 1;
                        firingRange[gun, i].y += gun;
                        break;

                    default:
                        throw new System.Exception("Unassigned OccupierRotation");
                }
            }
        }
        return firingRange;
    }
}
