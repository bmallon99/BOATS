﻿using System.Collections;
using System.Collections.Generic;
 using System.Reflection;
 using UnityEngine;

public class ShipMenuSelector : MonoBehaviour
{
    private PlayerControls player;
    private GameObject myShip;
    void Start()
    {
        player = Camera.main.GetComponent<PlayerControls>();
    }
    
    public void HoldShip(GameObject shipPrefab)
    {
        bool differentShip = myShip != player.heldObject;
        if (player.state == MenuState.Idle || differentShip)
        {
            if (differentShip)
            {
                Destroy(player.heldObject);
            }
            myShip = Instantiate(shipPrefab, player.worldMousePosition, new Quaternion());
            player.heldObject = myShip;
            player.state = MenuState.HoldingNewShip;    
        }
        else
        {
            Destroy(myShip);
            player.state = MenuState.Idle;
        }
    }
}
