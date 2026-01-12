using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor; // Only import this in the editor, otherwise it will cause build errors
#endif

namespace Ronin.Core
{
    [CreateAssetMenu(fileName = "PriorityConfig", menuName = "Ronin/Scanner/PriorityConfig")]
    public class TargetScannerPriorityList : ScriptableObject
    {
        [SerializeField] private List<MonoScript> priorityObjects;
        public List<ILockable> Priority => null; 
    }
}
