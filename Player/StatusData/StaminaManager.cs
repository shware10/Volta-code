using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

/// <summary>
/// 플레이어 스테미너 관리 클래스
/// </summary>
public class StaminaManager : MonoBehaviour, IPlayerStateListener
{
    private float maxStamina = 100f;

    private float _stamina;

    // 스테미나 프로퍼티
    public float stamina
    {
        get { return _stamina; }
        set
        {
            // 값 변경 시 자동 clamp
            _stamina = Mathf.Clamp(value, 0f, maxStamina);

            // UI 리스너에게 변경 알림
            OnStaminaChanged?.Invoke(_stamina, maxStamina);
        }
    }

    // 스테미나 변경 이벤트
    public delegate void OnStaminaChangedEvent(float stamina, float maxStamina);
    public event OnStaminaChangedEvent OnStaminaChanged;

    [Header("스테미너 증가/감소 량")]
    public float jumpAmount = 20f;                      // 점프 시 즉시 감소량
    [SerializeField] private float runAmount = -15f;    // 달릴 때 초당 감소량
    [SerializeField] private float recoverAmount = 15f; // 회복량

    private Coroutine ChangeRoutine;

    private MovementStateManager movement;
    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        movement = GetComponent<MovementStateManager>();
    }

    void Start()
    {
        // 로컬 플레이어만 UI 이벤트 연결
        if (pv.IsMine)
        {
            var SListeners = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
                .OfType<IStaminaListener>();

            foreach (var listener in SListeners)
                OnStaminaChanged += listener.OnStaminaChanged;
        }

        stamina = maxStamina; // 초기값 설정
    }

    // 이벤트 해제
    void OnDestroy()
    {
        var SListeners = FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None)
            .OfType<IStaminaListener>();

        foreach (var listener in SListeners)
            OnStaminaChanged -= listener.OnStaminaChanged;
    }

    // 플레이어 상태 변경 시 호출
    public void OnPlayerStateChanged(MovementBaseState curState)
    {
        // 기존 코루틴 중지
        if (ChangeRoutine != null)
            StopCoroutine(ChangeRoutine);

        // 상태별 처리
        if (curState == movement.Run)
        {
            // 달리기 상태 시 지속 감소
            ChangeRoutine = StartCoroutine(ChangeStamina(runAmount));
        }
        else if (curState == movement.Jump)
        {
            // 점프 시 즉시 감소
            stamina -= jumpAmount;
            ChangeRoutine = StartCoroutine(ChangeStamina(recoverAmount));
        }
        else
        {
            // 기본 상태시 회복
            ChangeRoutine = StartCoroutine(ChangeStamina(recoverAmount));
        }
    }

    /// <summary>
    /// 스테미나 변경 코루틴
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    IEnumerator ChangeStamina(float amount)
    {
        // 감소
        if (amount < 0)
        {
            while (stamina > 0)
            {
                stamina += amount * Time.deltaTime;
                yield return null;
            }
        }
        // 회복
        else
        {
            while (stamina < maxStamina)
            {
                stamina += amount * Time.deltaTime;
                yield return null;
            }
        }
    }


}