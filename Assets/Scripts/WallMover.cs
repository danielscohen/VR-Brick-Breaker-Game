using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMover : MonoBehaviour
{

    Animator _anim;
    void OnEnable() {
        PowerUpManager.onStartPowerUp += StartMovingWall;
        PowerUpManager.onStopPowerUp += StopMovingWall;
    }

    void OnDisable() {
        PowerUpManager.onStartPowerUp -= StartMovingWall;
        PowerUpManager.onStopPowerUp -= StopMovingWall;
    }
    void Awake() {
        _anim = GetComponent<Animator>();
        _anim.enabled = false;
    }
    void StartMovingWall(PowerUpType type){
        if(type == PowerUpType.MoveWalls && !_anim.isActiveAndEnabled){
            _anim.enabled = true;
        }
    }
    void StopMovingWall(PowerUpType type){
        if(type == PowerUpType.MoveWalls && !_anim.isActiveAndEnabled){
            _anim.enabled = false;
        }
    }

}
