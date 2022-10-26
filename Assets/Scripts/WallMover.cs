using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMover : MonoBehaviour
{
    [SerializeField] float _moveDuration = 15f;

    Animator _anim;
    void OnEnable() {
        PowerUpController.onPLayerCaughtPowerUp += MoveWall;
    }

    void OnDisable() {
        PowerUpController.onPLayerCaughtPowerUp -= MoveWall;
    }
    void Awake() {
        _anim = GetComponent<Animator>();
        _anim.enabled = false;
    }
    void MoveWall(PowerUpType type){
        if(type == PowerUpType.ArenaSpin && !_anim.isActiveAndEnabled){
            StartCoroutine(MoveWallCo());
        }
    }

    IEnumerator MoveWallCo()
    {
        float timePassed = 0;

        _anim.enabled = true;
        while(timePassed < _moveDuration){
            timePassed += Time.deltaTime;

            yield return null;
        }
        _anim.enabled = false;
    }
    // Start is called before the first frame update
}
