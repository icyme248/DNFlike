using QFramework;

namespace GameArchitecture.Units.PlayerStates
{
    /// <summary>
    /// 待机状态
    /// </summary>
    public class PlayerIdleState : AbstractState<PlayerStateType, PlayerStateMachine>
    {
        public PlayerIdleState(FSM<PlayerStateType> fsm, PlayerStateMachine target) : base(fsm, target) { }

        protected override void OnEnter()
        {
            mOwner.PlayAnimation("Idle");
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

            if (mOwner.inputSystem.HasMoveInput())
            {
                mFSM.ChangeState(PlayerStateType.Run);
            }
        }

        protected override void OnFixedUpdate()
        {
            mOwner.CheckGrounded();
            mOwner.StopMoving();
        }

    }
}

