using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct HighScore{
    public string name;
    public string date;
    public int score;
}
public static class HighScoresManager
{
    public static List<HighScore> LoadHighScores(Difficulty diff){
        List<HighScore> scores = new List<HighScore>();
        for(int i = 0; i < 10; i++){
            string name = PlayerPrefs.GetString($"name_{diff}_{i}", "None");
            string date = PlayerPrefs.GetString($"date_{diff}_{i}", "None");
            int score = PlayerPrefs.GetInt($"score_{diff}_{i}", -1);
            
            scores.Add(new HighScore(){name = name, date = date, score = score});
        }

        return scores;
    }
    public static void SaveHighScores(Difficulty diff, List<HighScore> scores){
        for(int i = 0; i < 10; i++){
            PlayerPrefs.SetString($"name_{diff}_{i}", scores[i].name);
            PlayerPrefs.SetString($"date_{diff}_{i}", scores[i].date);
            PlayerPrefs.SetInt($"score_{diff}_{i}", scores[i].score);
        }
        PlayerPrefs.Save();

    }
}
