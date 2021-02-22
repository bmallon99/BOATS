using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuInfoController : MonoBehaviour
{
    public InfoTextCollection infoTextPrefabs;
    private GameObject _currentText;
    private PlayerControls _player;
    private TileSystem _tileSystem;

    void Start()
    {
        _player = Camera.main.GetComponent<PlayerControls>();
        _tileSystem = GameObject.FindObjectOfType<TileSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateInfoText(MenuState state)
    {
        Destroy(_currentText);
        //GameObject _prefab = infoTextPrefabs[state];
        _currentText = Instantiate(infoTextPrefabs[state], transform);
        if (state != MenuState.Idle && state != MenuState.HoldingNewShip)
        {
            Text SellText = GameObject.FindWithTag("Sell Text").GetComponent<Text>();
            BoatBehavior boatBehavior = _player.selectedObject.GetComponent<BoatBehavior>();
            SellText.text = "Sell (+$" + boatBehavior.sellValue.ToString() + ")";
            updateHealthText(_player.selectedObject);
        }
    }

    public void updateHealthText(GameObject boat)
    {
        if (boat == _player.selectedObject)
        {
            BoatBehavior boatBehavior = boat.GetComponent<BoatBehavior>();
            if (boatBehavior.Health <= 0)
            {
                _player.state = MenuState.Idle;
                return;
            }
            Text HealthText = GameObject.FindWithTag("Health Text").GetComponent<Text>();
            HealthText.text = "Health: " + boatBehavior.Health.ToString();
        }
    }

    public void SellShip()
    {
        _tileSystem.Sold(_player.selectedObject);
        _player.state = MenuState.Idle;
    }
}
