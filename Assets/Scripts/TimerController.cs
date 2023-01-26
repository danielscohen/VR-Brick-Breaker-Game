using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerController : MonoBehaviour
{
    [SerializeField] GameDifficultySettings _beginnerSettings;
    [SerializeField] GameDifficultySettings _normalSettings;
    [SerializeField] GameDifficultySettings _expertSettings;
    public static event Action<string, float> onUpdateTimer;
    float timeRemaining;
    float timeLimit;
    bool timerIsRunning = false;

    void OnEnable() {
        GameController.onStartGame += SetStartingTime;
        GameController.onGameOver += StopTimer;
    }

    void OnDisable() {
        GameController.onStartGame -= SetStartingTime;
        GameController.onGameOver -= StopTimer;
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
                timeLimit = _beginnerSettings.timeLimit;
                break;
            case Difficulty.Normal:
                timeLimit = _normalSettings.timeLimit;
                break;
            case Difficulty.Expert:
                timeLimit = _expertSettings.timeLimit;
                break;
        }

        timeRemaining = timeLimit;
        timerIsRunning = true;
        DisplayTime(timeRemaining);
        
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        string timerText = string.Format("{0:00}:{1:00}", minutes, seconds);
        onUpdateTimer?.Invoke(timerText, timeToDisplay / timeLimit);
    }

    void StopTimer(){
        timerIsRunning = false;
    }
}
