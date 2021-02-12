using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TileSystem : MonoBehaviour
{
    public int SpawnInterval;
    private int _ticksSinceLastSpawn;
    private TileOccupier[,] _tileArray;
    private Tilemap _tilemap;
    private Vector2Int[] _selectedTiles;
    private List<GameObject> _friendlyBoats;
    private List<GameObject> _enemyBoats;

    //possibly temporary
    public GameObject[] EnemyBoatPrefabs;
    

    // Start is called before the first frame update
    void Start()
    {
        _tileArray = new TileOccupier[TileConstants.TileMapWidth, TileConstants.TileMapHeight];
        _tilemap = GetComponent<Tilemap>();
        _selectedTiles = new Vector2Int[0];
        _ticksSinceLastSpawn = 1;

        // Clear Tile Flags
        for (int i = 0; i < TileConstants.TileMapWidth; i++)
        {
            for (int j = 0; j < TileConstants.TileMapHeight; j++)
            {
                _tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
            }
        }
        // Start main game loop
        StartCoroutine(MainTimerCoroutine());
    }


    // MARK - Main Game Loop


    IEnumerator MainTimerCoroutine()
    {
        // Spawning based on specified interval
        if (_ticksSinceLastSpawn > SpawnInterval)
        {
            PlaceShip(EnemyBoatPrefabs[0], GetSpawnLocation());
            _ticksSinceLastSpawn = 1;
        }
        // All enemies move (and act)
        foreach (GameObject enemy in _enemyBoats)
        {
            enemy.GetComponent<EnemyBoatBehaviour>().Move();
        }
        // All friendlies check if they can fire and do so
        foreach (GameObject friend in _friendlyBoats)
        {
            friend.GetComponent<FriendlyBoatBehaviour>().CheckFire();
        }
        _ticksSinceLastSpawn++;
        yield return new WaitForSeconds(1f);
        StartCoroutine(MainTimerCoroutine());
    }


    // MARK - Ship Placement


    public bool PlaceShip(GameObject boat, Vector2Int location)
    {
        if (IsTilePointInBounds(location))
        {
            TileOccupier occupier = boat.GetComponent<TileOccupier>();
            Vector3 boatPosition = TileToWorldPoint(occupier.GetFocusCoordinate(location));

            Vector2Int[] occupyingTiles = occupier.GetTilesOccupied(location);

            // If all tiles are empty (i.e. valid for placing)
            if (occupyingTiles.All(tile => IsTileEmpty(tile)))
            {
                GameObject newShip = Instantiate(boat, boatPosition, Quaternion.Euler(Vector3.back * (int)occupier.rotation));
                TileOccupier newShipOccupier = newShip.GetComponent<TileOccupier>();
                if (newShipOccupier.type==TileOccupierType.Friendly)
                {
                    _friendlyBoats.Add(newShip);
                } else
                {
                    _enemyBoats.Add(newShip);
                }

                foreach (Vector2Int tile in occupyingTiles)
                {
                    _tileArray[tile.x, tile.y] = newShipOccupier;
                }

                return true;
            }
        }

        return false;
    }


    // MARK - Enemy Ship Spawning
    

    public Vector2Int GetSpawnLocation()
    {
        return new Vector2Int(0, 0);
    }


    // MARK - Tile Coordinate System


    public bool IsTilePointInBounds(Vector2Int tileCoordinate)
    {
        return tileCoordinate.x >= 0 && tileCoordinate.x < TileConstants.TileMapWidth
            && tileCoordinate.y >= 0 && tileCoordinate.y < TileConstants.TileMapHeight;
    }

    // Tiles are "empty" if they are valid & don't have something there
    public bool IsTileEmpty(Vector2Int tileCoordinate)
    {
        if (IsTilePointInBounds(tileCoordinate))
        {
            return _tileArray[tileCoordinate.x, tileCoordinate.y] == null;
        }

        return false;
    }

    public Vector3 TileToWorldPoint(Vector2Int tileCoordinate)
    {
        return _tilemap.layoutGrid.CellToWorld(new Vector3Int(tileCoordinate.x, tileCoordinate.y, 0));
    }

    public Vector2Int WorldToTilePoint(Vector3 worldCoordinate)
    {
        return (Vector2Int) _tilemap.layoutGrid.WorldToCell(worldCoordinate);
    }


    // MARK - Selected Tile


    public void SelectTiles(Vector2Int[] tiles)
    {
        foreach (Vector2Int tile in _selectedTiles)
        {
            if (IsTilePointInBounds(tile))
            {
                _tilemap.SetColor(new Vector3Int(tile.x, tile.y, 0), Color.white);
            }
        }

        _selectedTiles = tiles;

        // Check if new tiles are valid
        Color selectionColor = tiles.All(tile => IsTileEmpty(tile)) ? Color.green : Color.red;

        // Set appropriate color for new tiles
        foreach (Vector2Int tile in tiles)
        {
            if (IsTilePointInBounds(tile))
            {
                _tilemap.SetColor(new Vector3Int(tile.x, tile.y, 0), selectionColor);
            }
        }
    }


    // MARK - Shooting Routine


    public void Fire(Vector2Int[] tiles)
    {
        StartCoroutine(FireCoroutine(tiles));
    }


    public TileOccupier CheckHit(Vector2Int location)
    {
        if (IsTilePointInBounds(location) && !IsTileEmpty(location))
        {
            return _tileArray[location.x, location.y];
        }
        else 
        {
            return null;
        }
    }

    IEnumerator FireCoroutine(Vector2Int[] tiles)
    {
        foreach (Vector2Int location in tiles)
        {
            if (IsTilePointInBounds(location))
            {
                // Change this to someting else
                _tilemap.SetColor(new Vector3Int(location.x, location.y, 0), Color.yellow);
            }
        }
        yield return new WaitForSeconds(0.5f);
        foreach (Vector2Int location in tiles)
        {
            if (IsTilePointInBounds(location))
            {
                _tilemap.SetColor(new Vector3Int(location.x, location.y, 0), Color.white);
            }
        }
    }
}
