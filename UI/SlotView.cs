using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotView : MonoBehaviour
{
    public Slot slot;
    private Image slotImage;
    private Outline outline;

    void Awake()
    {
        slotImage = transform.GetChild(0).GetComponent<Image>();
        outline = GetComponent<Outline>();
    }

    public void InitSlotItem(SlotData slotData)
    {
        if (slotData == null) slotImage.sprite = null;
        else slotImage.sprite = slotData.item.itemImage;
    }

    public void Select(bool isSelect)
    {
        outline.enabled = isSelect;
    }
}