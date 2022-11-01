using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] AudioSource _musicSource;
    [SerializeField] AudioSource _sFXSource;
    [SerializeField] GameObject _posSFXSourcePrefab;
    [SerializeField] List<AudioClip> _audioClipsList;
    Dictionary<AudioTypes, AudioClip> _audioClips;
    private void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }
    }
    void Start()
    {
        _audioClips = new Dictionary<AudioTypes, AudioClip>(){
            {AudioTypes.ButtonSelect, _audioClipsList[0]}
        };
    }

    public void PlayButtonPressAudio(){
        PlayAudio(AudioTypes.ButtonSelect);
    }

    public void PlayGameMusic(){
        _musicSource.Play();
    }
    public AudioSource StartFracAudio(Vector3 pos){
        AudioSource source = Instantiate(_posSFXSourcePrefab, pos, Quaternion.identity).GetComponent<AudioSource>();
        source.clip = _audioClips[AudioTypes.BallHitBrick2];
        source.loop = true;
        source.Play();
        return source;
    }
    public void StopFracAudio(AudioSource source){
        Destroy(source.gameObject);
    }

    public void PlayAudio(AudioTypes aType){
        StartCoroutine(PlayAudioCo(aType));
    }

    IEnumerator PlayAudioCo(AudioTypes aType){
        _musicSource.Pause();
        _sFXSource.clip = _audioClips[aType];
        _sFXSource.Play();
        yield return new WaitForSeconds(_audioClips[aType].length);
        _musicSource.Play();
    }
    public void PlayAudio(AudioTypes aType, Vector3 pos){
        AudioSource.PlayClipAtPoint(_audioClips[aType], pos);
    }

}
