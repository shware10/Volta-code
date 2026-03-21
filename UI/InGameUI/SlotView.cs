using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 퀵슬롯의 개별 슬롯 UI를 관리하는 뷰 클래스
public class SlotView : MonoBehaviour
{
    private Slot slot;
    private Image slotImage; // 아이템 아이콘 표시용 이미지
    private Outline outline; // 선택 표시용 외곽선

    void Awake()
    {
        // 첫 번째 자식 오브젝트의를 가져와 아이콘으로 사용
        slotImage = transform.GetChild(0).GetComponent<Image>();

        // 현재 오브젝트에 붙은 Outline 컴포넌트 가져오기
        outline = GetComponent<Outline>();
    }

    // 슬롯에 아이템 데이터를 세팅하는 함수
    public void InitSlotItem(SlotData slotData)
    {
        if (slotData == null)
        {
            // 슬롯이 비어있으면 이미지 제거
            slotImage.sprite = null;
        }
        else
        {
            // 아이템 이미지로 갱신
            slotImage.sprite = slotData.item.itemImage;
        }
    }

    // 슬롯 선택 여부를 시각적으로 표시
    public void Select(bool isSelect)
    {
        // 선택 상태에 따라 외곽선 활성/비활성
        outline.enabled = isSelect;
    }
}