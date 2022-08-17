using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelController : MonoBehaviour
{
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
