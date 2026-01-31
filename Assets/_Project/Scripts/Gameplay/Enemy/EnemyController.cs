using System.Collections.Generic;
using Ronin.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Ronin.Gameplay
{
    public class EnemyController : MonoBehaviour, ILockable, IAttackable
    {
        Transform ILockable.Transform => transform;

        bool ILockable.CanLock => true;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private PlayerController player;

        [SerializeField] private List<IMadable> priorityList;
        [SerializeField] private float visualAngle;
        [SerializeField] private float visualDistance;
        public float DetectorRadius => visualDistance;
        private StateMachine _stateMachine;
        
        private TargetScanner<IMadable> _targetScanner;
        private CircleScanTargets<IMadable> _circleScanner;
        private SelectSector _selectSector;
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            
            _selectSector = new SelectSector(priorityList, visualAngle);
            _circleScanner = new CircleScanTargets<IMadable>(visualDistance);
            _targetScanner = new TargetScanner<IMadable>(_circleScanner);

            _stateMachine = new StateMachine();
            IState idleState = new IdleState(this);
            _stateMachine.AddState(idleState);
            IState trackState = new TrackState(this);
            _stateMachine.AddState(trackState);
            _stateMachine.SetInitState(idleState);

            At(idleState, trackState,  new FuncPredicate(() => (_targetScanner.GetTarget(transform, _selectSector) != null)) );
        }
        private void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        // Update is called once per frame
        void Update()
        {
            _stateMachine.Update();
        }
        
        void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        public void OnAttack()
        {
            
        }

        public void DoRotate()
        {
            transform.Rotate(transform.forward, 10f);
        }
        
        public void FollowPlayer()
        {
            agent.SetDestination(player.transform.position);
        }
        
    }

}