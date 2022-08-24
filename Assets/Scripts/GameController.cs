using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public static event Action<GameOverReason> onGameOver;
    public static event Action onResumeGame;
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
    }

    void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        Debug.Log("started");
        SetGameDifficulty(difficulty);
        Time.timeScale = 1;
        onResumeGame?.Invoke();
    }

    void SetTimeScaleToZero() {
        Time.timeScale = 0;
    }
    void SetTimeScaleToOne() {
        Time.timeScale = 1;
    }

    public void EndGame(GameOverReason reason) {
        Debug.Log($"GAME OVER: {reason}");
        onGameOver?.Invoke(reason);
    }

}
