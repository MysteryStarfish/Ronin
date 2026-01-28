using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;

namespace Ronin.Gameplay
{
    public class ChargeAttackState : BaseState
    {
        private CountdownTimer _chargeTimer;
        private readonly HashSet<Timer> _timers;
        private float _chargeTime;
        public float Progress => _chargeTimer.Process;

        private readonly PlayerController _player;
        private void SetupTimer()
        {
            _chargeTimer = new CountdownTimer(_chargeTime);
            _timers.Add(_chargeTimer);

            _chargeTimer.OnStop += _player.OnFinishChargeAttack;
        }
        public ChargeAttackState(PlayerController player, float chargeTime) : base(player)
        {
            _player = player;
            _chargeTime = chargeTime;
            _timers = new HashSet<Timer>();
            SetupTimer();
        }

        public override void OnEnter()
        {
            _chargeTimer.Start();
            _player.HandleStopMove();
        }

        public override void Update()
        {
            base.Update();
            foreach (var timer in _timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }
    }
}