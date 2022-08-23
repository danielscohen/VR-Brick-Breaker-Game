using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerController : MonoBehaviour
{
    public static event Action<string> onUpdateTimer;
    [SerializeField] float timeRemaining = 300f;
    bool timerIsRunning = false;
    void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
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
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        string timerText = string.Format("{0:00}:{1:00}", minutes, seconds);
        onUpdateTimer?.Invoke(timerText);
    }
}
