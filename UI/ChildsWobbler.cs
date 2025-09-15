using System.Collections.Generic;
using UnityEngine;

public class ChildsWobbler : MonoBehaviour
{
    List<Transform> childs = new List<Transform>();
    List<Vector3> originPos = new List<Vector3>();
    [SerializeField] private float wobblingSpeed = 10f;
    [SerializeField] private float wobblingStrength = 2f;
    void Awake()
    {
        foreach (Transform child in transform)
        {
            childs.Add(child);
            originPos.Add(child.localPosition);
        }
    }
    void Update()
    {
        Wobbling();
    }

    void Wobbling()
    {
        for (int i = 0; i < childs.Count; ++i)
        {

            Vector3 offset = new Vector3(0, Mathf.Sin(Time.time * wobblingSpeed + i) * wobblingStrength, 0);

            childs[i].transform.localPosition = originPos[i] + offset;
        }
    }
}
