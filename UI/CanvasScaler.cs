using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CanvasScaler : MonoBehaviour
{
    public Camera targetCamera;
    public float camDistance = 10f;

    private RectTransform rectTransform;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        rectTransform = GetComponent<RectTransform>();

        ScaleCanvasToCameraView();
    }

    void ScaleCanvasToCameraView()
    {
        // Viewport (0,0)�� (1,1)�� ���� ��ǥ�� ��ȯ
        Vector3 worldBottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector3 worldTopRight = targetCamera.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

        // ���� ���� ũ�� �� �߽� ���
        Vector3 worldSize = worldTopRight - worldBottomLeft;
        Vector3 worldCenter = (worldTopRight + worldBottomLeft) * 0.5f;

        // ĵ���� ��ġ �� ũ�� ����
        transform.position = worldCenter;
        rectTransform.sizeDelta = new Vector2(worldSize.x , worldSize.y) * 100;
    }
}
