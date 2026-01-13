using System;
using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;

namespace Ronin.Gameplay
{
    public class SelectEmergency : ISelectTargetStrategy<ILockable>
    {
        private List<ILockable> _priorityList;
        private float _innerRadius;
        public SelectEmergency(List<ILockable> priorityList, float innerRadius)
        {
            _priorityList = priorityList;
            _innerRadius = innerRadius;
        }

        public ILockable SelectTarget(Transform transform, List<ILockable> targets, ILockable ignore=null)
        {
            ILockable resultTarget = null;
            float minDistance = Single.MaxValue;
            foreach (ILockable target in targets)
            {
                if (!target.CanLock) continue;
                float distance = Vector2.Distance(transform.position, target.Transform.position);
                if (distance > _innerRadius) continue;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    resultTarget = target;
                }
            }

            return resultTarget;
        }
    }
}