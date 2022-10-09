using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandRayController : MonoBehaviour
{
    [SerializeField] GameObject _leftHandRay;
    [SerializeField] GameObject _rightHandRay;
    [SerializeField] GameObject _leftHand;
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

    public void DisableLeftHand(SelectEnterEventArgs args){
        _leftHand.SetActive(false);
    }
    public void EnableLeftHand(SelectExitEventArgs args){
        _leftHand.SetActive(true);
    }
}
