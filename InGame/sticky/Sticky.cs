using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sticky : MonoBehaviour,IBarStateListener,IGameStateListener
{
    public SpriteRenderer rdr;
    public StickySO stickyData;
    protected float bonusScore;
    protected Transform poolPos;
    
    List<Bar> barList;
    List<Net> netList;

    protected virtual void Awake()
    {
        barList = GameObject.FindGameObjectWithTag("RectangleHandler")
                 .GetComponent<RectangleHandler>().barList;
        netList = GameObject.FindGameObjectWithTag("NetHandler")
                 .GetComponent<NetHandler>().netList;
        poolPos = GameObject.FindGameObjectWithTag("StickyPool").GetComponent<Transform>();
        
        rdr = GetComponent<SpriteRenderer>();
    }

    public virtual void Init(Material mat, float amount)
    {
        rdr.material = mat;
        bonusScore = amount;
    }

    protected void Subscribe() // gamestate/bar/net 충돌 구독
    {
        GameManager.Instance.OnStateChanged += OnStateChanged;
        for (int i = 0; i < barList.Count; ++i)
        {
            barList[i].FitColor += Fit;
            barList[i].UnfitColor += Unfit;
            netList[i].OnNet += Unfit;
        }
    }

    protected void Unsubscribe() // gamestate/bar/net 충돌 구독 취소
    {
        GameManager.Instance.OnStateChanged -= OnStateChanged;
        for (int i = 0; i < barList.Count; ++i)
        {
            barList[i].FitColor -= Fit;
            barList[i].UnfitColor -= Unfit;
            netList[i].OnNet -= Unfit;
        }
    } 

    // 인터페이스
    public virtual void Fit()
    {
    }

    public virtual void Unfit()
    {
    }

    public virtual void OnStateChanged(GameState state) { }

}
