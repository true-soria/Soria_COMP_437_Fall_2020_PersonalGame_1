using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class TimedEvent : MonoBehaviour, IComparable<TimedEvent>
{
    public enum EventType
    {
        Neutral,
        Reverse,
        Restore,
        LockSection,
        Split

    }
    
    public float startTime;
    public float bpm;
    public int pointsPerBeat;
    public EventType type;

    private float _tolerance = 0.0001f;


    public TimedEvent(float startTime, float bpm, int pointsPerBeat, EventType type)
    {
        this.startTime = startTime;
        this.bpm = bpm;
        this.pointsPerBeat = pointsPerBeat;
        this.type = type;
    }

    public int CompareTo(TimedEvent other)
    {
        if (startTime > other.startTime)
            return 1;
        if (Math.Abs(startTime - other.startTime) < _tolerance)
            return 0;
        return -1;
    }

}
