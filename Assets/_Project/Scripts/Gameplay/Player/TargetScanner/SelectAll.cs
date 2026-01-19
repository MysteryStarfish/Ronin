using System;
using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;

namespace Ronin.Gameplay
{
    public class SelectAll : ISelectTargetStrategy<IAttackable>
    {
        public void SelectTargets(Transform transform, List<IAttackable> targets, List<IAttackable> results, IAttackable ignore=null)
        {
            results.Clear();
            results.AddRange(targets);
        }
    }
}