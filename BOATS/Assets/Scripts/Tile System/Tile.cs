using UnityEngine;

enum TileStatus
{
    Empty,
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

class Tile
{
    public TileStatus status;
    public GameObject occupyingObject;

}