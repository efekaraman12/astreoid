using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    [Header("Settings")]
    [SerializeField] private int gameSceneIndex = 1;
    [SerializeField] private float transitionDelay = 0.5f;

    private void Start()
    {
        // Button listeners'larý ekle
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Play Button is not assigned in SceneChanger!");
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogError("Exit Button is not assigned in SceneChanger!");
        }
    }

    private void OnDestroy()
    {
        // Button listeners'larý temizle
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(StartGame);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(ExitGame);
        }
    }

    public void StartGame()
    {
        Debug.Log("Starting game...");
        // Geçiþ efekti için delay ekleyebilirsiniz
        Invoke(nameof(LoadGameScene), transitionDelay);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}