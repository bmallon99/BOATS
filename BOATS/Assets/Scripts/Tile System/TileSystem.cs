using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TileSystem : MonoBehaviour
{
    public GameObject prefab;
    private TileOccupier[,] _tileArray;
    private Tilemap _tilemap;
    private Vector2Int[] _selectedTiles;

    // Start is called before the first frame update
    void Start()
    {
        _tileArray = new TileOccupier[TileConstants.TileMapWidth, TileConstants.TileMapHeight];
        _tilemap = GetComponent<Tilemap>();
        _selectedTiles = new Vector2Int[0];

        // Clear Tile Flags
        for (int i = 0; i < TileConstants.TileMapWidth; i++)
        {
            for (int j = 0; j < TileConstants.TileMapHeight; j++)
            {
                _tilemap.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
            }
        }
    }


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
                StartCoroutine(AttackCoroutine(newShip));

                foreach (Vector2Int tile in occupyingTiles)
                {
                    _tileArray[tile.x, tile.y] = newShipOccupier;
                }

                return true;
            }
        }

        return false;
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


    // MARK - Collision Detection


    public TileOccupier CheckCollision(Vector2Int location)
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

    public void Fire(Vector2Int[] tiles)
    {
        StartCoroutine(FireCoroutine(tiles));
    }

    IEnumerator AttackCoroutine(GameObject boat)
    {
        yield return new WaitForSeconds(1);
        boat.GetComponent<Shoot>().Fire_straight_line();
        yield return new WaitForSeconds(5);
        StartCoroutine(AttackCoroutine(boat));
    }

    IEnumerator FireCoroutine(Vector2Int[] tiles)
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
}
