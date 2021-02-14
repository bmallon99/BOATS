using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class FriendlyBoatBehaviour : MonoBehaviour
{
    public int totalHealth;
    public virtual int health
    {
        get => totalHealth;
        set
        {
            totalHealth = value;
            UpdateHealthValue(value);
        }
    }

    public GameObject healthTextPrefab;

    protected virtual GameObject myHealthText { get; set; }

    public abstract void CheckFire();

    public virtual void UpdateHealthValue(int newHealth)
    {
        if (myHealthText != null)
        {
            if (newHealth == 0)
            {
                Destroy(myHealthText);
            }
            else
            {
                myHealthText.GetComponentInChildren<Text>().text = newHealth.ToString();
            }
        }
    }

    protected virtual void InitHealthBar()
    {
        GameObject _healthBars = GameObject.FindGameObjectWithTag("Health Bars");
        myHealthText = Instantiate(healthTextPrefab, transform.position, new Quaternion(), _healthBars.transform);
    }
}
