using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ronin.Core
{
    public class EnemyDetector
    {
        private readonly IDetectorStrategy _detector;
        private readonly LayerMask _layerMask;
        private readonly float _radius;

        private List<Transform> _targets;

        public EnemyDetector(float radius, LayerMask layerMask)
        {
            _detector = new CircleDetectorStrategy(radius);
            _radius = radius;
            this._layerMask = layerMask;
            _targets = new List<Transform>();
        }

        public void Update(Transform transform)
        {
            _detector.GetTargets(transform, _targets, _layerMask);
        }

        public GameObject FindClosestTarget(Transform transform)
        {
            if (_targets == null) return null;
            float minDistance = Mathf.Infinity;
            GameObject closetTarget = null;
            foreach (var target in _targets)
            {
                float distance = (target.position - transform.position).magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closetTarget = target.gameObject;
                }
            }

            return closetTarget;
        }

        public Transform FindLeftTarget()
        {
            throw new System.NotImplementedException();
        }

        public Transform FindLeftRight()
        {
            throw new System.NotImplementedException();
        }
    }
}
