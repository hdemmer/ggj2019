using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Button button;
    public GameObject gameWonImage;
    public CanvasGroup menuScreen;
    public CanvasGroup loadingScreen;

    private static StartScene _instance;

    public static StartScene Instance
    {
        get => _instance;
    }

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _instance = this;
        button.onClick.AddListener(() => { StartGame(); });
        HideLoadingScreen();
    }
    
    private void StartGame()
    {
        button.enabled = false;
        StartCoroutine(StartRoutine());
    }
    
    private IEnumerator StartRoutine()
    {
        yield return ShowLoadingScreen();
        menuScreen.gameObject.SetActive(false);
        yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }


    public Coroutine HideLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.alpha = 1f;
        return StartCoroutine(FadeLoadingScreen());
    }
    
    public Coroutine RestartGame()
    {
        Time.timeScale = 0f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.alpha = 0f;
        return StartCoroutine(StartReset());
    }
    
    private IEnumerator FadeLoadingScreen()
    {
        var t = 0f;
        var duration = 0.4f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            var alpha = 1f - (t / duration);
            loadingScreen.alpha = alpha;
            yield return null;
        }

        loadingScreen.alpha = 0f;
        loadingScreen.gameObject.SetActive(false);
    }
    
    public Coroutine ShowLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.alpha = 0f;
        return StartCoroutine(FadeInLoadingScreen());
    }
    
    private IEnumerator FadeInLoadingScreen()
    {
        var t = 0f;
        var duration = 0.4f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            var alpha = (t / duration);
            loadingScreen.alpha = alpha;
            yield return null;
        }

        loadingScreen.alpha = 1f;
    }
    
    private IEnumerator StartReset()
    {
        gameWonImage.gameObject.SetActive(true);
        menuScreen.gameObject.SetActive(false);
        yield return ShowLoadingScreen();
        menuScreen.gameObject.SetActive(true);
        yield return SceneManager.LoadSceneAsync("Empty", LoadSceneMode.Single);
        yield return null;
        yield return Resources.UnloadUnusedAssets();
        Time.timeScale = 1f;
        yield return HideLoadingScreen();
        button.enabled = true;
    }
}
