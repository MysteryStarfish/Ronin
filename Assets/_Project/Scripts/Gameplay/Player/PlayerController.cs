using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;
using Ronin.Input;
using UnityEngine.Serialization;

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
        [SerializeField] private Transform direction;
        [SerializeField] private float innerRadius = 10f;
        [SerializeField] private float radius = 10f;
        
        [Header("Attack Settings")] 
        [SerializeField] private Collider2D attackCollider;
        [SerializeField] private float startup = 0.1f;
        [SerializeField] private float strike = 0.1f;
        [SerializeField] private float recovery = 0.2f;

        public float InnerRadius => innerRadius;
        public float DetectorRadius => radius;
        
        private Vector2 _moveInput;
        private Vector2 _dashDirection;
        private bool _runSwitch = false;
        private bool _sneakSwitch = false;
        private bool _lockSwitch = false;
        private bool _isAttack = false;
        private bool CanDash => _dashCooldownTimer.IsFinished;

        [SerializeField] private TargetScannerPriorityList priorityList;
        private TargetScanner<ILockable> _targetLockableScanner;
        private CircleScanTargets _circleScanner;
        private SelectLeftMost _selectLeftMost;
        private SelectRightMost _selectRightMost;
        private SelectClosest _selectClosest;
        private SelectEmergency _selectEmergency;
        private ILockable _currentTarget;
        public Transform AimTarget => _currentTarget?.Transform;
        
        private TargetScanner<IAttackable> _targetAttackableScanner;
        private ColliderScanTargets _colliderScanner;
        private SelectAll _selectAll;
        private List<IAttackable> _attackResultsBuffer;

        private PlayerMovement _player;
        private StateMachine _stateMachine;
        

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _attackResultsBuffer = new List<IAttackable>();
            
            SetupPlayer();
            SetupTimers();
            SetupStateMachine();
            SetupTargetLockableScanner();
            SetupTargetAttackableScanner();
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
        
        private void SetupTargetLockableScanner()
        {
            _circleScanner = new CircleScanTargets(radius);

            _selectLeftMost = new SelectLeftMost(priorityList.Priority);
            _selectRightMost = new SelectRightMost(priorityList.Priority);
            _selectClosest = new SelectClosest(priorityList.Priority);
            _selectEmergency = new SelectEmergency(priorityList.Priority, innerRadius);
            
            _targetLockableScanner = new TargetScanner<ILockable>(_circleScanner);
        }
        
        private void SetupTargetAttackableScanner()
        {
            _colliderScanner = new ColliderScanTargets(attackCollider);

            _selectAll = new SelectAll();
            
            _targetAttackableScanner = new TargetScanner<IAttackable>(_colliderScanner);
        }


        private ILockable SelectLeft()
        {
            var result = _targetLockableScanner.GetTarget(direction, _selectEmergency);
            if (result != null) return result;
            result = _targetLockableScanner.GetTarget(direction, _selectLeftMost, _currentTarget);
            return result;
        }
        private ILockable SelectRight()
        {
            var result = _targetLockableScanner.GetTarget(direction, _selectEmergency);
            if (result != null) return result;
            result = _targetLockableScanner.GetTarget(direction, _selectRightMost, _currentTarget);
            return result;
        }
        private ILockable SelectClosest()
        {
            var result = _targetLockableScanner.GetTarget(direction, _selectEmergency);
            if (result != null) return result;
            result = _targetLockableScanner.GetTarget(direction, _selectClosest, _currentTarget);
            return result;
        }

        private void SetupPlayer()
        {
            _player = new PlayerMovement(_rb, direction);
        }

        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine();

            IState walkState = new WalkState(this);
            IState runState = new RunState(this);
            IState sneakState = new SneakState(this);
            IState dashState = new DashState(this);
            IState attackState = new AttackState(this, startup, strike, recovery);
            
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
            Any(attackState, new FuncPredicate(() => _isAttack));
            At(attackState, walkState, new FuncPredicate(() => !_isAttack));
            
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
            inputReader.AttackEvent += OnStartAttack;
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
            inputReader.AttackEvent -= OnStartAttack;
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
            _currentTarget = SelectClosest();
            if (_currentTarget == null) _lockSwitch = false;
        }

        private void OnLockClosest()
        {
            _currentTarget = SelectClosest();
            if (_currentTarget != null) _lockSwitch = true;
        }
        private void OnLockLeft()
        {
            _lockSwitch = true;
            _currentTarget = SelectLeft();
            if (_currentTarget != null) _lockSwitch = true;
        }
        private void OnLockRight()
        {
            _lockSwitch = true;
            _currentTarget = SelectRight();
            if (_currentTarget != null) _lockSwitch = true;
        }

        public void OnStartAttack()
        {
            _isAttack = true;
        }

        public void OnActiveAttack()
        {
            _targetAttackableScanner.GetTargets(transform, _selectAll, _attackResultsBuffer);
            foreach (var target in _attackResultsBuffer)
            {
                target.OnAttack();
            }
        }
        public void OnFinishAttack()
        {
            _isAttack = false;
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

        private void HandleRotation()
        {
            if (!_lockSwitch || _currentTarget == null) _player.HandleRotation(_moveInput);
            else _player.HandleRotation(_currentTarget.Transform.position - direction.position);
        }
        public void HandleWalk()
        {
            _player.HandleMove(_moveInput, walkSpeed);
        }
        public void HandleStopMove()
        {
            _player.HandleMove(_moveInput, 0);
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
        private readonly Transform _direction;
        
        public PlayerMovement(Rigidbody2D rb, Transform direction)
        {
            _rb = rb;
            _direction = direction;
        }
        public void HandleMove(Vector2 direction, float speed)
        {
            _rb.linearVelocity = direction * speed;
        }
        public void HandleRotation(Vector2 targetPosition)
        {
            _direction.FaceDirection2D(targetPosition);
        }
    }
}