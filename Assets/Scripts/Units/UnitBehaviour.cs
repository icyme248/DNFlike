using UnityEngine;
using QFramework;
using GameArchitecture;

namespace GameArchitecture.Units
{
    /// <summary>
    /// 单位行为基类 - 所有单位的通用行为
    /// 纯通用功能，不包含玩家特有的东西（如Input）
    /// 可被Player、Enemy、NPC等继承复用
    /// </summary>
    public class UnitBehaviour : MonoBehaviour, IController
    {
        [Header("组件引用")]
        public Animator animator;
        public Rigidbody rb;
        public SpriteRenderer spriteRenderer;
        public BoxCollider boxCollider;
        [Header("移动参数")]
        public float moveSpeed = 5f;
        public float airMoveSpeed = 2.5f;

        [Header("攻击参数")]
        public float attackMoveSpeed = 0.5f; // 攻击时的默认前进速度
        public float jumpAttackMoveSpeedMultiplier = 5f; // 跳跃攻击时的横向速度倍率（相对地面攻击）

        [Header("跳跃参数")]
        public float jumpForce = 10f;
        public float jumpAttackFallSpeed = 15f; // 下落攻击的快速下落速度
        public float groundCheckDistance = 0.1f; // 加大检测距离
        public LayerMask groundLayer;
        public bool showGroundCheckDebug = true; // 调试用

        // 是否在地面上
        public bool isGrounded { get; protected set; }

        // 地面检测起点（相对transform.position的偏移）
        private Vector3 groundCheckOriginOffset;

        protected virtual void Awake()
        {
            // 自动获取组件
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (rb == null) rb = GetComponentInChildren<Rigidbody>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (boxCollider == null) boxCollider = GetComponentInChildren<BoxCollider>();
            groundLayer = LayerMask.GetMask("Ground");

            // 预计算地面检测起点偏移（从BoxCollider底部开始）
            CalculateGroundCheckOffset();
        }

        /// <summary>
        /// 计算地面检测起点的偏移
        /// </summary>
        private void CalculateGroundCheckOffset()
        {
            if (boxCollider != null)
            {
                // 计算BoxCollider底部相对于transform.position的偏移
                float offsetY = boxCollider.bounds.min.y - transform.position.y;
                groundCheckOriginOffset = new Vector3(0, offsetY, 0);
            }
            else
            {
                // 如果没有BoxCollider，使用默认值
                groundCheckOriginOffset = Vector3.zero;
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public void PlayAnimation(string animName)
        {
            animator?.Play(animName);
        }

        /// <summary>
        /// 地面移动：设置XZ轴速度，保留Y轴速度
        /// </summary>
        public void Move(Vector2 direction, float speed)
        {
            if (rb != null)
            {
                rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, direction.y * speed * 1.3f /*Z轴补偿*/);
            }
        }

        /// <summary>
        /// 空中移动：调整XZ轴速度，保留Y轴速度
        /// </summary>
        public void AirMove(Vector2 direction)
        {
            if (rb != null)
            {
                rb.velocity = new Vector3(direction.x * airMoveSpeed, rb.velocity.y, direction.y * airMoveSpeed);
            }
        }

        /// <summary>
        /// 停止水平移动：清零XZ轴速度，保留Y轴速度
        /// </summary>
        public void StopMoving()
        {
            if (rb != null)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }

        /// <summary>
        /// 翻转精灵
        /// </summary>
        public void FlipSprite(float direction)
        {
            if (spriteRenderer != null && direction != 0)
            {
                spriteRenderer.flipX = direction < 0;
            }
        }

        /// <summary>
        /// 获取当前朝向（1=右，-1=左）
        /// </summary>
        public float GetFacingDirection()
        {
            if (spriteRenderer != null)
            {
                return spriteRenderer.flipX ? -1f : 1f;
            }
            return 1f;
        }

        /// <summary>
        /// 攻击时的移动（支持地面和空中）
        /// </summary>
        /// <param name="inputX">水平输入</param>
        /// <param name="speedMultiplier">速度倍率（地面攻击=1.0，空中攻击可传入jumpAttackMoveSpeedMultiplier）</param>
        public void AttackMove(float inputX, float speedMultiplier = 1f)
        {
            if (rb == null) return;

            float facingDir = GetFacingDirection();
            float moveMultiplier;

            if (inputX == 0)
            {
                // 无方向输入：默认前进
                moveMultiplier = 1f;
            }
            else
            {
                float inputDir = Mathf.Sign(inputX);
                if (inputDir == facingDir)
                {
                    // 按住前方向键：1.5倍速度
                    moveMultiplier = 1.5f;
                }
                else
                {
                    // 按住后方向键：保持原地
                    moveMultiplier = 0f;
                }
            }

            // 应用移动（支持自定义速度倍率）
            rb.velocity = new Vector3(
                facingDir * attackMoveSpeed * speedMultiplier * moveMultiplier, 
                rb.velocity.y, 
                0
            );
        }

        /// <summary>
        /// 快速下落：设置向下的速度
        /// </summary>
        public void FastFall()
        {
            if (rb != null)
            {
                rb.velocity = new Vector3(rb.velocity.x, -jumpAttackFallSpeed, rb.velocity.z);
            }
        }

        /// <summary>
        /// 地面检测：从碰撞体底部向下射线检测
        /// </summary>
        public bool CheckGrounded()
        {
            if (rb == null) return true;

            Vector3 origin = transform.position + groundCheckOriginOffset;
            bool hitGround = Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer);
            
            if (showGroundCheckDebug)
            {
                Debug.DrawRay(origin, Vector3.down * groundCheckDistance, hitGround ? Color.green : Color.red);
            }
            
            isGrounded = hitGround;
            return hitGround;
        }

        /// <summary>
        /// 跳跃：清零水平速度，设置垂直速度
        /// </summary>
        public void Jump()
        {
            if (rb != null)
            {
                rb.velocity = new Vector3(0, jumpForce, 0);
                isGrounded = false;
            }
        }

        public IArchitecture GetArchitecture() => GameApp.Interface;
    }
}

