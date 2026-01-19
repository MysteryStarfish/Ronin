using System;
using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;

namespace Ronin.Gameplay
{
    public class SelectClosest : ISelectTargetStrategy<ILockable>
    {
        private List<ILockable> _priorityList;
        public SelectClosest(List<ILockable> priorityList)
        {
            _priorityList = priorityList;
        }

        public void SelectTargets(Transform transform, List<ILockable> targets, List<ILockable> results, ILockable ignore=null)
        {
            results.Clear();
            ILockable resultTarget = null;
            float minDistance = Single.MaxValue;
            foreach (ILockable target in targets)
            {
                if (!target.CanLock) continue;
                float distance = Vector2.Distance(transform.position, target.Transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    resultTarget = target;
                }
            }
            results.Add(resultTarget);
        }
    }
}