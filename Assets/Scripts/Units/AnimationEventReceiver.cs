using UnityEngine;

namespace GameArchitecture.Units
{
    /// <summary>
    /// 动画事件接收器：挂载在Animator所在节点，转发事件到父节点的StateMachine
    /// 通用设计：所有动画事件统一通过 OnAnimationEvent(string eventName) 处理
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
        /// 通用动画事件处理入口
        /// 在Unity动画编辑器中，可以添加事件并传入字符串参数来触发不同行为
        /// 例如：OnAnimationEvent("WindowOpen")、OnAnimationEvent("SkillEnd") 等
        /// 字符串会自动转换为类型安全的枚举
        /// </summary>
        /// <param name="eventName">事件名称，由动画编辑器传入</param>
        public void OnAnimationEvent(string eventName)
        {
            // 尝试将字符串转换为枚举（忽略大小写）
            if (System.Enum.TryParse<PlayerStates.AnimationEventType>(eventName, true, out var eventType))
            {
                stateMachine?.OnAnimationEvent(eventType);
            }
            else
            {
                Debug.LogWarning($"AnimationEventReceiver: Unknown event name '{eventName}'. Please use one of the AnimationEventType enum values.");
            }
        }
    }
}



