using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] GameObject _powerUpPrefab;

    void OnEnable() {
        BrickFrag.onSpawnPowerUp += CreateNewPowerUp;
    }

    void OnDisable() {
        BrickFrag.onSpawnPowerUp -= CreateNewPowerUp;
    }

    void CreateNewPowerUp(Vector3 pos){
        GameObject powerUp = Instantiate(_powerUpPrefab, pos, Quaternion.identity);
    }
}
