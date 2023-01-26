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
    TouchScreenKeyboard keyboard;
    [SerializeField] TextMeshProUGUI _playerHealthText;
    [SerializeField] TextMeshProUGUI _ballsRemainingText;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] Image _timerCircle;
    [SerializeField] Image _redTimerCircle;
    [SerializeField] TextMeshProUGUI _gameOverText;
    [SerializeField] TextMeshProUGUI _gameOverTextHS;
    [SerializeField] GameObject _startScreen;
    [SerializeField] GameObject _settingsScreen;
    [SerializeField] GameObject _howToPlayScreen;
    [SerializeField] GameObject _controllsScreen;
    [SerializeField] GameObject _timerPointsUI;
    [SerializeField] GameObject _powerUpsUI;
    [SerializeField] GameObject _ballsRemainingUI;
    [SerializeField] GameObject _pauseScreen;
    [SerializeField] GameObject _gameOverScreen;
    [SerializeField] GameObject _gameOverScreenHS;
    [SerializeField] GameObject _menuUI;
    [SerializeField] GameObject _doublePointsPanel;
    [SerializeField] GameObject _negativePointsPanel;
    [SerializeField] GameObject _movePanel;
    [SerializeField] GameObject _rotatePanel;
    [SerializeField] GameObject _wall1;
    [SerializeField] GameObject _wall2;
    [SerializeField] GameObject _racket;
    [SerializeField] GameObject _gameTitle;
    [SerializeField] GameObject _pausedText;
    [SerializeField] GameObject _gameOverBigText;
    [SerializeField] GameObject _victoryText;
    [SerializeField] Image _doublePointsTimer;
    [SerializeField] Image _negativePointsTimer;
    [SerializeField] Image _moveTimer;
    [SerializeField] Image _rotateTimer;
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _sFXVolumeSlider;
    [SerializeField] TMP_Dropdown _diffDropDown;
    [SerializeField] GameObject _highScorePromptScreen;
    [SerializeField] GameObject _highScoreInputScreen;
    [SerializeField] GameObject _beginnerHighScoreScreen;
    [SerializeField] GameObject _normalHighScoreScreen;
    [SerializeField] GameObject _expertHighScoreScreen;
    [SerializeField] GameObject _BallHeldPauseScreen;
    [SerializeField] GameObject _fireworks;
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
        _rotatePanel.SetActive(false);
        _wall1.SetActive(false);
        _wall2.SetActive(false);

    }


    void CollectGameScreens(){
        _gameScreens = new List<GameObject>();
        _gameScreens.Add(_startScreen);
        _gameScreens.Add(_controllsScreen);
        _gameScreens.Add(_pauseScreen);
        _gameScreens.Add(_gameOverScreen);
        _gameScreens.Add(_gameOverScreenHS);
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

    public void LeaveHighScoresScreen(){
        if(GameController.Instance.CurrentGameState == GameState.GameOver){
            GameController.Instance.Restart();
        }
        else {
            ShowStartScreen();
        }
    }
    
    public IEnumerator PlayFireworks(){
        for(int i = 0; i < 8; i++){
            float x = Random.Range(-2f, 2f);
            float y = Random.Range(-1.7f, 1.7f);
            float z = Random.Range(1004.7f, 1008f);
            Instantiate(_fireworks, new Vector3(x, y, z), Quaternion.identity);
            AudioManager.Instance.PlayAudio(AudioTypes.Fireworks, new Vector3(x, y, z));
            yield return new WaitForSecondsRealtime(0.2f);
        }

    }
    
    public void ShowSettingsScreen(){
        SetOnlyScreenActive(_settingsScreen);
        _musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        _sFXVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
        _diffDropDown.value = (int)GameController.Instance.GameDifficulty;

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
        if(numBalls == 0){
            _ballsRemainingText.text = $"No Orbs Left";
        } else if (numBalls == 1){
            _ballsRemainingText.text = $"{numBalls} Orb Left";

        } else {
            _ballsRemainingText.text = $"{numBalls} Orbs Left";
        }
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
            // AudioManager.Instance.PlayAudio(AudioTypes.TimerFlash);
            yield return new WaitForSeconds(0.5f);
            _timerText.enabled = true;
            yield return new WaitForSeconds(0.5f);
            _timerText.enabled = false;
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
            case PowerUpType.RotateWalls:
                timer = _rotatePanel;
                timerImage = _rotateTimer;
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
        _pausedText.transform.GetChild(0).transform.gameObject.SetActive(false);
        _wall1.SetActive(true);
        _wall2.SetActive(true);
        // SetTargetVisible(_wall1);
        // SetTargetVisible(_wall2);
        _gameTitle.SetActive(true);
        _uIMenuActive = false;
        _timerPointsUI.SetActive(true);
        _powerUpsUI.SetActive(true);
        _ballsRemainingUI.SetActive(true);
        DeactivateAllScreens();
    }

    public IEnumerator AnimateEnterTitleText(){
        yield return StartCoroutine(AnimateEnterBigText(_gameTitle));
    }
    public IEnumerator AnimateExitTitleText(){
        yield return StartCoroutine(AnimateExitBigText(_gameTitle));
    }

    public IEnumerator AnimateEnterGameWonText(){
        yield return StartCoroutine(AnimateEnterBigText(_victoryText));
    }
    public IEnumerator AnimateExitGameWonText(){
        yield return StartCoroutine(AnimateExitBigText(_victoryText));
    }
    public IEnumerator AnimateEnterGameLostText(){
        yield return StartCoroutine(AnimateEnterBigText(_gameOverBigText));
    }
    public IEnumerator AnimateExitGameLostText(){
        yield return StartCoroutine(AnimateExitBigText(_gameOverBigText));
    }
    IEnumerator AnimateEnterBigText(GameObject text){
        text.SetActive(true);
        yield return StartCoroutine(text.GetComponent<BigTextAnimator>().AnimateEnter());
    }
    IEnumerator AnimateExitBigText(GameObject text){
        yield return StartCoroutine(text.GetComponent<BigTextAnimator>().AnimateExit());
    }
    public void ShowStartScreen() {
        _uIMenuActive = true;
        // _timerPointsUI.SetActive(false);
        // _ballsRemainingUI.SetActive(false);
        SetOnlyScreenActive(_startScreen);
    }
    void ShowPauseScreen() {
        // SetTargetInvisible(_wall1);
        // SetTargetInvisible(_wall2);
        _pausedText.transform.GetChild(0).transform.gameObject.SetActive(true);
        _uIMenuActive = true;
        // _timerPointsUI.SetActive(false);
        // _powerUpsUI.SetActive(false);
        // _ballsRemainingUI.SetActive(false);
        SetOnlyScreenActive(_pauseScreen);
    }
    void SetTargetInvisible(GameObject Target)
    {
        foreach (Transform child in Target.transform)
        {
            child.gameObject.GetComponent<Renderer>().enabled = false;
        }
    }
    void SetTargetVisible(GameObject Target)
    {
        foreach (Transform child in Target.transform)
        {
            child.gameObject.GetComponent<Renderer>().enabled = true;
        }
    }

    void GameOverUIActions(){
        // _wall1.SetActive(false);
        // _wall2.SetActive(false);
        // _timerPointsUI.SetActive(false);
        // _powerUpsUI.SetActive(false);
        // _ballsRemainingUI.SetActive(false);
        AudioManager.Instance.ResetGameMusicSpeed();
        StopCoroutine(FlashTimer());

    }

    public IEnumerator ShowBallHeldPauseScreen(){
        _uIMenuActive = true;
        _BallHeldPauseScreen.SetActive(true);
        yield return new WaitForSeconds(2f);
        _BallHeldPauseScreen.SetActive(false);
        _uIMenuActive = false;
    }

    public void ShowGameOverScreen(GameOverReason reason) {
        _uIMenuActive = true;
        string gameOverText;

        SetOnlyScreenActive(_gameOverScreen);

        switch (reason) {
            case GameOverReason.GameWon:
                gameOverText = $"You Got a Score of: {_playerHealthText.text}!";
                break;
            case GameOverReason.GameWonHS:
                SetOnlyScreenActive(_gameOverScreenHS);
                gameOverText = $"You Got a Highscore of {_playerHealthText.text}!";
                break;
            case GameOverReason.HealthRanOut:
                gameOverText = "You Ran Out of Health!";
                break;
            case GameOverReason.TimeRanOut:
                gameOverText = "You Ran Out of Time!";
                break;
            default:
                gameOverText = "You Ran Out of Orbs!";
                break;
        }

        _gameOverText.text = gameOverText;
        if(reason == GameOverReason.GameWonHS){
            _gameOverTextHS.text = gameOverText;
            StartCoroutine(ShowGameOverScreenHS());
        }
    }
    public IEnumerator ShowGameOverScreenHS() {


        yield return new WaitForSecondsRealtime(3f);

        ShowHighScorePromptScreen();
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
        if(_highScoreName == "") _highScoreName = "Anonymous";
        if(_highScoreName.Length > 10){
            _highScoreName = _highScoreName.Substring(0, 10);
        }
        GameController.Instance.SubmitHighScore(_highScoreName);
    }
    public void LoadHighScoreBoards(){
        List<HighScore>[] highScoreData = new List<HighScore>[3];
        highScoreData[0] = HighScoresManager.LoadHighScores(Difficulty.Beginner);
        highScoreData[1] = HighScoresManager.LoadHighScores(Difficulty.Normal);
        highScoreData[2] = HighScoresManager.LoadHighScores(Difficulty.Expert);

        foreach(Transform container in _highScoreContainer){
            foreach(Transform entry in container){
                Destroy(entry.gameObject);
            }
        }

        float templateHeight = 10f;

        for(int j = 0; j < 3; j++){
            for(int i = 0; i < 10; i++){
                Transform entryTransform = Instantiate(_highScoreEntry, _highScoreContainer[j]);
                RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
                entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
                entryTransform.gameObject.SetActive(true);

                int rank = i + 1;

                if(highScoreData[j][i].score == -1){
                entryTransform.Find("Rank Text").GetComponent<TextMeshProUGUI>().text = "";
                entryTransform.Find("Name Text").GetComponent<TextMeshProUGUI>().text = "";
                entryTransform.Find("Date Text").GetComponent<TextMeshProUGUI>().text = "";
                entryTransform.Find("Score Text").GetComponent<TextMeshProUGUI>().text = "";
                // Debug.Log("got to return");
                continue;
                }

                entryTransform.Find("Rank Text").GetComponent<TextMeshProUGUI>().text = rank.ToString();
                entryTransform.Find("Name Text").GetComponent<TextMeshProUGUI>().text = highScoreData[j][i].name;
                entryTransform.Find("Date Text").GetComponent<TextMeshProUGUI>().text = highScoreData[j][i].date;
                entryTransform.Find("Score Text").GetComponent<TextMeshProUGUI>().text = highScoreData[j][i].score.ToString();

                // Debug.Log($"{highScoreData[j][i].name} {highScoreData[j][i].date} {highScoreData[j][i].score.ToString()}");
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
    public void ShowKeyBoard(){
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }
    
}
