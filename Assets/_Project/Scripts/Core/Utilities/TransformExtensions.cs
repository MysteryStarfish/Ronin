using UnityEngine;

namespace Ronin.Core
{
    public static class TransformExtensions
    {
        /// <summary>
        /// 讓 2D 物件的 "右邊 (X軸)" 指向目標
        /// </summary>
        public static void LookAt2D(this Transform transform, Vector2 targetPosition)
        {
            Vector2 direction = targetPosition - (Vector2)transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }

        /// <summary>
        /// 平滑版本的 LookAt2D (適合敵人轉向)
        /// </summary>
        public static void SmoothLookAt2D(this Transform transform, Vector2 targetPosition, float speed)
        {
            Vector2 direction = targetPosition - (Vector2)transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        
            // 使用 RotateTowards 或 Slerp
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
    
        /// <summary>
        /// 平滑版本的 LookAt2D (適合敵人轉向)
        /// </summary>
        public static void FaceDirection2D(this Transform transform, Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f) return;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = targetRotation;
        }
    
        /// <summary>
        /// 平滑版本的 LookAt2D (適合敵人轉向)
        /// </summary>
        public static void SmoothFaceDirection2D(this Transform transform, Vector2 direction, float speed)
        {
            if (direction.sqrMagnitude < 0.001f) return;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
    }
}