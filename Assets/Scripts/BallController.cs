using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Vector3 velocity;
    Rigidbody ballRb;
    Vector3 playerPos;
    [SerializeField] float Gravity = 1.0f;

    private void Awake() {
        ballRb = GetComponent<Rigidbody>();
    }

    private void Start() {
        playerPos = GameObject.Find("Main Camera").transform.position;
    }
    private void FixedUpdate() {
        velocity = ballRb.velocity;
        Vector3 dir = (playerPos - transform.position).normalized;
        ballRb.AddForce(dir * Gravity);
    }
}
