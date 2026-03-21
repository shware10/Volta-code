using System.Collections.Generic;
using UnityEngine;

/// 퀵슬롯 타입 정의
public enum Slot
{
    WeaponSlot,   // 무기 슬롯
    ItemSlot1,    // 1-4 일반 아이템 슬롯
    ItemSlot2,
    ItemSlot3,
    ItemSlot4,
    BatterySlot,  // 배터리 슬롯
    Max           // 슬롯 개수 계산용
}

/// <summary>
/// 퀵슬롯 관리 클래스
/// </summary>
public class Quickslot : MonoBehaviour, ISwapListener
{
    // 슬롯 데이터 배열
    public SlotData[] slots = new SlotData[(int)Slot.Max];

    [SerializeField] private SlotView[] slotsViews;

    // 슬롯 변경 시 UI 등에 알리기 위한 이벤트
    public delegate void OnQuickslotChangedEvent(SlotData[] slots);
    public event OnQuickslotChangedEvent OnQuickslotChanged;

    // 슬롯 선택 변경 이벤트 (이전 인덱스, 현재 인덱스)
    public delegate void OnQuickslotSelectedEvent(int newIdx);
    public event OnQuickslotSelectedEvent OnQuickslotSelected;

    void Start()
    {
        // 로컬 플레이어만 UI 리스너 등록
        if (pv.IsMine)
        {
            var DroppableUIs = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
                .OfType<DroppableUI>();

            foreach(var dui in DroppableUIs)
            {
                dui.OnSwap += OnSwap;
            }

            var SlotChangedListeners = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
                .OfType<IQuickSlotChangeListener>();

            foreach (var listener in HpListeners)
                OnQuickslotChanged += listener.OnQuickslotChanged;

            var SlotSelectedListeners = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
                .OfType<IQuickSlotSelectedListener>();

            foreach (var listener in DeathListeners)
                OnQuickslotSelected += listener.OnQuickslotSelected;
        }
    }


    /// <summary>
    /// 아이템 획득 시 호출
    /// </summary>
    public void AddItem(ItemData item)
    {
        if (item == null) return;

        // 새 슬롯 데이터 생성
        slots[idx] = new SlotData { item = item };
        // 아이템 타입에 따라 무기 / 일반 아이템 분기
        if (item.ItemType <= 10) // weapon
        {
            // 무기는 WeaponSlot에 고정
            slots[(int)Slot.WeaponSlot] = slot;
        }
        else // 일반 아이템
        {
            int idx = CanAdd();

            // 빈 슬롯이 있으면 추가
            if (idx != -1)
            {
                slots[idx] = slot;
            }
            else return; // 슬롯 가득 찼으면 추가 실패
        }

        // 슬롯 변경 이벤트 발생 (UI 갱신 등)
        OnQuickslotChanged?.Invoke(slots);
    }

    /// <summary>
    /// 아이템을 추가할 수 있는 빈 슬롯 찾기
    /// </summary>
    private int CanAdd()
    {
        // ItemSlot1 ~ ItemSlot4 범위 탐색 (1~4)
        for (int i = (int)Slot.ItemSlot1; i < (int)Slot.ItemSlot1 + maxitem; ++i)
        {
            if (slots[i] == null) return i;
        }
        return -1; // 빈 슬롯 없음
    }

    /// <summary>
    /// 아이템 제거 (버리기 / 사망 시)
    /// </summary>
    public void RemoveItem(int idx)
    {
        // 해당 슬롯 비우기
        slots[idx] = null;

        // UI 갱신 이벤트
        OnQuickslotChanged?.Invoke(slots);
    }

    /// <summary>
    /// 슬롯 간 아이템 위치 교환 (드래그 앤 드롭)
    /// </summary>
    public void OnSwap(int idx1, int idx2)
    {
        // 튜플을 이용한 간단한 swap
        (slots[idx1], slots[idx2]) = (slots[idx2], slots[idx1]);

        // UI 갱신 이벤트
        OnQuickslotChanged?.Invoke(slots);
    }
}