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
    AudioSource _audioSource;

    [SerializeField] InputActionReference pauseReference;


    private void OnEnable() {
        pauseReference.action.started += OnPausePressed;
    }
    private void OnDisable() {
        pauseReference.action.started -= OnPausePressed;
    }



    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame() {
        Application.Quit();
    }




    void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }

        CurrentGameState = GameState.Started;
        Time.timeScale = 0;
        _audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        onLoadGame?.Invoke();
        _audioSource.Play();
    }

    void SetGameDifficulty(int difficulty) {
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

    public void StartGame(int difficulty) {
        SetGameDifficulty(difficulty);
        Time.timeScale = 1;
        CurrentGameState = GameState.Running;
        onStartGame?.Invoke();
        onResumeGame?.Invoke();
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
        onResumeGame?.Invoke();
    }
    void PauseGame() {
        Time.timeScale = 0;
        CurrentGameState = GameState.Paused;
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
    }

}
