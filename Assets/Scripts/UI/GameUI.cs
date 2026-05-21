using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    [Header("Joysticks")]
    public Joystick movementJoystick;
    public Joystick attackJoystick;

    [Header("Action Buttons")]
    public Button superButton;
    public Button gadget1Button;
    public Button gadget2Button;

    [Header("HUD")]
    public Image healthBarFill;
    public Image superBarFill;
    public Text healthText;
    public Text countdownText;
    public Text killFeedText;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public Text gameOverText;

    private BaseCharacter localPlayer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (superButton != null)
            superButton.onClick.AddListener(OnSuperPressed);

        if (gadget1Button != null)
            gadget1Button.onClick.AddListener(OnGadget1Pressed);

        if (gadget2Button != null)
            gadget2Button.onClick.AddListener(OnGadget2Pressed);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (localPlayer == null) return;

        if (healthBarFill != null)
            healthBarFill.fillAmount = localPlayer.currentHealth / localPlayer.maxHealth;

        if (superBarFill != null)
            superBarFill.fillAmount = localPlayer.currentSuperCharge / localPlayer.maxSuperCharge;

        if (healthText != null)
            healthText.text = $"{Mathf.RoundToInt(localPlayer.currentHealth)}/{Mathf.RoundToInt(localPlayer.maxHealth)}";

        if (countdownText != null && GameManager.Instance != null)
        {
            float timeLeft = GameManager.Instance.gameTime;
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            countdownText.text = $"{minutes:00}:{seconds:00}";
        }

        if (superButton != null)
            superButton.interactable = localPlayer.currentSuperCharge >= localPlayer.maxSuperCharge;

        if (gadget1Button != null)
            gadget1Button.interactable = localPlayer.gadget1Timer <= 0f;

        if (gadget2Button != null)
            gadget2Button.interactable = localPlayer.gadget2Timer <= 0f;
    }

    public void SetLocalPlayer(BaseCharacter player)
    {
        localPlayer = player;

        if (InputManager.Instance != null)
        {
            InputManager.Instance.movementJoystick = movementJoystick;
            InputManager.Instance.attackJoystick = attackJoystick;
        }
    }

    void OnSuperPressed()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.superPressed = true;
    }

    void OnGadget1Pressed()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.gadget1Pressed = true;
    }

    void OnGadget2Pressed()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.gadget2Pressed = true;
    }

    public void ShowGameOver(string message)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        if (gameOverText != null)
            gameOverText.text = message;
    }
}
