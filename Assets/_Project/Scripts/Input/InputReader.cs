using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ronin.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Ronin/Input/Input Reader")]
    public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions
    {
        private PlayerInputActions _playerInputActions;
        public event Action<Vector2> MoveEvent = delegate {};
        public event Action RunEvent = delegate {};
        public event Action SneakEvent = delegate {};
        public event Action DashEvent = delegate {};
        public event Action LockClosestEvent = delegate {};
        public event Action LockLeftEvent = delegate {};
        public event Action LockRightEvent = delegate {};
        public event Action UnLockEvent = delegate {};
        public event Action AttackEvent = delegate {};
        
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
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                RunEvent?.Invoke();
            }
        }

        public void OnSneak(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                SneakEvent?.Invoke();
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                DashEvent?.Invoke();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                AttackEvent?.Invoke();
            }
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
            if (context.phase == InputActionPhase.Performed)
            {
                UnLockEvent?.Invoke();
            }
        }

        public void OnLockLeft(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                LockLeftEvent?.Invoke();
            }
        }

        public void OnLockRight(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                LockRightEvent?.Invoke();
            }
        }

        public void OnLockClosest(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                LockClosestEvent.Invoke();
            }
        }
    }
}
