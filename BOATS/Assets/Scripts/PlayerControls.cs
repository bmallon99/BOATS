using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControls : MonoBehaviour
{
    public GameObject heldObject;
    public bool holding;

    private TileSystem _tileSystem;

    public Vector3 worldMousePosition => Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());

    // Start is called before the first frame update
    void Start()
    {
        _tileSystem = FindObjectOfType<TileSystem>();
        holding = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseMoved(InputValue positionValue)
    {
        // "Ghost" ship underneath the player's mouse when selected to place
        if (holding)
        {
            Vector2 position = positionValue.Get<Vector2>();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            heldObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, -1);

            // Update Selected Tiles
            Vector2Int tilePosition = _tileSystem.WorldToTilePoint(worldPosition);
            TileOccupier occupier = heldObject.GetComponent<TileOccupier>();
            _tileSystem.SelectTiles(occupier.GetTilesOccupied(tilePosition));
        }
    }

    public void OnPrimaryClick()
    {
        // Ship Placement
        if (holding)
        {
            Vector2Int tilePosition = _tileSystem.WorldToTilePoint(worldMousePosition);
            if (_tileSystem.PlaceShip(heldObject, tilePosition))
            {
                holding = false;
                Destroy(heldObject);
                _tileSystem.SelectTiles(new Vector2Int[0]);
            }
        }
    }

    public void OnSecondaryClick()
    {
        // Rotate the ship
        if (holding)
        {
            TileOccupier occupier = heldObject.GetComponent<TileOccupier>();
            occupier.rotation = (OccupierRotation) (((int)occupier.rotation + 90) % 360);
            occupier.gameObject.transform.Rotate(Vector3.back, 90);

            // Update Selected Tiles
            Vector2Int tilePosition = _tileSystem.WorldToTilePoint(worldMousePosition);
            _tileSystem.SelectTiles(occupier.GetTilesOccupied(tilePosition));
        }
    }
}
