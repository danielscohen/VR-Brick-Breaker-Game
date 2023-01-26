using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragController : MonoBehaviour
{
    Vector3 _camPos;
    float collForce;
    Vector3 gravityDir;
    public int FragSize { get; set; }
    float startTime;


    public float nDist;

    Rigidbody fragRb;
    bool gravityApplied = false;
    [SerializeField] float expMag = 1.0f;
    [SerializeField] float Gravity = 1.0f;
    [SerializeField] int maxFragFadeSize = 3;
    [SerializeField] float fadeDuration = 4f;
    [SerializeField] float fragLifeTime = 16f;
    [SerializeField] float _startIntensity = 1f;
    [SerializeField] float _maxIntensity = 10f;
    [SerializeField] float _flashDuration = 0.1f;
    [SerializeField] GameObject _expPrefab;
    [SerializeField] GameObject _ptsPrefab;
    [SerializeField] AudioClip _explosionAudio;
    public static event Action<int, Vector3> onFragCollideWithPlayer;

    BrickFrag brickScript;
    public bool toDisable;
    bool timerOn;
    ParticleSystem _explosionPS;
    Vector3 _brickPos;


    public void Init(int fragSize, Vector3 brickPos, Vector3 brickSize, Vector3 collisionPt, float collForce, BrickFrag brickScript) {
        timerOn = true;
        startTime = Time.time;
        this.collForce = collForce;
        this.brickScript = brickScript;
        this.FragSize = fragSize;
        this._brickPos = brickPos;

        fragRb = GetComponent<Rigidbody>();
        nDist = UtilFunctions.CalcDistScore(Vector3.Distance(transform.position, collisionPt), brickSize) / 100f;
    }


    private void Awake() {
        toDisable = false;
        timerOn = false;
    }

    void Start() {
        _camPos = Camera.main.transform.position;
    }


    private void Update() {
        if (transform.childCount == 0 && toDisable) {
            toDisable = false;
            gameObject.SetActive(false);
        }
        if((Time.time - startTime) > fragLifeTime && timerOn) {
            timerOn = false;
            //StartCoroutine(FadeOut(0.3f, 0f, true));
        }
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("Triggered by: " + other.tag);
        if (other.CompareTag("Player Collider")) {
            onFragCollideWithPlayer?.Invoke(FragSize, transform.position);
            DeleteFrag();
        }
    }



    public void EnableGravity() {
        gravityApplied = true;
    }




    public void DeleteFrag() {
        int numChild = transform.childCount;
        for(int i = numChild - 1; i >= 0; i--) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.DetachChildren();
        gameObject.SetActive(false);
    }
    
    public void MakeFragExplode(List<Vector3> fracPts) {
        Vector3 minPt = UtilFunctions.FindClosestFracPt(transform.position, fracPts);
        // float distFromFracPt = Vector3.Distance(minPt, transform.position);
        //Vector3 expForce = ((transform.position - minPt)).normalized * (1 / nDist) * expMag * collForce;
        Vector3 expForce = ((transform.position - minPt)).normalized * expMag * collForce * 0.5f;
        StartCoroutine(MakeFragFlash());
        fragRb.AddForce(expForce, ForceMode.Impulse);
        fragRb.AddExplosionForce(expMag * collForce, _brickPos, 5f);
        // ApplyZAxisForces(expMag * collForce);
        MakeFall();
    }

    void ApplyZAxisForces( float force){
        fragRb.AddForce(Vector3.back * force, ForceMode.Impulse);
    }

    public void MakeFall() {
        // EnableGravity();
        fragRb.useGravity = true;
        if (FragSize <= maxFragFadeSize && gameObject.activeSelf) {
            StartCoroutine(FadeOut(fadeDuration, 0, true));
        } else if (FragSize > maxFragFadeSize && gameObject.activeSelf) {
            StartCoroutine(FadeOutIfStuck());
        }
    }

    IEnumerator FadeOutIfStuck() {
        Vector3 origPos = transform.localPosition;
        yield return new WaitForSeconds(2f);
        if(Vector3.Distance(origPos, transform.localPosition) < 0.2f) {
            StartCoroutine(FadeOut(fadeDuration, 0, true));
        }
    }


    public IEnumerator FadeOut(float duration, float tranPer, bool destroy) {
        List<Material> childMats = GetFragMats();

        float time = 0f;
        Color startColor = childMats[0].GetColor("_EmissionColor") / _startIntensity;
        Color endColor = startColor;
        endColor.a = tranPer;
       
        while(time < duration) {
            foreach (Material mat in childMats) {
                Color color = Color.Lerp(startColor, endColor, time / duration);
                mat.SetColor("_EmissionColor", color * _startIntensity);
            }
            time += Time.deltaTime;
            yield return null;
        }
        if (destroy) {
            DeleteFrag();
        }

    }
    IEnumerator MakeFragFlash() {
        List<Material> childMats = GetFragMats();

        float time = 0f;
        Color color = childMats[0].GetColor("_EmissionColor") / _startIntensity;
       
        while(time < _flashDuration) {
            foreach (Material mat in childMats) {
                float intensity = Mathf.Lerp(_startIntensity, _maxIntensity, time / _flashDuration);
                mat.SetColor("_EmissionColor", color * intensity);
            }
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;
        while(time < _flashDuration * 1.1f) {
            foreach (Material mat in childMats) {
                // mat.color = Color.Lerp(mat.GetColor("_EmissionColor"), color, time / (_flashDuration * 1.2f));
                float intensity = Mathf.Lerp(_maxIntensity, _startIntensity, time / (_flashDuration * 1.1f));
                mat.SetColor("_EmissionColor", color * intensity);
            }
            time += Time.deltaTime;
            yield return null;
        }
    }

    List<Material> GetFragMats() {
        var Mats = new List<Material>();
        foreach (Transform child in transform) {
            Mats.Add(child.gameObject.GetComponent<Renderer>().material);
        }
        return Mats;
    }


    public void DisableGravity() {
        gravityApplied = false;
    }

}
