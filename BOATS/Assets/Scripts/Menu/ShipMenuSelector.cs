﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShipMenuSelector : MonoBehaviour
{
    private PlayerControls _player;
    private GameObject _myShip;
    private BoatBehavior _boatBehavior;
    private TileSystem _tileSystem;

    void Start()
    {
        _player = Camera.main.GetComponent<PlayerControls>();
        _boatBehavior = GetComponent<BoatBehavior>();
        _tileSystem = GameObject.FindObjectOfType<TileSystem>();
    }
    
    public void HoldShip(GameObject shipPrefab)
    {
        bool differentShip = _myShip != _player.selectedObject;
        _player.SetSelectedShipWhite();
        // hold new ship
        if (_player.state != MenuState.HoldingNewShip || differentShip)
        {
            if (_player.state == MenuState.HoldingNewShip && differentShip)
            {
                Destroy(_player.selectedObject);
            }
            _myShip = Instantiate(shipPrefab, _player.worldMousePosition, new Quaternion());
            _player.selectedObject = _myShip;
            _player.state = MenuState.HoldingNewShip;    
        }
        // put back
        else if (_player.state == MenuState.HoldingNewShip)
        {
            // unhighlight
            _tileSystem.SelectTiles(new Vector2Int[0], new Vector2Int[0,0]);
            Destroy(_myShip);
            _player.state = MenuState.Idle;
        }
    }
}
