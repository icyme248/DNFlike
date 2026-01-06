using UnityEngine;
using QFramework;

namespace GameArchitecture.Units.PlayerStates
{
    /// <summary>
    /// 攻击状态 - 支持三段连招
    /// </summary>
    public class PlayerAttackState : AbstractState<PlayerStateType, PlayerStateMachine>
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
            // 窗口期内检测攻击输入（支持重新按下或长按）
            if (comboWindowOpen && !comboInputBuffered)
            {
                if (mOwner.inputSystem.GetAttackKeyDown() || mOwner.inputSystem.IsAttackPressed())
                {
                    comboInputBuffered = true;
                }
            }
        }

        protected override void OnFixedUpdate()
        {
            // 攻击时的移动逻辑
            Vector2 inputVector = mOwner.inputSystem.GetInputVector();
            mOwner.AttackMove(inputVector.x);
        }

        #region 动画事件处理

        /// <summary>
        /// 动画事件：连招窗口期开启
        /// </summary>
        public void OnComboWindowOpen()
        {
            comboWindowOpen = true;
            comboInputBuffered = false;
        }

        /// <summary>
        /// 动画事件：连招窗口期关闭
        /// </summary>
        public void OnComboWindowClose()
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

        /// <summary>
        /// 动画事件：攻击动画结束（第三段攻击）
        /// </summary>
        public void OnAttackAnimationEnd()
        {
            mFSM.ChangeState(PlayerStateType.Idle);
        }

        #endregion
    }
}

