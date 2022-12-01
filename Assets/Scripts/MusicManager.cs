using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private AudioClip[] _audioClipsVocal;
    [SerializeField] private AudioClip[] _audioClipsInstrumental;

    private AudioSource[] _audioSourcesVocal;
    private AudioSource[] _audioSourcesInstrumental;
    private AudioSource _currentSource;

    private const float _measureLength = 3f;
    [SerializeField] private int _measuresPerVerse = 16;
    private const int _measuresInVocalVerse6 = 17;
    private const float _fadeOutTime = _measureLength / 32;
    private const float _timingOffset = 1f / 60 + _fadeOutTime;


    [Space]
    [Header("Debug properties")]

    [SerializeField] private float _timer = 0;
    [SerializeField] private int _measure = 0;
    [SerializeField] private int _level = 0;
    [SerializeField] private bool _isVocal = false;
    [SerializeField] private string _nowPlaying;

    public int Level { get { return _level; } set { _level = value; } }
    private AudioSource CurrentSource
    {
        get { return _currentSource; }
        set
        {
            if (_currentSource != null)
                StartCoroutine(FadeOut(_currentSource));
            _timer = 0;
            _measure = 0;

            _currentSource = value;
            Debug.Log($"Start source {_currentSource.clip}");
            _nowPlaying = _currentSource.clip.ToString();
            _currentSource.Play();
            GameManager.Instance.OnMusicChanged(Level);
        }
    }

    private void Start()
    {
        InitMusicVocal();
        InitMusicInstrumental();
        CurrentSource = _audioSourcesInstrumental[0];
    }

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer >= _measureLength - _timingOffset)
        {
            _timer -= _measureLength;
            _measure++;
            StartOfMeasure();
        }
    }

    private void StartOfMeasure()
    {
        if (_isVocal)
        {
            if (CurrentSource != _audioSourcesVocal[Level])
                CurrentSource = _audioSourcesVocal[Level];

            else if (Level < 6 && _measure >= _measuresPerVerse
                || (Level == 6 && _measure >= _measuresInVocalVerse6))
            {
                _isVocal = false;
                CurrentSource = _audioSourcesInstrumental[Level];
            }
        }
        if (!_isVocal)
        {
            if (CurrentSource != _audioSourcesInstrumental[Level])
            {
                _isVocal = true;
                CurrentSource = _audioSourcesVocal[Level];
            }
        }
    }

    private IEnumerator FadeOut(AudioSource source)
    {
        while (source.volume > 0)
        {
            source.volume -= Time.deltaTime / _fadeOutTime;
            yield return null;
        }
        Debug.Log($"Stop source {source.clip}");
        source.Stop();
    }

    private void InitMusicVocal()
    {
        _audioSourcesVocal = new AudioSource[_audioClipsVocal.Length];
        for (int i = 0; i < _audioClipsVocal.Length; i++)
        {
            if (_audioClipsVocal[i] == null)
                continue;

            _audioSourcesVocal[i] = gameObject.AddComponent<AudioSource>();
            _audioSourcesVocal[i].clip = _audioClipsVocal[i];
            _audioSourcesVocal[i].loop = true;
        }
    }

    private void InitMusicInstrumental()
    {
        _audioSourcesInstrumental = new AudioSource[_audioClipsInstrumental.Length];
        for (int i = 0; i < _audioClipsInstrumental.Length; i++)
        {
            if (_audioClipsInstrumental[i] == null)
                continue;

            _audioSourcesInstrumental[i] = gameObject.AddComponent<AudioSource>();
            _audioSourcesInstrumental[i].clip = _audioClipsInstrumental[i];
            _audioSourcesInstrumental[i].loop = true;
        }
    }

}
