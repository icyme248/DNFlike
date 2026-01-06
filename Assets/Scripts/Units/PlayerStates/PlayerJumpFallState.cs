using UnityEngine;
using QFramework;

namespace GameArchitecture.Units.PlayerStates
{
    /// <summary>
    /// 跳跃下落状态
    /// </summary>
    public class PlayerJumpFallState : AbstractState<PlayerStateType, PlayerStateMachine>
    {
        public PlayerJumpFallState(FSM<PlayerStateType> fsm, PlayerStateMachine target) : base(fsm, target) { }

        protected override void OnEnter()
        {
            mOwner.PlayAnimation("Fall");
        }

        protected override void OnUpdate()
        {
            // 下落攻击检测
            if (mOwner.inputSystem.GetAttackKeyDown())
            {
                mFSM.ChangeState(PlayerStateType.JumpAttack);
                return;
            }

            // 着地检测
            if (mOwner.isGrounded)
            {
                mFSM.ChangeState(PlayerStateType.Idle);
            }
        }

        protected override void OnFixedUpdate()
        {
            mOwner.CheckGrounded();

            // 空中移动控制
            if (mOwner.inputSystem.HasMoveInput())
            {
                Vector2 moveVector = mOwner.inputSystem.GetInputVector();
                mOwner.AirMove(moveVector);
                mOwner.FlipSprite(moveVector.x);
            }
            else
            {
                // 无输入时停止水平移动
                mOwner.StopMoving();
            }
        }
    }
}


