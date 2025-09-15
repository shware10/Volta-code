using UnityEngine;
using System.Collections.Generic;

public class LifeHandler : MonoBehaviour
{
    public List<Transform> lifeList = new List<Transform>();  

    public void Awake()
    {
        foreach(Transform child in transform)
        {
            lifeList.Add(child);
        }
    }
    public void InitLife()
    {
        for (int i = 0; i < lifeList.Count; ++i)
        {
            lifeList[i].gameObject.SetActive(true);
        }
    }
}
