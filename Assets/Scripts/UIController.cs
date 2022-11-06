using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    int playerHealth;
    bool _uIMenuActive;
    bool _timerFlashCoActive = false;
    string _highScoreName;
    [SerializeField] TextMeshProUGUI _playerHealthText;
    [SerializeField] TextMeshProUGUI _ballsRemainingText;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] Image _timerCircle;
    [SerializeField] Image _redTimerCircle;
    [SerializeField] TextMeshProUGUI _gameOverText;
    [SerializeField] GameObject _startScreen;
    [SerializeField] GameObject _settingsScreen;
    [SerializeField] GameObject _howToPlayScreen;
    [SerializeField] GameObject _controllsScreen;
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
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _sFXVolumeSlider;
    [SerializeField] GameObject _highScorePromptScreen;
    [SerializeField] GameObject _highScoreInputScreen;
    [SerializeField] GameObject _beginnerHighScoreScreen;
    [SerializeField] GameObject _normalHighScoreScreen;
    [SerializeField] GameObject _expertHighScoreScreen;
    [SerializeField] TMP_InputField _highScoreNameInput;
    [SerializeField] Transform _highScoreEntry;
    [SerializeField] Transform[] _highScoreContainer;
    List<GameObject> _gameScreens;

    void OnEnable() {
        BallManager.onBallsLeftCountChange += UpdateBallsRemainingText;
        TimerController.onUpdateTimer += UpdateTimerText;
        PlayerPointsManager.onUpdatePlayerPoints += UpdatePLayerHealthText;
        PowerUpManager.onUpdatePowerUpTime += UpdatePowerUpTimer;
        GameController.onResumeGame += ShowRunningGameUI;
        GameController.onLoadGame += ShowStartScreen;
        GameController.onPauseGame += ShowPauseScreen;
        GameController.onGameOver += GameOverUIActions;
        
    }

    void OnDisable() {
        BallManager.onBallsLeftCountChange -= UpdateBallsRemainingText;
        TimerController.onUpdateTimer -= UpdateTimerText;
        PlayerPointsManager.onUpdatePlayerPoints -= UpdatePLayerHealthText;
        PowerUpManager.onUpdatePowerUpTime -= UpdatePowerUpTimer;
        GameController.onResumeGame -= ShowRunningGameUI;
        GameController.onLoadGame -= ShowStartScreen;
        GameController.onPauseGame -= ShowPauseScreen;
        GameController.onGameOver -= GameOverUIActions;
    }
    private void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }
        CollectGameScreens();
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

    void CollectGameScreens(){
        _gameScreens = new List<GameObject>();
        _gameScreens.Add(_startScreen);
        _gameScreens.Add(_controllsScreen);
        _gameScreens.Add(_pauseScreen);
        _gameScreens.Add(_gameOverScreen);
        _gameScreens.Add(_settingsScreen);
        _gameScreens.Add(_howToPlayScreen);
        _gameScreens.Add(_highScoreInputScreen);
        _gameScreens.Add(_highScorePromptScreen);
        _gameScreens.Add(_expertHighScoreScreen);
        _gameScreens.Add(_normalHighScoreScreen);
        _gameScreens.Add(_beginnerHighScoreScreen);
    }
    void SetOnlyScreenActive(GameObject screen){
        foreach(GameObject s in _gameScreens){
            s.SetActive(false);
        }
        screen.SetActive(true);
    }
    void DeactivateAllScreens(){
        foreach(GameObject s in _gameScreens){
            s.SetActive(false);
        }
    }

    void Update() {
        if(_uIMenuActive){
            Vector3 vHeadPos = Camera.main.transform.position;
            Vector3 vGazeDir = Camera.main.transform.forward;
            _menuUI.transform.position = (vHeadPos + vGazeDir * 2.5f) + new Vector3(0.0f, -.40f, 0.0f);
            Vector3 vRot = Camera.main.transform.eulerAngles;
            vRot.z = 0;
            // if(GameController.Instance.CurrentGameState == GameState.Started){
            //     vRot.y *= -1;
            // }
            _menuUI.transform.eulerAngles = vRot;
        }
    }
    
    public void ShowSettingsScreen(){
        SetOnlyScreenActive(_settingsScreen);
        _musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        _sFXVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
    }
    public void ShowHowToPlayScreen(){
        SetOnlyScreenActive(_howToPlayScreen);
    }
    public void ShowControllsScreen(){
        SetOnlyScreenActive(_controllsScreen);
    }

    public void OnMusicVolumeSliderChanged(float vol){
        if(_settingsScreen.activeSelf){
            AudioManager.Instance.SetMusicVolume(vol);
        }
    }
    public void OnSFXVolumeSliderChanged(float vol){
        if(_settingsScreen.activeSelf){
            AudioManager.Instance.SetSFXVolume(vol);
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

    void ShowRunningGameUI() {
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
        DeactivateAllScreens();
    }
    public void ShowStartScreen() {
        _uIMenuActive = true;
        _timerPointsUI.SetActive(false);
        _ballsRemainingUI.SetActive(false);
        SetOnlyScreenActive(_startScreen);
    }
    void ShowPauseScreen() {
        _wall1.SetActive(false);
        _wall2.SetActive(false);
        _pausedText.enabled = true;
        _uIMenuActive = true;
        _timerPointsUI.SetActive(false);
        _powerUpsUI.SetActive(false);
        _ballsRemainingUI.SetActive(false);
        SetOnlyScreenActive(_pauseScreen);
    }

    void GameOverUIActions(){
        _wall1.SetActive(false);
        _wall2.SetActive(false);
        _racket.SetActive(false);
        _timerPointsUI.SetActive(false);
        _powerUpsUI.SetActive(false);
        _ballsRemainingUI.SetActive(false);
        AudioManager.Instance.ResetGameMusicSpeed();
        StopCoroutine(FlashTimer());

    }

    public void ShowGameOverScreen(GameOverReason reason) {
        _uIMenuActive = true;
        string gameOverText;

        SetOnlyScreenActive(_gameOverScreen);

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

    public void ShowHighScorePromptScreen(){
        _uIMenuActive = true;
        SetOnlyScreenActive(_highScorePromptScreen);
    }
    public void ShowHighScoreInputScreen(){
        _highScorePromptScreen.SetActive(false);
        _highScoreInputScreen.SetActive(true);
        SetOnlyScreenActive(_highScoreInputScreen);
    }

    public void OnHighScoreNameSubmitted(){
        _highScoreName = _highScoreNameInput.text;
        if(_highScoreName == "") _highScoreName = "<Anonymous>";
        GameController.Instance.SubmitHighScore(_highScoreName);
    }
    public void LoadHighScoreBoards(){
        List<HighScore>[] highScoreData = new List<HighScore>[3];
        highScoreData[0] = HighScoresManager.LoadHighScores(Difficulty.Beginner);
        highScoreData[1] = HighScoresManager.LoadHighScores(Difficulty.Normal);
        highScoreData[2] = HighScoresManager.LoadHighScores(Difficulty.Expert);

        float templateHeight = 10f;

        for(int j = 0; j < 3; j++){
            for(int i = 0; i < 10; i++){
                Transform entryTransform = Instantiate(_highScoreEntry, _highScoreContainer[j]);
                RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
                entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
                entryTransform.gameObject.SetActive(true);

                int rank = i + 1;

                if(highScoreData[j][i].score == -1){
                entryTransform.Find("Rank Text").GetComponent<TextMeshProUGUI>().text = rank.ToString();
                entryTransform.Find("Name Text").GetComponent<TextMeshProUGUI>().text = "";
                entryTransform.Find("Date Text").GetComponent<TextMeshProUGUI>().text = "";
                entryTransform.Find("Score Text").GetComponent<TextMeshProUGUI>().text = "";
                return;
                }

                entryTransform.Find("Rank Text").GetComponent<TextMeshProUGUI>().text = rank.ToString();
                entryTransform.Find("Name Text").GetComponent<TextMeshProUGUI>().text = highScoreData[j][i].name;
                entryTransform.Find("Date Text").GetComponent<TextMeshProUGUI>().text = highScoreData[j][i].date;
                entryTransform.Find("Score Text").GetComponent<TextMeshProUGUI>().text = highScoreData[j][i].score.ToString();
            }
        }
    }

    public void ShowHighScoresScreens(){
        LoadHighScoreBoards();
        switch(GameController.Instance.GameDifficulty){
            case Difficulty.Beginner:
                SetOnlyScreenActive(_beginnerHighScoreScreen);
                break;
            case Difficulty.Normal:
                SetOnlyScreenActive(_normalHighScoreScreen);
                break;
            default:
                SetOnlyScreenActive(_expertHighScoreScreen);
                break;
        }
    }

    public void ShowBeginnerHighScoresScreen(){
        SetOnlyScreenActive(_beginnerHighScoreScreen);
    }
    public void ShowNormalHighScoresScreen(){
        SetOnlyScreenActive(_normalHighScoreScreen);
    }
    public void ShowExpertHighScoresScreen(){
        SetOnlyScreenActive(_expertHighScoreScreen);
    }
    
}
