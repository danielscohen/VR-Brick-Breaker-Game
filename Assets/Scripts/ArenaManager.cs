using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{

    int _numBricksRemaining = 0;
    [SerializeField] float _rotateTime = 15f;

    Animator _anim;


    void Awake() {
        _anim = GetComponent<Animator>();
        _anim.enabled = false;
    }

    public int NumBricksRemaining {
        get { return _numBricksRemaining; }
        set {
            _numBricksRemaining = value;
            if(_numBricksRemaining == 0) {
                GameController.Instance.EndGame(GameOverReason.GameWon);
            }
        }
    }

    void RotateArena(PowerUpType type){
        if(type == PowerUpType.MoveWalls && !_anim.isActiveAndEnabled){
            StartCoroutine(RotateArenaCo());
        }
    }

    IEnumerator RotateArenaCo()
    {
        float timePassed = 0;

        _anim.enabled = true;
        while(timePassed < _rotateTime){
            timePassed += Time.deltaTime;

            yield return null;
        }
        _anim.enabled = false;
    }
}
