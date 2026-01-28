using UnityEngine;

namespace Ronin.Core
{
    public class TimeScaleManager
    {
        private float _slowMotionScale; 
        private float _defaultFixedDeltaTime; 
        private bool _isSlowMotionActive = false; 

        public TimeScaleManager(float scale)
        {
            _slowMotionScale = scale;
            _defaultFixedDeltaTime = Time.fixedDeltaTime;
        }

        public void StartSlowMotion()
        {
            if (_isSlowMotionActive) return;

            Time.timeScale = _slowMotionScale;
            Time.fixedDeltaTime = _defaultFixedDeltaTime * Time.timeScale;
            _isSlowMotionActive = true;
        }

        public void StopSlowMotion()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = _defaultFixedDeltaTime;
            _isSlowMotionActive = false;
        }

        public void SetScale(float scale)
        {
            _slowMotionScale = scale;
        }
    }
}