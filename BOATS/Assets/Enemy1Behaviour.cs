using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Behaviour : EnemyBoatBehaviour
{
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

    }
}
