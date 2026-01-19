using System.Collections.Generic;
using UnityEngine;

namespace Ronin.Core
{
    public class ColliderScanTargets : IScanTargetsStrategy<IAttackable>
    {
        private readonly Collider2D _collider2D;
        private readonly List<Collider2D> _hitBuffer;
        public ColliderScanTargets(Collider2D collider2D)
        {
            _collider2D = collider2D;
            _hitBuffer = new List<Collider2D>();
        }
        public void GetTargets(Transform player, List<IAttackable> targets)
        {
            targets.Clear();

            var enemyFilter = ContactFilter2D.noFilter;
    
            int hitCount = Physics2D.OverlapCollider(_collider2D, enemyFilter, _hitBuffer);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hitCollider = _hitBuffer[i];
                
                // 從 Collider 取得 IAttackable
                if (hitCollider.TryGetComponent<IAttackable>(out var attackable))
                {
                    // 只有當這個物件真的有實作 ILockable 時，才加入清單
                    targets.Add(attackable);
                }
            }
        }
    }
}