using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HistoryRoom : HistoryItem
{
    private Collider[] obstacles = new Collider[0];
    
    private void Awake()
    {
        var his = GetComponentsInChildren<HistoryItem>();
        foreach (var hi in his)
        {
            hi.startTimeline = startTimeline;
            hi.endTimeline = endTimeline;
        }

        obstacles = GetComponentsInChildren<Collider>();
    }

    public void CallUpdate(float timeline)
    {
        var opacity = TheGame.Instance.CurrentFade(startTimeline, endTimeline);
        
        foreach (var obstacle in obstacles)
        {
            obstacle.enabled = opacity > 0.1f;
        }
    }
}
