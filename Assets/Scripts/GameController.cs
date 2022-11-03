using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{

    public static event Action<GameOverReason> onGameOver;
    public static event Action onResumeGame;
    public static event Action onStartGame;
    public static event Action onLoadGame;
    public static event Action onPauseGame;
    public static GameController Instance { get; private set; }
    public Difficulty GameDifficulty { get; private set; }
    public GameState CurrentGameState { get; private set; }

    [SerializeField] InputActionReference pauseReference;
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _camOffset;


    private void OnEnable() {
        pauseReference.action.started += OnPausePressed;
    }
    private void OnDisable() {
        pauseReference.action.started -= OnPausePressed;
    }



    public void Restart() {
        AudioManager.Instance.PlayAudio(AudioReason.GameRestarted);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame() {
        AudioManager.Instance.PlayAudio(AudioReason.GameQuit);
        Application.Quit();
    }




    void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }

        CurrentGameState = GameState.Started;
        // Time.timeScale = 0;
    }

    void Start() {
        onLoadGame?.Invoke();
        AudioManager.Instance.PlayGameMusic();
    }

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
        StartCoroutine(RotatePlayerUpwards());
        CurrentGameState = GameState.Running;
        AudioManager.Instance.PlayAudio(AudioReason.GameStarted);
        onStartGame?.Invoke();
        onResumeGame?.Invoke();
    }

    IEnumerator RotatePlayerUpwards(){
        float rotDuration = 4f;
        float time = 0;
        Quaternion startAngle = _player.transform.rotation;
        Quaternion targetAngle = Quaternion.identity;
        yield return new WaitForSeconds(2f);
        while(time < rotDuration){
            _player.transform.rotation = Quaternion.Lerp(startAngle, targetAngle, time / rotDuration);
            time += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        _player.transform.rotation = Quaternion.Euler(270, 0, 0);
        _camOffset.transform.rotation = Quaternion.Euler(90, 0, 0);



    }

    void SetPLayerStartHealth() {


    }

    void OnPausePressed(InputAction.CallbackContext context){
        if(CurrentGameState == GameState.Running){
            PauseGame();
        } else if(CurrentGameState == GameState.Paused){
            ContinueGame();
        }
    }

    public void ContinueGame() {
        Time.timeScale = 1;
        CurrentGameState = GameState.Running;
        AudioManager.Instance.PlayAudio(AudioReason.GameResumed);
        onResumeGame?.Invoke();
    }
    void PauseGame() {
        Time.timeScale = 0;
        CurrentGameState = GameState.Paused;
        AudioManager.Instance.PlayAudio(AudioReason.GamePaused);
        onPauseGame?.Invoke();
    }

    void SetTimeScaleToZero() {
        Time.timeScale = 0;
    }
    void SetTimeScaleToOne() {
        Time.timeScale = 1;
    }

    public void EndGame(GameOverReason reason) {
        Time.timeScale = 0;
        CurrentGameState = GameState.GameOver;
        onGameOver?.Invoke(reason);
        if(reason == GameOverReason.GameWon){
            AudioManager.Instance.PlayAudio(AudioReason.GameWon);
        } else{
            AudioManager.Instance.PlayAudio(AudioReason.GameLost);
        }
    }

}
