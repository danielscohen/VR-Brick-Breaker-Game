using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public static event Action<PowerUpType> onPLayerCaughtPowerUp;
    public PowerUpType Type {get; set;}
    Vector3 _camPos;
    [SerializeField] float _moveSpeed = 1f;

    [SerializeField] GameObject _expPrefab;
    [SerializeField] AudioClip _explosionAudio;

    AudioSource _audioSource;
    ParticleSystem _explosionPS;
    bool _firstTrigger = true;



    void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }
    void Start() {
        _camPos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.back * Time.deltaTime * _moveSpeed;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player Collider") && _firstTrigger){
            _firstTrigger = false;
            onPLayerCaughtPowerUp?.Invoke(Type);
            Instantiate(_expPrefab, transform.position, transform.rotation).GetComponent<ParticleSystem>();
            AudioSource.PlayClipAtPoint(_explosionAudio, transform.position);
        }
    }

}
