using UnityEngine;
using QFramework;
using GameArchitecture.Units.PlayerStates;
using System.ComponentModel;

namespace GameArchitecture.Units
{
    /// <summary>
    /// 玩家状态机
    /// </summary>
    public class PlayerStateMachine : PlayerBehaviour
    {
        private FSM<PlayerStateType> fsm;

        public PlayerStateType curState;

        protected override void Start()
        {
            base.Start();

            // 先检测一次地面状态
            CheckGrounded();

            // 初始化状态机
            InitializeStateMachine();
        }

        void Update()
        {
            // 更新输入系统
            inputSystem?.OnUpdate();
            
            // 更新状态机
            fsm?.Update();
            curState = fsm.CurrentStateId;
        }

        void FixedUpdate()
        {
            fsm?.FixedUpdate();
        }

        /// <summary>
        /// 初始化状态机（可被子类重写以使用不同的状态）
        /// </summary>
        protected virtual void InitializeStateMachine()
        {
            fsm = new FSM<PlayerStateType>();
            fsm.AddState(PlayerStateType.Idle, new PlayerIdleState(fsm, this));
            fsm.AddState(PlayerStateType.Run, new PlayerRunState(fsm, this));
            fsm.AddState(PlayerStateType.Attack, new PlayerAttackState(fsm, this));
            fsm.AddState(PlayerStateType.JumpUp, new PlayerJumpUpState(fsm, this));
            fsm.AddState(PlayerStateType.JumpFall, new PlayerJumpFallState(fsm, this));
            fsm.AddState(PlayerStateType.JumpAttack, new PlayerJumpAttackState(fsm, this));
            
            // 根据是否在地面决定初始状态
            PlayerStateType startState = isGrounded ? PlayerStateType.Idle : PlayerStateType.JumpFall;
            fsm.StartState(startState);

            Debug.Log($"{gameObject.name} StateMachine initialized, isGrounded={isGrounded}, startState={startState}");
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState(PlayerStateType state)
        {
            fsm?.ChangeState(state);
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        public PlayerStateType GetCurrentState()
        {
            return fsm.CurrentStateId;
        }

        #region 动画事件回调

        /// <summary>
        /// 动画事件：连招窗口期开启（转发到AttackState）
        /// </summary>
        public void OnComboWindowOpen()
        {
            if (fsm.CurrentState is PlayerAttackState attackState)
            {
                attackState.OnComboWindowOpen();
            }
        }

        /// <summary>
        /// 动画事件：连招窗口期关闭（转发到AttackState）
        /// </summary>
        public void OnComboWindowClose()
        {
            if (fsm.CurrentState is PlayerAttackState attackState)
            {
                attackState.OnComboWindowClose();
            }
        }

        /// <summary>
        /// 动画事件：攻击动画结束（转发到AttackState）
        /// </summary>
        public void OnAttackAnimationEnd()
        {
            if (fsm.CurrentState is PlayerAttackState attackState)
            {
                attackState.OnAttackAnimationEnd();
            }
        }

        /// <summary>
        /// 动画事件：下落攻击结束（转发到JumpAttackState）
        /// </summary>
        public void OnJumpAttackEnd()
        {
            if (fsm.CurrentState is PlayerJumpAttackState jumpAttackState)
            {
                jumpAttackState.OnJumpAttackEnd();
            }
        }

        #endregion
    }

    /// <summary>
    /// 玩家状态类型枚举
    /// </summary>
    public enum PlayerStateType
    {
        Idle,
        Run,
        Attack,
        JumpUp,
        JumpFall,
        JumpAttack
    }
}

