using System.Collections.Generic;
using UnityEngine;

namespace Ronin.Core
{
    public interface ISelectTargetStrategy<T>
    {
        public void SelectTargets(Transform transform, List<T> targets, List<T> results, T ignore);
        public void SelectTargets(Transform transform, List<T> targets, List<T> results) {
            SelectTargets(transform, targets, results, default);
        }
    }
}