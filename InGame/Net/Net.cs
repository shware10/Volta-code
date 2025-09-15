using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Net : MonoBehaviour
{
    public delegate void OnTrigger();
    public event OnTrigger OnNet;

    void OnTriggerEnter(Collider other)
    {
        Jet jet = other.gameObject.GetComponent<Jet>();
        jet.Net();
        AudioManager.Instance.PlaySFX(1);
        OnNet?.Invoke();
    }
}
