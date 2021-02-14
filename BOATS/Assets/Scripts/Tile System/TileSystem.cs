using System;
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

    private readonly System.Random _random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        _tileArray = new TileOccupier[TileConstants.TileMapWidth, TileConstants.TileMapHeight];
        _tilemap = GetComponent<Tilemap>();
        _selectedTiles = new Vector2Int[0];
        _ticksSinceLastSpawn = 1;
        _friendlyBoats = new List<GameObject>();
        _enemyBoats = new List<GameObject>();

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
        // All enemies move (and act)
        foreach (GameObject enemy in _enemyBoats)
        {
            enemy.GetComponent<BoatBehavior>().Turn();
        }

        // Spawning based on specified interval
        if (_ticksSinceLastSpawn > SpawnInterval)
        {
            PlaceShip(EnemyBoatPrefabs[0], GetSpawnLocation());
            _ticksSinceLastSpawn = 1;
        }
        
        // All friendlies check if they can fire and do so
        foreach (GameObject friend in _friendlyBoats)
        {
            friend.GetComponent<BoatBehavior>().Turn();
        }
        _ticksSinceLastSpawn++;
        yield return new WaitForSeconds(1f);
        StartCoroutine(MainTimerCoroutine());
    }


    // MARK - Ship Placement

    public bool PlaceShip(GameObject boatPrefab, Vector2Int location)
    {
        if (IsTilePointInBounds(location))
        {
            TileOccupier occupier = boatPrefab.GetComponent<TileOccupier>();
            Vector2Int boatCoordinate = occupier.GetFocusCoordinate(location);
            Vector3 boatPosition = TileToWorldPoint(boatCoordinate);

            Vector2Int[] occupyingTiles = occupier.GetTilesOccupied(location);

            // If all tiles are empty (i.e. valid for placing)
            if (occupyingTiles.All(tile => IsTileEmpty(tile)))
            {
                GameObject newShip = Instantiate(boatPrefab, boatPosition, Quaternion.Euler(Vector3.back * (int)occupier.rotation));
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

                newShipOccupier.GetComponent<BoatBehavior>().BoatPosition = location;
                return true;
            }
        }

        return false;
    }

    public bool TryMove(TileOccupier occupier, Vector2Int[] oldTiles, Vector2Int[] newTiles)
    {
        if (newTiles.All(tile => {
            return IsTileEmpty(tile) || oldTiles.Contains(tile);
        }))
        {
            foreach (Vector2Int tile in oldTiles)
            {
                _tileArray[tile.x, tile.y] = null;
            }
            foreach (Vector2Int tile in newTiles)
            {
                _tileArray[tile.x, tile.y] = occupier;
            }
            return true;
        }
        return false;
    }


    // MARK - Enemy Ship Spawning
    

    public Vector2Int GetSpawnLocation()
    {
        int quadrant = _random.Next(4);
        TileOccupier occupier = EnemyBoatPrefabs[0].GetComponent<TileOccupier>();

        switch (quadrant)
        {
            case 0:
                occupier.rotation = (OccupierRotation)0;
                return new Vector2Int(30, 12);

            case 1:
                occupier.rotation = (OccupierRotation)90;
                return new Vector2Int(16, 1);
                

            case 2:
                occupier.rotation = (OccupierRotation)180;
                return new Vector2Int(1, 12);

            case 3:
                occupier.rotation = (OccupierRotation)270;
                return new Vector2Int(16, 22);

            default:
                throw new System.Exception("Random Failed");
        }
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


    public void Fire(Vector2Int Start, Vector2Int target)
    {
        //graphical aspect of shooting
    }

    // Returns true if hit something
    public bool ApplyDamage(TileOccupierType src, Vector2Int location, int damage)
    {
        if (IsTilePointInBounds(location) && !IsTileEmpty(location))
        {
            TileOccupier dst = _tileArray[location.x, location.y];
            if (src == dst.type)
            {
                return false;
            }
            else if (src == TileOccupierType.Friendly && dst.type == TileOccupierType.Base)
            {
                return false;
            }

            dst.TakeDamage(damage);
            return true;
        }
        return false;
    }

    // Old firing implementation, can use to show range with some changes
    IEnumerator ShowRange(Vector2Int[] tiles)
    {
        foreach (Vector2Int location in tiles)
        {
            if (IsTilePointInBounds(location))
            {
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


    // MARK - Ship Death


    public void Died(GameObject deadBoat, Vector2Int location)
    {
        TileOccupier occupier = deadBoat.GetComponent<TileOccupier>();
        Vector3 boatPosition = TileToWorldPoint(occupier.GetFocusCoordinate(location));

        Vector2Int[] occupyingTiles = occupier.GetTilesOccupied(location);

        foreach (Vector2Int tile in occupyingTiles)
        {
            _tileArray[tile.x, tile.y] = null;
        }

        if (occupier.type == TileOccupierType.Friendly)
        {
            _friendlyBoats.Remove(deadBoat);
        }
        else
        {
            _enemyBoats.Remove(deadBoat);
        }
        Destroy(deadBoat);
    }
}
