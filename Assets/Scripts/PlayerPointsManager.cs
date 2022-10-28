using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerPointsManager : MonoBehaviour
{

    public static event Action<int> onUpdatePlayerPoints;
    public static event Action<int> onUpdatePowerUpTime;
    int _points = 0;
    bool _negPoints = false;
    bool _doublePts = false;
    [SerializeField] float _powerUpTime = 20f;
    void OnEnable() {
        FragController.onFragCollideWithPlayer += ChangePlayerPts;
        PowerUpManager.onStartPowerUp += ApplyPointsModifier;
        PowerUpManager.onStopPowerUp += RemovePointsModifier;
    }
    private void OnDisable() {
        FragController.onFragCollideWithPlayer -= ChangePlayerPts;
        PowerUpManager.onStartPowerUp -= ApplyPointsModifier;
        PowerUpManager.onStopPowerUp -= RemovePointsModifier;
    }

    private void Start() {
        onUpdatePlayerPoints?.Invoke(_points);
    }
    void ChangePlayerPts(int ptDelta) {
        ptDelta = _negPoints ? -ptDelta : ptDelta;
        if(_doublePts){
            ptDelta *= 2;
        }
        _points += ptDelta;
        if(_points < 0) _points = 0;
        onUpdatePlayerPoints?.Invoke(_points);
    }
    void ApplyPointsModifier(PowerUpType type){
        if(type == PowerUpType.NegativePts){
            _negPoints = true;
        } 
        else if (type == PowerUpType.DoublePoints){
            _doublePts = true;
        }
    }
    void RemovePointsModifier(PowerUpType type){
        if(type == PowerUpType.NegativePts){
            _negPoints = false;
        } 
        else if (type == PowerUpType.DoublePoints){
            _doublePts = false;
        }
    }
}
