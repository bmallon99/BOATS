using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScores : MonoBehaviour
{

    public GameObject[] entries;
    // Start is called before the first frame update
    void Start()
    {
        ScoreEntry[] fuckYou = ScoresData.scores;
        //GameObject[] entries = GameObject.FindGameObjectsWithTag("Score Entry");
        for (int i = 0; i < 5; i++)
        {
            FillFields(entries[i], ScoresData.scores[i]);
        }
        
        GameObject scoreText = GameObject.FindGameObjectWithTag("Current Score");
        scoreText.GetComponent<Text>().text = ScoresData.finalScore.ToString();
    }

    private void FillFields(GameObject newObject, ScoreEntry entry)
    {
        Text[] textObjects = newObject.GetComponentsInChildren<Text>();
        textObjects[1].text = entry.score.ToString();
        if (entry.newEntry)
        {
            textObjects[0].color = Color.green;
            textObjects[1].color = Color.green;
            textObjects[2].color = Color.green;
        }
        else
        {
            textObjects[2].GetComponent<InputField>().enabled = false;
        }
        textObjects[2].text = entry.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
