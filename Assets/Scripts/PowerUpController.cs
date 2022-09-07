using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public PowerUpType type;  
    Rigidbody _rb;
    Vector3 _camPos;
    [SerializeField] float Gravity = 2.0f;

    void Awake() {
        _rb = GetComponent<Rigidbody>();
    }
    void Start() {
        _camPos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = (_camPos - transform.position).normalized;
        _rb.AddForce(dir * Gravity);
    }
}
