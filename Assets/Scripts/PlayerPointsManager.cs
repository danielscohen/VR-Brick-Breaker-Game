using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerPointsManager : MonoBehaviour
{

    public static event Action<int> onUpdatePlayerPoints;
    int _points = 0;
    bool _negPoints = false;
    bool _doublePts = false;
    [SerializeField] float _powerUpTime = 20f;
    void OnEnable() {
        FragController.onFragCollideWithPlayer += ChangePlayerPts;
        PowerUpController.onPLayerCaughtPowerUp += ModifyPoints;
    }
    private void OnDisable() {
        FragController.onFragCollideWithPlayer -= ChangePlayerPts;
        PowerUpController.onPLayerCaughtPowerUp -= ModifyPoints;
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
    void ModifyPoints(PowerUpType type){
        if(type == PowerUpType.NegativePts){
            StartCoroutine(NegatePoints());
        } 
        else if (type == PowerUpType.DoublePoints){
            StartCoroutine(DoublePoints());
        }
    }
    IEnumerator NegatePoints()
    {
        float timePassed = 0;

        _negPoints = true;
        while(timePassed < _powerUpTime){
            timePassed += Time.deltaTime;

            yield return null;
        }
        _negPoints = false;
    }
    IEnumerator DoublePoints()
    {
        float timePassed = 0;

        _doublePts = true;
        while(timePassed < _powerUpTime){
            timePassed += Time.deltaTime;

            yield return null;
        }
        _doublePts = false;
    }
}
