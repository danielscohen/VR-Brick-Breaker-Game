using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwapRacketBall : MonoBehaviour
{
    [SerializeField] InputActionReference _swapReference;
    [SerializeField] GameObject _racket;
    [SerializeField] GameObject _ball;
    // Start is called before the first frame update
    private void OnEnable() {
        _swapReference.action.started += onSwapPressed;
    }
    private void OnDisable() {
        _swapReference.action.started -= onSwapPressed;
    }
    void onSwapPressed(InputAction.CallbackContext context){
        // Debug.Log($"swap pressed {count}");
        // count++;
        if(GameController.Instance.CurrentGameState == GameState.Running){
            Swap();
        }
    }

    void Swap(){
        _racket = GameObject.Find("Racket Socket");
        _ball = GameObject.Find("Ball Socket");
        Vector3 temp = _ball.transform.position;
        _ball.transform.position = _racket.transform.position;
        _racket.transform.position = temp;
    }
}
