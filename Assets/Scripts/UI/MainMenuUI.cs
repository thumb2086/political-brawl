using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;
    public GameObject characterSelectPanel;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitPressed);
    }

    void OnStartPressed()
    {
        gameObject.SetActive(false);
        if (characterSelectPanel != null)
            characterSelectPanel.SetActive(true);
    }

    void OnQuitPressed()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
