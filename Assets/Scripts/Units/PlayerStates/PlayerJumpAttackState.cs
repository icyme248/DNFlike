using UnityEngine;
using QFramework;

namespace GameArchitecture.Units.PlayerStates
{
    /// <summary>
    /// 空中下落攻击状态
    /// </summary>
    public class PlayerJumpAttackState : AbstractState<PlayerStateType, PlayerStateMachine>, IAnimationEventHandler
    {
        private float stateEnterTime;
        private const float MIN_STATE_DURATION = 1.0f; // 最小持续时间（保证动画完整性）

        public PlayerJumpAttackState(FSM<PlayerStateType> fsm, PlayerStateMachine target) : base(fsm, target) { }

        protected override void OnEnter()
        {
            mOwner.PlayAnimation("JumpAttack");
            mOwner.FastFall();
            stateEnterTime = Time.time;
        }

        protected override void OnUpdate()
        {
            // 进入状态后一段时间内不检测着地，防止动画被提前打断
            float elapsed = Time.time - stateEnterTime;
            if (elapsed < MIN_STATE_DURATION)
                return;

            // 超过最小持续时间后，着地则结束（保底机制）
            if (mOwner.isGrounded)
            {
                mFSM.ChangeState(PlayerStateType.Idle);
            }
        }

        protected override void OnFixedUpdate()
        {
            mOwner.CheckGrounded();
            
            // 根据方向键控制水平移动（传入速度倍率）
            Vector2 inputVector = mOwner.inputSystem.GetInputVector();
            mOwner.AttackMove(inputVector.x, mOwner.jumpAttackMoveSpeedMultiplier);
        }

        #region IAnimationEventHandler 实现

        /// <summary>
        /// 处理动画事件（通用接口）
        /// </summary>
        public void HandleAnimationEvent(AnimationEventType eventType)
        {
            switch (eventType)
            {
                case AnimationEventType.SkillEnd:
                    // 技能结束
                    mFSM.ChangeState(PlayerStateType.Idle);
                    break;

                default:
                    Debug.LogWarning($"PlayerJumpAttackState: Unhandled animation event '{eventType}'");
                    break;
            }
        }

        #endregion
    }
}

