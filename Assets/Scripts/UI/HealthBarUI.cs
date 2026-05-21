using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image healthFill;
    public Image superFill;
    public Text healthText;
    public BaseCharacter target;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (target == null || target.IsDead())
        {
            if (healthFill != null) healthFill.fillAmount = 0f;
            if (superFill != null) superFill.fillAmount = 0f;
            return;
        }

        if (healthFill != null)
            healthFill.fillAmount = target.currentHealth / target.maxHealth;

        if (superFill != null)
            superFill.fillAmount = target.currentSuperCharge / target.maxSuperCharge;

        if (healthText != null)
            healthText.text = $"{Mathf.RoundToInt(target.currentHealth)}/{Mathf.RoundToInt(target.maxHealth)}";

        if (mainCamera != null)
            transform.LookAt(mainCamera.transform);
    }

    public void SetTarget(BaseCharacter character)
    {
        target = character;
    }
}
