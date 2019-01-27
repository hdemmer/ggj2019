using System;
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
    public ContemplationEffect contemplationPrefab;

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

    private float _previousTimeline = -1f;
    private bool _isRunning;

    private void Awake()
    {
        contemplationPrefab.gameObject.SetActive(false);
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
        
        slider.onValueChanged.AddListener((v) =>
        {
            AudioManager.Instance.UseSlider();
        });

        StartScene.Instance.HideLoadingScreen();
    }

    private void OnDisable()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void GameOver()
    {
        _isRunning = false;
        Time.timeScale = 0f;
        cat.StopAllCoroutines();
        cat.enabled = false;
        var st = StartScene.Instance;
        st.RestartGame();
    }

    void Update()
    {
        if (!_isRunning) return;

        var dizzy = GetTotalDizziness();
        var am = AudioManager.Instance;
        am.SetDizzy(dizzy);
        var darkPast = 0f;
        if (timeline < lowestTimeline - 1f)
        {
            darkPast = Mathf.Clamp01((lowestTimeline - timeline) - 1);
        }
        am.SetDarkPast(darkPast);
        
        var ts = 1f + Mathf.Clamp(dizzy*3f,0f,3f); 
        Time.timeScale = _isRunning ? ts : 0f;

        
        var val = slider.value;
        timeline = 1 + Mathf.Clamp01(val) * (LIVES - 1);
        
        // HACK!
        if (val == 0f)
        {
            GameOver();
            return;
        }

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

    private float lowestTimeline = 9f;
    public float GetTotalDizziness()
    {
        var res = dizziness;
        if (cat.targets.Count > 0)
        {
            var targetTimeline = cat.targets[0].startTimeline;
            if (targetTimeline < lowestTimeline)
            {
                lowestTimeline = targetTimeline;
            }
            var d = Mathf.Abs(timeline - lowestTimeline);
            d -= 1f;
            if (d < 0f) d = 0f;
            if (timeline > lowestTimeline) d = 0f;
            res += d;
        }

        return res;
    }

    public void PlayContemplateParticles(CatItem target)
    {
        foreach (var gc in target.gcs)
        {
            var tr = gc.transform;
            var c = GameObject.Instantiate(contemplationPrefab, tr, worldPositionStays:false);
            c.mf.sharedMesh = gc.gameObject.GetComponent<MeshFilter>().sharedMesh;
            c.gameObject.SetActive(true);
            c.Play();
        }

    }
}
