using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class PlayerPointsManager : MonoBehaviour
{

    public static event Action<int> onUpdatePlayerPoints;
    public static event Action<int> onUpdatePowerUpTime;
    int _points = 0;
    bool _negPoints = false;
    bool _doublePts = false;
    [SerializeField] float _powerUpTime = 20f;
    [SerializeField] GameObject _ptsPrefabPos;
    [SerializeField] GameObject _ptsPrefabNeg;
    void OnEnable() {
        FragController.onFragCollideWithPlayer += ChangePlayerPts;
        PowerUpManager.onStartPowerUp += ApplyPointsModifier;
        PowerUpManager.onStopPowerUp += RemovePointsModifier;
    }
    private void OnDisable() {
        FragController.onFragCollideWithPlayer -= ChangePlayerPts;
        PowerUpManager.onStartPowerUp -= ApplyPointsModifier;
        PowerUpManager.onStopPowerUp -= RemovePointsModifier;
    }

    private void Start() {
        onUpdatePlayerPoints?.Invoke(_points);
    }
    void ChangePlayerPts(int ptDelta, Vector3 pos) {
        ptDelta = _negPoints ? -ptDelta : ptDelta;
        if(_doublePts){
            ptDelta *= 2;
        }
        _points += ptDelta;
        if(_points < 0) _points = 0;
        onUpdatePlayerPoints?.Invoke(_points);
        AnimatePoints(ptDelta, pos);
        AudioTypes type = _negPoints ? AudioTypes.NegPts : AudioTypes.PosPts;
        AudioManager.Instance.PlayAudio(type, pos);
    }
    void AnimatePoints(int pts, Vector3 pos){
        GameObject ptsModel;
        if(_negPoints){
            ptsModel = Instantiate(_ptsPrefabNeg, pos + Vector3.forward * 0.3f, Quaternion.identity);
        } else{
            ptsModel = Instantiate(_ptsPrefabPos, pos + Vector3.forward * 0.3f, Quaternion.identity);
        }
        TextMeshPro ptsText = ptsModel.transform.GetChild(0).GetComponent<TextMeshPro>();
        string sign = _negPoints ? "-" : "+";
        ptsText.text = $"{sign}{pts}";
    }
    void ApplyPointsModifier(PowerUpType type){
        if(type == PowerUpType.NegativePts){
            _negPoints = true;
        } 
        else if (type == PowerUpType.DoublePoints){
            _doublePts = true;
        }
    }
    void RemovePointsModifier(PowerUpType type){
        if(type == PowerUpType.NegativePts){
            _negPoints = false;
        } 
        else if (type == PowerUpType.DoublePoints){
            _doublePts = false;
        }
    }
    public int GetScore(){
        return _points;
    }
}
