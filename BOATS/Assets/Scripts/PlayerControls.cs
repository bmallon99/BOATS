﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MenuState {
    Idle,
    HoldingNewShip,
    SelectingDefender1,
    SelectingDefender2
}

public class PlayerControls : MonoBehaviour
{
    public GameObject heldObject;
    private MenuInfoController _controls;
    private MenuState _state;
    public MenuState state
    {
        get => _state;
        set
        {
            _state = value;
            if (_controls != null)
            {
                _controls.updateInfoText(value);
            }
        }
    }
    private TileSystem _tileSystem;
    public Vector3 worldMousePosition => Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());

    // Start is called before the first frame update
    void Start()
    {
        _tileSystem = FindObjectOfType<TileSystem>();
        _controls = FindObjectOfType<MenuInfoController>();
        state = MenuState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseMoved(InputValue positionValue)
    {
        // "Ghost" ship underneath the player's mouse when selected to place
        if (state == MenuState.HoldingNewShip)
        {
            Vector2 position = positionValue.Get<Vector2>();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            heldObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, -1);

            // Update Selected Tiles
            Vector2Int tilePosition = _tileSystem.WorldToTilePoint(worldPosition);
            TileOccupier occupier = heldObject.GetComponent<TileOccupier>();
            BoatBehavior behavior = heldObject.GetComponent<BoatBehavior>();
            _tileSystem.SelectTiles(occupier.GetTilesOccupied(tilePosition), behavior.GetFiringRange(occupier.GetFocusCoordinate(tilePosition)));
        }
    }

    public void OnPrimaryClick()
    {
        // Ship Placement
        Vector2Int tilePosition = _tileSystem.WorldToTilePoint(worldMousePosition);
        switch (state)
        {
            case MenuState.HoldingNewShip:
                
                if (_tileSystem.PlaceFriendlyShip(heldObject, tilePosition))
                {
                    state = MenuState.Idle;
                    Destroy(heldObject);
                    _tileSystem.SelectTiles(new Vector2Int[0], new Vector2Int[0, 0]);
                }
                break;
            case MenuState.Idle:
                // if you aren't holding something, you probably want to see info about a ship
                if (_tileSystem.IsTilePointInBounds(tilePosition))
                {
                    TileOccupier selectedShip = _tileSystem.GetSelectedShip(tilePosition);
                    if (selectedShip != null)
                    {
                        BoatBehavior boatBehavior = selectedShip.GetComponent<BoatBehavior>();
                        switch (boatBehavior.GetType().Name)
                        {
                            case "Defender1Behaviour":
                                state = MenuState.SelectingDefender1;
                                break;
                            case "Defender2Behaviour":
                                state = MenuState.SelectingDefender2;
                                break;
                            default:
                                throw new System.Exception("Can't find boat type");

                        }
                    }
                }
                
                break;
            default:
                // this is for clicking out of the menu for a selected ship
                if (_tileSystem.IsTilePointInBounds(tilePosition))
                {
                    state = MenuState.Idle;
                }
                break;
        }
        
    }

    public void OnSecondaryClick()
    {
        // Rotate the ship
        if (state == MenuState.HoldingNewShip)
        {
            TileOccupier occupier = heldObject.GetComponent<TileOccupier>();
            occupier.rotation = (OccupierRotation) (((int)occupier.rotation + 90) % 360);
            occupier.gameObject.transform.Rotate(Vector3.back, 90);

            // Update Selected Tiles
            Vector2Int tilePosition = _tileSystem.WorldToTilePoint(worldMousePosition);
            BoatBehavior behavior = heldObject.GetComponent<BoatBehavior>();
            _tileSystem.SelectTiles(occupier.GetTilesOccupied(tilePosition), behavior.GetFiringRange(occupier.GetFocusCoordinate(tilePosition)));
        }
    }
}
