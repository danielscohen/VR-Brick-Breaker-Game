using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    int playerHealth;
    [SerializeField] TextMeshProUGUI _playerHealthText;
    [SerializeField] TextMeshProUGUI _ballPowerText;
    [SerializeField] TextMeshProUGUI _ballsRemainingText;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] TextMeshProUGUI _gameOverText;
    [SerializeField] GameObject _startScreen;
    [SerializeField] GameObject _screenOverlay;
    [SerializeField] GameObject _pauseScreen;
    [SerializeField] GameObject _gameOverScreen;
    void OnEnable() {
        PlayerCollider.onFragCollideWithPlayer += DecreasePlayerHealthText;
        BallManager.onBallThrowPowerChange += UpdateThrowPowerText;
        BallManager.onBallsLeftCountChange += UpdateBallsRemainingText;
        TimerController.onUpdateTimer += UpdateTimerText;
        GameController.onResumeGame += ShowScreenOverlay;
        GameController.onGameOver += ShowGameOverScreen;
        GameController.onStartGame += ShowStartScreen;
        GameController.onPauseGame += ShowPauseScreen;
        
    }

    void OnDisable() {
        PlayerCollider.onFragCollideWithPlayer -= DecreasePlayerHealthText;
        BallManager.onBallThrowPowerChange -= UpdateThrowPowerText;
        BallManager.onBallsLeftCountChange -= UpdateBallsRemainingText;
        TimerController.onUpdateTimer -= UpdateTimerText;
        GameController.onResumeGame -= ShowScreenOverlay;
        GameController.onGameOver -= ShowGameOverScreen;
        GameController.onStartGame -= ShowStartScreen;
        GameController.onPauseGame -= ShowPauseScreen;
    }

    void Start() {
        playerHealth = GameController.Instance.PlayerHealth;
        _playerHealthText.text = playerHealth.ToString();
    }

    void DecreasePlayerHealthText(int healthDecVal) {
        playerHealth -= healthDecVal;
        _playerHealthText.text = playerHealth.ToString();
    }
    void UpdateThrowPowerText(float power) {
        _ballPowerText.text = string.Format("{0:N2}", power);
    }
    void UpdateBallsRemainingText(int numBalls) {
        _ballsRemainingText.text = $"Balls Left: {numBalls}";
    }
    void UpdateTimerText(string time) {
        _timerText.text = time;
    }

    void ShowScreenOverlay() {
        _screenOverlay.SetActive(true);
        _startScreen.SetActive(false);
        _pauseScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
    }
    void ShowStartScreen() {
        _screenOverlay.SetActive(false);
        _startScreen.SetActive(true);
        _pauseScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
    }
    void ShowPauseScreen() {
        _screenOverlay.SetActive(false);
        _startScreen.SetActive(false);
        _pauseScreen.SetActive(true);
        _gameOverScreen.SetActive(false);
    }

    void ShowGameOverScreen(GameOverReason reason) {
        string gameOverText;

        _screenOverlay.SetActive(false);
        _gameOverScreen.SetActive(true);

        switch (reason) {
            case GameOverReason.GameWon:
                gameOverText = $"Congrats, You Won!\n You Got a Score of {playerHealth}";
                break;
            case GameOverReason.HealthRanOut:
                gameOverText = "You Ran-Out of Health!";
                break;
            case GameOverReason.TimeRanOut:
                gameOverText = "You Ran-Out of Time!";
                break;
            default:
                gameOverText = "You Ran-Out of Balls!";
                break;
        }

        _gameOverText.text = gameOverText;
    }
    
}
