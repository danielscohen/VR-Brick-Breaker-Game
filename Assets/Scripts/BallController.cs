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
        playerPos = Camera.main.transform.position;
    }
    private void FixedUpdate() {
        velocity = ballRb.velocity;
        Vector3 dir = new Vector3(0,0,-1);
        ballRb.AddForce(dir * Gravity);
    }
}
