using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSocketController : MonoBehaviour
{
    private void OnEnable() {
        BallManager.onBallsLeftCountChange += DisableSocket;
        
    }

    private void OnDisable() {
        BallManager.onBallsLeftCountChange -= DisableSocket;
    }

    void DisableSocket(int count){
        if(count == 0){
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
        }
    }
}
