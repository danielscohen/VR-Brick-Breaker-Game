using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    int playerHealth;
    bool _uIMenuActive;
    [SerializeField] TextMeshProUGUI _playerHealthText;
    [SerializeField] TextMeshProUGUI _ballsRemainingText;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] Image _timerCircle;
    [SerializeField] Image _redTimerCircle;
    [SerializeField] TextMeshProUGUI _gameOverText;
    [SerializeField] GameObject _startScreen;
    [SerializeField] GameObject _timerPointsUI;
    [SerializeField] GameObject _ballsRemainingUI;
    [SerializeField] GameObject _pauseScreen;
    [SerializeField] GameObject _gameOverScreen;
    [SerializeField] GameObject _menuUI;
    [SerializeField] GameObject _doublePointsPanel;
    [SerializeField] GameObject _negativePointsPanel;
    [SerializeField] GameObject _movePanel;
    [SerializeField] Image _doublePointsTimer;
    [SerializeField] Image _negativePointsTimer;
    [SerializeField] Image _moveTimer;
    void OnEnable() {
        BallManager.onBallsLeftCountChange += UpdateBallsRemainingText;
        TimerController.onUpdateTimer += UpdateTimerText;
        PlayerPointsManager.onUpdatePlayerPoints += UpdatePLayerHealthText;
        PowerUpManager.onUpdatePowerUpTime += UpdatePowerUpTimer;
        GameController.onResumeGame += ShowScreenOverlay;
        GameController.onGameOver += ShowGameOverScreen;
        GameController.onLoadGame += ShowStartScreen;
        GameController.onPauseGame += ShowPauseScreen;
        
    }

    void OnDisable() {
        BallManager.onBallsLeftCountChange -= UpdateBallsRemainingText;
        TimerController.onUpdateTimer -= UpdateTimerText;
        PlayerPointsManager.onUpdatePlayerPoints -= UpdatePLayerHealthText;
        PowerUpManager.onUpdatePowerUpTime -= UpdatePowerUpTimer;
        GameController.onResumeGame -= ShowScreenOverlay;
        GameController.onGameOver -= ShowGameOverScreen;
        GameController.onLoadGame -= ShowStartScreen;
        GameController.onPauseGame -= ShowPauseScreen;
    }

    private void Start() {
        _doublePointsPanel.SetActive(false);
        _negativePointsPanel.SetActive(false);
        _movePanel.SetActive(false);
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
    void UpdateBallsRemainingText(int numBalls) {
        _ballsRemainingText.text = $"{numBalls} Orbs Left";
    }
    void UpdateTimerText(string time, float timePer) {
        Image hiddenTimer;
        Image timer;
        if(timePer < 0.25){
            timer = _redTimerCircle;
            hiddenTimer = _timerCircle;
        }
        else {
            timer = _timerCircle;
            hiddenTimer = _redTimerCircle;
        }
        _timerText.text = time;
        hiddenTimer.enabled = false;
        timer.enabled = true;
        timer.fillAmount = timePer;
    }
    void UpdatePowerUpTimer(PowerUpType type, float timePer) {
        GameObject timer;
        Image timerImage;
        switch(type){
            case PowerUpType.DoublePoints:
                timer = _doublePointsPanel;
                timerImage = _doublePointsTimer;
                break;
            case PowerUpType.MoveWalls:
                timer = _movePanel;
                timerImage = _moveTimer;
                break;
            default:
                timer = _negativePointsPanel;
                timerImage = _negativePointsTimer;
                break;
        }
        if(timePer <= 0){ timer.SetActive(false);}
        else{
            timer.SetActive(true);
            timerImage.fillAmount = timePer;
        }
    }

    void ShowScreenOverlay() {
        _uIMenuActive = false;
        _timerPointsUI.SetActive(true);
        _ballsRemainingUI.SetActive(true);
        _startScreen.SetActive(false);
        _pauseScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
    }
    void ShowStartScreen() {
        _uIMenuActive = true;
        _timerPointsUI.SetActive(false);
        _ballsRemainingUI.SetActive(false);
        _startScreen.SetActive(true);
        _pauseScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
    }
    void ShowPauseScreen() {
        _uIMenuActive = true;
        _timerPointsUI.SetActive(false);
        _ballsRemainingUI.SetActive(false);
        _startScreen.SetActive(false);
        _pauseScreen.SetActive(true);
        _gameOverScreen.SetActive(false);
    }

    void ShowGameOverScreen(GameOverReason reason) {
        _uIMenuActive = true;
        string gameOverText;

        _timerPointsUI.SetActive(false);
        _ballsRemainingUI.SetActive(false);
        _gameOverScreen.SetActive(true);

        switch (reason) {
            case GameOverReason.GameWon:
                gameOverText = $"Congrats, You Won!\n Your Score: {_playerHealthText.text}";
                break;
            case GameOverReason.HealthRanOut:
                gameOverText = "Game Over\nYou Ran Out of Health";
                break;
            case GameOverReason.TimeRanOut:
                gameOverText = "Game Over\nYou Ran Out of Time";
                break;
            default:
                gameOverText = "Game Over\nYou Ran Out of Orbs";
                break;
        }

        _gameOverText.text = gameOverText;
    }
    
}
