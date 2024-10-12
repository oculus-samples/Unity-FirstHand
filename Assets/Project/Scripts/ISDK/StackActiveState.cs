// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Tracks 2 sets of ActiveStates,
    /// if the most recent one to change is in the _activators list then this returns true
    /// if the most recent is in the _deactivators this returns false
    /// </summary>
    public class StackActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        ReferenceActiveState _emptyStack;
        [SerializeField]
        List<ReferenceActiveState> _activators = new List<ReferenceActiveState>();
        [SerializeField]
        List<ReferenceActiveState> _deactivators = new List<ReferenceActiveState>();

        List<(ReferenceActiveState, bool)> _stack = new List<(ReferenceActiveState, bool)>();

        public bool Active => _stack.Count == 0 ? _emptyStack : _stack[_stack.Count - 1].Item2;

        private void Update()
        {
            for (int i = _stack.Count - 1; i >= 0; i--)
            {
                if (!_stack[i].Item1.Active) _stack.RemoveAt(i);
            }

            AddNewStatesToStack(_activators, true);
            AddNewStatesToStack(_deactivators, false);
        }

        private void AddNewStatesToStack(List<ReferenceActiveState> list, bool value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i]) continue; // not active
                if (_stack.FindIndex(x => x.Item1.Equals(list[i])) != -1) continue; //active and not changed

                // became active
                _stack.Add((list[i], value));
            }
        }
    }
}
