using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public static event Action<GameOverReason> onGameOver;
    public static event Action onResumeGame;
    public static event Action onStartGame;
    public static event Action onPauseGame;
    public int PlayerHealth { get; private set; }
    public static GameController Instance { get; private set; }
    public Difficulty GameDifficulty { get; private set; }
    public GameState CurrentGameState { get; private set; }

    [SerializeField] int playerStartHealth;



    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Restart();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            PauseGame();
        }
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame() {
        Application.Quit();
    }

    void OnEnable() {
        PlayerCollider.onFragCollideWithPlayer += DecreasePlayerHealth;
    }

    private void OnDisable() {
        PlayerCollider.onFragCollideWithPlayer -= DecreasePlayerHealth;
    }


    void DecreasePlayerHealth(int healthValDecreaseBy) {
        PlayerHealth -= healthValDecreaseBy;
        if (PlayerHealth <= 0) {
            EndGame(GameOverReason.HealthRanOut);
        }
    }

    void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }

        PlayerHealth = playerStartHealth;
        CurrentGameState = GameState.Started;
        Time.timeScale = 0;
    }

    void Start() {
        onStartGame?.Invoke();
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
        onResumeGame?.Invoke();
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
