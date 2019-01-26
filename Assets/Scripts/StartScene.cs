using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Button button;

    void Awake()
    {
        button.onClick.AddListener(() => { SceneManager.LoadScene("Game", LoadSceneMode.Single); });
    }
}
