using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Ronin.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Ronin/Input/Input Reader")]
    public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions
    {
        private PlayerInputActions _playerInputActions;
        public event UnityAction<Vector2> OnMoveEvent = delegate {};
        public event UnityAction OnRunEvent = delegate {};
        public event UnityAction OnDashEvent = delegate {};
        private void OnEnable()
        {
            if (_playerInputActions == null)
            {
                _playerInputActions = new PlayerInputActions();
                _playerInputActions.Player.SetCallbacks(this);
            }
            _playerInputActions.Player.Enable();
        }
        private void OnDisable()
        {
            _playerInputActions.Player.Disable();
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                OnRunEvent?.Invoke();
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                OnDashEvent?.Invoke();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnChargeAttack(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnSpecialAttack(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnUnlock(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnLockLeft(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnLockRight(InputAction.CallbackContext context)
        {
            // noop
        }
    }
}
