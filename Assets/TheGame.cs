using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public HistoryItem[] items = new HistoryItem[0];
    public Cat cat;

    private bool _isRunning;

    private void Awake()
    {
        slider.value = 1f;
        _instance = this;
    }

    private IEnumerator Start()
    {
        SceneManager.LoadScene("RoomObjects", LoadSceneMode.Additive);

        items = GameObject.FindObjectsOfType<HistoryItem>();
        _isRunning = true;
        yield break;
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
        if (_isRunning)
        {
            timeline = 1 + Mathf.Clamp01(slider.value) * (LIVES - 1);
        }

        Time.timeScale = _isRunning ? 1f : 0f;
        foreach (var item in items)
        {
            item.CallUpdate(timeline);
        }
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
