using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Vector3 PrevVelocity { get; private set; }
    public bool JustCollidedWithBrick { get; private set; }
    float _collTime;
    [SerializeField] float _nextCollWaitTime = 0.01f;

    Rigidbody ballRb;
    Vector3 _camPos;
    [SerializeField] float Gravity = 2.0f;
    public int BallID { get; set; }
    public bool GravityEnabled { get; set; }

    void Awake() {
        
        ballRb = GetComponent<Rigidbody>();
        GravityEnabled = false;
    }

    void Start() {
        _camPos = Camera.main.transform.position;
    }

    void OnEnable() {
        StartCoroutine(VelocityCacher());
        JustCollidedWithBrick = false;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Frag")) {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
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

    void FixedUpdate() {
        if (!GravityEnabled) return;
        Vector3 dir = (_camPos - transform.position).normalized;
        ballRb.AddForce(dir * Gravity);
    }

    public void EnableGravity(){
        GravityEnabled = true;
    }
    public void DisableGravity(){
        GravityEnabled = false;
    }
}
