using System.Collections.Generic;
using UnityEngine;

public class NetHandler : MonoBehaviour
{
    public List<Net> netList = new List<Net>();
    private RectangleHandler rectangleHandler;

    void Awake()
    {
        foreach (Transform netTransform in transform)
        {
            Net net = netTransform.gameObject.GetComponent<Net>();
            netList.Add(net);
        }
        rectangleHandler = GameObject.FindGameObjectWithTag("RectangleHandler")
                             .GetComponent<RectangleHandler>();
    }
    void Start()
    {
        for(int i = 0; i < netList.Count; ++i)
        {
            netList[i].OnNet += GameManager.Instance.LoseLife;
            netList[i].OnNet += rectangleHandler.InitBarLevel;
        }
    }
}
