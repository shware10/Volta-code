using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 앉기 상태
/// </summary>
public class CrouchState : MovementBaseState
{
    // 상태 진입 시 호출
    public override void EnterState(MovementStateManager movement)
    {
        // Crouch 애니메이션 활성화
        movement.anim.SetBool("Crouching", true);
    }

    // 매 프레임 상태 업데이트
    public override void UpdateState(MovementStateManager movement)
    {
        // 점프 중이 아닐 때만 상태 전이 처리
        if (!movement.jumped)
        {
            // C키 다시 누르거나 Space 입력 시 앉기 해제
            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Space))
            {
                // 이동 중이면 Walk 또는 Run 상태로 복귀
                if (movement.moveDir.magnitude > 0.1f)
                {
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
                else // 이동 없으면 Idle로 복귀
                {
                    ExitState(movement, movement.Idle);
                    return;
                }
            }

            // 앉기 이동 속도
            movement.currentMoveSpeed = movement.crouchSpeed;
            return;
        }
    }

    // 상태 종료 및 다음 상태로 전환
    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        // Crouch 애니메이션 비활성화
        movement.anim.SetBool("Crouching", false);

        // 상태 전환
        movement.SwitchState(state);
    }
}