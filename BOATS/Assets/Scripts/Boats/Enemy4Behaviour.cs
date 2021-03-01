using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy4Behaviour : BoatBehavior
{
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
        Move();
        Attack();
    }

    protected override bool Move()
    {
        Vector2Int[] occupyingTiles = _boatTileInfo.GetTilesOccupied(BoatPosition);
        Vector2Int nextPosition = NextForwardPosition(_boatTileInfo, BoatPosition);
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

    protected override bool Attack()
    {

        bool attackSuccessful = false;
        Vector2Int[,] firingRange = GetFiringRange(_tileGrid.WorldToTilePoint(transform.position));

        for (int gun = 0; gun < 3; gun++)
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
        // 3 guns, so 3 rows
        Vector2Int[,] firingRange = new Vector2Int[3, range];
        
        for (int i = 0; i < range; i++)
        {
            switch (_boatTileInfo.rotation)
            {
                case OccupierRotation.Zero:
                    // forward gun
                    firingRange[0, i] = new Vector2Int(position.x - (1 + i), position.y);
                    // top middle gun
                    firingRange[1, i] = new Vector2Int(position.x + 1, position.y + (1 + i));
                    // bottom right gun
                    firingRange[2, i] = new Vector2Int(position.x + 2, position.y - (1 + i));
                    break;

                case OccupierRotation.Clockwise90:
                    // forward gun
                    firingRange[0, i] = new Vector2Int(position.x, position.y + i);
                    // top middle gun
                    firingRange[1, i] = new Vector2Int(position.x + (1 + i), position.y - 2);
                    // bottom right gun
                    firingRange[2, i] = new Vector2Int(position.x - (1 + i), position.y - 3);
                    break;

                case OccupierRotation.Clockwise180:
                    // forward gun
                    firingRange[0, i] = new Vector2Int(position.x + i, position.y - 1);
                    // top middle gun
                    firingRange[1, i] = new Vector2Int(position.x - 2, position.y - (2 + i));
                    // bottom right gun
                    firingRange[2, i] = new Vector2Int(position.x - 3, position.y + i);
                    break;

                case OccupierRotation.Clockwise270:
                    // forward gun
                    firingRange[0, i] = new Vector2Int(position.x - 1, position.y - (1 + i));
                    // top middle gun
                    firingRange[1, i] = new Vector2Int(position.x - (2 + i), position.y + 1);
                    // bottom right gun
                    firingRange[2, i] = new Vector2Int(position.x + i, position.y + 2);
                    break;

                default:
                    throw new System.Exception("Unassigned OccupierRotation");
            }
        }
        return firingRange;
    }
}
