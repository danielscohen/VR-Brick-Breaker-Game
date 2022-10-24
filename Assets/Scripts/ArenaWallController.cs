using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaWallController : MonoBehaviour
{
    Renderer _wallRen;
    [SerializeField] float _startIntensity;
    [SerializeField] float _maxIntensity;
    [SerializeField] float _flashDuration;
    void Awake() {
        _wallRen = GetComponent<Renderer>();
        
    }
    void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Ball")) {
            StartCoroutine(MakeFragFlash());
        }
    }
    IEnumerator MakeFragFlash() {
        Material wallMat = _wallRen.material;

        float time = 0f;
        Color startColor = wallMat.GetColor("_EmissionColor") / _startIntensity;
        Color opaqueColor = startColor;
        opaqueColor.a = 1;
       
        while(time < _flashDuration) {
            float intensity = Mathf.Lerp(_startIntensity, _maxIntensity, time / _flashDuration);
            Color color = Color.Lerp(startColor, opaqueColor, time / _flashDuration);
            wallMat.SetColor("_EmissionColor", color * intensity);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;
        while(time < _flashDuration * 1.1f) {
            float intensity = Mathf.Lerp(_maxIntensity, _startIntensity, time / (_flashDuration * 1.1f));
            Color color = Color.Lerp(opaqueColor, startColor, time / (_flashDuration * 1.1f));
            wallMat.SetColor("_EmissionColor", color * intensity);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
