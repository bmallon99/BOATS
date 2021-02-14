using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class InfoTextCollection : ScriptableObject
{
    [Serializable]
    public class InfoTextPrefab
    {
        // key
        public MenuState state;
        // value
        public GameObject infoTextPrefab;
    }

    public InfoTextPrefab[] allInfoTextPrefabs;
    private Dictionary<MenuState, GameObject> _enumToGameObject;

    public GameObject this[MenuState state]
    {
        get
        {
            InitializeDictionary();
            return _enumToGameObject[state];
        }
    }

    //public void Awake()
    //{
    //    InitializeDictionary();
    //}

    private void InitializeDictionary()
    {
        if (_enumToGameObject == null)
        {
            _enumToGameObject = new Dictionary<MenuState, GameObject>();
            foreach (InfoTextPrefab infoPrefab in allInfoTextPrefabs)
            {
                _enumToGameObject[infoPrefab.state] = infoPrefab.infoTextPrefab;
            }
        }
    }
    
}
