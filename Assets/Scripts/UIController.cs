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
        
    }

    void OnDisable() {
        PlayerCollider.onFragCollideWithPlayer -= DecreasePlayerHealthText;
        BallManager.onBallThrowPowerChange -= UpdateThrowPowerText;
        BallManager.onBallsLeftCountChange -= UpdateBallsRemainingText;
        TimerController.onUpdateTimer -= UpdateTimerText;
        GameController.onResumeGame -= ShowScreenOverlay;
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
}
