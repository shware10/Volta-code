using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 걷기 상태
/// </summary>
public class WalkState : MovementBaseState
{
    // 상태 진입 시 호출
    public override void EnterState(MovementStateManager movement)
    {
        // 걷기 사운드 재생
        movement.audioState((int)AudioManager.Sfx.SFX_move_walk);

        // 기본 이동 속도로 설정
        movement.currentMoveSpeed = movement.walkSpeed;

        // 애니메이터 Walking 상태 활성화
        movement.anim.SetBool("Walking", true);
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

            // Shift 입력 시 Run 상태로 전이
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                ExitState(movement, movement.Run);
                return;
            }
            // 앉기 입력 시 Crouch 상태로 전이
            else if (Input.GetKeyDown(KeyCode.C))
            {
                ExitState(movement, movement.Crouch);
                return;
            }
            // 이동 입력이 거의 없으면 Idle 상태로 전이
            else if (movement.moveDir.magnitude < 0.1f)
            {
                ExitState(movement, movement.Idle);
                return;
            }

            // 뒤로 이동 시 느린 속도, 앞으로 이동 시 기본 속도
            if (movement.zAxis < 0)
                movement.currentMoveSpeed = movement.walkBackSpeed;
            else
                movement.currentMoveSpeed = movement.walkSpeed;
        }
    }

    // 상태 종료 및 다음 상태로 전환
    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        // 걷기 사운드 정지
        AudioManager.instance.StopSfx(AudioManager.Sfx.SFX_move_walk);

        // 애니메이터 Walking 비활성화
        movement.anim.SetBool("Walking", false);

        // 상태 전환
        movement.SwitchState(state);
    }
}