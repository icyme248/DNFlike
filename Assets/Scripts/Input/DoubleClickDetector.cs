using UnityEngine;
using QFramework;

namespace GameArchitecture.Utilities
{
    /// <summary>
    /// 双击检测工具
    /// 位于Utility层,提供纯粹的双击检测算法
    /// 无业务逻辑,可复用于任何需要双击检测的场景
    /// </summary>
    public class DoubleClickDetector : IUtility
    {
        /// <summary>
        /// 双击检测的时间阈值(秒)
        /// </summary>
        public float DoubleClickThreshold { get; set; }

        /// <summary>
        /// 单个按钮的双击状态
        /// </summary>
        private class ButtonState
        {
            public float lastClickTime;
            public bool isDoubleClick;
        }

        private System.Collections.Generic.Dictionary<string, ButtonState> buttonStates;

        public DoubleClickDetector(float threshold = 0.3f)
        {
            DoubleClickThreshold = threshold;
            buttonStates = new System.Collections.Generic.Dictionary<string, ButtonState>();
        }

        /// <summary>
        /// 检测按钮是否双击
        /// </summary>
        /// <param name="buttonName">按钮名称</param>
        /// <param name="isPressed">当前帧按钮是否按下</param>
        /// <returns>是否触发双击</returns>
        public bool DetectDoubleClick(string buttonName, bool isPressed)
        {
            if (!buttonStates.ContainsKey(buttonName))
            {
                buttonStates[buttonName] = new ButtonState();
            }

            ButtonState state = buttonStates[buttonName];
            state.isDoubleClick = false;

            if (isPressed)
            {
                float currentTime = Time.time;
                float timeSinceLastClick = currentTime - state.lastClickTime;

                if (timeSinceLastClick <= DoubleClickThreshold && timeSinceLastClick > 0)
                {
                    state.isDoubleClick = true;
                    state.lastClickTime = 0; // 重置,避免连续触发
                }
                else
                {
                    state.lastClickTime = currentTime;
                }
            }

            return state.isDoubleClick;
        }

        /// <summary>
        /// 重置指定按钮的状态
        /// </summary>
        public void ResetButton(string buttonName)
        {
            if (buttonStates.ContainsKey(buttonName))
            {
                buttonStates[buttonName].lastClickTime = 0;
                buttonStates[buttonName].isDoubleClick = false;
            }
        }

        /// <summary>
        /// 重置所有按钮状态
        /// </summary>
        public void ResetAll()
        {
            buttonStates.Clear();
        }
    }
}

