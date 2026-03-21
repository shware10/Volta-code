using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data.SqlTypes;
using ExitGames.Client.Photon;

// 드래그된 아이템을 받아서 슬롯 간 교환을 처리하는 클래스
public class DroppableUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image slotImage;   // 슬롯 배경 이미지
    private Color preColor;    // 기본 색상
    private Color hoverColor;  // 마우스 올렸을 때 색상

    // 슬롯 간 교환 이벤트 
    public delegate void OnDropEvent(int idx1, int idx2);
    public event OnDropEvent OnSwap;

    void Awake()
    {
        // 슬롯의 Image 컴포넌트 가져오기
        slotImage = GetComponent<Image>();

        // 기본 색상 저장
        preColor = slotImage.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 반투명 흰색으로 하이라이트
        hoverColor = Color.white;
        hoverColor.a = 0.6f;

        slotImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 원래 색상으로 복구
        slotImage.color = preColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 드래그 중인 오브젝트가 있고, 아이콘이 있는 경우만 처리
        if (eventData.pointerDrag.GetComponent<Image>().sprite != null)
        {
            // 현재 슬롯에 있는 아이콘
            Transform droppedIcon = transform.GetChild(0);

            // 드래그 중인 아이콘
            Transform draggedIcon = eventData.pointerDrag.transform;

            // 드래그 시작 시 원래 슬롯
            Transform dragParent = draggedIcon.GetComponent<DraggableUI>().preSlot;

            int idx1 = (int)dragParent.GetComponent<SlotView>().slot; // 이전 슬롯
            int idx2 = (int)GetComponent<SlotView>().slot;            // 현재 슬롯

            // 드래그된 아이콘을 드롭한 슬롯으로 이동
            draggedIcon.SetParent(transform);

            // 드래그된 아이콘 위치 초기화
            draggedIcon.position = Vector3.zero;

            // 기존 슬롯에 있던 아이콘을 드래그가 시작된 슬롯으로 이동
            droppedIcon.SetParent(dragParent);

            // 위치 초기화
            droppedIcon.position = Vector3.zero;

            // 실제 데이터 교환을 위해 이벤트 호출
            OnSwap?.Invoke(idx1, idx2);
        }
    }
}