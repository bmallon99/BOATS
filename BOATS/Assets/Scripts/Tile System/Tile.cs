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

public enum Quadrant
{
    Top,
    Right,
    Bottom,
    Left
}

public class EnemySpawnTile
{
    public Quadrant Quadrant;
    public Vector2Int TilePosition;
    public bool Active;

    public EnemySpawnTile(Quadrant quadrant, Vector2Int position, bool active)
    {
        Quadrant = quadrant;
        TilePosition = position;
        Active = active;
    }
}