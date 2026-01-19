using System;
using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;

namespace Ronin.Gameplay
{
    public class SelectLeftMost : ISelectTargetStrategy<ILockable>
    {
        private List<ILockable> _priorityList;
        public SelectLeftMost(List<ILockable> priorityList)
        {
            _priorityList = priorityList;
        }
        public void SelectTargets(Transform transform, List<ILockable> targets, List<ILockable> results, ILockable ignore)
        {
            results.Clear();
            ILockable resultTarget = null;
            float minAngle = Single.MaxValue;
            foreach (ILockable target in targets)
            {
                if (!target.CanLock) continue;
                if (target == ignore) continue;
                Vector2 targetVector = target.Transform.position - transform.position;
                Vector2 faceDirection = transform.right;
                float targetAngle = faceDirection.CounterclockwiseAngle(targetVector);
                if (targetAngle < minAngle)
                {
                    minAngle = targetAngle;
                    resultTarget = target;
                }
            }
            results.Add(resultTarget ?? ignore); 
        }
    }
}