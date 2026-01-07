using UnityEngine;
using QFramework;

namespace GameArchitecture.Units.PlayerStates
{
    /// <summary>
    /// 攻击状态 - 支持三段连招
    /// </summary>
    public class PlayerAttackState : AbstractState<PlayerStateType, PlayerStateMachine>, IAnimationEventHandler
    {
        private readonly string[] comboAnimations = { "Attack1", "Attack2", "Attack3" };
        
        private int currentComboIndex;
        private bool comboWindowOpen;
        private bool comboInputBuffered;

        public PlayerAttackState(FSM<PlayerStateType> fsm, PlayerStateMachine target) : base(fsm, target) { }

        protected override void OnEnter()
        {
            // 重置连招数据
            currentComboIndex = 0;
            comboWindowOpen = false;
            comboInputBuffered = false;
            
            // 播放第一段攻击动画
            mOwner.PlayAnimation(comboAnimations[0]);
        }

        protected override void OnUpdate()
        {
            // 窗口期内可以通过跳跃取消后摇
            if (comboWindowOpen)
            {
                // 优先检测跳跃输入（取消后摇）
                if (mOwner.inputSystem.GetJumpKeyDown())
                {
                    mFSM.ChangeState(PlayerStateType.JumpUp);
                    return;
                }

                // 检测攻击输入（继续连招）
                if (!comboInputBuffered)
                {
                    if (mOwner.inputSystem.GetAttackKeyDown() || mOwner.inputSystem.IsAttackPressed())
                    {
                        comboInputBuffered = true;
                    }
                }
            }
        }

        protected override void OnFixedUpdate()
        {
            // 攻击时的移动逻辑
            Vector2 inputVector = mOwner.inputSystem.GetInputVector();
            mOwner.AttackMove(inputVector.x);
        }

        #region IAnimationEventHandler 实现

        /// <summary>
        /// 处理动画事件（通用接口）
        /// </summary>
        public void HandleAnimationEvent(AnimationEventType eventType)
        {
            switch (eventType)
            {
                case AnimationEventType.WindowOpen:
                    // 技能窗口期开启（可接受输入）
                    comboWindowOpen = true;
                    comboInputBuffered = false;
                    break;

                case AnimationEventType.WindowClose:
                    // 技能窗口期关闭
                    HandleWindowClose();
                    break;

                case AnimationEventType.SkillEnd:
                    // 技能结束
                    mFSM.ChangeState(PlayerStateType.Idle);
                    break;

                default:
                    Debug.LogWarning($"PlayerAttackState: Unhandled animation event '{eventType}'");
                    break;
            }
        }

        /// <summary>
        /// 处理窗口关闭逻辑
        /// </summary>
        private void HandleWindowClose()
        {
            comboWindowOpen = false;

            // 如果窗口期内有输入且未到最后一段，进入下一段
            if (comboInputBuffered && currentComboIndex < comboAnimations.Length - 1)
            {
                currentComboIndex++;
                mOwner.PlayAnimation(comboAnimations[currentComboIndex]);
            }
            else
            {
                // 否则回到Idle
                mFSM.ChangeState(PlayerStateType.Idle);
            }
        }

        #endregion
    }
}

