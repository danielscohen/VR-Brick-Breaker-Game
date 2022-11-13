using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _powerUpPrefabs;
    [SerializeField] float _powerUpTime = 20f;
    public static event Action<PowerUpType> onStartPowerUp;
    public static event Action<PowerUpType> onStopPowerUp;
    public static event Action<PowerUpType, float> onUpdatePowerUpTime;
    Array _types;
    Dictionary<PowerUpType, float> _timeRemaining;
    Dictionary<PowerUpType, bool> _isPowerUpActive;

    void OnEnable() {
        BrickFrag.onSpawnPowerUp += CreateNewPowerUp;
        PowerUpController.onPLayerCaughtPowerUp += ActivatePowerUp;
    }

    void OnDisable() {
        BrickFrag.onSpawnPowerUp -= CreateNewPowerUp;
        PowerUpController.onPLayerCaughtPowerUp -= ActivatePowerUp;
    }

    private void Start() {
        _types = Enum.GetValues(typeof(PowerUpType));
        _timeRemaining = new Dictionary<PowerUpType, float>{
            {PowerUpType.MoveWalls, _powerUpTime},
            {PowerUpType.RotateWalls, _powerUpTime},
            {PowerUpType.DoublePoints, _powerUpTime},
            {PowerUpType.NegativePts, _powerUpTime}
        };
        _isPowerUpActive = new Dictionary<PowerUpType, bool>{
            {PowerUpType.MoveWalls, false},
            {PowerUpType.RotateWalls, false},
            {PowerUpType.DoublePoints, false},
            {PowerUpType.NegativePts, false}
        };
    }

    void CreateNewPowerUp(Vector3 pos){
        int random = (new System.Random()).Next(_types.Length);
        GameObject powerUp = Instantiate(_powerUpPrefabs[random], pos, Quaternion.identity);
        powerUp.GetComponent<PowerUpController>().Type = (PowerUpType)_types.GetValue(random);
    }

    void ActivatePowerUp(PowerUpType type){
        if(type == PowerUpType.ExtraBall) return;
        if(_isPowerUpActive[type]){
            _timeRemaining[type] = _powerUpTime;
        } else {
            StartCoroutine(StartTimer(type));
        }
    }
    IEnumerator StartTimer(PowerUpType type)
    {
        _isPowerUpActive[type] = true;
        _timeRemaining[type] = _powerUpTime;
        onStartPowerUp?.Invoke(type);

        while(_timeRemaining[type] > 0){
            _timeRemaining[type] -= Time.deltaTime;
            onUpdatePowerUpTime?.Invoke(type, _timeRemaining[type] / _powerUpTime);
            yield return null;
        }
        onUpdatePowerUpTime?.Invoke(type, _timeRemaining[type]);
        onStopPowerUp?.Invoke(type);
        _isPowerUpActive[type] = false;
    }
}
