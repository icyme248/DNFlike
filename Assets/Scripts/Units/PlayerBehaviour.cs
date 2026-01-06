using UnityEngine;
using QFramework;
using GameArchitecture.Systems;

namespace GameArchitecture.Units
{
    /// <summary>
    /// 玩家行为基类 - 玩家特有的行为
    /// 继承UnitBehaviour，添加玩家专用功能（如输入系统）
    /// </summary>
    public class PlayerBehaviour : UnitBehaviour
    {
        // 输入系统（玩家专用，供子类和State访问）
        public GameInputSystem inputSystem { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void Start()
        {
            // 获取输入系统
            inputSystem = this.GetSystem<GameInputSystem>();
        }
    }
}





