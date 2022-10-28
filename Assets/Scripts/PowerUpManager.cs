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
        
    }

    void CreateNewPowerUp(Vector3 pos){
        int random = (new System.Random()).Next(_types.Length);
        GameObject powerUp = Instantiate(_powerUpPrefabs[random], pos, Quaternion.identity);
        powerUp.GetComponent<PowerUpController>().Type = (PowerUpType)_types.GetValue(random);
    }

    void ActivatePowerUp(PowerUpType type){
        if(type == PowerUpType.ExtraBall) return;
        StartCoroutine(StartTimer(type));
    }
    IEnumerator StartTimer(PowerUpType type)
    {
        float timeRemaining = _powerUpTime;
        onStartPowerUp?.Invoke(type);

        while(timeRemaining > 0){
            timeRemaining -= Time.deltaTime;
            onUpdatePowerUpTime?.Invoke(type, timeRemaining / _powerUpTime);
            yield return null;
        }
        onUpdatePowerUpTime?.Invoke(type, timeRemaining);
        onStopPowerUp?.Invoke(type);
    }
}
