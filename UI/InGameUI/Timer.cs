using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

// 게임 상태 변화를 받아서 타이머를 실행하는 클래스
public class Timer : MonoBehaviour, IGameStateListener
{
    private PhotonView pv;     // RPC 호출을 위한 PhotonView

    public int startTime = 10; // 카운트다운 시작 시간

    [SerializeField]
    private TextMeshProUGUI countDownTMP; // 카운트 다운 텍스트

    // 타이머가 0이 되었을 때 호출되는 이벤트
    public delegate void OnZeroEvent();
    public static event OnZeroEvent OnZero;

    // 게임 상태 변경 시 호출됨
    public void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Ready:
                // Ready 상태가 되면 카운트다운 시작
                StartCountDown();
                break;
        }
    }

    private void Awake()
    {
        // PhotonView 컴포넌트 가져오기 (RPC 사용 위해 필요)
        pv = GetComponent<PhotonView>();

        // 같은 오브젝트에 붙어있는 TMP 가져오기
        countDownTMP = GetComponent<TextMeshProUGUI>();
    }

    // 카운트다운 시작 함수
    public void StartCountDown()
    {
        // 마스터 클라이언트만 RPC를 실행하도록 제한
        if (PhotonNetwork.IsMasterClient)
        {
            // 모든 클라이언트에게 CountDown 함수 실행 요청
            pv.RPC(nameof(CountDown), RpcTarget.AllViaServer, startTime);
        }
    }

    // RPC로 호출되는 함수 (모든 클라이언트에서 실행됨)
    [PunRPC]
    void CountDown(int startTime)
    {
        // 코루틴 실행해서 시간 감소 처리
        StartCoroutine(ShowLoop(startTime));
    }

    // 실제 카운트다운 로직
    IEnumerator ShowLoop(int startTime)
    {
        int time = startTime;

        // 시간이 0보다 크면 반복
        while (time > 0)
        {
            // UI 텍스트 갱신
            countDownTMP.SetText($"{time}");

            // 1초 대기
            yield return new WaitForSeconds(1f);

            // 시간 감소
            --time;
        }

        // 카운트다운 끝나면 오브젝트 비활성화
        gameObject.SetActive(false);

        // 0초 이벤트 호출
        OnZero?.Invoke();
    }
}