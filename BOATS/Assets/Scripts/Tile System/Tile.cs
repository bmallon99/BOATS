using UnityEngine;

public enum TileOccupierType
{
    Friendly,
    Enemy,
    Base,
    EnemySpawn
}

static class TileConstants
{
    public static int TileMapWidth = 32;
    public static int TileMapHeight = 24;
}