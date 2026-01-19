using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;

namespace Ronin.Gameplay
{
    public class AttackState : BaseState
    {
        private CountdownTimer _attackStartupTimer;
        private CountdownTimer _attackStrikeTimer;
        private CountdownTimer _attackRecoveryTimer;
        private readonly HashSet<Timer> _timers;
        private readonly float _startup;
        private readonly float _strike;
        private readonly float _recovery;
        private readonly PlayerController _player;
        private void SetupTargetAttackableScanner()
        {
            _attackStartupTimer = new CountdownTimer(_startup);
            _timers.Add(_attackStartupTimer);
            _attackStrikeTimer = new CountdownTimer(_strike);
            _timers.Add(_attackStrikeTimer);
            _attackRecoveryTimer = new CountdownTimer(_recovery);
            _timers.Add(_attackRecoveryTimer);

            _attackStartupTimer.OnStart += _player.OnStartAttack;
            _attackStartupTimer.OnStop += _attackStrikeTimer.Start;
            
            _attackStrikeTimer.OnStart += _player.OnActiveAttack;
            _attackStrikeTimer.OnStop += _attackRecoveryTimer.Start;
            
            _attackRecoveryTimer.OnStop += _player.OnFinishAttack;
        }
        public AttackState(PlayerController player, float startup, float strike, float recovery) : base(player)
        {
            _player = player;
            _startup = startup;
            _strike = strike;
            _recovery = recovery;
            _timers = new HashSet<Timer>();
            SetupTargetAttackableScanner();
        }

        public override void OnEnter()
        {
            _attackStartupTimer.Start();
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