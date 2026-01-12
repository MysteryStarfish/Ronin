using System.Collections.Generic;
using UnityEngine;

namespace Ronin.Core
{
    public interface ISelectTargetStrategy<T>
    {
        public T SelectTarget(Transform transform, List<T> targets);
    }
}