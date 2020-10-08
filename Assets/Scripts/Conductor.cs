using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    public String[] tagSequence;
    public int bpm = 60;
    public int pointsPerBeat = 4;
    public float songStart = 10f;

    private AudioSource _song;
    private float _beatDelta;
    private float _pointsDelta;
    private float _timeElapsed;
    private float _timeUntilNextBeat;
    private float _timeUntilPointsCheck;
    private int _activeComponents;
    private int _currentTagIndex;
    private GameObject[][] _beatGroups;
    

    void Awake () {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
#endif
    }

    void Start()
    {
        _song = GetComponent<AudioSource>();
        _song.Play();
        _beatDelta = 1 / (bpm / 60f);
        _pointsDelta = _beatDelta / pointsPerBeat;
        _timeElapsed = 0;
        _timeUntilNextBeat = songStart;
        _timeUntilPointsCheck = _timeUntilNextBeat + (_pointsDelta / 2);
        _currentTagIndex = tagSequence.Length - 1;
        
        _beatGroups = new GameObject[tagSequence.Length][];
        for (int i = 0; i < tagSequence.Length; i++)
        {
            _beatGroups[i] = GameObject.FindGameObjectsWithTag(tagSequence[i]);
            foreach (GameObject go in _beatGroups[i])
                go.SetActive(false);
        }

    }

    void Update()
    {
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed > _timeUntilNextBeat)
        {
            _timeUntilNextBeat += _beatDelta;
            PlatformerPlayer.InMyZone = false;
            ChangeActiveBeatGroup();
        }

        if (_timeElapsed > _timeUntilPointsCheck)
        {
            if (PlatformerPlayer.InMyZone)
                PlatformerPlayer.AddScore();
            _timeUntilPointsCheck += _pointsDelta;
        }
        SongManager();
    }

    private void ChangeActiveBeatGroup()
    {
        foreach (GameObject go in _beatGroups[_currentTagIndex])
            go.SetActive(false);
    
        _currentTagIndex = (_currentTagIndex + 1) % tagSequence.Length;
        foreach (GameObject go in _beatGroups[_currentTagIndex])
            go.SetActive(true);
    }

    private void SongManager()
    {
        if (PauseMenu.GamePaused && _song.isPlaying)
        {
            _song.Pause();
        }
        else if (!PauseMenu.GamePaused && !_song.isPlaying)
        {
            _song.UnPause();
        }
    }
}
