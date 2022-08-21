using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    public static event Action<int> fragCollideWithPlayer;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Triggered by: " + other.tag);
        if (other.CompareTag("Frag") || other.CompareTag("Ball") || other.CompareTag("Particle")) {
            if (other.gameObject.transform.parent != null && other.CompareTag("Frag")) {
                FragController frag = other.gameObject.GetComponentInParent<FragController>();
                if (frag.FragSize > 0) {
                    fragCollideWithPlayer?.Invoke(frag.FragSize);
                    frag.FragSize = 0;
                }

                other.gameObject.transform.parent = null;
                frag.toDisable = true;
            }
            other.gameObject.SetActive(false);
        }
    }
}
