﻿using System.Collections;
using System.Collections.Generic;
 using System.Reflection;
 using UnityEngine;

public class ShipMenuSelector : MonoBehaviour
{
    public void HoldShip(GameObject shipPrefab)
    {
        PlayerControls player = Camera.main.GetComponent<PlayerControls>();
        player.heldObject = Instantiate(shipPrefab, player.worldMousePosition, new Quaternion());
        player.holding = true;
    }
}
