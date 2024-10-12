// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Profiling;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Checks for an ActiveState to have changed in Update
    /// Similar to ActiveStateSelector but using ReferenceActiveState and designed for inheritance
    /// </summary>
    public abstract class ActiveStateObserver : MonoBehaviour
    {
        [SerializeField]
        protected ReferenceActiveState _activeState;
        [SerializeField]
        private When _updateMode = When.Update;

        string _profilerTag1;
        string _profilerTag2;

        protected bool Active { get; private set; }

        protected virtual void Reset()
        {
            var activeStates = GetComponents<IActiveState>();
            for (int i = 0; i < activeStates.Length; i++)
            {
                IActiveState state = activeStates[i];
                if ((object)state != this)
                {
                    _activeState.InjectActiveState(state);
                    return;
                }
            }
        }

        protected virtual void Start()
        {
            _activeState.AssertNotNull($"{name} ({GetType()}) requires an IActiveState assigned");
            Active = _activeState.Active;
        }

        protected virtual void Update()
        {
            if ((_updateMode & When.Update) != 0)
            {
                UpdateActive();
            }
        }

        protected virtual void LateUpdate()
        {
            if ((_updateMode & When.LateUpdate) != 0)
            {
                UpdateActive();
            }
        }

        private void UpdateActive()
        {
            if (_profilerTag1 == null) _profilerTag1 = $"{GetType().Name}.UpdateActive";
            Profiler.BeginSample(_profilerTag1);
            if (Active != _activeState.Active)
            {
                Active = !Active;
                if (_profilerTag2 == null) _profilerTag2 = $"{GetType().Name}.HandleActiveStateChanged";
                Profiler.BeginSample(_profilerTag2);
                HandleActiveStateChanged();
                Profiler.EndSample();
            }
            Profiler.EndSample();
        }

        protected abstract void HandleActiveStateChanged();

        [System.Flags]
        public enum When
        {
            Update = 1 << 0,
            LateUpdate = 1 << 1
        }
    }
}
