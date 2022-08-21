using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragController : MonoBehaviour
{
    Vector3 playerPos;
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

    BrickFrag brickScript;
    public bool toDisable;
    bool timerOn;


    public void Init(int fragSize, Vector3 brickSize, Vector3 collisionPt, Vector3 playerPos, float collForce, BrickFrag brickScript) {
        timerOn = true;
        startTime = Time.time;
        this.playerPos = playerPos;
        this.collForce = collForce;
        this.brickScript = brickScript;
        this.FragSize = fragSize;

        fragRb = GetComponent<Rigidbody>();
        nDist = UtilFunctions.CalcDistScore(Vector3.Distance(transform.position, collisionPt), brickSize) / 100f;
    }


    private void Awake() {
        toDisable = false;
        timerOn = false;
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

    void FixedUpdate() {
        if (gravityApplied) {
        gravityDir = (playerPos - transform.position);
            fragRb.AddForce(gravityDir * Gravity);
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
    
    public void ApplyFracForce(List<Vector3> fracPts) {
        Vector3 minPt = UtilFunctions.FindClosestFracPt(transform.position, fracPts);
        float distFromFracPt = Vector3.Distance(minPt, transform.position);
        //Vector3 expForce = ((transform.position - minPt)).normalized * (1 / nDist) * expMag * collForce;
        Vector3 expForce = ((transform.position - minPt)).normalized * expMag * collForce;
        fragRb.AddForce(expForce, ForceMode.Impulse);
        MakeFall();
    }

    public void MakeFall() {
        EnableGravity();
        if (FragSize <= maxFragFadeSize && gameObject.activeSelf) {
            StartCoroutine(FadeOut(fadeDuration, 0, true));
        }
    }


    public IEnumerator FadeOut(float duration, float tranPer, bool destroy) {
        List<Material> childMats = GetFragMats();

        float time = 0f;
        Color startColor = childMats[0].color;
        Color endColor = startColor;
        endColor.a = tranPer;
       
        while(time < duration) {
            foreach (Material mat in childMats) {
                mat.color = Color.Lerp(startColor, endColor, time / duration);
            }
            time += Time.deltaTime;
            yield return null;
        }
        if (destroy) {
            DeleteFrag();
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
