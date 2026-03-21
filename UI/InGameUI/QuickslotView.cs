using System.Collections.Generic;
using UnityEngine;

// 퀵슬롯 전체 UI를 관리하는 View 클래스
public class QuickslotView : MonoBehaviour, IQuickSlotChangeListener, IQuickSlotSelectedListener
{
    // 개별 슬롯 UI를 담고 있는 배열
    public SlotView[] slotViews = new SlotView[(int)Slot.Max];

    // 슬롯 데이터가 변경될 때 호출됨
    public void OnQuickslotChanged(SlotData[] slots)
    {
        // 모든 슬롯을 순회하면서 UI 갱신
        for (int i = 0; i < slots.Length; ++i)
        {
            // 각 슬롯에 맞는 데이터를 확인해 아이콘 업데이트
            slotViews[i].InitSlotItem(slots[i]);
        }
    }

    // 선택된 슬롯이 변경될 때 호출됨
    public void OnQuickslotSelected(int preIdx, int curIdx)
    {
        // 이전 슬롯 외곽선 해제 
        slotViews[preIdx].Select(false);

        // 현재 슬롯 외곽선 표시
        slotViews[curIdx].Select(true);
    }
}