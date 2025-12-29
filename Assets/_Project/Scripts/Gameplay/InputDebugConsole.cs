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
            _inputReader.MoveEvent += Move;
            _inputReader.RunEvent += Run;
            _inputReader.DashEvent += Dash;
            // _inputReader.AttackEvent += OnAttack; // 如果你有寫的話
        }

        private void OnDisable()
        {
            if (_inputReader == null) return;

            _inputReader.MoveEvent -= Move;
            _inputReader.RunEvent -= Run;
            _inputReader.DashEvent -= Dash;
        }

        private void Move(Vector2 movement)
        {
            Debug.Log($"[Input] Move: {movement}");
        }

        private void Run()
        {
            Debug.Log($"[Input] Run Pressed!");
        }

        private void Dash()
        {
            Debug.Log($"[Input] Dash Pressed!");
        }
    }
}