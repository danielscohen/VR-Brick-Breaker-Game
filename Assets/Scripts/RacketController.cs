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
        SetRacketInvisible();
    }
    public void ReturnRacketToSocket(SelectExitEventArgs args){
        transform.position = _socketLoc.position;
        transform.rotation = Quaternion.identity;
    }

    public void EnableCollisionsWithBalls(SelectExitEventArgs args){
        Physics.IgnoreLayerCollision(3, 8, false);
    }
    public void DisableCollisionsWithBalls(SelectEnterEventArgs args){
        Physics.IgnoreLayerCollision(3, 8, true);
    }

    void SetRacketInvisible(){
        foreach(Transform child in gameObject.transform){
            if(child.CompareTag("Racket")){
                child.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
        transform.position = new Vector3(-1000f, -1000f, -1000f);
    }
    void SetRacketVisible(){
        foreach(Transform child in gameObject.transform){
            if(child.CompareTag("Racket")){
                child.gameObject.GetComponent<Renderer>().enabled = true;
            }
        }
        transform.position = _socketLoc.position;

    }
    void EnableRacket(){
        gameObject.SetActive(true);
    }
    void DisableRacket(){
        gameObject.SetActive(false);
    }
}
