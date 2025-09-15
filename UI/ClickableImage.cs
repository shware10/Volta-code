using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ClickableImage : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public delegate void OnPointerEvent(CanvasGroup cg);
    public event OnPointerEvent OnEnter;
    public event OnPointerEvent OnExit;
    public event OnPointerEvent OnUp;
    public event OnPointerEvent OnDown;
    public event OnPointerEvent OnClick;
    public delegate void OnHoldEvent(bool isHold);

    private CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke(cg);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke(cg);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDown?.Invoke(cg);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnUp?.Invoke(cg);
        OnClick?.Invoke(cg);
    }
}
