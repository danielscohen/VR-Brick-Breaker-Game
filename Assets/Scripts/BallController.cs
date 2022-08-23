using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Vector3 PrevVelocity { get; private set; }
    Rigidbody ballRb;
    [SerializeField] float Gravity = 2.0f;
    public int BallID { get; set; }
    public bool GravityEnabled { get; set; }

    void Awake() {
        ballRb = GetComponent<Rigidbody>();
        GravityEnabled = true;
    }

    void OnEnable() {
        StartCoroutine(VelocityCacher());
    }

    IEnumerator VelocityCacher() {
        while (true) {
            PrevVelocity = ballRb.velocity;
            yield return null;
        }
    }

    void FixedUpdate() {
        if (!GravityEnabled) return;
        Vector3 dir = new Vector3(0,0,-1);
        ballRb.AddForce(dir * Gravity);
    }
}
