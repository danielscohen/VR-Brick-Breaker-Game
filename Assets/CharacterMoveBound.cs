using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveBound : MonoBehaviour
{
    // [SerializeField] Transform _topWall;
    // [SerializeField] Transform _bottomWall;
    // [SerializeField] Transform _rightWall;
    // [SerializeField] Transform _leftWall;
    // [SerializeField] Transform _arenaWalls;
    // [SerializeField] float _topMargin = 1f;
    // [SerializeField] float _bottomMargin = 1f;
    // [SerializeField] float _rightMargin = 1f;
    // [SerializeField] float _leftMargin = 1f;
    [SerializeField] float _topBodyBound;
    [SerializeField] float _bottomBodyBound;
    [SerializeField] float _rightBodyBound;
    [SerializeField] float _leftBodyBound;
    [SerializeField] float _topHandBound;
    [SerializeField] float _bottomHandBound;
    [SerializeField] float _rightHandBound;
    [SerializeField] float _leftHandBound;
    [SerializeField] GameObject _leftHand;
    [SerializeField] GameObject _rightHand;
    [SerializeField] GameObject _leftHandController;
    [SerializeField] GameObject _rightHandController;

    void Start() {
        // _topBound = _arenaWalls.TransformPoint(_topWall.position).y - _topMargin;
        // _bottomBound = _arenaWalls.TransformPoint(_bottomWall.position).y + _bottomMargin;
        // _rightBound = _arenaWalls.TransformPoint(_rightWall.position).x - _rightMargin;
        // _leftBound = _arenaWalls.TransformPoint(_leftWall.position).x + _leftMargin;

        // Debug.Log($"top: {_topBound}");
        // Debug.Log($"bottom: {_bottomBound}");
        // Debug.Log($"left: {_leftBound}");
        // Debug.Log($"right: {_rightBound}");
        
    }
    void Update() {
        CheckBodyBounds();
        // CheckLeftHandBounds();
        // CheckRightHandBounds();
        // Debug.Log($"Body: {transform.position}\nLeft Hand: {_leftHand.transform.position}\nRight Hand: {_rightHand.transform.position}");
    }

    void CheckBodyBounds()
    {
        if(transform.position.x < _leftBodyBound) transform.position = new Vector3(_leftBodyBound, transform.position.y, transform.position.z);
        if(transform.position.x > _rightBodyBound) transform.position = new Vector3(_rightBodyBound, transform.position.y, transform.position.z);
        if(transform.position.y < _bottomBodyBound) transform.position = new Vector3(transform.position.x, _bottomBodyBound, transform.position.z);
        if(transform.position.y > _topBodyBound) transform.position = new Vector3(transform.position.x, _topBodyBound, transform.position.z);
    }
    // void CheckLeftHandBounds()
    // {
    //     if(_leftHand.transform.position.x < _leftHandBound || _leftHand.transform.position.x > _rightHandBound
    //      || _leftHand.transform.position.y < _bottomHandBound || _leftHand.transform.position.y > _topHandBound) {
    //         _leftHandController.SetActive(false);
    //      } else _leftHandController.SetActive(true);
    // }
    // void CheckRightHandBounds()
    // {
    //     if(_rightHand.transform.position.x < _leftHandBound || _rightHand.transform.position.x > _rightHandBound
    //      || _rightHand.transform.position.y < _bottomHandBound || _rightHand.transform.position.y > _topHandBound) {
    //         _rightHandController.SetActive(false);
    //      } else _rightHandController.SetActive(true);
    // }
}
