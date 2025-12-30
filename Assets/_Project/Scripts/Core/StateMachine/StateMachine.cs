using System;
using System.Collections.Generic;

namespace Ronin.Core
{
    public class StateMachine
    {
        private readonly Dictionary<Type, StateNode> _nodes = new();
        public IEnumerable<StateNode> AllNodes => _nodes.Values;
        private readonly HashSet<ITransition> _anyTransitions = new();
        private StateNode _current;

        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                ChangeState(transition.To);
            }
            
            _current.State?.Update();
        }
        public void FixedUpdate()
        {
            _current.State?.FixedUpdate();
        }
        public void AddState(IState state)
        {
            GetOrAddNodes(state);
        }
        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            if (from == to) return;
            GetOrAddNodes(from).AddTransition(to, condition);
        }
        public void AddAnyTransition(IState to, IPredicate condition)
        {
            _anyTransitions.Add(new Transition(to, condition));
        }
        public void SetInitState(IState state)
        {
            _current = GetOrAddNodes(state);
            _current.State?.OnEnter();
        }
        private StateNode GetOrAddNodes(IState state)
        {
            var node = _nodes.GetValueOrDefault(state.GetType());
            if (node != null) return node;
            
            _nodes[state.GetType()] = new StateNode(state);
            node = _nodes[state.GetType()];
            return node;
        }

        private ITransition GetTransition()
        {
            foreach (var transition in _anyTransitions)
            {
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
            }

            foreach (var transition in _current.Transitions)
            {
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
            }

            return null;
        }
        private void ChangeState(IState state)
        {
            if (state == _current.State) return;

            var prevState = _current.State;
            var nextState = state;
    
            prevState.OnExit();
            nextState.OnEnter();
            _current = GetOrAddNodes(state);
        }

        public class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }
            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }
            public void AddTransition(IState to, IPredicate condition)
            {
                var transition = new Transition(to, condition);
                Transitions.Add(transition);
            }
        }
    }
}