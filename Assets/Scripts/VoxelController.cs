using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelController : MonoBehaviour
{
    public static event Action<int> onFragCollideWithPlayer;
    private void OnTriggerEnter(Collider other) {
        //Debug.Log("Triggered by: " + other.tag);
        if (other.CompareTag("Player Collider")) {
            if (gameObject.transform.parent != null) {
                FragController frag = gameObject.GetComponentInParent<FragController>();
                if (frag.FragSize > 0) {
                    onFragCollideWithPlayer?.Invoke(frag.FragSize);
                    frag.FragSize = 0;
                }

                gameObject.transform.parent = null;
                frag.toDisable = true;
            }
            gameObject.SetActive(false);
        }
    }
    //void OnDestroy() {
    //    if (transform.parent != null) {
    //        if (transform.childCount <= 1) {
    //            Destroy(transform.parent.gameObject, 0.1f);
    //        }
    //    }

    //}

    private void OnDisable() {
        //if (transform.parent != null) {
        //    if (transform.parent.gameObject.activeSelf == true) {
        //        transform.parent.gameObject.SetActive(false);
        //    }
        //}
        //transform.parent = null;
    }
}
