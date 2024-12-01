using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip[] _musicClips;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _musicClips[0];
        _audioSource.Play();
        
    }
    int currentClip = 0;

    // Update is called once per frame
    void Update()
    {
        
        if(_audioSource.time + 0.25f < _musicClips[currentClip].length && !_audioSource.isPlaying)
        {
            Debug.Log(_musicClips[currentClip].length + "   " + _audioSource.time);
            _audioSource.Play();
            return;
        }
        if (!_audioSource.isPlaying)
        {
            currentClip++;
            if (currentClip >= _musicClips.Length)
            {
                currentClip = 0;
            }
            _audioSource.clip = _musicClips[currentClip];
            _audioSource.Play();
        }
        
    }
}
