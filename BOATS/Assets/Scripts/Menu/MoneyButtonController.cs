using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyButtonController : MonoBehaviour
{
    public int value;

    private TileSystem tileSystem;

    private void Start()
    {
        tileSystem = FindObjectOfType<TileSystem>();
    }

    public void HandleClick()
    {
        tileSystem.money += value;
        Destroy(gameObject);
    }
}
