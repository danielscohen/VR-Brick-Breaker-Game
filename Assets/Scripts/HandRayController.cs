using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandRayController : MonoBehaviour
{
    [SerializeField] GameObject _leftHandRay;
    [SerializeField] GameObject _rightHandRay;
    [SerializeField] GameObject _leftHand;
    [SerializeField] GameObject _rightHand;
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
        _leftHand.SetActive(false);
        _rightHand.SetActive(false);
    }
    void DisableRays(){
        _leftHandRay.SetActive(false);
        _rightHandRay.SetActive(false);
        _leftHand.SetActive(true);
        _rightHand.SetActive(true);
    }

    public void DisableLeftHand(SelectEnterEventArgs args){
        _leftHand.SetActive(false);
    }
    public void EnableLeftHand(SelectExitEventArgs args){
        _leftHand.SetActive(true);
    }
}
