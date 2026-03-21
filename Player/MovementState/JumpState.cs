using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 점프 상태
/// </summary>
public class JumpState : MovementBaseState
{
    // 상태 진입 시 호출
    public override void EnterState(MovementStateManager movement)
    {
        // 공중 상태에서 중복 트리거 방지하기 위해 이전 상태가 지상 상태일 때만 점프 애니메이션 트리거 실행
        if (movement.previousState == movement.Run ||
            movement.previousState == movement.Walk ||
            movement.previousState == movement.Idle)
        {
            movement.anim.SetTrigger("Jump");
        }

        // 점프 시작 사운드 재생
        movement.audioState((int)AudioManager.Sfx.SFX_move_jumpstart);
    }

    // 매 프레임 상태 업데이트
    public override void UpdateState(MovementStateManager movement)
    {
        // 점프 상태이고, 착지했을 경우
        if (movement.jumped == true && movement.IsGrounded())
        {
            // 점프 상태 해제
            movement.jumped = false;

            // 착지 사운드 재생
            movement.audioState((int)AudioManager.Sfx.SFX_move_jumpend);

            // 이동 입력이 있는 경우
            if (movement.moveDir.magnitude > 0.1f)
            {
                // Shift 키 입력 여부에 따라 Run 또는 Walk로 전이
                if (Input.GetKey(KeyCode.LeftShift))
                    movement.SwitchState(movement.Run);
                else
                    movement.SwitchState(movement.Walk);
            }
            else
            {
                // 이동 입력 없으면 Idle로 전이
                movement.SwitchState(movement.Idle);
            }
        }
    }
}