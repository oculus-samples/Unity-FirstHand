// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Sets the value of a ProgressTracker when an ActiveState becomes true
    /// </summary>
    public class ConditionalSetProgress : ActiveStateObserver, IActiveState
    {
        [SerializeField]
        private ProgressTracker _progressTracker;
        [SerializeField]
        private int _progress;

        bool IActiveState.Active => _progressTracker.Progress >= _progress;

        protected override void Start()
        {
            base.Start();
            HandleActiveStateChanged();
        }

        protected override void HandleActiveStateChanged()
        {
            if (Active) ApplyProgress();
        }

        public void ApplyProgress()
        {
            _progressTracker.SetProgress(_progress);
        }
    }
}
