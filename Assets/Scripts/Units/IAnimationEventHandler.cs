namespace GameArchitecture.Units.PlayerStates
{
    /// <summary>
    /// 动画事件类型枚举
    /// </summary>
    public enum AnimationEventType
    {
        /// <summary>技能窗口期开启（可接受输入）</summary>
        WindowOpen,
        
        /// <summary>技能窗口期关闭</summary>
        WindowClose,
        
        /// <summary>技能/动画结束</summary>
        SkillEnd,
        
        /// <summary>伤害判定帧</summary>
        DamageFrame,
        
        /// <summary>无敌帧开始</summary>
        InvincibleStart,
        
        /// <summary>无敌帧结束</summary>
        InvincibleEnd,
        
        /// <summary>生成特效</summary>
        SpawnEffect,
        
        /// <summary>播放音效</summary>
        PlaySound
    }

    /// <summary>
    /// 动画事件处理接口
    /// 状态实现此接口以响应动画事件
    /// </summary>
    public interface IAnimationEventHandler
    {
        /// <summary>
        /// 处理动画事件
        /// </summary>
        /// <param name="eventType">事件类型（类型安全的枚举）</param>
        void HandleAnimationEvent(AnimationEventType eventType);
    }
}

