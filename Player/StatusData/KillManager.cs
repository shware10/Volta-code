using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 킬 카운팅 클래스
/// </summary>
public class KillManager : MonoBehaviour
{
    public int killCount { get; set; } = 0;

    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    // 킬 증가 RPC
    [PunRPC]
    private void RpcAddKillCount()
    {
        killCount += 1;
    }

    /// <summary>
    /// 킬 증가 요청 Rpc 함수 
    /// </summary>
    public void AddKillCount()
    {
        pv.RPC("RpcAddKillCount", RpcTarget.Owner);
    }
}