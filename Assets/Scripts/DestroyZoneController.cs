using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZoneController : MonoBehaviour
{
    public static event Action<int> onBallLost;
    private void OnTriggerEnter(Collider other) {
        //Debug.Log("Triggered by: " + other.tag);
        if(other.CompareTag("Frag") || other.CompareTag("Particle")) {
            if (other.gameObject.transform.parent != null && other.CompareTag("Frag")) {
                FragController frag = other.gameObject.transform.parent.GetComponent<FragController>();
                other.gameObject.transform.parent = null;
                frag.toDisable = true;
            }
            other.gameObject.SetActive(false);
        } else if (other.CompareTag("Ball")) {
            onBallLost?.Invoke(other.GetComponent<BallController>().BallID);
        }
        // } else if (other.CompareTag("PowerUp")) {
        //     Destroy(other.gameObject);
        // }
    }
}
