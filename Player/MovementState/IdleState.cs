using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대기 상태
/// </summary>
public class IdleState : MovementBaseState
{
    // 상태 진입 시 호출
    public override void EnterState(MovementStateManager movement)
    {
        // Idle 애니메이션 활성화
        movement.anim.SetBool("Idle", true);
    }

    // 매 프레임 상태 업데이트
    public override void UpdateState(MovementStateManager movement)
    {
        // 점프 중이 아닐 때만 상태 전이 처리
        if (!movement.jumped)
        {
            // 점프 입력 + 스태미너 충분할 때만 Jump 상태로 전이
            if (Input.GetKeyDown(KeyCode.Space) &&
                movement.staminaManager.stamina >= movement.staminaManager.jumpAmount)
            {
                // 착지 후 복귀를 위한 이전 상태 저장
                movement.previousState = this;

                ExitState(movement, movement.Jump);
                return;
            }

            // 이동 입력이 있을 경우
            if (movement.moveDir.magnitude > 0.1f)
            {
                // Shift 누르고 있으면 Run, 아니면 Walk
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    ExitState(movement, movement.Run);
                    return;
                }
                else
                {
                    ExitState(movement, movement.Walk);
                    return;
                }
            }

            // 앉기 입력 시 Crouch 상태로 전이
            if (Input.GetKeyDown(KeyCode.C))
            {
                ExitState(movement, movement.Crouch);
                return;
            }
        }
    }

    // 상태 종료 및 다음 상태로 전환
    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        // Idle 애니메이션 비활성화
        movement.anim.SetBool("Idle", false);

        // 상태 전환
        movement.SwitchState(state);
    }
}