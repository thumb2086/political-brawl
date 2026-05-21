using UnityEngine;
using UnityEngine.UI;

public class SuperBarUI : MonoBehaviour
{
    public Image fill;
    public Text chargeText;
    public GameObject readyIndicator;

    private BaseCharacter target;

    public void SetTarget(BaseCharacter character)
    {
        target = character;
    }

    void Update()
    {
        if (target == null || target.IsDead())
        {
            if (fill != null) fill.fillAmount = 0f;
            if (chargeText != null) chargeText.text = "0%";
            if (readyIndicator != null) readyIndicator.SetActive(false);
            return;
        }

        float charge = target.currentSuperCharge / target.maxSuperCharge;
        if (fill != null) fill.fillAmount = charge;
        if (chargeText != null) chargeText.text = $"{Mathf.RoundToInt(charge * 100)}%";
        if (readyIndicator != null) readyIndicator.SetActive(charge >= 1f);
    }
}
