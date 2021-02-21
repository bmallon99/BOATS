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
            if (newHealth == 0)
            {
                Destroy(MyHealthText);
            }
            else
            {
                MyHealthText.GetComponentInChildren<Text>().text = newHealth.ToString();
            }
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
        float adjustment = 1.0f;
        switch (myTileOccupier.rotation)
        {
            case OccupierRotation.Zero:
                return new Vector3(pos.x + adjustment, pos.y - (0.5f * adjustment));
            case OccupierRotation.Clockwise90:
                return new Vector3(pos.x + (0.5f * adjustment), pos.y - (2.5f * adjustment));
            case OccupierRotation.Clockwise180:
                return new Vector3(pos.x - adjustment, pos.y - (1.5f * adjustment));
            case OccupierRotation.Clockwise270:
                return new Vector3(pos.x - (0.5f * adjustment), pos.y - (0.5f * adjustment));
            default:
                return pos;
        }
    }

    public void DestroyHealthText()
    {
        Destroy(MyHealthText);
    }
}
