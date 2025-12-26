using UnityEngine;
using Ronin.Input; // 記得引用你的 Input Namespace

namespace Ronin.Gameplay // 假設這是在 Gameplay 層
{
    public class InputDebugConsole : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader; // 拖曳你的 ScriptableObject 進來

        private void OnEnable()
        {
            if (_inputReader == null) return;

            // 訂閱所有事件，並印出 Log
            _inputReader.OnMoveEvent += OnMove;
            _inputReader.OnRunEvent += OnRun;
            _inputReader.OnDashEvent += OnDash;
            // _inputReader.AttackEvent += OnAttack; // 如果你有寫的話
        }

        private void OnDisable()
        {
            if (_inputReader == null) return;

            _inputReader.OnMoveEvent -= OnMove;
            _inputReader.OnRunEvent -= OnRun;
            _inputReader.OnDashEvent -= OnDash;
        }

        private void OnMove(Vector2 movement)
        {
            Debug.Log($"[Input] Move: {movement}");
        }

        private void OnRun()
        {
            Debug.Log($"[Input] Run Pressed!");
        }

        private void OnDash()
        {
            Debug.Log($"[Input] Dash Pressed!");
        }
    }
}