using Ronin.Core;

namespace Ronin.Gameplay
{
    public class EnemyBaseState : IState
    {
        protected readonly EnemyController Enemy;

        protected EnemyBaseState(EnemyController enemy)
        {
            Enemy = enemy;
        }
        public virtual void OnEnter()
        {
            // noop
        }
        public virtual void Update()
        {
            // noop
        }

        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnExit()
        {
            // noop
        }
    }
}