using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerController : MonoBehaviour
{
    [SerializeField] GameDifficultySettings _beginnerSettings;
    [SerializeField] GameDifficultySettings _normalSettings;
    [SerializeField] GameDifficultySettings _expertSettings;
    public static event Action<string> onUpdateTimer;
    float timeRemaining;
    bool timerIsRunning = false;

    void OnEnable() {
        GameController.onStartGame += SetStartingTime;
    }

    void OnDisable() {
        GameController.onStartGame -= SetStartingTime;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                GameController.Instance.EndGame(GameOverReason.TimeRanOut);
            }
        }
    }
    void SetStartingTime() {
        switch (GameController.Instance.GameDifficulty) {
            case Difficulty.Beginner:
                timeRemaining = _beginnerSettings.timeLimit;
                break;
            case Difficulty.Normal:
                timeRemaining = _normalSettings.timeLimit;
                break;
            case Difficulty.Expert:
                timeRemaining = _expertSettings.timeLimit;
                break;
        }

        timerIsRunning = true;
        DisplayTime(timeRemaining);
        
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        string timerText = string.Format("{0:00}:{1:00}", minutes, seconds);
        onUpdateTimer?.Invoke(timerText);
    }
}
