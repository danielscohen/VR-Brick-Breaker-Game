using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] bool lockCursor = true;
    [SerializeField] float _moveSpeed = 1f;
    [SerializeField] GameDifficultySettings _beginnerSettings;
    [SerializeField] GameDifficultySettings _normalSettings;
    [SerializeField] GameDifficultySettings _expertSettings;

    public static event Action<int> onUpdatePlayerHealth;

    GameObject _racket;

    int _health;

    float cameraPitch = 0.0f;
    void OnEnable() {
        GameController.onStartGame += SetStartingHealth;
        PlayerCollider.onFragCollideWithPlayer += DecreasePlayerHealth;
    }

    void SetStartingHealth() {
        switch (GameController.Instance.GameDifficulty) {
            case Difficulty.Beginner:
                _health = _beginnerSettings.playerStartHealth;
                break;
            case Difficulty.Normal:
                _health = _normalSettings.playerStartHealth;
                break;
            case Difficulty.Expert:
                _health = _expertSettings.playerStartHealth;
                break;
        }

        onUpdatePlayerHealth?.Invoke(_health);
        
    }

    private void OnDisable() {
        GameController.onStartGame -= SetStartingHealth;
        PlayerCollider.onFragCollideWithPlayer -= DecreasePlayerHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        _racket = GameObject.Find("Racket");
        _racket.SetActive(false);

        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.CurrentGameState == GameState.Running) {
            if (Input.GetKeyDown(KeyCode.Mouse2)) {
                ResetCamera();
            }
            UpdateMouseLook();
        }

        if (Input.GetKey(KeyCode.W)) {
            transform.position += Vector3.up * Time.deltaTime * _moveSpeed;
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.position += Vector3.down * Time.deltaTime * _moveSpeed;
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.position += Vector3.right * Time.deltaTime * _moveSpeed;
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.position += Vector3.left * Time.deltaTime * _moveSpeed;
        }

        if (Input.GetKey(KeyCode.Mouse1)) {
            _racket.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1)) {
            _racket.SetActive(false);
        }
    }

    void DecreasePlayerHealth(int healthValDecreaseBy) {
        _health -= healthValDecreaseBy;
        onUpdatePlayerHealth?.Invoke(_health);
        if (_health <= 0) {
            GameController.Instance.EndGame(GameOverReason.HealthRanOut);
        }
    }

    void UpdateMouseLook() {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        cameraPitch = -mouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.Rotate(Vector3.right * cameraPitch);

        transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);

    }

    void ResetCamera() {

        playerCamera.localEulerAngles = Vector3.right * 0f;

        transform.eulerAngles = Vector3.up * 0f;
    }
}
