using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;

public class TileSystem : MonoBehaviour
{
    private int _ticksSinceLastSpawn;
    private int _quadrantsActive;
    private TileOccupier[,] _tileArray;
    private Tilemap _tilemap;
    private Vector2Int[] _selectedTiles;
    private Vector2Int[,] _selectedRangeTiles;
    private List<GameObject> _friendlyBoats;
    private List<GameObject> _enemyBoats;
    private EnemySpawnTile[] _enemySpawnTiles;
    private Vector2Int[] _enemySpawnPositions;
    private Text _warningText;
    private Text _moneyText;
    private Text _scoreText;

    // Curreny and Score
    private int _score;
    private int _money;
    public int score
    {
        get => _score;
        set
        {
            _score = value;
            _scoreText.text = "Score " + _score.ToString();
        }
    }
    public int money
    {
        get => _money;
        set
        {
            _money = value;
            _moneyText.text = _money.ToString();
        }
    }

    // Unity Editor
    public int SpawnInterval;
    public GameObject[] EnemyBoatPrefabs; // possibly temporary
    public GameObject bulletPrefab;

    private readonly System.Random _random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        _tileArray = new TileOccupier[TileConstants.TileMapWidth, TileConstants.TileMapHeight];
        _tilemap = GetComponent<Tilemap>();
        _selectedTiles = new Vector2Int[0];
        _selectedRangeTiles = new Vector2Int[0,0];
        _ticksSinceLastSpawn = 1;
        _quadrantsActive = 0;
        _friendlyBoats = new List<GameObject>();
        _enemyBoats = new List<GameObject>();
        _warningText = GameObject.Find("WarningText").GetComponent<Text>();
        _moneyText = GameObject.Find("CurrentMoney").GetComponent<Text>();
        _scoreText = GameObject.Find("CurrentScore").GetComponent<Text>();

        // Clear Tile Flags
        for (int i = 0; i < TileConstants.TileMapWidth; i++)
        {
            for (int j = 0; j < TileConstants.TileMapHeight; j++)
            {
                _tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
            }
        }

        TileOccupier baseOccupier = GameObject.FindGameObjectWithTag("Base").GetComponent<TileOccupier>();
        baseOccupier.GetComponent<BoatBehavior>().InitBoat(new Vector2Int(13, 9));
        // Make the base tiles occupied
        for (int x = 13; x <= 18; x++)
        {
            for (int y = 9; y <= 14; y++)
            {
                _tileArray[x, y] = baseOccupier;
            }
        }

        // Starting Money
        money = 200;
        score = 0;

        _InitEnemySpawnTiles();

        _warningText.text = "WARNING! Enemies Incoming";
        StartCoroutine(EnemyIncoming(1));

        // Start main game loop
        StartCoroutine(MainTimerCoroutine());
    }

    // Initializers
    private void _InitEnemySpawnTiles()
    {
        _enemySpawnTiles = new EnemySpawnTile[24];
        Vector2Int topOrigin = new Vector2Int(13, 23);
        Vector2Int rightOrigin = new Vector2Int(31, 9);
        Vector2Int bottomOrigin = new Vector2Int(13, 0);
        Vector2Int leftOrigin = new Vector2Int(0, 9);

        for (int i = 0; i < 6; i++)
        {
            _enemySpawnTiles[i] = new EnemySpawnTile(Quadrant.Top, new Vector2Int(topOrigin.x + i, topOrigin.y), true);
            _enemySpawnTiles[i + 6] = new EnemySpawnTile(Quadrant.Right, new Vector2Int(rightOrigin.x, rightOrigin.y + i), false);
            _enemySpawnTiles[i + 12] = new EnemySpawnTile(Quadrant.Bottom, new Vector2Int(bottomOrigin.x + i, bottomOrigin.y), false);
            _enemySpawnTiles[i + 18] = new EnemySpawnTile(Quadrant.Left, new Vector2Int(leftOrigin.x, leftOrigin.y + i), false);
        }
        _enemySpawnPositions = _enemySpawnTiles.Select(t => t.TilePosition).ToArray<Vector2Int>();
    }


    // MARK - Main Game Loop


    IEnumerator MainTimerCoroutine()
    {
        // All enemies move (and act)
        foreach (GameObject enemy in _enemyBoats)
        {
            enemy.GetComponent<BoatBehavior>().Turn();
        }

        CheckScore();

        // Spawning based on specified interval
        if (_ticksSinceLastSpawn > SpawnInterval)
        {
            PlaceEnemyShip(EnemyBoatPrefabs[0], GetSpawnLocation());
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

    public bool PlaceFriendlyShip(GameObject boatPrefab, Vector2Int location)
    {
        BoatBehavior newShipBehavior = boatPrefab.GetComponent<BoatBehavior>();
        if (newShipBehavior.value > money)
        {
            StartCoroutine(NoMoney());
            return false;
        }
        else if (IsTilePointInBounds(location))
        {
            TileOccupier occupier = boatPrefab.GetComponent<TileOccupier>();
            Vector2Int boatCoordinate = occupier.GetFocusCoordinate(location);
            Vector3 boatPosition = TileToWorldPoint(boatCoordinate);

            Vector2Int[] occupyingTiles = occupier.GetTilesOccupied(location);

            // If all tiles are empty (i.e. valid for placing)
            if (occupyingTiles.All(tile => IsTileEmpty(tile)) &&
                occupyingTiles.All(tile => !(_enemySpawnPositions.Contains<Vector2Int>(tile))))
            {
                money -= newShipBehavior.value;
                GameObject newShip = Instantiate(boatPrefab, boatPosition, Quaternion.Euler(Vector3.back * (int)occupier.rotation));
                TileOccupier newShipOccupier = newShip.GetComponent<TileOccupier>();
          
                _friendlyBoats.Add(newShip);               

                foreach (Vector2Int tile in occupyingTiles)
                {
                    _tileArray[tile.x, tile.y] = newShipOccupier;
                }

                newShipOccupier.GetComponent<BoatBehavior>().InitBoat(location);
                return true;
            }
        }

        return false;
    }

    public bool PlaceEnemyShip(GameObject boatPrefab, Vector2Int location)
    {
        
        TileOccupier occupier = boatPrefab.GetComponent<TileOccupier>();
        Vector2Int boatCoordinate = occupier.GetFocusCoordinate(location);
        Vector3 boatPosition = TileToWorldPoint(boatCoordinate);

        Vector2Int[] occupyingTiles = occupier.GetTilesOccupied(location);
        Vector2Int[] visibleTiles = Array.FindAll<Vector2Int>(occupyingTiles, t => IsTilePointInBounds(t));

        // If all visible tiles are empty (i.e. valid for placing)
        if (visibleTiles.All(tile => IsTileEmpty(tile)))
        {
            GameObject newShip = Instantiate(boatPrefab, boatPosition, Quaternion.Euler(Vector3.back * (int)occupier.rotation));
            TileOccupier newShipOccupier = newShip.GetComponent<TileOccupier>();
            _enemyBoats.Add(newShip);

            foreach (Vector2Int tile in visibleTiles)
            {
                _tileArray[tile.x, tile.y] = newShipOccupier;
            }

            newShipOccupier.GetComponent<BoatBehavior>().InitBoat(location);
            return true;
        }
        

        return false;
    }

    public bool TryMove(TileOccupier occupier, Vector2Int[] oldTiles, Vector2Int[] newTiles)
    {
        // only check the currently visible tiles since enemies can be partially off screen
        if (occupier.type == TileOccupierType.Enemy)
        {
            oldTiles = Array.FindAll<Vector2Int>(oldTiles, t => IsTilePointInBounds(t));
            newTiles = Array.FindAll<Vector2Int>(newTiles, t => IsTilePointInBounds(t));
        }

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

    IEnumerator NoMoney()
    {
        if (_warningText.text.Equals(""))
        {
            _warningText.text = "No Money";
            yield return new WaitForSeconds(1);
            _warningText.text = "";
        }
    }

    // MARK - Enemy Ship Spawning
    

    public Vector2Int GetSpawnLocation()
    {
        int numActive = Array.FindAll<EnemySpawnTile>(_enemySpawnTiles, t => t.Active).Length;
        int tileIndex = _random.Next(numActive);
        EnemySpawnTile tile = _enemySpawnTiles[tileIndex];
        TileOccupier occupier = EnemyBoatPrefabs[0].GetComponent<TileOccupier>();

        switch (tile.Quadrant)
        {
            case Quadrant.Right:
                occupier.rotation = (OccupierRotation)0;
                break;

            case Quadrant.Bottom:
                occupier.rotation = (OccupierRotation)90;
                break;
                

            case Quadrant.Left:
                occupier.rotation = (OccupierRotation)180;
                break;

            case Quadrant.Top:
                occupier.rotation = (OccupierRotation)270;
                break;

            default:
                throw new System.Exception("Random Failed");
        }

        return tile.TilePosition;
    }

    void CheckScore()
    {
        if (_quadrantsActive >= 4)
        {
            return;
        }
        else if (score > 100 * (_quadrantsActive+1))
        {
            _quadrantsActive++;
            _warningText.text = "WARNING! Enemies Incoming";
            StartCoroutine(EnemyIncoming(1));
        }
    }

    IEnumerator EnemyIncoming(int count)
    {
        if (count>5)
        {
            for (int i = 0; i < 6; i++)
            {
                _enemySpawnTiles[i + (6 * _quadrantsActive)].Active = true;
            }
            _warningText.text = "";
            yield break;
        }
        for (int i = 0; i < 6; i++)
        {
            Vector2Int location = _enemySpawnTiles[i + (6 * (_quadrantsActive))].TilePosition;
            if (IsTilePointInBounds(location))
            {
                _tilemap.SetColor(new Vector3Int(location.x, location.y, 0), Color.red);
            }
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < 6; i++)
        {
            Vector2Int location = _enemySpawnTiles[i + (6 * (_quadrantsActive))].TilePosition;
            if (IsTilePointInBounds(location))
            {
                _tilemap.SetColor(new Vector3Int(location.x, location.y, 0), Color.white);
            }
        }
        yield return new WaitForSeconds(0.7f);
        count++;
        StartCoroutine(EnemyIncoming(count));
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


    public void SelectTiles(Vector2Int[] tiles, Vector2Int[,] range)
    {
        foreach (Vector2Int tile in _selectedTiles)
        {
            if (IsTilePointInBounds(tile))
            {
                _tilemap.SetColor(new Vector3Int(tile.x, tile.y, 0), Color.white);
            }
        }
        
        _selectedTiles = tiles;

        foreach (Vector2Int tile in _selectedRangeTiles)
        {
            if (IsTilePointInBounds(tile))
            {
                _tilemap.SetColor(new Vector3Int(tile.x, tile.y, 0), Color.white);
            }
        }

        _selectedRangeTiles = range;

        // Check if new tiles are valid
        Color selectionColor = (tiles.All(tile => IsTileEmpty(tile)) &&
                                tiles.All(tile => !(_enemySpawnPositions.Contains<Vector2Int>(tile))))
                                ? Color.green : Color.red;

        // Set appropriate color for new tiles
        foreach (Vector2Int tile in tiles)
        {
            if (IsTilePointInBounds(tile))
            {
                _tilemap.SetColor(new Vector3Int(tile.x, tile.y, 0), selectionColor);
            }
        }

        foreach (Vector2Int tile in range)
        {
            if (IsTilePointInBounds(tile))
            {
                _tilemap.SetColor(new Vector3Int(tile.x, tile.y, 0), Color.yellow);
            }
        }
    }


    // MARK - Shooting Routine


    // Returns true if hit something
    public bool ApplyDamage(TileOccupierType src, Vector2Int start, Vector2Int target, int damage)
    {
        if (IsTilePointInBounds(target) && !IsTileEmpty(target))
        {
            TileOccupier dst = _tileArray[target.x, target.y];
            if (src == dst.type)
            {
                return false;
            }
            else if (src == TileOccupierType.Friendly && dst.type == TileOccupierType.Base)
            {
                return false;
            }

            FireAnimation(start, target, () =>
            {
                // completion handler
                if (dst != null)
                {
                    dst.TakeDamage(damage);
                }
            });

            return true;
        }
        return false;
    }

    private void FireAnimation(Vector2Int start, Vector2Int target, Action completionHandler)
    {
        IEnumerator FireRoutine()
        {
            float dist = Vector2.Distance(start, target);
            float speed = TileConstants.TileMapWidth / 0.8f; // dist / time
            float time = (dist / speed) / TileConstants.TileMapWidth; // time = (dist / speed) / iterations
            GameObject bullet = Instantiate(bulletPrefab, new Vector3(start.x, start.y, 0), new Quaternion());

            // Loop 1s
            for (float update = 0; update < TileConstants.TileMapWidth; update++)
            {
                // do (update + 1) so the bullet starts a bit off the firing ship
                // and lands on top of the receiving ship
                Vector2 newPos = Vector2.Lerp(start, target, (update + 1) / TileConstants.TileMapWidth);
                bullet.transform.position = newPos;

                yield return new WaitForSeconds(time);
            }

            completionHandler();
            Destroy(bullet);
        }

        StartCoroutine(FireRoutine());
    }


    // MARK - Ship Removal

    public void Sold(GameObject soldBoat, Vector2Int location)
    {
        TileOccupier occupier = soldBoat.GetComponent<TileOccupier>();
        Vector3 boatPosition = TileToWorldPoint(occupier.GetFocusCoordinate(location));
        BoatBehavior behavior = soldBoat.GetComponent<BoatBehavior>();

        Vector2Int[] occupyingTiles = occupier.GetTilesOccupied(location);

        foreach (Vector2Int tile in occupyingTiles)
        {
            _tileArray[tile.x, tile.y] = null;
        }

        money += behavior.sellValue;
        _friendlyBoats.Remove(soldBoat);
        
        Destroy(soldBoat);
    }

    public void Died(GameObject deadBoat)
    {
        TileOccupier occupier = deadBoat.GetComponent<TileOccupier>();
        BoatBehavior deadBehavior = deadBoat.GetComponent<BoatBehavior>();

        Vector2Int[] occupyingTiles = occupier.GetTilesOccupied(deadBehavior.BoatPosition);

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
            money += deadBehavior.value;
            score += deadBehavior.value;
            _enemyBoats.Remove(deadBoat);
        }
        Destroy(deadBoat);
    }

    // MARK - Menu On Clicks

    public TileOccupier GetSelectedShip(Vector2Int position)
    {
        return _tileArray[position.x, position.y];
    }
}
