using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data.SqlTypes;
using ExitGames.Client.Photon;
// using static UnityEditor.Progress;


public class DroppableUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image slotImage;
    private Color preColor;
    private Color hoverColor;

    public delegate void OnDropEvent(int idx1, int idx2);
    public event OnDropEvent OnSwap;

    void Awake()
    {
        slotImage = GetComponent<Image>();
        preColor = slotImage.color;
    }

    //마우스 포인터가 현재 아이템 슬롯 영역 내부로 들어갈 때 1회 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverColor = Color.white;
        hoverColor.a = 0.6f;
        slotImage.color = hoverColor;
    }

    //마우스 포인터가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        slotImage.color = preColor;
    }

    // 현재 아이템 슬롯 영역 내부에서 드롭을 했을 때 1회 호출
    public void OnDrop(PointerEventData eventData)
    {
        // pointerDrag = 드래그중인 아이콘 / 드래그하고있는 아이콘이 있으면
        if(eventData.pointerDrag.GetComponent<Image>().sprite != null)
        {
            Transform droppedIcon = transform.GetChild(0);
            Transform draggedIcon = eventData.pointerDrag.transform;

            Transform dragParent = draggedIcon.GetComponent<DraggableUI>().preSlot;

            int idx1 = (int)dragParent.GetComponent<SlotView>().slot;
            int idx2 = (int)transform.GetComponent<SlotView>().slot;

            draggedIcon.SetParent(transform);
            draggedIcon.position = Vector3.zero;

            droppedIcon.SetParent(dragParent);
            droppedIcon.position = Vector3.zero;

            OnSwap?.Invoke(idx1, idx2);
        }
    }
}
