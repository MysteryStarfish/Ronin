using Ronin.Gameplay;

namespace Ronin.Core
{
    public class LocomotionState : BaseState {
        public LocomotionState(PlayerController player) : base(player) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Player.HandleLocomotion();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }

    public class WalkState : LocomotionState
    {
        public WalkState(PlayerController player) : base(player) { }
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void Update()
        {
            base.Update();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Player.HandleWalk();
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
    
    public class RunState : LocomotionState
    {
        public RunState(PlayerController player) : base(player) { }
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void Update()
        {
            base.Update();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Player.HandleRun();
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
    
    public class SneakState : LocomotionState
    {
        public SneakState(PlayerController player) : base(player) { }
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void Update()
        {
            base.Update();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Player.HandleSneak();
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}