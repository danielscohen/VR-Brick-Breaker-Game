using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    int playerHealth;
    bool _uIMenuActive;
    [SerializeField] TextMeshProUGUI _playerHealthText;
    [SerializeField] TextMeshProUGUI _ballPowerText;
    [SerializeField] TextMeshProUGUI _ballsRemainingText;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] TextMeshProUGUI _gameOverText;
    [SerializeField] GameObject _startScreen;
    [SerializeField] GameObject _screenOverlay;
    [SerializeField] GameObject _pauseScreen;
    [SerializeField] GameObject _gameOverScreen;
    [SerializeField] GameObject _menuUI;
    void OnEnable() {
        BallManager.onBallThrowPowerChange += UpdateThrowPowerText;
        BallManager.onBallsLeftCountChange += UpdateBallsRemainingText;
        TimerController.onUpdateTimer += UpdateTimerText;
        PlayerPointsManager.onUpdatePlayerPoints += UpdatePLayerHealthText;
        GameController.onResumeGame += ShowScreenOverlay;
        GameController.onGameOver += ShowGameOverScreen;
        GameController.onLoadGame += ShowStartScreen;
        GameController.onPauseGame += ShowPauseScreen;
        
    }

    void OnDisable() {
        BallManager.onBallThrowPowerChange -= UpdateThrowPowerText;
        BallManager.onBallsLeftCountChange -= UpdateBallsRemainingText;
        TimerController.onUpdateTimer -= UpdateTimerText;
        PlayerPointsManager.onUpdatePlayerPoints -= UpdatePLayerHealthText;
        GameController.onResumeGame -= ShowScreenOverlay;
        GameController.onGameOver -= ShowGameOverScreen;
        GameController.onLoadGame -= ShowStartScreen;
        GameController.onPauseGame -= ShowPauseScreen;
    }

    void Update() {
        if(_uIMenuActive){
            Vector3 vHeadPos = Camera.main.transform.position;
            Vector3 vGazeDir = Camera.main.transform.forward;
            _menuUI.transform.position = (vHeadPos + vGazeDir * 2.5f) + new Vector3(0.0f, -.40f, 0.0f);
            Vector3 vRot = Camera.main.transform.eulerAngles; vRot.z = 0;
            _menuUI.transform.eulerAngles = vRot;
        }
    }


    void UpdatePLayerHealthText(int health) {
        _playerHealthText.text = health.ToString();
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
        _uIMenuActive = false;
        _screenOverlay.SetActive(true);
        _startScreen.SetActive(false);
        _pauseScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
    }
    void ShowStartScreen() {
        _uIMenuActive = true;
        _screenOverlay.SetActive(false);
        _startScreen.SetActive(true);
        _pauseScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
    }
    void ShowPauseScreen() {
        _uIMenuActive = true;
        _screenOverlay.SetActive(false);
        _startScreen.SetActive(false);
        _pauseScreen.SetActive(true);
        _gameOverScreen.SetActive(false);
    }

    void ShowGameOverScreen(GameOverReason reason) {
        _uIMenuActive = true;
        string gameOverText;

        _screenOverlay.SetActive(false);
        _gameOverScreen.SetActive(true);

        switch (reason) {
            case GameOverReason.GameWon:
                gameOverText = $"Congrats, You Won!\n You Got a Score of {_playerHealthText.text}";
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
