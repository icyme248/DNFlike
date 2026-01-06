using UnityEngine;

namespace GameArchitecture.Units
{
    /// <summary>
    /// 动画事件接收器：挂载在Animator所在节点，转发事件到父节点的StateMachine
    /// </summary>
    public class AnimationEventReceiver : MonoBehaviour
    {
        private PlayerStateMachine stateMachine;

        private void Awake()
        {
            // 获取父节点的状态机
            stateMachine = GetComponentInParent<PlayerStateMachine>();
            
            if (stateMachine == null)
            {
                Debug.LogError($"AnimationEventReceiver on {gameObject.name} cannot find PlayerStateMachine in parent!");
            }
        }

        /// <summary>
        /// 动画事件：连招窗口期开启
        /// </summary>
        public void OnComboWindowOpen()
        {
            stateMachine?.OnComboWindowOpen();
        }

        /// <summary>
        /// 动画事件：连招窗口期关闭
        /// </summary>
        public void OnComboWindowClose()
        {
            stateMachine?.OnComboWindowClose();
        }

        /// <summary>
        /// 动画事件：攻击动画结束
        /// </summary>
        public void OnAttackAnimationEnd()
        {
            stateMachine?.OnAttackAnimationEnd();
        }

        /// <summary>
        /// 动画事件：下落攻击结束
        /// </summary>
        public void OnJumpAttackEnd()
        {
            stateMachine?.OnJumpAttackEnd();
        }
    }
}



