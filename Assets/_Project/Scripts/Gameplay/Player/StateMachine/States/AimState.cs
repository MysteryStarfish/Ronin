using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ronin.Gameplay
{
    public class AimState : BaseState
    {
        private TimeScaleManager _timeScaleManager;
        private Timer _aimTimer;
        private readonly HashSet<Timer> _timers;
        private float _aimTime;
        private float _slowMotionScale;
        private Volume _targetVolume;
        private float _fadeSpeed;

        private Timer _recoverTimer;
        

        private readonly PlayerController _player;
        private void SetupTimer()
        {
            _aimTimer = new CountdownTimer(_aimTime);
            _timers.Add(_aimTimer);

            _timeScaleManager = new TimeScaleManager(_slowMotionScale);

            _aimTimer.OnStop += _player.OnFinishAim;
            _aimTimer.OnStart += _timeScaleManager.StartSlowMotion;
            _aimTimer.OnStop += _timeScaleManager.StopSlowMotion;
        }
        public AimState(PlayerController player, float aimTime, float slowMotionScale, Volume targetVolume, float fadeSpeed) : base(player)
        {
            _player = player;
            _aimTime = aimTime;
            _slowMotionScale = slowMotionScale;
            _targetVolume = targetVolume;
            _fadeSpeed = fadeSpeed;
            _timers = new HashSet<Timer>();
            SetupTimer();
        }

        public override void OnEnter()
        {
            _aimTimer.Start();
            _player.HandleStopMove();
        }

        public override void Update()
        {
            base.Update();
            foreach (var timer in _timers)
            {
                timer.Tick(Time.unscaledDeltaTime);
            }
            _targetVolume.weight = Mathf.Lerp(_targetVolume.weight, 1f, Time.unscaledDeltaTime * _fadeSpeed);
        }

        public override void OnExit()
        {
            _aimTimer.Stop();
            _targetVolume.weight = 0f;
        }
    }
}