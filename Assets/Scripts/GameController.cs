using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{

    public static event Action onGameOver;
    public static event Action onResumeGame;
    public static event Action onStartGame;
    public static event Action onLoadGame;
    public static event Action onPauseGame;
    public static event Action onHighScore;
    public static GameController Instance { get; private set; }
    public Difficulty GameDifficulty { get; private set; }
    public GameState CurrentGameState { get; private set; }
    public bool BallIsBeingHeld { get; set; }

    [SerializeField] InputActionReference pauseReference;
    [SerializeField] InputActionReference endGameRef;
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _camOffset;
    List<HighScore> _highScores;
    int _score;

    Vector3 _playerStartPos;
    Vector3 _playerMenuPos;
    Vector3 _playerLastPos;


    private void OnEnable() {
        pauseReference.action.started += OnPausePressed;
        // endGameRef.action.started += onEndGame;
    }
    private void OnDisable() {
        pauseReference.action.started -= OnPausePressed;
        // endGameRef.action.started -= onEndGame;
    }

    // private void Update() {
    //     Debug.Log(_player.transform.position);
    // }



    public void Restart() {
        AudioManager.Instance.PlayAudio(AudioReason.GameRestarted);
        PersistentValues.IsFirstScene = false;
        PersistentValues.MusicVolume = AudioManager.Instance.GetMusicVolume();
        PersistentValues.SFXVolume = AudioManager.Instance.GetSFXVolume();
        PersistentValues.GameDifficulty = GameDifficulty;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame() {
        AudioManager.Instance.PlayAudio(AudioReason.GameQuit);
        Application.Quit();
    }

    void ResetPlayerPosition(){
        _playerLastPos = _player.transform.position;
        _player.transform.position = _playerMenuPos;
    }
    void RestorePlayerPosition(){
        _player.transform.position = _playerLastPos;
    }




    void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }


        CurrentGameState = GameState.Started;
        BallIsBeingHeld = false;
        if(PersistentValues.IsFirstScene){
            GameDifficulty = Difficulty.Normal;
        } 
        else{
            GameDifficulty = PersistentValues.GameDifficulty;
        }
        // Time.timeScale = 0;
    }

    void Start() {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame(){
        Time.timeScale = 1;
        AudioManager.Instance.PlayGameMusic();
        yield return StartCoroutine(UIController.Instance.AnimateEnterTitleText());
        onLoadGame?.Invoke();
        _playerMenuPos = _player.transform.position;
        // TestPLayerPrefs();

    }

    // private void Update() {
    //     Debug.Log($"var: {GameDifficulty}, Persist: {PersistentValues.GameDifficulty}");
    // }

    public void SetGameDifficulty(int difficulty) {
        switch (difficulty) {
            case 0:
                GameDifficulty = Difficulty.Beginner;
                break;
            case 1:
                GameDifficulty = Difficulty.Normal;
                break;
            default:
                GameDifficulty = Difficulty.Expert;
                break;
        }
    }

    public void StartGame() {
        // Time.timeScale = 1;
        // StartCoroutine(RotatePlayerUpwards());
        StartCoroutine(StartGameCo());
    }

    IEnumerator StartGameCo(){
        AudioManager.Instance.StopGameMusic();
        AudioManager.Instance.PlayButtonPressedAudio();
        CurrentGameState = GameState.Running;
        onStartGame?.Invoke();
        onResumeGame?.Invoke();
        yield return StartCoroutine(UIController.Instance.AnimateExitTitleText());
        _player.transform.position = new Vector3(_playerMenuPos.x, _playerMenuPos.y, _playerMenuPos.z - 1000f);
        AudioManager.Instance.PlayGameMusic();
        BallIsBeingHeld = false;
    }

    void TestPLayerPrefs(){
        HighScore newHighScore = new HighScore{name = "dan", date = DateTime.Now.ToString("d-M-yyyy"), score = 5};
        List<HighScore> hs = new List<HighScore>();
        hs.Add(newHighScore);
        for(int i = 0; i < 9; i++){
            hs.Add(new HighScore{name = "a", date = "a", score = -1});
        }

        HighScoresManager.SaveHighScores(GameDifficulty, hs);
    }

    IEnumerator RotatePlayerUpwards(){
        float rotDuration = 4f;
        float time = 0;
        Quaternion startAngle = _player.transform.rotation;
        Quaternion targetAngle = Quaternion.Euler(-90, 0, 0);
        yield return new WaitForSeconds(2f);
        while(time < rotDuration){
            _player.transform.rotation = Quaternion.Lerp(startAngle, targetAngle, time / rotDuration);
            time += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
    }


    void OnPausePressed(InputAction.CallbackContext context){
        if(CurrentGameState == GameState.Running){
            PauseGame();
        } else if(CurrentGameState == GameState.Paused){
            ContinueGame();
        }
    }
    void onEndGame(InputAction.CallbackContext context){
        EndGame(GameOverReason.GameWon);
    }

    public void ContinueGame() {
        Time.timeScale = 1;
        RestorePlayerPosition();
        CurrentGameState = GameState.Running;
        AudioManager.Instance.PlayAudio(AudioReason.GameResumed);
        onResumeGame?.Invoke();
    }
    void PauseGame() {
        if(BallIsBeingHeld){
            StartCoroutine(UIController.Instance.ShowBallHeldPauseScreen());
            return;
        }
        ResetPlayerPosition();
        CurrentGameState = GameState.Paused;
        AudioManager.Instance.PlayAudio(AudioReason.GamePaused);
        onPauseGame?.Invoke();
        Time.timeScale = 0;
    }


    void SetTimeScaleToZero() {
        Time.timeScale = 0;
    }
    void SetTimeScaleToOne() {
        Time.timeScale = 1;
    }

    public void DebugEndGame(){
        EndGame(GameOverReason.GameWon);
    }

    public void EndGame(GameOverReason reason) {
        StartCoroutine(EndGameCo(reason));
    }

    IEnumerator EndGameCo(GameOverReason reason){
        ResetPlayerPosition();
        CurrentGameState = GameState.GameOver;
        onGameOver?.Invoke();
        if(reason == GameOverReason.GameWon){
            AudioManager.Instance.PlayAudio(AudioReason.GameWon);
            yield return StartCoroutine(UIController.Instance.AnimateEnterGameWonText());
            StartCoroutine(UIController.Instance.PlayFireworks());
            _score = GameObject.Find("Player Points Manager").GetComponent<PlayerPointsManager>().GetScore();
            _highScores = HighScoresManager.LoadHighScores(GameDifficulty);

            int index = CheckIfHighScore(_score, _highScores);

            if(index == -1){
                UIController.Instance.ShowGameOverScreen(GameOverReason.GameWon);
            } else {
                UIController.Instance.ShowGameOverScreen(GameOverReason.GameWonHS);
            }
        } else{
            AudioManager.Instance.PlayAudio(AudioReason.GameLost);
            yield return StartCoroutine(UIController.Instance.AnimateEnterGameLostText());
            UIController.Instance.ShowGameOverScreen(reason);
        }


    }

    int CheckIfHighScore(int score, List<HighScore> scores){
        for(int i = 9; i > -2; i--){
            if(i == -1){
                return 0;
            }
            if(score < scores[i].score && i < 9){
                return i + 1;
            }
        }

        return -1;
    }

    public void SubmitHighScore(string name){
        int index = CheckIfHighScore(_score, _highScores);
        HighScore newHighScore = new HighScore{name = name, date = DateTime.Now.ToString("d/M/yyyy"), score = _score};
        _highScores.Insert(index, newHighScore);
        if(_highScores.Count > 10){
            _highScores.RemoveAt(10);
        }
        HighScoresManager.SaveHighScores(GameDifficulty, _highScores);
        UIController.Instance.ShowHighScoresScreens();
    }


}
