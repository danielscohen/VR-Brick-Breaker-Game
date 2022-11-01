using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Vector3 PrevVelocity { get; private set; }
    public bool JustCollidedWithBrick { get; private set; }
    float _collTime;
    [SerializeField] float _nextCollWaitTime = 0.01f;
    public static event Action<int> onBallLost;

    Rigidbody ballRb;
    Vector3 _camPos;
    [SerializeField] float Gravity = 2.0f;
    [SerializeField] float _maxSpeedSqrd = 2.0f;
    public int BallID { get; set; }
    public bool GravityEnabled { get; set; }


    void Awake() {
        
        ballRb = GetComponent<Rigidbody>();
        ballRb.useGravity = true;
        GravityEnabled = true;
    }

    void Start() {
        _camPos = Camera.main.transform.position;
    }

    private void Update() {
        if(transform.position.z < -0.5){
            // Debug.Log("Delete Ball");
            onBallLost?.Invoke(BallID);
        }
        // if(ballRb.velocity.sqrMagnitude > 0){
        //     Debug.Log($"Velocity: {ballRb.velocity.sqrMagnitude}");
        // }
    }

    private void FixedUpdate() {
        if(ballRb.velocity.sqrMagnitude > _maxSpeedSqrd){
            ballRb.velocity *= 0.80f;
        }
        
    }

    void OnEnable() {
        StartCoroutine(VelocityCacher());
        JustCollidedWithBrick = false;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Frag")) {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
        if (collision.collider.CompareTag("Arena Wall")) {
            AudioManager.Instance.PlayAudio(AudioTypes.BallHitArenaWall, transform.position);
        }
    }


    public bool IsCollisionAllowed() {
        if (!JustCollidedWithBrick || (Time.time - _collTime) > _nextCollWaitTime) {
            JustCollidedWithBrick = true;
            _collTime = Time.time;
            return true;
        }

        return false;
    }

    IEnumerator VelocityCacher() {
        while (true) {
            PrevVelocity = ballRb.velocity;
            yield return null;
        }
    }

    // private void FixedUpdate() {
    //     if(ballRb.velocity.magnitude > maxSpeed){
    //         ballRb.velocity = ballRb.velocity.normalized * maxSpeed;
    //     }
    // }

    // void FixedUpdate() {
    //     if (!GravityEnabled) return;
    //     // Vector3 dir = (_camPos - transform.position).normalized;
    //     // ballRb.AddForce(Vector3.back * Gravity, ForceMode.Acceleration);
    //     ballRb.useGravity = true;
    // }

    public void EnableGravity(){
        GravityEnabled = true;
    }
    public void DisableGravity(){
        GravityEnabled = false;
    }
}
