using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] GameDifficultySettings difficultySettings;
    [SerializeField] GameObject _ballPrefab;
    [SerializeField] GameObject _throwPt;
    [SerializeField] float throwForce;
    [SerializeField] float throwUpwordForce;


    List<GameObject> activeBalls = new List<GameObject>(); 
    Stack<GameObject> ballPool = new Stack<GameObject>();

    public static event Action<float> onBallThrowPowerChange;
    public static event Action<int> onBallsLeftCountChange;

    int _ballsRemaining;
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
        onBallThrowPowerChange?.Invoke(0f);
        onBallsLeftCountChange?.Invoke(_ballsRemaining);
    }

    void Update() {
        if(Input.GetKeyDown(throwKey) && readyToThrow && _ballsRemaining > 0) {
            keyPressed = true;
            timeDown = Time.time;
        }

        if(Input.GetKeyUp(throwKey) && readyToThrow && _ballsRemaining > 0) {
            keyPressed = false;
            timePressed = Time.time - timeDown;
            Throw(timePressed);
        }

        if (keyPressed) {
            onBallThrowPowerChange?.Invoke(Time.time - timeDown);
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
        ball.GetComponent<BallController>().GravityEnabled = false;
        ballRB.velocity = Vector3.zero;
        ballRB.angularVelocity = Vector3.zero;
        ball.SetActive(false);
        ballPool.Push(ball);
    }

    GameObject LoadNewBall() {
        GameObject ball = null;
        if (ballPool.Count > 0) {
            ball = ballPool.Pop();
        } else {
            ball = Instantiate(_ballPrefab);
            ball.GetComponent<BallController>().BallID = _maxBallID;
            _maxBallID++;
        }


        _ballsRemaining--;
        onBallsLeftCountChange?.Invoke(_ballsRemaining);
        return ball;
    }
    private void Throw(float timePressed) {
        var ball = LoadNewBall();

        activeBalls.Add(ball);

        ball.SetActive(true);

        Rigidbody ballRB = ball.GetComponent<Rigidbody>();
        ballRB.position = _throwPt.transform.position;

        Vector3 forceToAdd = (_throwPt.transform.forward * throwForce  + transform.up * throwUpwordForce) * timePressed;

        ballRB.AddForce(forceToAdd, ForceMode.Impulse);

    }
}
