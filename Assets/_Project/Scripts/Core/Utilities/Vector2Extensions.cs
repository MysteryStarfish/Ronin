using UnityEngine;

namespace Ronin.Core
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// 從 form 到 to 逆時針轉了幾度 (0~360)
        /// </summary>
        public static float ClockwiseAngle(this Vector2 from, Vector2 to)
        {
            var angle = Vector2.SignedAngle(from, to);
            if (angle < 0) return angle + 360;
            return angle;
        }
        
        /// <summary>
        /// 從 form 到 to 順時針轉了幾度 (0~360)
        /// </summary>
        public static float CounterclockwiseAngle(this Vector2 from, Vector2 to)
        {
            return ClockwiseAngle(to, from);
        }
    }
}