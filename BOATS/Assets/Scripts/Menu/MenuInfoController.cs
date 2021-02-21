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
        //GameObject _prefab = infoTextPrefabs[state];
        _currentText = Instantiate(infoTextPrefabs[state], transform);
        
    }
}
