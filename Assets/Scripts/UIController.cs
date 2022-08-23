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
    void OnEnable() {
        PlayerCollider.fragCollideWithPlayer += DecreasePlayerHealthText;
        BallManager.onBallThrowPowerChange += UpdateThrowPowerText;
        BallManager.onBallsLeftCountChange += UpdateBallsRemainingText;
        TimerController.onUpdateTimer += UpdateTimerText;
        
    }

    void OnDisable() {
        PlayerCollider.fragCollideWithPlayer -= DecreasePlayerHealthText;
        BallManager.onBallThrowPowerChange -= UpdateThrowPowerText;
        BallManager.onBallsLeftCountChange -= UpdateBallsRemainingText;
        TimerController.onUpdateTimer -= UpdateTimerText;
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
}
