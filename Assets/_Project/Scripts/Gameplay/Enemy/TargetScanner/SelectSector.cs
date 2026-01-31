using System;
using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;

namespace Ronin.Gameplay
{
    public class SelectSector : ISelectTargetStrategy<IMadable>
    {
        private List<IMadable> _priorityList;
        private float _angle;
        public SelectSector(List<IMadable> priorityList, float angle)
        {
            _priorityList = priorityList;
            _angle = angle;
        }
        public void SelectTargets(Transform transform, List<IMadable> targets, List<IMadable> results, IMadable ignore)
        {
            results.Clear();
            IMadable resultTarget = null;
            foreach (IMadable target in targets)
            {
                if (target == ignore) continue;
                Vector2 targetVector = target.Transform.position - transform.position;
                Vector2 faceDirection = transform.right;
                float targetAngle = faceDirection.Angle(targetVector);
                if (targetAngle <= _angle)
                {
                    resultTarget = target;
                }
            }
            results.Add(resultTarget ?? ignore); 
        }
    }
}