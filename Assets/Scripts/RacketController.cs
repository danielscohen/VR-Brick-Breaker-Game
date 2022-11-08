using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RacketController : MonoBehaviour
{
    [SerializeField] Transform _socketLoc;
    void OnEnable() {
        GameController.onPauseGame += DisableRacket;
        GameController.onResumeGame += EnableRacket;
        GameController.onGameOver += DisableRacket;
    }

    void OnDisable() {
        GameController.onPauseGame -= DisableRacket;
        GameController.onResumeGame -= EnableRacket;
        GameController.onGameOver -= DisableRacket;
    }
    private void Start() {
        transform.position = _socketLoc.position;
    }
    public void ReturnRacketToSocket(SelectExitEventArgs args){
        transform.position = _socketLoc.position;
        transform.rotation = Quaternion.identity;
    }
    void DisableRacket(){
        gameObject.SetActive(false);
    }
    void EnableRacket(){
        gameObject.SetActive(true);
    }
}
