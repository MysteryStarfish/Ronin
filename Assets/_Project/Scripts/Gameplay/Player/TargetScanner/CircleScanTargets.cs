using System.Collections.Generic;
using UnityEngine;

namespace Ronin.Core
{
    public class CircleScanTargets : IScanTargetsStrategy<ILockable>
    {
        private readonly float _radius;
        private readonly List<Collider2D> _hitBuffer;
        public CircleScanTargets(float radius)
        {
            _radius = radius;
            _hitBuffer = new List<Collider2D>();
        }
        public void GetTargets(Transform player, List<ILockable> targets)
        {
            targets.Clear();
            
            var enemyFilter = new ContactFilter2D
            {
                useTriggers = false,
                useLayerMask = false,
            };
    
            int hitCount = Physics2D.OverlapCircle(player.position, _radius, enemyFilter, _hitBuffer);
    
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hitCollider = _hitBuffer[i];
                
                // 從 Collider 取得 ILockable
                if (hitCollider.TryGetComponent<ILockable>(out var lockable))
                {
                    // 只有當這個物件真的有實作 ILockable 時，才加入清單
                    targets.Add(lockable);
                }
            }
        }
    }
}