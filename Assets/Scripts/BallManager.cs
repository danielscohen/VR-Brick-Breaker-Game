using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] GameDifficultySettings difficultySettings;
    [SerializeField] GameObject _ballPrefab;
    [SerializeField] GameObject _throwPt;
    [SerializeField] TextMeshProUGUI powerText;
    [SerializeField] float throwForce;
    [SerializeField] float throwUpwordForce;


    List<GameObject> activeBalls = new List<GameObject>(); 
    Stack<GameObject> ballPool = new Stack<GameObject>();

    int _ballsRemaining;
    GameObject _loadedBall = null;
    int _maxBallID = 0;
    KeyCode throwKey = KeyCode.Mouse0;
    float timeDown;
    float timePressed;
    bool keyPressed = false;
    bool readyToThrow = true;

    void OnEnable() {
        DestroyZoneController.onBallLost += ManageBallLost;
    }

    void OnDisable() {
        DestroyZoneController.onBallLost -= ManageBallLost;
    }

    void Awake() {
        _ballsRemaining = difficultySettings.ballLimit;
    }

    void Start() {
        UpdateThrowPowerText(0);
        LoadNewBall();
    }

    void Update() {
        if(Input.GetKeyDown(throwKey) && readyToThrow && _loadedBall != null) {
            keyPressed = true;
            timeDown = Time.time;
        }

        if(Input.GetKeyUp(throwKey) && readyToThrow && _loadedBall != null) {
            keyPressed = false;
            timePressed = Time.time - timeDown;
            Throw(timePressed);
        }

        if (keyPressed) {
            UpdateThrowPowerText(Time.time - timeDown);
        }
    }

    void ManageBallLost(int ballID) {
        var index = activeBalls.FindIndex(x => x.GetComponent<BallController>().BallID == ballID);
        if(index < 0) {
            Debug.LogError($"Ball with ballID {ballID} was lost but never thrown.");
            return;
        }
        var ball = activeBalls[index];
        activeBalls.RemoveAt(index);
        DisableBall(ball);
        if (activeBalls.Count == 0 && _ballsRemaining == 0) {
            GameController.Instance.EndGame();
            return;
        }

    }

    void DisableBall(GameObject ball) {
        var ballRB = ball.GetComponent<Rigidbody>();
        ballRB.velocity = Vector3.zero;
        ballRB.angularVelocity = Vector3.zero;
        ball.SetActive(false);
        ballPool.Push(ball);
    }

    void LoadNewBall() {
        if (ballPool.Count > 0) {
            _loadedBall = ballPool.Pop();
        } else {
            _loadedBall = Instantiate(_ballPrefab);
            _maxBallID++;
            _loadedBall.GetComponent<BallController>().BallID = _maxBallID;
        }

        _loadedBall.GetComponent<Rigidbody>().position = _throwPt.transform.position;
        _ballsRemaining--;
    }
    void UpdateThrowPowerText(float power) {
        powerText.text = string.Format("{0:N2}", power);
    }
    private void Throw(float timePressed) {


        Rigidbody ballRB = _loadedBall.GetComponent<Rigidbody>();

        Vector3 forceToAdd = (_throwPt.transform.forward * throwForce  + transform.up * throwUpwordForce) * timePressed;

        ballRB.AddForce(forceToAdd, ForceMode.Impulse);

        if (_ballsRemaining > 0) {
            LoadNewBall();
        }

    }
}
