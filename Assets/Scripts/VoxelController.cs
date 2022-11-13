using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelController : MonoBehaviour
{
    bool _visibleInScene = false;
    Renderer voxRen;

    void OnEnable() {
        // GameController.onPauseGame += SetVoxInvisible;
        // GameController.onResumeGame += SetVoxVisible;
        // GameController.onGameOver += SetVoxInvisible;
    }

    void OnDisable() {
        // GameController.onPauseGame -= SetVoxInvisible;
        // GameController.onResumeGame -= SetVoxVisible;
        // GameController.onGameOver -= SetVoxInvisible;
        _visibleInScene = false;
    }
    private void Awake() {
        voxRen = gameObject.GetComponent<Renderer>();
    }
    void SetVoxVisible(){
        if(!_visibleInScene) return;
        voxRen.enabled = true;
    }
    void SetVoxInvisible(){
        if(voxRen.enabled){
            voxRen.enabled = false;
            _visibleInScene = true;
        }
    }

}
