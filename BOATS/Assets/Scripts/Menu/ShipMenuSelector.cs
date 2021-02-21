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

    void Start()
    {
        _player = Camera.main.GetComponent<PlayerControls>();
        _boatBehavior = GetComponent<BoatBehavior>();
    }
    
    public void HoldShip(GameObject shipPrefab)
    {
        bool differentShip = _myShip != _player.heldObject;
        if (_player.state != MenuState.HoldingNewShip || differentShip)
        {
            if (differentShip)
            {
                Destroy(_player.heldObject);
            }
            _myShip = Instantiate(shipPrefab, _player.worldMousePosition, new Quaternion());
            _player.heldObject = _myShip;
            _player.state = MenuState.HoldingNewShip;    
        }
        else
        {
            Destroy(_myShip);
            _player.state = MenuState.Idle;
        }
    }
}
