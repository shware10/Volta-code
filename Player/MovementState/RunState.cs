using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 달리기 상태
/// </summary>
public class RunState : MovementBaseState
{
    // 상태 진입 시 호출
    public override void EnterState(MovementStateManager movement)
    {
        // 달리기 사운드 재생
        movement.audioState((int)AudioManager.Sfx.SFX_move_run);

        // 애니메이터에 Running 상태 활성화
        movement.anim.SetBool("Running", true);
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

            // Shift 키를 뗐거나 스태미너가 0이면 Walk 상태로 전이
            if (Input.GetKeyUp(KeyCode.LeftShift) ||
                movement.staminaManager.stamina <= 0)
            {
                ExitState(movement, movement.Walk);
                return;
            }
            // 이동 입력이 거의 없으면 Idle 상태로 전이
            else if (movement.moveDir.magnitude < 0.1f)
            {
                ExitState(movement, movement.Idle);
                return;
            }

            // 앉기 입력 시 Crouch 상태로 전이
            if (Input.GetKeyDown(KeyCode.C))
            {
                ExitState(movement, movement.Crouch);
                return;
            }

            // 뒤로 이동 시 속도 감소, 앞으로 이동 시 기본 달리기 속도
            if (movement.zAxis < 0)
                movement.currentMoveSpeed = movement.runBackSpeed;
            else
                movement.currentMoveSpeed = movement.runSpeed;
        }
    }

    // 상태 종료 및 다음 상태로 전환
    public void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        // 달리기 사운드 정지
        AudioManager.instance.StopSfx(AudioManager.Sfx.SFX_move_run);

        // 애니메이터 Running 비활성화
        movement.anim.SetBool("Running", false);

        // 상태 전환
        movement.SwitchState(state);
    }
}