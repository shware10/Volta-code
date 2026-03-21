using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Unity.VisualScripting;

/// <summary>
/// 플레이어의 시점 회전을 관리하는 클래스
/// </summary>
public class AimStateManager : MonoBehaviourPun
{
    // Cinemachine에서 사용하는 입력 축 상태 (마우스 X/Y)
    public Cinemachine.AxisState xAxis, yAxis;

    // 카메라가 따라갈 위치
    [SerializeField] Transform camFollowPos;

    // UI 상태를 확인할 매니저
    [SerializeField] UIManager uiManager;

    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        // 씬에서 UIManager 찾아서 참조
        uiManager = FindObjectOfType<UIManager>();

        // 로컬 플레이어만 카메라 설정
        if (pv.IsMine)
        {
            // 현재 씬의 Cinemachine 카메라 가져오기
            var followCam = FindObjectOfType<CinemachineVirtualCamera>();

            // 카메라가 따라갈 대상 설정
            followCam.Follow = this.camFollowPos.transform;

            // 카메라가 바라볼 대상 설정
            followCam.LookAt = this.camFollowPos.transform;
        }
    }

    void Update()
    {
        // 로컬 플레이어 + UI가 활성화되지 않았을 때만 입력 처리
        if (pv.IsMine && uiManager.isUIActivate == false)
        {
            // 마우스 입력 기반으로 Axis 값 업데이트
            xAxis.Update(Time.deltaTime);
            yAxis.Update(Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        // 로컬 플레이어만 회전 적용
        if (pv.IsMine)
        {
            // 카메라 Pivot
            camFollowPos.localEulerAngles = new Vector3(
                -yAxis.Value,                             // 위아래 회전
                camFollowPos.localEulerAngles.y,
                camFollowPos.localEulerAngles.z
            );

            // 플레이어 몸체 회전
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                xAxis.Value,
                transform.eulerAngles.z
            );
        }
    }
}