using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField] List<AudioClip> _audioClipsList;
    Dictionary<AudioTypes, AudioClip> _audioClips;
    private void OnEnable() {
    }
    private void OnDisable() {
    }
    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        _audioClips = new Dictionary<AudioTypes, AudioClip>(){
            {AudioTypes.ButtonSelect, _audioClipsList[0]}
        };
    }

    public void PlayButtonPressAudio(){
        PlayClip(AudioTypes.ButtonSelect);
    }

    void PlayClip(AudioTypes aType){
        _audioSource.PlayOneShot(_audioClips[aType]);
    }

}
