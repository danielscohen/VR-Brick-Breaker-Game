using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    enum GameState {
        StartScreen, Running, Paused, EndScreen
    }
    public static event Action<GameOverReason> onGameOver;
    public int PlayerHealth { get; private set; }
    GameState CurrentGameState { get; set; }
    public static GameController Instance { get; private set; }

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
        PlayerCollider.fragCollideWithPlayer += DecreasePlayerHealth;
    }

    private void OnDisable() {
        PlayerCollider.fragCollideWithPlayer -= DecreasePlayerHealth;
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
        CurrentGameState = GameState.StartScreen;
    }

    void Start() {

        

        
    }

    public void EndGame(GameOverReason reason) {
        Debug.Log($"GAME OVER: {reason}");
        onGameOver?.Invoke(reason);
    }

}
