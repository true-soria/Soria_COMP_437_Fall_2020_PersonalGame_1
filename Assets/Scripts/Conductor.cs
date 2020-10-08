using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;

public class Conductor : MonoBehaviour
{
    public static readonly int BeatRotation = 4;  
    public String[] tagSequence;
    public float bpm = 60.0f;
    public float pointsPerBeat = 4;
    public float songStart = 10f;
    public GameObject[][] BeatGroups;
    public int currentTagIndex;
    public bool beatDisorder = false;
    public PauseMenu pauseMenu;

    private AudioSource _song;
    private float _beatDelta;
    private float _pointsDelta;
    private float _timeUntilNextBeat;
    private float _timeUntilPointsCheck;
    private int _activeComponents;



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
        _song.loop = false;
        _beatDelta = 1 / (bpm / 60f);
        _pointsDelta = _beatDelta / pointsPerBeat;
        _timeUntilNextBeat = songStart;
        _timeUntilPointsCheck = _timeUntilNextBeat + (_pointsDelta / 2);
        currentTagIndex = tagSequence.Length - 1;
        
        BeatGroups = new GameObject[tagSequence.Length][];
        for (int i = 0; i < tagSequence.Length; i++)
        {
            BeatGroups[i] = GameObject.FindGameObjectsWithTag(tagSequence[i]);
            foreach (GameObject go in BeatGroups[i])
                go.SetActive(false);
        }
    }

    public void SongUpdater(TimedEvent change)
    {
        bpm = change.bpm; 
        _beatDelta = 1 / (bpm / 60f);
        pointsPerBeat = change.pointsPerBeat;
        _pointsDelta = _beatDelta / pointsPerBeat;
    }

    public float GetTimeElapsed()
    {
        return _song.time;
    }

    void Update()
    {
        if (_song.time > _timeUntilNextBeat)
        {
            _timeUntilNextBeat += _beatDelta;
            PlatformerPlayer.InMyZone = false;
            ChangeActiveBeatGroup();
        }

        if (_song.time > _timeUntilPointsCheck)
        {
            if (PlatformerPlayer.InMyZone)
                PlatformerPlayer.AddScore();
            _timeUntilPointsCheck += _pointsDelta;
        }
        if (_song.time >= _song.clip.length)
        {
            pauseMenu.EndPause();
        }
        SongManager();
    }

    private void ChangeActiveBeatGroup()
    {
        if (beatDisorder)
        {
            for (int i = 0; i < BeatRotation; i++)
            {
                foreach (GameObject go in BeatGroups[i])
                    go.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject go in BeatGroups[currentTagIndex])
                go.SetActive(false);
        }

        currentTagIndex = (currentTagIndex + 1) % tagSequence.Length;
        foreach (GameObject go in BeatGroups[currentTagIndex])
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
