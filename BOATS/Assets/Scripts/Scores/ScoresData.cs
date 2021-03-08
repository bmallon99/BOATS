using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct ScoreEntry
{
    public int score;
    public string name;
    public bool newEntry;

    public ScoreEntry(int currScore, string currName, bool isNewEntry)
    {
        score = currScore;
        name = currName;
        newEntry = isNewEntry;
    }
}

public static class ScoresData
{
    public static ScoreEntry[] scores = new ScoreEntry[5];
    public static int finalScore = 0;
    public static bool doInit = true;

    public static void InitScores()
    {
        if (doInit)
        {
            AddScore(new ScoreEntry(30000, "MVJ", false));
            AddScore(new ScoreEntry(4000, "JHW", false));
            AddScore(new ScoreEntry(20000, "AH", false));
            scores[3] = new ScoreEntry(0, "___", false);
            scores[4] = new ScoreEntry(0, "___", false);
            doInit = false;
        }
    }

    public static void AddScore(ScoreEntry newScore)
    {
        ScoreEntry[] newScoresArray = new ScoreEntry[6];

        for (int i = 0; i < 5; i++)
        {
            newScoresArray[i] = scores[i];
            newScoresArray[i].newEntry = false;
        }
        newScoresArray[5] = newScore;

        newScoresArray = newScoresArray.OrderByDescending(entry => entry.score).ToArray<ScoreEntry>();

        // only use top 5 scores
        for (int i = 0; i < 5; i++)
        {
            scores[i] = newScoresArray[i];
        }
    }

    public static void SetFinalScore(int score)
    {
        finalScore = score;
    }

    public static void SetNewName(string name)
    {
        for (int i = 0; i < 5; i++)
        {
            if (scores[i].newEntry)
            {
                scores[i].name = name;
            }
        }
    }
}
