using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] GameObject [] powerUpPrefabs;

    void OnEnable() {
        BrickFrag.onSpawnPowerUp += CreateNewPowerUp;
    }

    void OnDisable() {
        BrickFrag.onSpawnPowerUp -= CreateNewPowerUp;
    }

    void CreateNewPowerUp(Vector3 pos){
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        GameObject powerUp = Instantiate(prefab, pos, Quaternion.identity);
    }
}
