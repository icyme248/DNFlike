using QFramework;
using GameArchitecture.Systems;
using GameArchitecture.Utilities;

namespace GameArchitecture
{
    /// <summary>
    /// 游戏架构核心
    /// 负责注册和管理所有的System、Model和Utility
    /// </summary>
    public class GameApp : Architecture<GameApp>
    {
        protected override void Init()
        {
            // 注册工具层 - 基础设施
            this.RegisterUtility(new InputActionUtility());
            this.RegisterUtility(new DoubleClickDetector(0.3f));
            
            // 注册系统层 - 业务逻辑
            this.RegisterSystem(new GameInputSystem());
            
            // 可以继续注册其他System和Model
            // this.RegisterModel(new PlayerModel());
            // this.RegisterSystem(new CombatSystem());
        }
    }
}

