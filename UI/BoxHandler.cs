using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BoxHandler : MonoBehaviour
{
    List<Transform> boxChilds = new List<Transform>();

    [SerializeField] private float wobblingSpeed = 1f;
    [SerializeField] private float wobblingStrength = 1f;

    void Awake()
    {
        foreach(Transform child in transform)
        {
            boxChilds.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Wobbling();
    }

    void Wobbling()
    {
        for (int i = 0; i < boxChilds.Count; ++i)
        {
            Vector3 offset = new Vector3(0, Mathf.Sin(Time.time * wobblingSpeed + i) * wobblingStrength, 0);

            boxChilds[i].transform.localPosition += offset;
        }
    }
}

