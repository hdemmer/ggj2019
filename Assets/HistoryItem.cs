using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryItem : MonoBehaviour
{
    public int startTimeline = 9;
    public int endTimeline = 9;
    private GlitchController gc;
    
    private void OnEnable()
    {
        gc = GetComponent<GlitchController>();
        TheGame.Instance.items.Add(this);
    }

    private void OnDisable()
    {
        var theGame = TheGame.Instance;
        if (theGame)
        {
            theGame.items.Remove(this);
        }
    }

    private float previousTimeline = -1;
    void Update()
    {
        var timeline = TheGame.Instance.timeline;
        if (timeline != previousTimeline && gc != null)
        {
            previousTimeline = timeline;
            var opacity = TheGame.Instance.CurrentFade(startTimeline, endTimeline);

			gc.disapear = 1f - opacity;
            
        }
    }
}
