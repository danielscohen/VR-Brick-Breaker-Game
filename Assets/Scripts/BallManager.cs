using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BallManager : MonoBehaviour
{
    [SerializeField] GameDifficultySettings _beginnerSettings;
    [SerializeField] GameDifficultySettings _normalSettings;
    [SerializeField] GameDifficultySettings _expertSettings;
    [SerializeField] GameObject _ballPrefab;
    [SerializeField] GameObject _throwPt;
    [SerializeField] Transform _loadLoc;
    [SerializeField] GameObject _ballSocket;
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
        DestroyZoneController.onBallLost += ManageBallReturn;
        BallController.onBallLost += ManageBallReturn;
        GameController.onStartGame += SetBallStartingCount;
        GameController.onStartGame += LoadNewBall;
        PowerUpController.onPLayerCaughtPowerUp += AddBallPowerUp;
        GameController.onPauseGame += SetActiveBallsInvisible;
        GameController.onResumeGame += SetActiveBallsVisible;
    }

    void OnDisable() {
        DestroyZoneController.onBallLost -= ManageBallReturn;
        BallController.onBallLost -= ManageBallReturn;
        GameController.onStartGame -= SetBallStartingCount;
        GameController.onStartGame -= LoadNewBall;
        PowerUpController.onPLayerCaughtPowerUp -= AddBallPowerUp;
        GameController.onPauseGame -= SetActiveBallsInvisible;
        GameController.onResumeGame -= SetActiveBallsVisible;
    }

    // private void Update() {
    //     Debug.Log($"ball remaining: {_ballsRemaining}");
    // }

    void Start() {
        onBallThrowPowerChange?.Invoke(0f);
        onBallsLeftCountChange?.Invoke(_ballsRemaining);
    }

    // void Update() {
    //     if (readyToThrow && GameController.Instance.CurrentGameState == GameState.Running && _ballsRemaining > 0) {
    //         if (Input.GetKeyDown(throwKey)) {
    //             keyPressed = true;
    //             timeDown = Time.time;
    //         }

    //         if (Input.GetKeyUp(throwKey) && keyPressed) {
    //             keyPressed = false;
    //             timePressed = Time.time - timeDown;
    //             Throw(timePressed);
    //         }

    //         if (keyPressed) {
    //             onBallThrowPowerChange?.Invoke(Time.time - timeDown);
    //         }
    //     }
    // }

    void SetBallStartingCount() {
        switch (GameController.Instance.GameDifficulty) {
            case Difficulty.Beginner:
                _ballsRemaining = _beginnerSettings.ballLimit;
                break;
            case Difficulty.Normal:
                _ballsRemaining = _normalSettings.ballLimit;
                break;
            case Difficulty.Expert:
                _ballsRemaining = _expertSettings.ballLimit;
                break;
        }

        onBallsLeftCountChange.Invoke(_ballsRemaining);
        
    }
    void ManageBallReturn(int ballID) {
        var index = activeBalls.FindIndex(x => x.GetComponent<BallController>().BallID == ballID);
        if(index < 0) {
            Debug.LogError($"Ball with ballID {ballID} was caught but never thrown.");
            return;
        }
        var ball = activeBalls[index];
        activeBalls.RemoveAt(index);
        Destroy(ball);
        if (activeBalls.Count == 0 && _ballsRemaining == 0) {
            GameController.Instance.EndGame(GameOverReason.BallsRanOut);
            return;
        }

    }

    void AddBallPowerUp(PowerUpType type){
        if(type == PowerUpType.ExtraBall){
            _ballsRemaining++;
            if(_ballsRemaining == 1){
                _ballSocket.GetComponent<XRSocketInteractor>().allowHover = true;
                _ballSocket.GetComponent<XRSocketInteractor>().allowSelect = true;
                LoadNewBall();
            }
            onBallsLeftCountChange?.Invoke(_ballsRemaining);
        }
    }

    public void SetActiveBallsInvisible(){
        foreach(GameObject ball in activeBalls){
            ball.GetComponent<BallController>().SetBallInvisible();
        }
    }
    public void SetActiveBallsVisible(){
        foreach(GameObject ball in activeBalls){
            ball.GetComponent<BallController>().SetBallVisible();
        }
    }
    public void DeactivateActiveBalls(){
        foreach(GameObject ball in activeBalls){
            ball.SetActive(false);
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

    void LoadNewBall() {
        AudioManager.Instance.PlayAudio(AudioTypes.LoadNewBall, _loadLoc.transform.position);
        GameObject ball = null;
        ball = Instantiate(_ballPrefab);
        ball.GetComponent<BallController>().BallID = _maxBallID;
        _maxBallID++;

        ball.transform.position = _loadLoc.position;
        ball.SetActive(true);


        activeBalls.Add(ball);
    }

    public void OnBallRemovedFromLoader(SelectExitEventArgs args){
        _ballsRemaining--;
        onBallsLeftCountChange?.Invoke(_ballsRemaining);
        if(_ballsRemaining > 0){
            LoadNewBall();
        } else {
            _ballSocket.GetComponent<XRSocketInteractor>().allowHover = false;
            _ballSocket.GetComponent<XRSocketInteractor>().allowSelect = false;
        }
    }
    // private void Throw(float timePressed) {
    //     var ball = LoadNewBall();

    //     activeBalls.Add(ball);

    //     ball.SetActive(true);

    //     Rigidbody ballRB = ball.GetComponent<Rigidbody>();
    //     ballRB.position = _throwPt.transform.position;

    //     Vector3 forceToAdd = (_throwPt.transform.forward * throwForce  + transform.up * throwUpwordForce) * timePressed;

    //     ballRB.AddForce(forceToAdd, ForceMode.Impulse);

    //     Debug.Log($"Throw Force: {forceToAdd.magnitude}");

    // }
}
