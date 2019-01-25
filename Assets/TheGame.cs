using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TheGame : MonoBehaviour
{
    public Slider slider;

    public float timeline;
    public const int LIVES = 9;

    private static TheGame _instance;
    public static TheGame Instance
    {
        get {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TheGame>();
            }
            return _instance;
        }
    }
    public List<HistoryItem> items;
    public Cat cat;

    private void Awake()
    {
        _instance = this;
    }

    private void OnDisable()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    void Update()
    {
        timeline = 1 + Mathf.Clamp01(slider.value) * (LIVES - 1);
    }

    public float CurrentFade(int startTimeline, int endTimeline)
    {
        var opacity = 1f;
        if (timeline <= startTimeline)
        {
            opacity = 1f - Mathf.Clamp01(startTimeline - timeline);
        } else if (timeline >= endTimeline)
        {
            opacity = 1f - Mathf.Clamp01(timeline - endTimeline);
        }

        return opacity;
    }
}
