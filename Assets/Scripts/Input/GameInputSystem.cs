using UnityEngine;
using QFramework;
using GameArchitecture.Utilities;

namespace GameArchitecture.Systems
{
    /// <summary>
    /// 游戏输入系统
    /// 位于System层,负责处理游戏输入相关的业务逻辑
    /// 包括:基础输入查询、双击检测、按键持续时长统计等
    /// </summary>
    public class GameInputSystem : AbstractSystem
    {
        private InputActionUtility inputUtility;
        private DoubleClickDetector doubleClickDetector;

        // 按住时长追踪
        private float attackHoldStartTime;
        private bool isAttackHeld;

        protected override void OnInit()
        {
            // 从架构中获取工具层依赖
            inputUtility = this.GetUtility<InputActionUtility>();
            doubleClickDetector = this.GetUtility<DoubleClickDetector>();

            // 初始化输入工具
            inputUtility.Initialize();
            inputUtility.Enable();

            // 初始化状态
            attackHoldStartTime = 0f;
            isAttackHeld = false;

            Debug.Log("GameInputSystem initialized");
        }

        protected override void OnDeinit()
        {
            inputUtility?.Cleanup();
            doubleClickDetector?.ResetAll();
            Debug.Log("GameInputSystem deinitialized");
        }

        /// <summary>
        /// 每帧更新,处理需要持续追踪的输入状态
        /// 注意:这个方法需要在MonoBehaviour的Update中调用
        /// </summary>
        public void OnUpdate()
        {
            UpdateAttackHoldState();
        }

        // ========== 基础按钮输入查询 ==========

        public bool GetAttackKeyDown()
        {
            return inputUtility.IsAttackTriggered();
        }

        public bool IsAttackPressed()
        {
            return inputUtility.IsAttackPressed();
        }

        public bool GetDefendKeyDown()
        {
            return inputUtility.IsDefendTriggered();
        }

        public bool GetGrabKeyDown()
        {
            return inputUtility.IsGrabTriggered();
        }

        public bool GetJumpKeyDown()
        {
            return inputUtility.IsJumpTriggered();
        }

        // ========== 双击检测 ==========

        public bool IsAttackDoubleClick()
        {
            return doubleClickDetector.DetectDoubleClick("Attack", inputUtility.IsAttackTriggered());
        }

        public bool IsJumpDoubleClick()
        {
            return doubleClickDetector.DetectDoubleClick("Jump", inputUtility.IsJumpTriggered());
        }

        // ========== 按键持续时长 ==========

        private void UpdateAttackHoldState()
        {
            bool isAttackPressed = inputUtility.IsAttackPressed();

            if (isAttackPressed && !isAttackHeld)
            {
                // 刚开始按住
                isAttackHeld = true;
                attackHoldStartTime = Time.time;
            }
            else if (!isAttackPressed && isAttackHeld)
            {
                // 松开按键
                isAttackHeld = false;
                attackHoldStartTime = 0f;
            }
        }

        public bool IsAttackHeld()
        {
            return isAttackHeld;
        }

        public float GetAttackHoldDuration()
        {
            if (isAttackHeld)
            {
                return Time.time - attackHoldStartTime;
            }
            return 0f;
        }

        // ========== 移动输入 ==========

        public Vector2 GetInputVector()
        {
            return inputUtility.GetMoveVector();
        }

        public bool HasMoveInput()
        {
            Vector2 moveVec = inputUtility.GetMoveVector();
            return moveVec.sqrMagnitude > 0.01f;
        }

        // ========== 系统控制 ==========

        public void EnableInput()
        {
            inputUtility.Enable();
        }

        public void DisableInput()
        {
            inputUtility.Disable();
        }
    }
}

