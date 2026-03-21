using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

/// <summary>
/// 플레이어 hp 관리 클래스
/// </summary>
public class HpManager : MonoBehaviour
{
    // 최대 체력 (외부에서 설정 가능)
    public float maxHp { get; set; } = 100;

    private float _hp;

    // 체력 프로퍼티
    public float hp
    {
        get { return _hp; }
        set
        {
            _hp = Mathf.Clamp(value, 0f, maxHp);
            // hp 변경 이벤트
            OnHpChanged?.Invoke(_hp, maxHp);

            // 체력이 0 이하이면 사망 처리
            if (_hp <= 0) Die();
        }
    }

    // 체력 변경 이벤트
    public delegate void OnHpChangedEvent(float hp, float maxhp);
    public event OnHpChangedEvent OnHpChanged;

    private MovementStateManager movementStateManager;
    private KillManager killManager;

    // 사망 이벤트
    public event Action OnDeath;

    // 피격 이벤트
    public event Action OnDamaged;

    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        movementStateManager = GetComponent<MovementStateManager>();
        killManager = GetComponent<KillManager>();
    }

    void Start()
    {
        // 로컬 플레이어만 UI 리스너 등록
        if (pv.IsMine)
        {
            var HpListeners = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
                .OfType<IHpListener>();

            foreach (var listener in HpListeners)
                OnHpChanged += listener.OnHpChanged;

            var DeathListeners = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
                .OfType<IDeathListener>();

            foreach (var listener in DeathListeners)
                OnDeath += listener.OnDeath;
        }

        // 피격 시 애니메이션/사운드
        OnDamaged += movementStateManager.OnDamaged;

        hp = maxHp;
    }

    void Update()
    {
        // 탈출 상태 체크 
        if (pv.IsMine)
        {
            if (GameManager.Instance.isEscape)
            {
                Escape();
                Debug.Log("탈출 성공");
            }
        }
    }

    /// <summary>
    /// 회복 함수
    /// </summary>
    /// <param name="recovery"></param>
    public void Recover(float recovery)
    {
        if (pv.IsMine)
        {
            if (hp > 0)
                hp += recovery;
        }
    }

    // 데미지 처리 RPC
    [PunRPC]
    private void RpcOnDamage(float damage, int attakActorNum)
    {
        hp -= damage;

        // 데미지 이벤트 실행
        OnDamaged?.Invoke();

        // 사망 시 공격자 킬 증가
        if (hp <= 0)
        {
            KillManager attackerKM =
                GameManager.Instance.ActorDict[attakActorNum]
                .GetComponent<KillManager>();

            attackerKM.AddKillCount();
        }
    }

    /// <summary>
    /// 공격자가 외부에서 데미지 호출하는 RPC 함수
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attakActorNum"></param>
    public void OnDamage(float damage, int attakActorNum)
    {
        pv.RPC(nameof(RpcOnDamage), RpcTarget.Owner, damage, attakActorNum);
    }


    // 사망 처리 RPC
    [PunRPC]
    private void RpcDie()
    {
        if (pv.IsMine)
        {
            OnDeath?.Invoke(); // UI 처리
        }
        // 다른 클라의 내 플레이어는 비활성화만
        GameManager.Instance.curPlayers -= 1;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 사망 처리 RPC 함수
    /// </summary>
    public void Die() =>
        pv.RPC(nameof(RpcDie), RpcTarget.AllViaServer);


    // 탈출 처리 RPC
    [PunRPC]
    private void RpcEscape()
    {
        if (pv.IsMine)
        {
            OnDeath?.Invoke(); // UI 실행
        }

        // 모든 플레이어 죽이기
        for (int i = 0; i < GameManager.Instance.playerObjects.Length; i++)
        {
            HpManager hpManager =
                GameManager.Instance.playerObjects[i].GetComponent<HpManager>();

            hpManager.hp = 0;
        }
    }

    /// <summary>
    /// 탈출 처리 RPC 함수
    /// </summary>
    public void Escape() =>
        pv.RPC(nameof(RpcEscape), RpcTarget.AllViaServer);
}