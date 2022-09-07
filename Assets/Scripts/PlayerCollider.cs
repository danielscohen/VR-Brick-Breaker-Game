using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    public static event Action<int> onFragCollideWithPlayer;
    public static event Action<int, BallReturnReason> onPlayerCaughtBall;
    public static event Action<PowerUpType> onPLayerCaughtPowerUp;

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("Triggered by: " + other.tag);
        if (other.CompareTag("Frag") || other.CompareTag("Particle")) {
            if (other.gameObject.transform.parent != null && other.CompareTag("Frag")) {
                FragController frag = other.gameObject.GetComponentInParent<FragController>();
                if (frag.FragSize > 0) {
                    onFragCollideWithPlayer?.Invoke(frag.FragSize);
                    frag.FragSize = 0;
                }

                other.gameObject.transform.parent = null;
                frag.toDisable = true;
            }
            other.gameObject.SetActive(false);
        } else if (other.CompareTag("Ball")) {
            onPlayerCaughtBall?.Invoke(other.gameObject.GetComponent<BallController>().BallID, BallReturnReason.BallCaught);
        } else if (other.CompareTag("PowerUp")) {
            onPLayerCaughtPowerUp?.Invoke(other.gameObject.GetComponent<PowerUpController>().type);
            Destroy(other.gameObject);
        }
    }
}
