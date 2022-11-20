using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMover : MonoBehaviour
{

    enum Wall {Front, Back};
    Animator _anim;
    // Transform _wall;
    [SerializeField] float _rotSpeed = 5f;
    [SerializeField] Wall _wall;
    bool _wallIsRotating = false;
    float _wallMoveDir = 1f;
    private void Start() {
        // _wall = transform.GetChild(0);
        
        if(_wall == Wall.Back){
            _rotSpeed *= -1;
            _wallMoveDir = -1f;
        }
        
    }
    private void Update() {
        if(_wallIsRotating){
            transform.Rotate(new Vector3(0, 0, _rotSpeed * Time.deltaTime));
        }
    }
    void OnEnable() {
        PowerUpManager.onStartPowerUp += StartMovingWall;
        PowerUpManager.onStopPowerUp += StopMovingWall;
        PowerUpManager.onStartPowerUp += StartRotatingWall;
        PowerUpManager.onStopPowerUp += StopRotatingWall;
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
    void StartRotatingWall(PowerUpType type){
        if(type == PowerUpType.RotateWalls){
            _rotSpeed *= -1;
            _wallIsRotating = true;
        }
    }
    void StopRotatingWall(PowerUpType type){
        if(type == PowerUpType.RotateWalls){
            _wallIsRotating = false;
        }
    }

}
