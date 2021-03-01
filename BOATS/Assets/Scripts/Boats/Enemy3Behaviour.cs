using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3Behaviour : BoatBehavior
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
        if (!Move())
        {
            Attack();
        }
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
        Vector2Int nextPosition = NextForwardPosition(_boatTileInfo, BoatPosition);
        return _tileGrid.ApplyDamage(_boatTileInfo.type, BoatPosition, nextPosition, damage);
    }
}
