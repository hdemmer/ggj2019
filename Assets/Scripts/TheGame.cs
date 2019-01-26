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

    public float dizziness = 0f;
    public DizzyEffect dizzyEffect;
    public AnimationCurve dizzyDecay;

    public AnimationCurve fadeCurve;

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

    private float _previousTimeline;
    private bool _isRunning;

    private void Awake()
    {
        slider.value = 1f;
        _instance = this;
        
        vCam.enabled = false;
        cat.gameObject.SetActive(false);

        var sl = slider.GetComponent<SliderListener>();
        sl.onPointerDown = OnPointerDown;
        sl.onPointerUp = OnPointerUp;
    }

    private Coroutine lerpRoutine;
    private void OnPointerUp()
    {
        if (lerpRoutine != null)
        {
            StopCoroutine(lerpRoutine);
        }
        lerpRoutine = null;
        StartCoroutine(LerpSlider());
    }

    private void OnPointerDown()
    {
        if (lerpRoutine != null)
        {
            StopCoroutine(lerpRoutine);
        }
        lerpRoutine = null;
    }

    IEnumerator LerpSlider()
    {
        var origin = slider.value;
        var closest = Mathf.Clamp01(Mathf.Round(origin * 8) / 8f);
        var t = 0f;
        var duration = 0.5f;
        while (t < duration)
        {
            t += Time.deltaTime;
            slider.value = Mathf.Lerp(origin, closest, t / duration);
            yield return null;
        }
        slider.value = closest;
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
        Time.timeScale = _isRunning ? 1f : 0f;

        if (!_isRunning) return;
        
        var val = slider.value;
        timeline = 1 + Mathf.Clamp01(slider.value) * (LIVES - 1);

        dizziness -= dizzyDecay.Evaluate(dizziness) * Time.deltaTime;
        if (dizziness < 0f) dizziness = 0f;
            
        if (_previousTimeline != timeline)
        {
            var delta = Mathf.Abs(_previousTimeline - timeline);

            if (_previousTimeline != -1)
            {
                dizziness += Mathf.Clamp(delta - Time.deltaTime,0f,5f) * 0.1f;
            }

            dizziness = Mathf.Clamp01(dizziness);
            
            foreach (var item in items)
            {
                item.CallUpdate(timeline);
            }

            _previousTimeline = timeline;
        }
        
        foreach (var historyRoom in historyRooms)
        {
            historyRoom.CallUpdate(timeline, cat);
        }

    }

    public float CurrentFade(int startTimeline, int endTimeline)
    {
        var t = 1f;
        if (timeline <= startTimeline)
        {
            t = 1f - Mathf.Clamp01(startTimeline - timeline);
        } else if (timeline >= endTimeline)
        {
            t = 1f - Mathf.Clamp01(timeline - endTimeline);
        }

        return fadeCurve.Evaluate(t);

    }
}
