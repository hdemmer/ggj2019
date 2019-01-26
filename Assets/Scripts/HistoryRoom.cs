using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryRoom : HistoryItem
{
    private void Awake()
    {
        var his = GetComponentsInChildren<HistoryItem>();
        foreach (var hi in his)
        {
            hi.startTimeline = startTimeline;
            hi.endTimeline = endTimeline;
        }
    }
}
