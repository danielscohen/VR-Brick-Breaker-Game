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
        _audioClips = new Dictionary<AudioTypes, AudioClip>();
        for(int i = 0; i < _audioClipsList.Count; i++){
            _audioClips.Add((AudioTypes)i, _audioClipsList[i]);
        }
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

    public void PlayAudio(AudioReason reason){
        StartCoroutine(PlayAudioCo(reason));
    }
    public void PlayAudio(AudioTypes aType){
        _sFXSource.clip = _audioClips[aType];
        _sFXSource.Play();
    }
    public void SpeedUpGameMusic(){
        _musicSource.pitch = 1.2f;
    }
    public void ResetGameMusicSpeed(){
        _musicSource.pitch = 1f;
    }

    IEnumerator PlayAudioCo(AudioReason reason){
        if(reason == AudioReason.GamePaused || reason == AudioReason.GameRestarted || reason == AudioReason.GameQuit){
            _musicSource.Pause();
            _sFXSource.clip = _audioClips[AudioTypes.GamePaused];
            _sFXSource.Play();
        } else if(reason == AudioReason.GameResumed){
            _sFXSource.clip = _audioClips[AudioTypes.GamePaused];
            _sFXSource.Play();
            yield return new WaitForSeconds(_audioClips[AudioTypes.GamePaused].length);
            _musicSource.UnPause();
        } else{
            AudioTypes aType = AudioTypes.ButtonSelect;
            switch(reason){
                case AudioReason.GameStarted:
                    aType = AudioTypes.ButtonSelect;
                    break;
                case AudioReason.GameWon:
                    aType = AudioTypes.GameWon;
                    break;
                case AudioReason.GameLost:
                    aType = AudioTypes.GameLost;
                    break;
            }
            _musicSource.Stop();
            _sFXSource.clip = _audioClips[aType];
            _sFXSource.Play();
            yield return new WaitForSeconds(4f);
            _musicSource.Play();
        }
    }
    public void PlayAudio(AudioTypes aType, Vector3 pos){
        StartCoroutine(PlayAudioCo(aType, pos));
    }
    IEnumerator PlayAudioCo(AudioTypes aType, Vector3 pos){
        AudioSource source = Instantiate(_posSFXSourcePrefab, pos, Quaternion.identity).GetComponent<AudioSource>();
        source.clip = _audioClips[aType];
        source.Play();
        yield return new WaitForSeconds(_audioClips[aType].length);
        Destroy(source.gameObject);
    }

}
