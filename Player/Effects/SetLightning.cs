using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 조합 시 라이트닝 파티클 on/off 관리 클래스
/// </summary>
public class SetLightning : MonoBehaviour
{

    private void OnEnable()
    {
        // 라이트닝 파티클 플레이
        gameObject.GetComponentsInChildren<ParticleSystem>()[0].Play();
        gameObject.GetComponentsInChildren<ParticleSystem>()[1].Play();
    }

    private void OnDisable()
    {
        // 라이트닝 파티클 스톱
        gameObject.GetComponentsInChildren<ParticleSystem>()[0].Stop();
        gameObject.GetComponentsInChildren<ParticleSystem>()[1].Stop();
    }

}
