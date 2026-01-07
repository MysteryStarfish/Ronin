using System.Collections.Generic;
using UnityEngine;

namespace Ronin.Core
{
    public class CircleDetectorStrategy : IDetectorStrategy
    {
        private readonly float _radius;
        private List<Collider2D> _hitBuffer;
        public CircleDetectorStrategy(float radius)
        {
            _radius = radius;
            _hitBuffer = new List<Collider2D>();
        }
        public int GetTargets(Transform player, List<Transform> targets, LayerMask layer)
        {
            targets.Clear();
            
            var enemyFilter = new ContactFilter2D
            {
                useTriggers = false,
                useLayerMask = true,
                layerMask = layer 
            };
    
            int hitCount = Physics2D.OverlapCircle(player.position, _radius, enemyFilter, _hitBuffer);
    
            for (int i = 0; i < hitCount; i++)
            {
                targets.Add(_hitBuffer[i].transform);
            }
            return hitCount;
        }
    }
}