using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameController : MonoBehaviour
{

    private InputField _inputField;
    private Text _displayText;
    private string _defaultText = "___";
    
    void Start()
    {
        _inputField = GetComponent<InputField>();
        //_inputField.onValueChanged.AddListener(UpdateName);
        _displayText = GetComponent<Text>();
    }

    // gets called when you finish editting name
    public void UpdateScores(string newText)
    {
        if (newText == null || newText == "")
        {
            _inputField.text = _defaultText;
        }
        ScoresData.SetNewName(_inputField.text);
    }
}
