using System.Collections.Generic;
using UnityEngine;

namespace Ronin.Core
{
    public interface IScanTargetsStrategy<T>
    {
        void GetTargets(Transform transform, List<T> targets);
    }
}