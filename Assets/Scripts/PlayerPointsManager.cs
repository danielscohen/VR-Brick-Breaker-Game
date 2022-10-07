using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerPointsManager : MonoBehaviour
{

    public static event Action<int> onUpdatePlayerPoints;
    int _points = 0;
    bool _negPoints = false;
    void OnEnable() {
        PlayerCollider.onFragCollideWithPlayer += ChangePlayerPts;
    }
    private void OnDisable() {
        PlayerCollider.onFragCollideWithPlayer -= ChangePlayerPts;
    }

    private void Start() {
        onUpdatePlayerPoints?.Invoke(_points);
    }
    void ChangePlayerPts(int ptDelta) {
        ptDelta = _negPoints ? -ptDelta : ptDelta;
        _points += ptDelta;
        if(_points < 0) _points = 0;
        onUpdatePlayerPoints?.Invoke(_points);
    }
}
