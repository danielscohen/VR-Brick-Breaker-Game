using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _powerUpPrefabs;
    Array _types;

    void OnEnable() {
        BrickFrag.onSpawnPowerUp += CreateNewPowerUp;
    }

    void OnDisable() {
        BrickFrag.onSpawnPowerUp -= CreateNewPowerUp;
    }

    private void Start() {
        _types = Enum.GetValues(typeof(PowerUpType));
        
    }

    void CreateNewPowerUp(Vector3 pos){
        int random = (new System.Random()).Next(_types.Length);
        GameObject powerUp = Instantiate(_powerUpPrefabs[random], pos, Quaternion.identity);
        powerUp.GetComponent<PowerUpController>().Type = (PowerUpType)_types.GetValue(random);
    }
}
