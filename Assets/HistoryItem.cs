﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryItem : MonoBehaviour
{
    public int startTimeline = 9;
    public int endTimeline = 9;
    private MeshRenderer mr;

    [SerializeField]
    private float opacity = 0f;
    
    private void OnEnable()
    {
        mr = GetComponent<MeshRenderer>();
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
        if (timeline != previousTimeline)
        {
            previousTimeline = timeline;
            var opacity = TheGame.Instance.CurrentFade(startTimeline, endTimeline);
            
            mr.material.color = new Color(1f,1f,1f,opacity);
            this.opacity = opacity;
        }
    }

    public float GetOpacity()
    {
        return opacity;
    }
}
