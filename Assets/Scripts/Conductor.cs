﻿using System;
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
    private float _timeElapsed;
    private float _timeUntilNextBeat;
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
        
        _beatDelta = 1 / (bpm / 60f);
        _timeElapsed = 0;
        _timeUntilNextBeat = songStart;
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
            ChangeActiveBeatGroup();
        }

    }

    private void ChangeActiveBeatGroup()
    {
        foreach (GameObject go in _beatGroups[_currentTagIndex])
            go.SetActive(false);
    
        _currentTagIndex = (_currentTagIndex + 1) % tagSequence.Length;
        foreach (GameObject go in _beatGroups[_currentTagIndex])
            go.SetActive(true);
    }
}
