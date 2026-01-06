using UnityEngine;
using UnityEngine.InputSystem;
using QFramework;

namespace GameArchitecture.Utilities
{
    /// <summary>
    /// 输入操作工具类
    /// 位于Utility层,负责封装Unity InputSystem的底层API
    /// 不包含业务逻辑,只是对Unity InputSystem的简单封装
    /// </summary>
    public class InputActionUtility : IUtility
    {
        private PlayerControls playerInput;
        private InputAction moveAction;
        private InputAction attackAction;
        private InputAction defendAction;
        private InputAction grabAction;
        private InputAction jumpAction;

        /// <summary>
        /// 初始化输入系统
        /// </summary>
        public void Initialize()
        {
            if (playerInput != null)
            {
                Debug.LogWarning("InputActionUtility already initialized!");
                return;
            }

            playerInput = new PlayerControls();
            
            // 获取各个InputAction的引用
            moveAction = playerInput.Player.Move;
            attackAction = playerInput.Player.Attack;
            defendAction = playerInput.Player.Defend;
            grabAction = playerInput.Player.Grab;
            jumpAction = playerInput.Player.Jump;
        }

        /// <summary>
        /// 启用输入
        /// </summary>
        public void Enable()
        {
            playerInput?.Enable();
        }

        /// <summary>
        /// 禁用输入
        /// </summary>
        public void Disable()
        {
            playerInput?.Disable();
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Cleanup()
        {
            if (playerInput != null)
            {
                playerInput.Disable();
                playerInput.Dispose();
                playerInput = null;
            }
        }

        // ========== 获取输入状态的基础方法 ==========

        public bool IsAttackPressed() => attackAction?.IsPressed() ?? false;
        public bool IsAttackTriggered() => attackAction?.triggered ?? false;
        
        public bool IsDefendPressed() => defendAction?.IsPressed() ?? false;
        public bool IsDefendTriggered() => defendAction?.triggered ?? false;
        
        public bool IsGrabPressed() => grabAction?.IsPressed() ?? false;
        public bool IsGrabTriggered() => grabAction?.triggered ?? false;
        
        public bool IsJumpPressed() => jumpAction?.IsPressed() ?? false;
        public bool IsJumpTriggered() => jumpAction?.triggered ?? false;

        public Vector2 GetMoveVector() => moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
    }
}

