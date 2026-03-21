using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 상태를 업 캐스팅으로 관리하기 위한 추상 클래스
/// </summary>
public abstract class MovementBaseState
{
    public abstract void EnterState(MovementStateManager movement);

    public abstract void UpdateState(MovementStateManager movement);
}
