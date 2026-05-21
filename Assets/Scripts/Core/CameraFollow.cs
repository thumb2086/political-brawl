using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10f);
    public float orthographicSize = 12f;

    void Start()
    {
        Camera camera = GetComponent<Camera>();
        if (camera != null)
            camera.orthographicSize = orthographicSize;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            if (GameManager.Instance != null && GameManager.Instance.players != null && GameManager.Instance.players.Length > 0)
                target = GameManager.Instance.players[0].transform;
            else
                return;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
