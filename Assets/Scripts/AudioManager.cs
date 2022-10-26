using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField] List<AudioClip> _audioClipsList;
    Dictionary<AudioTypes, AudioClip> _audioClips;
    private void OnEnable() {
        PlayerCollider.onPLayerCaughtPowerUp += PlayPowerUpCatchAudio;
    }
    private void OnDisable() {
        PlayerCollider.onPLayerCaughtPowerUp -= PlayPowerUpCatchAudio;
    }
    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        _audioClips.Add(AudioTypes.PowerUpCatch, _audioClipsList[0]);
    }

    void PlayPowerUpCatchAudio(PowerUpType type){
        PlayClip(AudioTypes.PowerUpCatch);
    }

    void PlayClip(AudioTypes aType){
    }

}
