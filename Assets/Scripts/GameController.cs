using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public int PlayerHealth { get; private set; }
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

    void BuildWall() {

    }

    void DecreasePlayerHealth(int healthValDecreaseBy) {
        PlayerHealth -= healthValDecreaseBy;
    }

    void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }

        PlayerHealth = playerStartHealth;
    }

}
