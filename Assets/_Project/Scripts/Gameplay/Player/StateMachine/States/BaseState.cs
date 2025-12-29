using Ronin.Gameplay;

namespace Ronin.Core
{
    public class BaseState: IState
    {
        protected readonly PlayerController Player;

        protected BaseState(PlayerController player)
        {
            Player = player;
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