using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    TextMeshProUGUI text;
    float interval = 0.5f;
    WaitForSeconds wts;
    Vector4 color;
    
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        wts = new WaitForSeconds(interval);
        color = text.color;
        gameObject.SetActive(false);
    }
    IEnumerator Start()
    {
        while(true)
        {
            color.w = 0;
            text.color = color;
            yield return wts;
            color.w = 1;
            text.color = color;
            yield return wts;
        }
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }
}
