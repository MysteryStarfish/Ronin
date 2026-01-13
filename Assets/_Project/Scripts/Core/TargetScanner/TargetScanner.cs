using System.Collections.Generic;
using UnityEngine;

namespace Ronin.Core
{
    public class TargetScanner<T>
    {
        private List<T> _targets;
        private IScanTargetsStrategy<T> _scanStrategy;

        public TargetScanner(IScanTargetsStrategy<T> scanStrategy)
        {
            _scanStrategy = scanStrategy;
            _targets = new List<T>();
        }


        public T GetTarget(Transform transform, ISelectTargetStrategy<T> targetSelector)
        {
            _scanStrategy.GetTargets(transform, _targets);
            return targetSelector.SelectTarget(transform, _targets);
        }
        public T GetTarget(Transform transform, ISelectTargetStrategy<T> targetSelector, T currentTarget)
        {
            _scanStrategy.GetTargets(transform, _targets);
            return targetSelector.SelectTarget(transform, _targets, currentTarget);
        }
    }
}