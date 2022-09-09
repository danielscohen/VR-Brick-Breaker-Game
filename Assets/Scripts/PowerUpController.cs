using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public PowerUpType Type {get; private set;}
    Vector3 _camPos;
    [SerializeField] float _moveSpeed = 1f;

    [SerializeField] List<Material> _powerUpMats;


    void Awake() {
        var types = Enum.GetValues(typeof(PowerUpType));
        System.Random random = new System.Random();
        Type = (PowerUpType)types.GetValue(random.Next(types.Length));
        GetComponent<Renderer>().material = _powerUpMats[(int)Type];
    }
    void Start() {
        _camPos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.back * Time.deltaTime * _moveSpeed;
    }
}
