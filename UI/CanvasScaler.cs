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
        // Viewport (0,0)과 (1,1)을 월드 좌표로 변환
        Vector3 worldBottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector3 worldTopRight = targetCamera.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

        // 월드 기준 크기 및 중심 계산
        Vector3 worldSize = worldTopRight - worldBottomLeft;
        Vector3 worldCenter = (worldTopRight + worldBottomLeft) * 0.5f;

        // 캔버스 위치 및 크기 설정
        transform.position = worldCenter;
        rectTransform.sizeDelta = new Vector2(worldSize.x , worldSize.y) * 100;
    }
}
