using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }

    public void SellShip()
    {
        _tileSystem.Sold(_player.selectedObject);
        _player.state = MenuState.Idle;
    }
}
