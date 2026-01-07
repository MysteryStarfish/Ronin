using System.Collections.Generic;
using UnityEngine;

namespace Ronin.Core
{
    internal interface IDetectorStrategy
    {
        public int GetTargets(Transform player, List<Transform> targets, LayerMask layer);
    }
}