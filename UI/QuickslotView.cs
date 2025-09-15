using System.Collections.Generic;
using UnityEngine;

public class QuickslotView : MonoBehaviour,IQuickSlotChangeListener,IQuickSlotSelectedListener
{
    public SlotView[] slotViews = new SlotView[(int)Slot.Max];

    public void OnQuickslotChanged(SlotData[] slots)
    {
        for(int i = 0; i < slots.Length; ++i)
        {
            slotViews[i].InitSlotItem(slots[i]);
        }
    }
    public void OnQuickslotSelected(int preIdx, int curIdx)
    {
        slotViews[preIdx].Select(false);
        slotViews[curIdx].Select(true);
    }
}

