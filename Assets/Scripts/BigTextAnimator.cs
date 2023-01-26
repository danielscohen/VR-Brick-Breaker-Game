using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BigTextAnimator : MonoBehaviour
{
    [SerializeField] List<Transform> _textLines;
    [SerializeField] float _duration;
    [SerializeField] float _moveDist;


    private void Start() {
    }

    public IEnumerator AnimateEnter(){
        for(int i = 0; i < _textLines.Count; i++){
            Transform text = _textLines[i];
            AudioManager.Instance.PlayAudio(AudioTypes.BigTextMove, text.position);
            text.gameObject.SetActive(true);
            bool dirRight = i % 2 == 0;
            if(dirRight){
                yield return StartCoroutine(AnimateEaseOut(new Vector3(-_moveDist, text.position.y, text.position.z), new Vector3(0, text.position.y, text.position.z), text));
            }
            else{
                yield return StartCoroutine(AnimateEaseOut(new Vector3(_moveDist, text.position.y, text.position.z), new Vector3(0, text.position.y, text.position.z), text));
            }
        }
    }
    public IEnumerator AnimateExit(){
        for(int i = 0; i < _textLines.Count; i++){
            Transform text = _textLines[i];
            AudioManager.Instance.PlayAudio(AudioTypes.BigTextMove, text.position);
            bool dirRight = i % 2 == 0;
            if(dirRight){
                yield return StartCoroutine(AnimateEaseOut(new Vector3(0, text.position.y, text.position.z), new Vector3(-_moveDist, text.position.y, text.position.z), text));
            }
            else{
                yield return StartCoroutine(AnimateEaseIn(new Vector3(0, text.position.y, text.position.z), new Vector3(_moveDist, text.position.y, text.position.z), text));
            }
            text.gameObject.SetActive(false);
        }
    }

    float Square(float t){
        return t * t;
    }

    float Flip( float t){
        return 1 - t;
    }

    IEnumerator AnimateEaseOut(Vector3 start, Vector3 end, Transform text){
        float time = 0;
        while(time < _duration){
            text.position = Vector3.Lerp(start, end, Flip(Square(Flip(time / _duration))));
            time += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator AnimateEaseIn(Vector3 start, Vector3 end, Transform text){
        float time = 0;
        while(time < _duration){
            text.position = Vector3.Lerp(start, end, Square(time / _duration));
            time += Time.deltaTime;
            yield return null;
        }
    }
}
