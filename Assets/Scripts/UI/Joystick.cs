using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform handle;
    public float handleRange = 80f;
    public bool isActive { get; private set; }

    private RectTransform baseRect;
    private Vector2 inputDirection;
    private Canvas canvas;
    private Camera uiCamera;

    void Start()
    {
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            uiCamera = null;
        else
            uiCamera = canvas.worldCamera;

        if (handle == null)
            handle = transform.Find("Handle")?.GetComponent<RectTransform>();

        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        isActive = false;
        inputDirection = Vector2.zero;

        if (handle != null)
            handle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isActive = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            baseRect, eventData.position, uiCamera, out position);

        position = Vector2.ClampMagnitude(position, handleRange);
        inputDirection = position.normalized;

        if (handle != null)
            handle.anchoredPosition = position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isActive = false;
        inputDirection = Vector2.zero;

        if (handle != null)
            handle.anchoredPosition = Vector2.zero;
    }

    public Vector2 GetDirection()
    {
        return inputDirection;
    }
}
