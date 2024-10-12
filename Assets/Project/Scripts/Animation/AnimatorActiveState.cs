// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Adapts an Animator bool parameter to an IActiveState
    /// </summary>
    public class AnimatorActiveState : ActiveStateObserver, IActiveState
    {
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private string _parameter;

        private int _id;

        bool IActiveState.Active => _animator.GetBool(_id);

        protected override void Reset()
        {
            base.Reset();
            _animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            _id = Animator.StringToHash(_parameter);
        }

        protected override void Start()
        {
            base.Start();
            HandleActiveStateChanged();
        }

        // Called by UnityEvents
        public void Set(bool value)
        {
            _animator.SetBool(_id, value);
        }

        protected override void HandleActiveStateChanged()
        {
            _animator.SetBool(_id, Active);
        }
    }
}
