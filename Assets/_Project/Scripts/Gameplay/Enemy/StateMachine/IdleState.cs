using Ronin.Core;
using UnityEngine.PlayerLoop;

namespace Ronin.Gameplay
{
    public class IdleState : EnemyBaseState
    {
        public IdleState(EnemyController enemy) : base(enemy)
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Enemy.DoRotate();
        }
    }
}