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
    protected Vector2Int _boatPosition;
    public Vector2Int BoatPosition { get; set; }

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

    protected virtual void InitHealthBar()
    {
        GameObject _healthBars = GameObject.FindGameObjectWithTag("Health Bars");
        MyHealthText = Instantiate(healthTextPrefab, transform.position, new Quaternion(), _healthBars.transform);
        Health = totalHealth;
    }



}
