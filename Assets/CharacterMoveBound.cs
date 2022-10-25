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
    [SerializeField] float _topBound;
    [SerializeField] float _bottomBound;
    [SerializeField] float _rightBound;
    [SerializeField] float _leftBound;

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
        CheckBounds();
    }

    void CheckBounds()
    {
        if(transform.position.x < _leftBound) transform.position = new Vector3(_leftBound, transform.position.y, transform.position.z);
        if(transform.position.x > _rightBound) transform.position = new Vector3(_rightBound, transform.position.y, transform.position.z);
        if(transform.position.y < _bottomBound) transform.position = new Vector3(transform.position.x, _bottomBound, transform.position.z);
        if(transform.position.y > _topBound) transform.position = new Vector3(transform.position.x, _topBound, transform.position.z);
    }
}
