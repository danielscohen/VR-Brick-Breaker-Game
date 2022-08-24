using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] bool lockCursor = true;
    [SerializeField] float _moveSpeed = 1f;

    float cameraPitch = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.CurrentGameState == GameState.Running) {
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
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
