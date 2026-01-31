using Ronin.Core;

namespace Ronin.Gameplay
{
    public class TrackState : EnemyBaseState
    {
        public TrackState(EnemyController enemy) : base(enemy)
        {
        }

        public override void Update()
        {
            base.FixedUpdate();
            Enemy.FollowPlayer();
        }
    }
}