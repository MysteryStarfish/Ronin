using UnityEngine;

namespace Ronin.Core
{
    public interface ILockable
    {
        public Transform Transform { get; }
        public bool CanLock { get; }
    }
}