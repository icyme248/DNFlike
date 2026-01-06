using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace BeatEmUpTemplate2D {

    //Input Manager for the Modern Unity Input System
    public class InputManager : MonoBehaviour {

        public static InputManager Instance { get; private set; }
        [Header("MODERN INPUTMANAGER. v1.0")]
        [ReadOnlyProperty] public string controlsScheme;
        public PlayerControls playerInput;
        private InputAction move;
        private InputAction punch;
        private InputAction kick;
        private InputAction defend;
        private InputAction grab;
        private InputAction jump;

        [Header("Double Click Settings")]
        [Tooltip("双击检测的最大时间间隔（秒）")]
        public float doubleClickThreshold = 0.3f;

        // 双击检测器
        private DoubleClickDetector punchDoubleClick;
        private DoubleClickDetector kickDoubleClick;
        private DoubleClickDetector jumpDoubleClick;

        // 按住时长追踪
        private float punchPressTime = 0f;
        private bool punchWasPressed = false;

        void Awake(){

            playerInput = new PlayerControls();
            controlsScheme = playerInput.ToString();

            //singleton pattern (only one InputManager allowed in a scene)
            if (Instance == null) {
                Instance = this;

            } else {
                Debug.Log("Multiple InputManagers found in this scene, there can be only one.");
                Destroy(gameObject);
            }

            // 初始化双击检测器
            punchDoubleClick = new DoubleClickDetector(doubleClickThreshold);
            kickDoubleClick = new DoubleClickDetector(doubleClickThreshold);
            jumpDoubleClick = new DoubleClickDetector(doubleClickThreshold);
	    }

        void OnEnable(){

            //subscribe to event
            InputSystem.onDeviceChange += OnDeviceChange;

            move = playerInput.Player.Move;
            punch = playerInput.Player.Punch;
            kick = playerInput.Player.Kick;
            defend = playerInput.Player.Defend;
            grab = playerInput.Player.Grab;
            jump = playerInput.Player.Jump;

            move.Enable();
            punch.Enable();
            kick.Enable();
            defend.Enable();
            grab.Enable();
            jump.Enable();
        }

        void OnDisable(){

            //unsubscribe from event
            InputSystem.onDeviceChange -= OnDeviceChange;

            move.Disable();
            punch.Disable();
            kick.Disable();
            defend.Disable();
            grab.Disable();
            jump.Disable();
        }

        void Update(){
            // 优化：使用局部变量缓存查询结果，避免多次调用
            bool punchPressed = punch.WasPressedThisFrame();
            bool kickPressed = kick.WasPressedThisFrame();
            bool jumpPressed = jump.WasPressedThisFrame();
            bool punchHeld = punch.IsPressed();

            // 更新双击检测器（只在按下时调用）
            if (punchPressed) {
                punchDoubleClick.OnButtonPressed();
                punchPressTime = 0f;
                punchWasPressed = true;
            }
            if (kickPressed) {
                kickDoubleClick.OnButtonPressed();
            }
            if (jumpPressed) {
                jumpDoubleClick.OnButtonPressed();
            }

            // 追踪按住时长（优化：减少逻辑判断）
            if (punchHeld) {
                if (punchWasPressed) {
                    punchPressTime += Time.deltaTime;
                }
            } else {
                punchWasPressed = false;
                punchPressTime = 0f;
            }
        }

        //get Punch button state
        public static bool PunchKeyDown(){
            return Instance.punch.WasPressedThisFrame();
        }

        //get Kick button state
        public static bool KickKeyDown(){
            return Instance.kick.WasPressedThisFrame();
        }

        //get Jump button state
        public static bool DefendKeyDown(){
            return Instance.defend.IsPressed();
        }

        //get Grab button state
        public static bool GrabKeyDown(){
            return Instance.grab.WasPressedThisFrame();
        }

        //get Jump button state
        public static bool JumpKeyDown(){
            return Instance.jump.WasPressedThisFrame();
        }

        // ========== 双击检测方法 ==========

        /// <summary>
        /// 检测Punch按钮是否被双击
        /// </summary>
        public static bool PunchDoubleClick(){
            return Instance.punchDoubleClick.IsDoubleClick();
        }

        /// <summary>
        /// 检测Kick按钮是否被双击
        /// </summary>
        public static bool KickDoubleClick(){
            return Instance.kickDoubleClick.IsDoubleClick();
        }

        /// <summary>
        /// 检测Jump按钮是否被双击
        /// </summary>
        public static bool JumpDoubleClick(){
            return Instance.jumpDoubleClick.IsDoubleClick();
        }

        /// <summary>
        /// 获取Punch按钮的按住时长（秒）
        /// </summary>
        public static float PunchHoldDuration(){
            return Instance.punchPressTime;
        }

        /// <summary>
        /// 检测Punch按钮是否正在被按住
        /// </summary>
        public static bool PunchIsHeld(){
            return Instance.punch.IsPressed();
        }

        //returns the directional input as a vector2
        public static Vector2 GetInputVector(){
            return Instance.move.ReadValue<Vector2>();
        }

        //detect joypad direction input
        public static bool JoypadDirInputDetected(){
            return (Instance.move.ReadValue<Vector2>().x != 0 || Instance.move.ReadValue<Vector2>().y != 0);
        }

        //detect device input change
        void OnDeviceChange(InputDevice device, InputDeviceChange change) {
            if (change == InputDeviceChange.Added) {
                InputUser.PerformPairingWithDevice(device, InputUser.all[0], InputUserPairingOptions.ForceNoPlatformUserAccountSelection);
            } else if (change == InputDeviceChange.Removed) {
            }
        }
    }

    /// <summary>
    /// 双击检测器
    /// 用于检测按钮是否在指定时间间隔内被按下两次
    /// </summary>
    public class DoubleClickDetector {
        private float lastClickTime;
        private float threshold;
        private bool doubleClickDetected;

        public DoubleClickDetector(float doubleClickThreshold) {
            this.threshold = doubleClickThreshold;
            this.lastClickTime = -1f;
            this.doubleClickDetected = false;
        }

        /// <summary>
        /// 当按钮被按下时调用此方法
        /// </summary>
        public void OnButtonPressed() {
            float currentTime = Time.time;
            float timeSinceLastClick = currentTime - lastClickTime;

            // 如果在阈值时间内再次点击，则检测到双击
            if (timeSinceLastClick > 0 && timeSinceLastClick < threshold) {
                doubleClickDetected = true;
                lastClickTime = -1f; // 重置，防止三击被检测为两次双击
            } else {
                doubleClickDetected = false;
                lastClickTime = currentTime;
            }
        }

        /// <summary>
        /// 检查是否检测到双击
        /// 注意：此方法只会在检测到双击的那一帧返回true，之后会自动重置
        /// </summary>
        public bool IsDoubleClick() {
            if (doubleClickDetected) {
                doubleClickDetected = false; // 消费掉双击标记
                return true;
            }
            return false;
        }

        /// <summary>
        /// 重置双击检测器
        /// </summary>
        public void Reset() {
            lastClickTime = -1f;
            doubleClickDetected = false;
        }
    }
}