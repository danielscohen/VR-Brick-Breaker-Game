using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    int playerHealth;
    bool _uIMenuActive;
    bool _timerFlashCoActive = false;
    [SerializeField] TextMeshProUGUI _playerHealthText;
    [SerializeField] TextMeshProUGUI _ballsRemainingText;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] Image _timerCircle;
    [SerializeField] Image _redTimerCircle;
    [SerializeField] TextMeshProUGUI _gameOverText;
    [SerializeField] GameObject _startScreen;
    [SerializeField] GameObject _settingsScreen;
    [SerializeField] GameObject _howToPlayScreen;
    [SerializeField] GameObject _timerPointsUI;
    [SerializeField] GameObject _powerUpsUI;
    [SerializeField] GameObject _ballsRemainingUI;
    [SerializeField] GameObject _pauseScreen;
    [SerializeField] GameObject _gameOverScreen;
    [SerializeField] GameObject _menuUI;
    [SerializeField] GameObject _doublePointsPanel;
    [SerializeField] GameObject _negativePointsPanel;
    [SerializeField] GameObject _movePanel;
    [SerializeField] GameObject _wall1;
    [SerializeField] GameObject _wall2;
    [SerializeField] GameObject _racket;
    [SerializeField] TextMeshPro _gameTitle;
    [SerializeField] TextMeshPro _pausedText;
    [SerializeField] TextMeshPro _gameOverBigText;
    [SerializeField] TextMeshPro _victoryText;
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
        _wall1.SetActive(false);
        _wall2.SetActive(false);
        _racket.SetActive(false);
        _gameTitle.enabled = true;
        _pausedText.enabled = false;
        _gameOverBigText.enabled = false;
        _victoryText.enabled = false;
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
    
    public void ShowSettingsScreen(){
        _startScreen.SetActive(false);
        _settingsScreen.SetActive(true);

    }
    public void ShowHowToPlayScreen(){
        _startScreen.SetActive(false);
        _howToPlayScreen.SetActive(true);
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
        if(timePer < 0.20){
            timer = _redTimerCircle;
            hiddenTimer = _timerCircle;
            _timerText.color = Color.red;
            if(!_timerFlashCoActive){
                AudioManager.Instance.SpeedUpGameMusic();
                StartCoroutine(FlashTimer());
            }
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
    IEnumerator FlashTimer(){
        _timerFlashCoActive = true;
        while(true){
            AudioManager.Instance.PlayAudio(AudioTypes.TimerFlash);
            _redTimerCircle.enabled = true;
            _timerText.enabled = true;
            yield return new WaitForSeconds(1f);
            _redTimerCircle.enabled = false;
            _timerText.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }
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
        _wall1.SetActive(true);
        _wall2.SetActive(true);
        _racket.SetActive(true);
        _gameTitle.enabled = false;
        _pausedText.enabled = false;
        _gameOverBigText.enabled = false;
        _victoryText.enabled = false;
        _uIMenuActive = false;
        _timerPointsUI.SetActive(true);
        _powerUpsUI.SetActive(true);
        _ballsRemainingUI.SetActive(true);
        _startScreen.SetActive(false);
        _pauseScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
    }
    public void ShowStartScreen() {
        _uIMenuActive = true;
        _timerPointsUI.SetActive(false);
        _ballsRemainingUI.SetActive(false);
        _startScreen.SetActive(true);
        _pauseScreen.SetActive(false);
        _gameOverScreen.SetActive(false);
        _settingsScreen.SetActive(false);
        _howToPlayScreen.SetActive(false);
    }
    void ShowPauseScreen() {
        _wall1.SetActive(false);
        _wall2.SetActive(false);
        _pausedText.enabled = true;
        _uIMenuActive = true;
        _timerPointsUI.SetActive(false);
        _powerUpsUI.SetActive(false);
        _ballsRemainingUI.SetActive(false);
        _startScreen.SetActive(false);
        _pauseScreen.SetActive(true);
        _gameOverScreen.SetActive(false);
    }

    void ShowGameOverScreen(GameOverReason reason) {
        _wall1.SetActive(false);
        _wall2.SetActive(false);
        _racket.SetActive(false);
        AudioManager.Instance.ResetGameMusicSpeed();
        StopCoroutine(FlashTimer());
        _uIMenuActive = true;
        string gameOverText;

        _timerPointsUI.SetActive(false);
        _powerUpsUI.SetActive(false);
        _ballsRemainingUI.SetActive(false);
        _gameOverScreen.SetActive(true);

        switch (reason) {
            case GameOverReason.GameWon:
                _victoryText.enabled = true;
                gameOverText = $"Your Score: {_playerHealthText.text}";
                break;
            case GameOverReason.HealthRanOut:
                _gameOverBigText.enabled = true;
                gameOverText = "You Ran Out of Health";
                break;
            case GameOverReason.TimeRanOut:
                _gameOverBigText.enabled = true;
                gameOverText = "You Ran Out of Time";
                break;
            default:
                _gameOverBigText.enabled = true;
                gameOverText = "You Ran Out of Orbs";
                break;
        }

        _gameOverText.text = gameOverText;
    }
    
}
