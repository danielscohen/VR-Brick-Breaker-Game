using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RacketController : MonoBehaviour
{
    [SerializeField] Transform _socketLoc;
    private void Start() {
        transform.position = _socketLoc.position;
    }
    void ReturnRacketToSocket(SelectExitEventArgs args){
        transform.position = _socketLoc.position;
        transform.rotation = Quaternion.identity;
    }
}
