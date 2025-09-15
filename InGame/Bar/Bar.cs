using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public SpriteRenderer rdr;
    public Color color;
    [SerializeField] private int _level = 1;
    public int level
    {
        get { return _level; }
        set { _level = Mathf.Clamp(value, 1, 3); }        
    }

    public delegate void OnTrigger();
    public event OnTrigger FitColor;
    public event OnTrigger UnfitColor;

    void Awake()
    {
        transform.localScale = Vector3.zero;
        FitColor += AddBarLevel;
        UnfitColor += InitBarLevel;
        gameObject.GetComponent<Collider>().enabled = false;

        rdr = GetComponent<SpriteRenderer>();
        setGlowAmount();
    }


    void OnTriggerEnter(Collider other)
    {
        Jet jet = other.gameObject.GetComponent<Jet>();

        if (jet.color == color)
        {
            FitColor.Invoke();
            jet.Fit();
            AudioManager.Instance.PlaySFX(0);
            DebugX.Log($"¸ÂÃãlevel: {level}");
        }
        else
        {
            UnfitColor.Invoke();
            jet.Unfit();
            AudioManager.Instance.PlaySFX(1);
            DebugX.Log($"Æ²¸² level: {level}");
        }
    }

    public void AddBarLevel()
    {
        ++level;
        setGlowAmount();
    }

    public void InitBarLevel()
    {
        level = 1;
        setGlowAmount();
    }


    public void setGlowAmount()
    {
        rdr.material.SetFloat("_GlowAmount", level);
    }
}
