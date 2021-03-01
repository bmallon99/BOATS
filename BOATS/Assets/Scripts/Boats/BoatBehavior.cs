using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BoatBehavior : MonoBehaviour
{
    // Abstract Functions to be implemented
    public abstract void Turn();
    protected abstract bool Move();
    protected abstract bool Attack();

    // Shared boat behavior
    public virtual Vector2Int[,] GetFiringRange(Vector2Int position)
    {
        throw new NotImplementedException();
    }

    public int range;
    public int damage;
    public int totalHealth;
    public int value;
    public int sellValue
    {
        get => value / 2;
    }
    protected Vector2Int _boatPosition;
    public Vector2Int BoatPosition { get; set; }

    private TileOccupier myTileOccupier;

    public virtual int Health
    {
        get => totalHealth;
        set
        {
            totalHealth = value;
            UpdateHealthValue(value);
        }
    }

    public GameObject healthTextPrefab;
    protected virtual GameObject MyHealthText { get; set; }

    public virtual void UpdateHealthValue(int newHealth)
    {
        if (MyHealthText != null)
        {
            //if (newHealth <= 0)
            //{
            //    Destroy(MyHealthText);
            //}
            //if (newHealth > 0)
            //{
            MyHealthText.GetComponentInChildren<Text>().text = newHealth.ToString();
            //}
        }
    }

    protected void InitHealthBar()
    {
        GameObject _healthBars = GameObject.FindGameObjectWithTag("Health Bars");
        myTileOccupier = GetComponent<TileOccupier>();
        MyHealthText = Instantiate(healthTextPrefab, AdjustDamageBarPosition(transform.position), new Quaternion(), _healthBars.transform);
        Health = totalHealth;
    }

    public void InitBoat(Vector2Int location)
    {
        InitHealthBar();
        BoatPosition = location;
    }

    // probably temporary
    protected virtual Vector3 AdjustDamageBarPosition(Vector3 pos)
    {
        //float adjustment = 1.0f;
        float width = (float)myTileOccupier.tilesWide;
        float height = (float)myTileOccupier.tilesTall;
        switch (myTileOccupier.rotation)
        {
            case OccupierRotation.Zero:
                return new Vector3(pos.x + (width / 2f), pos.y - 0.5f);
            case OccupierRotation.Clockwise90:
                return new Vector3(pos.x + (height / 2f), pos.y - width - 0.5f);
            case OccupierRotation.Clockwise180:
                return new Vector3(pos.x - (width / 2f), pos.y - height - 0.5f);
            case OccupierRotation.Clockwise270:
                return new Vector3(pos.x - (height / 2f), pos.y - 0.5f);
            default:
                return pos;
        }
    }

    public void DestroyHealthText()
    {
        Destroy(MyHealthText);
    }

    public static void DestroyBoat(GameObject boat)
    {
        BoatBehavior boatBehavior = boat.GetComponent<BoatBehavior>();
        boatBehavior.DestroyHealthText();
        Destroy(boat);
    }

    // returns the next position location 
    protected Vector2Int NextForwardPosition(TileOccupier boatTileInfo, Vector2Int currPosition)
    {
        switch (boatTileInfo.rotation)
        {
            case OccupierRotation.Zero:
                return new Vector2Int(currPosition.x - 1, currPosition.y);
            case OccupierRotation.Clockwise90:
                return new Vector2Int(currPosition.x, currPosition.y + 1);
            case OccupierRotation.Clockwise180:
                return new Vector2Int(currPosition.x + 1, currPosition.y);
            case OccupierRotation.Clockwise270:
                return new Vector2Int(currPosition.x, currPosition.y - 1);
            default:
                throw new System.Exception("Unassigned OccupierRotation");
        }
    }
}
