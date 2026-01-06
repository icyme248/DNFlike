using UnityEngine;
using QFramework;

namespace GameArchitecture.Units.PlayerStates
{
    /// <summary>
    /// 跳跃上升状态
    /// </summary>
    public class PlayerJumpUpState : AbstractState<PlayerStateType, PlayerStateMachine>
    {
        public PlayerJumpUpState(FSM<PlayerStateType> fsm, PlayerStateMachine target) : base(fsm, target) { }

        protected override void OnEnter()
        {
            mOwner.PlayAnimation("jump");
            mOwner.CheckGrounded();
            mOwner.Jump();
        }

        protected override void OnUpdate()
        {
            // 切换到下落状态
            if (mOwner.rb != null && mOwner.rb.velocity.y < 0)
            {
                mFSM.ChangeState(PlayerStateType.JumpFall);
            }
        }

        protected override void OnFixedUpdate()
        {
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


