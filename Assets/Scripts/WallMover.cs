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
        _anim.enabled = true;
        _anim.speed = 0;
    }
    void StartMovingWall(PowerUpType type){
        if(type == PowerUpType.MoveWalls){
            _anim.speed = 1.5f;
        }
    }
    void StopMovingWall(PowerUpType type){
        if(type == PowerUpType.MoveWalls){
            _anim.speed = 0;
        }
    }

}
