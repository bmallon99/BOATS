using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInfoController : MonoBehaviour
{
    public InfoTextCollection infoTextPrefabs;
    private GameObject _currentText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateInfoText(MenuState state)
    {
        Destroy(_currentText);
        //GameObject prefab = infoTextPrefabs[MenuState.Idle]; 
        switch (state)
        {
            case MenuState.Idle:
                //prefab = infoTextPrefabs[MenuState.Idle];
                _currentText = Instantiate(infoTextPrefabs[MenuState.Idle], transform);
                return;
            case MenuState.HoldingNewShip:
                //prefab = infoTextPrefabs[MenuState.HoldingNewShip];
                _currentText = Instantiate(infoTextPrefabs[MenuState.HoldingNewShip], transform);
                return;
        }
        //_currentText = Instantiate(prefab, transform);
    }
}
