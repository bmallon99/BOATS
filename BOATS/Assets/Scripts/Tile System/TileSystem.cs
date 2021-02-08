using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSystem : MonoBehaviour
{
    public GameObject prefab;
    public Tile fireTile;
    public Tile originalTile;
    private TileOccupier[,] _tileArray;
    private Tilemap _tilemap;
    private Vector2Int[] _selectedTiles;

    // Start is called before the first frame update
    void Start()
    {
        _tileArray = new TileOccupier[TileConstants.TileMapWidth, TileConstants.TileMapHeight];
        _tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public bool TilesAreEmpty(Vector2Int[] locations)
    {
        foreach (Vector2Int location in locations)
        {
            if (_tileArray[location.x, location.y] != null)
            {
                return false;
            }
        }

        return true;
    }

    public bool PlaceShip(GameObject boat, Vector2Int location)
    {
        if (IsTilePointInBounds(location))
        {
            TileOccupier occupier = boat.GetComponent<TileOccupier>();
            Vector3 boatPosition = TranslateTileToCoordinates(occupier.GetFocusCoordinate(location));

            Vector2Int[] occupyingTiles = occupier.GetTilesOccupied(location);

            if (TilesAreEmpty(occupyingTiles))
            {
                Instantiate(boat, boatPosition, Quaternion.Euler(Vector3.back * (int)occupier.rotation));

                foreach (Vector2Int tile in occupier.GetTilesOccupied(location))
                {
                    _tileArray[tile.x, tile.y] = occupier;
                }

                return true;
            }
        }

        return false;
    }

    public TileOccupier CheckCollision(Vector2Int[] tiles)
    {
        foreach (Vector2Int location in tiles)
        {
            if (IsTilePointInBounds(location) && _tileArray[location.x, location.y] != null)
            {
                return _tileArray[location.x, location.y];
            }
        }
        return null;
    }

    public void Fire(Vector2Int[] tiles)
    {
        StartCoroutine(FireCoroutine(tiles));
    }

    IEnumerator AttackCoroutine()
    {
        //[boat].GetComponent<Shoot>().Fire_straight_line();
        yield return new WaitForSeconds(5);
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator FireCoroutine(Vector2Int[] tiles)
    {
        foreach (Vector2Int location in tiles)
        {
            if (IsTilePointInBounds(location))
            {
                _tilemap.SetTile(new Vector3Int(location.x, location.y, 0), fireTile);
            }
        }
        yield return new WaitForSeconds(0.5f);
        foreach (Vector2Int location in tiles)
        {
            if (IsTilePointInBounds(location))
            {
                _tilemap.SetTile(new Vector3Int(location.x, location.y, 0), originalTile);
            }
        }
    }


    // MARK - Tile Coordinate System

    public bool IsTilePointInBounds(Vector2Int tileCoordinate)
    {
        return tileCoordinate.x >= 0 && tileCoordinate.x < TileConstants.TileMapWidth
            && tileCoordinate.y >= 0 && tileCoordinate.y < TileConstants.TileMapHeight;
    }

    public Vector3 TranslateTileToCoordinates(Vector2Int tileCoordinate)
    {
        return _tilemap.layoutGrid.CellToWorld(new Vector3Int(tileCoordinate.x, tileCoordinate.y, 0));
    }

    public Vector2Int TranslateCoordinatesToTile(Vector3 worldCoordinate)
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

        foreach (Vector2Int tile in tiles)
        {
            if (IsTilePointInBounds(tile))
            {
                _tilemap.SetColor(new Vector3Int(tile.x, tile.y, 0), Color.green);
            }
        }
    }
}
