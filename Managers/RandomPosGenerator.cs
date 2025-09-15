using UnityEngine;
using System.Collections.Generic;

public class RandomPosGenerator : MonoBehaviour
{
    struct ViewportArea
    {
        public Vector2 min; // 0~1 ºäÆ÷Æ® ÁÂÇ¥ (Bottom-Left)
        public Vector2 max; // 0~1 ºäÆ÷Æ® ÁÂÇ¥ (Top-Right)
    }

    [SerializeField] private Camera targetCamera;
    [SerializeField] private float horizontalOffset;
    [SerializeField] private float verticalOffset;

    private List<ViewportArea> viewportAreas = new List<ViewportArea>();

    void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(0, 0, 10f));
        Vector3 topRight = targetCamera.ViewportToWorldPoint(new Vector3(1, 1, 10f));
        horizontalOffset = 0.14f;
        verticalOffset = 0.14f * ((topRight.x - bottomLeft.x) / (topRight.y - bottomLeft.y)); // ÇØ»óµµ ºñÀ² * 0.1;

        InitViewportAreas();
    }

    void InitViewportAreas()
    {
        viewportAreas.Clear();

        // ¿ÞÂÊ ¿µ¿ª 
        viewportAreas.Add(new ViewportArea
        {
            min = new Vector2(0f, 0f),
            max = new Vector2(0.5f - horizontalOffset, 1f)
        });

        // ¿À¸¥ÂÊ ¿µ¿ª
        viewportAreas.Add(new ViewportArea
        {
            min = new Vector2(0.5f + horizontalOffset, 0f),
            max = new Vector2(1f, 1f)
        });

        // ¾Æ·¡ ¿µ¿ª
        viewportAreas.Add(new ViewportArea
        {
            min = new Vector2(0f, 0f),
            max = new Vector2(1f, 0.5f - verticalOffset)
        });

        // À­ ¿µ¿ª
        viewportAreas.Add(new ViewportArea
        {
            min = new Vector2(0f, 0.5f + verticalOffset),
            max = new Vector2(1f, 1f)
        });
    }

    public Vector3 GetRandomPos(GameObject prefab)
    {
        int randIdx = Random.Range(0, viewportAreas.Count);
        ViewportArea selected = viewportAreas[randIdx];

        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
        Vector3 size = sr.bounds.size;


        // ·£´ý ÁÂÇ¥ ¼±ÅÃ
        Vector2 randomViewportPos = new Vector2(
            Random.Range(selected.min.x, selected.max.x),
            Random.Range(selected.min.y, selected.max.y)
        );

        // Viewport -> World º¯È¯
        Vector3 worldPos = targetCamera.ViewportToWorldPoint(new Vector3(randomViewportPos.x, randomViewportPos.y, 10f));
        worldPos.z = 0f;

        return worldPos;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || targetCamera == null) return;

        Gizmos.color = Color.green;
        foreach (var area in viewportAreas)
        {
            Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(area.min.x, area.min.y, 10f));
            Vector3 topRight = targetCamera.ViewportToWorldPoint(new Vector3(area.max.x, area.max.y, 10f));

            Gizmos.DrawLine(bottomLeft, new Vector3(topRight.x, bottomLeft.y, 0));
            Gizmos.DrawLine(bottomLeft, new Vector3(bottomLeft.x, topRight.y, 0));
            Gizmos.DrawLine(topRight, new Vector3(topRight.x, bottomLeft.y, 0));
            Gizmos.DrawLine(topRight, new Vector3(bottomLeft.x, topRight.y, 0));
        }
    }
}

