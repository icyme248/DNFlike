using UnityEngine;
using QFramework;

namespace GameArchitecture.Units.PlayerStates
{
    /// <summary>
    /// 奔跑状态
    /// </summary>
    public class PlayerRunState : AbstractState<PlayerStateType, PlayerStateMachine>
    {
        public PlayerRunState(FSM<PlayerStateType> fsm, PlayerStateMachine target) : base(fsm, target) { }

        protected override void OnEnter()
        {
            mOwner.PlayAnimation("Run");
        }

        protected override void OnUpdate()
        {
            // 掉落检测
            if (!mOwner.isGrounded && mOwner.rb != null && mOwner.rb.velocity.y < -0.1f)
            {
                mFSM.ChangeState(PlayerStateType.JumpFall);
                return;
            }

            // 输入检测
            if (mOwner.inputSystem.GetJumpKeyDown())
            {
                mFSM.ChangeState(PlayerStateType.JumpUp);
                return;
            }

            if (mOwner.inputSystem.GetAttackKeyDown())
            {
                mFSM.ChangeState(PlayerStateType.Attack);
                return;
            }

            if (!mOwner.inputSystem.HasMoveInput())
            {
                mFSM.ChangeState(PlayerStateType.Idle);
                return;
            }
        }

        protected override void OnFixedUpdate()
        {
            mOwner.CheckGrounded();

            Vector2 moveVector = mOwner.inputSystem.GetInputVector();
            mOwner.Move(moveVector, mOwner.moveSpeed);
            mOwner.FlipSprite(moveVector.x);
        }

        protected override void OnExit()
        {
            mOwner.StopMoving();
        }
    }
}

