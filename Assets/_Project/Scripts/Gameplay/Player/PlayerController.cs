using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;
using Ronin.Input;

namespace Ronin.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]

    public class PlayerController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private InputReader inputReader;

        [Header("Components")] 
        private Rigidbody2D _rb;

        [Header("Movement Settings")] 
        [SerializeField] private float walkSpeed = 5f;
        
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float sneakSpeed = 2f;
        
        [Header("Dash Settings")] 
        [SerializeField] private float dashSpeed = 30f;
        [SerializeField] private float dashDuration = 0.1f;
        [SerializeField] private float dashCooldown = 0.2f;
        private HashSet<Timer> _timers;
        private CountdownTimer _dashTimer;
        private CountdownTimer _dashCooldownTimer;

        [Header("Detector Setting")] 
        [SerializeField] private float radius = 10f;
        [SerializeField] private LayerMask layerMask;
        public float DetectorRadius => radius;
        
        private Vector2 _moveInput;
        private Vector2 _dashDirection;
        private bool _runSwitch = false;
        private bool _sneakSwitch = false;
        private bool _lockSwitch = false;
        private bool CanDash => _dashCooldownTimer.IsFinished;

        [SerializeField] private TargetScannerPriorityList priorityList;
        private TargetScanner<ILockable> _targetScanner;
        private CircleScanTargets _circleScanner;
        private SelectLeftMost _selectLeftMost;
        private SelectRightMost _selectRightMost;
        private SelectClosest _selectClosest;
        private Transform _currentTarget;
        public Transform AimTarget => _currentTarget;

        private PlayerMovement _player;
        private StateMachine _stateMachine;
        

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            
            SetupPlayer();
            SetupTimers();
            SetupStateMachine();
            SetupTargetScanner();
        }

        private void SetupTimers()
        {
            _timers = new HashSet<Timer>();
            
            _dashTimer = new CountdownTimer(dashDuration);
            _timers.Add(_dashTimer);
            _dashCooldownTimer = new CountdownTimer(dashCooldown);
            _timers.Add(_dashCooldownTimer);
            
            _dashTimer.OnStart += () => { _dashDirection = _moveInput; };
            _dashTimer.OnStop += _dashCooldownTimer.Start;
        }
        
        private void SetupTargetScanner()
        {
            _circleScanner = new CircleScanTargets(radius, layerMask);
            _targetScanner = new TargetScanner<ILockable>(_circleScanner);

            _selectLeftMost = new SelectLeftMost(priorityList.Priority);
            _selectRightMost = new SelectRightMost(priorityList.Priority);
            _selectClosest = new SelectClosest(priorityList.Priority);
        }

        private void SetupPlayer()
        {
            _player = new PlayerMovement(_rb);
        }

        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine();

            IState walkState = new WalkState(this);
            IState runState = new RunState(this);
            IState sneakState = new SneakState(this);
            IState dashState = new DashState(this);
            
            _stateMachine.AddState(walkState);
            _stateMachine.AddState(runState);
            _stateMachine.AddState(sneakState);
            _stateMachine.AddState(dashState);

            AtLocomotion(runState, new FuncPredicate(() => _runSwitch));
            At(runState, walkState, new FuncPredicate(() => !_runSwitch));
            AtLocomotion(sneakState, new FuncPredicate(() => _sneakSwitch));
            At(sneakState, walkState, new FuncPredicate(() => !_sneakSwitch));
            AtLocomotion(dashState, new FuncPredicate(() => _dashTimer.IsRunning));
            At(dashState, walkState, new FuncPredicate(() => _dashTimer.IsFinished));
            
            _stateMachine.SetInitState(walkState);
        }

        private void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
        private void AtLocomotion(IState to, IPredicate condition)
        {
            foreach (var node in _stateMachine.AllNodes)
            {
                if (node.State is not LocomotionState locomotionState) continue;
                if (locomotionState == to) continue;
                _stateMachine.AddTransition(locomotionState, to, condition);
            }
        }
        private void OnEnable()
        {
            inputReader.MoveEvent += OnMove;
            inputReader.RunEvent += OnRun;
            inputReader.SneakEvent += OnSneak;
            inputReader.DashEvent += OnDash;
            inputReader.LockClosestEvent += OnLockClosest;
            inputReader.LockLeftEvent += OnLockLeft;
            inputReader.LockRightEvent += OnLockRight;
            inputReader.UnLockEvent += OnUnLock;
        }
        private void OnDisable()
        {
            inputReader.MoveEvent -= OnMove;
            inputReader.RunEvent -= OnRun;
            inputReader.SneakEvent -= OnSneak;
            inputReader.DashEvent -= OnDash;
            inputReader.LockClosestEvent -= OnLockClosest;
            inputReader.LockClosestEvent -= OnLockClosest;
            inputReader.LockClosestEvent -= OnLockClosest;
            inputReader.UnLockEvent -= OnUnLock;
        }


        private void OnMove(Vector2 input)
        {
            _moveInput = input;
        }
        private void OnRun()
        {
            _runSwitch = !_runSwitch;
            _sneakSwitch = false;
        }
        private void OnSneak()
        {
            _sneakSwitch = !_sneakSwitch;
            _runSwitch = false;
        }
        private void OnDash()
        {
            if (CanDash) _dashTimer.Start();
        }
        private void OnUnLock()
        {
            _lockSwitch = !_lockSwitch;
            if (!_lockSwitch)
            {
                _currentTarget = null;
                return;
            }
            _currentTarget = TryGetTarget(_selectClosest);
            if (_currentTarget == null) _lockSwitch = false;
        }

        private void OnLockClosest()
        {
            _currentTarget = TryGetTarget(_selectClosest);
            if (_currentTarget != null) _lockSwitch = true;
        }
        private void OnLockLeft()
        {
            _lockSwitch = true;
            _currentTarget = TryGetTarget(_selectLeftMost);
            if (_currentTarget != null) _lockSwitch = true;
        }
        private void OnLockRight()
        {
            _lockSwitch = true;
            _currentTarget = TryGetTarget(_selectRightMost);
            if (_currentTarget != null) _lockSwitch = true;
        }
        private Transform TryGetTarget(ISelectTargetStrategy<ILockable> strategy)
        {
            var target = _targetScanner.GetTarget(transform, strategy);
            if (target == null) return null;
            return target.Transform;
        }
        private void Update()
        {
            HandleStateMachine();
            HandleTimer();
        }

        private void HandleStateMachine()
        {
            _stateMachine.Update();
        }

        private void HandleTimer()
        {
            foreach (Timer timer in _timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }
        public void HandleLocomotion()
        {
            HandleRotation();
        }
        public void HandleRotation()
        {
            if (!_lockSwitch || !_currentTarget) _player.HandleRotation(_moveInput);
            else _player.HandleRotation(_currentTarget.transform.position - transform.position);
        }
        public void HandleWalk()
        {
            _player.HandleMove(_moveInput, walkSpeed);
        }
        public void HandleRun()
        {
            _player.HandleMove(_moveInput, runSpeed);
        }
        public void HandleSneak()
        {
            _player.HandleMove(_moveInput, sneakSpeed);
        }
        public void HandleDash()
        {
            _player.HandleMove(_dashDirection, dashSpeed);
        }

        public void HandleFinishedDash()
        {
            // noop
        }
    }

    public class PlayerMovement
    {
        private readonly Rigidbody2D _rb;
        
        public PlayerMovement(Rigidbody2D rb)
        {
            _rb = rb;
        }
        public void HandleMove(Vector2 direction, float speed)
        {
            _rb.linearVelocity = direction * speed;
        }
        public void HandleRotation(Vector2 targetPosition)
        {
            _rb.transform.FaceDirection2D(targetPosition);
        }
    }
}