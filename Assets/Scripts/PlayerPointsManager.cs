using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerPointsManager : MonoBehaviour
{

    public static event Action<int> onUpdatePlayerPoints;
    int _points = 0;
    bool _negPoints = false;
    [SerializeField] float _negPointsTime = 20f;
    void OnEnable() {
        FragController.onFragCollideWithPlayer += ChangePlayerPts;
        PowerUpController.onPLayerCaughtPowerUp += StartNegatingPoints;
    }
    private void OnDisable() {
        FragController.onFragCollideWithPlayer -= ChangePlayerPts;
        PowerUpController.onPLayerCaughtPowerUp -= StartNegatingPoints;
    }

    private void Start() {
        onUpdatePlayerPoints?.Invoke(_points);
    }
    void ChangePlayerPts(int ptDelta) {
        ptDelta = _negPoints ? -ptDelta : ptDelta;
        _points += ptDelta;
        if(_points < 0) _points = 0;
        onUpdatePlayerPoints?.Invoke(_points);
    }
    void StartNegatingPoints(PowerUpType type){
        if(type == PowerUpType.NegativePts){
            StartCoroutine(NegatePoints());
        }
    }
    IEnumerator NegatePoints()
    {
        float timePassed = 0;

        _negPoints = true;
        while(timePassed < _negPointsTime){
            timePassed += Time.deltaTime;

            yield return null;
        }
        _negPoints = false;
    }
}
