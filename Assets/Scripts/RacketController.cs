using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RacketController : MonoBehaviour
{
    [SerializeField] Transform _socketLoc;
    void OnEnable() {
        GameController.onPauseGame += SetRacketInvisible;
        GameController.onResumeGame += SetRacketVisible;
        GameController.onGameOver += SetRacketInvisible;
        GameController.onLoadGame += SetRacketInvisible;
    }

    void OnDisable() {
        GameController.onPauseGame -= SetRacketInvisible;
        GameController.onResumeGame -= SetRacketVisible;
        GameController.onGameOver -= SetRacketInvisible;
        GameController.onLoadGame -= SetRacketInvisible;
        
    }
    private void Start() {
        transform.position = _socketLoc.position;
    }
    public void ReturnRacketToSocket(SelectExitEventArgs args){
        transform.position = _socketLoc.position;
        transform.rotation = Quaternion.identity;
    }
    void SetRacketInvisible(){
        foreach(Transform child in gameObject.transform){
            child.gameObject.GetComponent<Renderer>().enabled = false;
        }
    }
    void SetRacketVisible(){
        foreach(Transform child in gameObject.transform){
            child.gameObject.GetComponent<Renderer>().enabled = true;
        }
    }
    void EnableRacket(){
        gameObject.SetActive(true);
    }
}
