using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRayController : MonoBehaviour
{
    [SerializeField] GameObject _leftHandRay;
    [SerializeField] GameObject _rightHandRay;
    void OnEnable() {
        GameController.onLoadGame += EnableRays;
        GameController.onPauseGame += EnableRays;
        GameController.onGameOver += EnableRays;
        GameController.onStartGame += DisableRays;
        GameController.onResumeGame += DisableRays;
    }
    void OnDisable() {
        GameController.onLoadGame -= EnableRays;
        GameController.onPauseGame -= EnableRays;
        GameController.onGameOver -= EnableRays;
        GameController.onStartGame -= DisableRays;
        GameController.onResumeGame -= DisableRays;
    }
    void EnableRays(){
        _leftHandRay.SetActive(true);
        _rightHandRay.SetActive(true);
    }
    void DisableRays(){
        _leftHandRay.SetActive(false);
        _rightHandRay.SetActive(false);
    }
    void EnableRays(GameOverReason r){
        _leftHandRay.SetActive(true);
        _rightHandRay.SetActive(true);
    }
    void DisableRays(GameOverReason r){
        _leftHandRay.SetActive(false);
        _rightHandRay.SetActive(false);
    }
}
