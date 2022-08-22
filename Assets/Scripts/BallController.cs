using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Vector3 velocity;
    Rigidbody ballRb;
    [SerializeField] float Gravity = 1.0f;
    public int BallID { get; set; }
    public bool GravityEnabled { get; set; }

    void Awake() {
        ballRb = GetComponent<Rigidbody>();
        GravityEnabled = true;
    }

    void FixedUpdate() {
        if (!GravityEnabled) return;
        velocity = ballRb.velocity;
        Vector3 dir = new Vector3(0,0,-1);
        ballRb.AddForce(dir * Gravity);
    }
}
