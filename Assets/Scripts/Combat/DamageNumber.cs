using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour
{
    public float duration = 0.8f;
    public float riseSpeed = 2f;
    private float timer;
    private Text text;
    private Color startColor;

    public void Show(float damage, Color color)
    {
        text = GetComponent<Text>();
        if (text == null)
        {
            text = gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
        }

        text.text = Mathf.RoundToInt(damage).ToString();
        text.color = color;
        startColor = color;
        timer = duration;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        timer -= Time.deltaTime;

        if (text != null)
        {
            Color c = startColor;
            c.a = timer / duration;
            text.color = c;
        }
    }
}
