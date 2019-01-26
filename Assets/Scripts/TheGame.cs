using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
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
    public CatItem[] catItems = new CatItem[0];
    public HistoryRoom[] historyRooms = new HistoryRoom[0]; 
    public Cat cat;
    public CinemachineVirtualCamera vCam;

    private bool _isRunning;

    private void Awake()
    {
        slider.value = 1f;
        _instance = this;
        
        vCam.enabled = false;
        cat.gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        for (var i=1;i<=9;i++)
        {
            yield return SceneManager.LoadSceneAsync("Room"+i.ToString(), LoadSceneMode.Additive);
        }
        yield return null;
        items = GameObject.FindObjectsOfType<HistoryItem>();
        catItems = GameObject.FindObjectsOfType<CatItem>();
        historyRooms = GameObject.FindObjectsOfType<HistoryRoom>();
        cat.gameObject.SetActive(true);
        vCam.enabled = true;
        cat.CallStart();
        _isRunning = true;
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
