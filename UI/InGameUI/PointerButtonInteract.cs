using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 포인터 이벤트를 이용해 버튼의 시각적 상호작용을 처리하는 클래스
public class PointerButtonInteract : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image img; // 버튼의 Image 컴포넌트

    [SerializeField] private Color preColor;   // 기본 색상
    [SerializeField] private Color hoverColor; // 상태 변화 시 사용할 색상

    void Awake()
    {
        // Image 컴포넌트 가져오기
        img = gameObject.GetComponent<Image>();

        // 초기 색상을 기본 색상으로 저장
        preColor = img.color;

        // hoverColor도 초기에는 같은 색으로 시작
        hoverColor = img.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 올라왔을 때 투명도를 낮춰서 hover 효과
        hoverColor.a = 0.7f;
        img.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 벗어나면 원래 색으로 복구
        img.color = preColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 클릭 시작 시 더 어둡게
        hoverColor = img.color;
        hoverColor.a = 0.3f;
        img.color = hoverColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 클릭이 끝나면 다시 기본 색상으로 복구
        img.color = preColor;
    }
}