using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Globalization;

public class ClickableText : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TextMeshProUGUI textMesh;

    //마우스 호버 이벤트 델리게이트
    public delegate void OnPointerEvent(TextMeshProUGUI clickedText);
    public event OnPointerEvent OnEnter;
    public event OnPointerEvent OnExit;
    public event OnPointerEvent OnClick;
    public event OnPointerEvent OnIdle;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    void OnEnable()
    {
        OnIdle?.Invoke(textMesh);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(textMesh);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke(textMesh);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke(textMesh);
    }
}
