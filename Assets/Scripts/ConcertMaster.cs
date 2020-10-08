using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ConcertMaster : MonoBehaviour
{
    [SerializeField] private TimedEvent[] events;
    [SerializeField] private Conductor conductor;
    
    private GameObject[][] _defaultBeatGroups;
    private int _eventIndex = 0;
    private TimedEvent _nextEvent = null;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BeatGroupInitializer());
        if (events.Length > 0)
        {
            Array.Sort(events);
            _nextEvent = events[_eventIndex];
        }
    }

    private IEnumerator BeatGroupInitializer()
    {
        yield return new WaitForSecondsRealtime(1);
        _defaultBeatGroups = new GameObject[conductor.BeatGroups.Length][];
        for (int i = 0; i < conductor.BeatGroups.Length; i++)
        {
            _defaultBeatGroups[i] = conductor.BeatGroups[i];
        }
    }
    
    private void FindNextEvent()
    {
        _eventIndex++;
        if (_eventIndex < events.Length)
            _nextEvent = events[_eventIndex];
        else
            _nextEvent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (_nextEvent != null && conductor.GetTimeElapsed() > _nextEvent.startTime)
        {
            conductor.beatDisorder = true;
            switch (_nextEvent.type)
            {
                case TimedEvent.EventType.Neutral:
                    break;
                case TimedEvent.EventType.Reverse:
                    for (int i = 0; i < conductor.BeatGroups.Length; i++)
                        conductor.BeatGroups[i] = _defaultBeatGroups[conductor.BeatGroups.Length - i - 1];
                    break;
                case TimedEvent.EventType.Restore:
                    for (int i = 0; i < conductor.BeatGroups.Length; i++)
                        conductor.BeatGroups[i] = _defaultBeatGroups[i];
                    break;
                case TimedEvent.EventType.LockSection:
                    for (int i = 0; i < conductor.BeatGroups.Length; i++)
                        conductor.BeatGroups[i] = conductor.BeatGroups[conductor.currentTagIndex];
                    break;
                case TimedEvent.EventType.Split:
                    conductor.BeatGroups[0] = _defaultBeatGroups[0];
                    conductor.BeatGroups[2] = _defaultBeatGroups[0];
                    conductor.BeatGroups[1] = _defaultBeatGroups[2];
                    conductor.BeatGroups[3] = _defaultBeatGroups[2];
                    break;
            }
            conductor.SongUpdater(_nextEvent);
            FindNextEvent();
        }
    }
}
