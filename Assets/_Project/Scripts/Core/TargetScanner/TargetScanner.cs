using System.Collections.Generic;
using UnityEngine;

namespace Ronin.Core
{
    public class TargetScanner<T>
    {
        private List<T> _targets;
        private List<T> _results;
        private IScanTargetsStrategy<T> _scanStrategy;

        public TargetScanner(IScanTargetsStrategy<T> scanStrategy)
        {
            _scanStrategy = scanStrategy;
            _targets = new List<T>();
            _results = new List<T>();
        }
        
        public T GetTarget(Transform transform, ISelectTargetStrategy<T> targetSelector)
        {
            _scanStrategy.GetTargets(transform, _targets);
            targetSelector.SelectTargets(transform, _targets, _results);
            return _results[0];
        }
        public T GetTarget(Transform transform, ISelectTargetStrategy<T> targetSelector, T currentTarget)
        {
            _scanStrategy.GetTargets(transform, _targets);
            targetSelector.SelectTargets(transform, _targets, _results, currentTarget);
            return _results[0];
        }

        public void GetTargets(Transform transform, ISelectTargetStrategy<T> targetSelector, List<T> results)
        {
            results.Clear();
            _scanStrategy.GetTargets(transform, _targets);
            targetSelector.SelectTargets(transform, _targets, results);
        }
        public void GetTargets(Transform transform, ISelectTargetStrategy<T> targetSelector, T currentTarget, List<T> results)
        {
            results.Clear();
            _scanStrategy.GetTargets(transform, _targets);
            targetSelector.SelectTargets(transform, _targets, results, currentTarget);
        }
    }
}