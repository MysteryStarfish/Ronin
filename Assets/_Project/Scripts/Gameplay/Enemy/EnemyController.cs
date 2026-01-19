using Ronin.Core;
using UnityEngine;

namespace Ronin.Gameplay
{
    public class EnemyController : MonoBehaviour, ILockable, IAttackable
    {
        Transform ILockable.Transform => transform;

        bool ILockable.CanLock => true;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnAttack()
        {
            
        }
    }

}