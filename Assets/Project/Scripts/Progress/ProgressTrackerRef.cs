// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// A reference/proxy to a ProgressTracker, to make it easier to work in prefabs
    /// </summary>
    public class ProgressTrackerRef : ProgressTracker
    {
        [SerializeField]
        ProgressTracker _progressTracker;
        [SerializeField]
        int _offset = 0;

        public override int Progress
        {
            get => _progressTracker.Progress - _offset;
            protected set => _progressTracker.SetProgress(value + _offset);
        }

        public override event Action WhenChanged
        {
            add => _progressTracker.WhenChanged += value;
            remove => _progressTracker.WhenChanged -= value;
        }

        public override void SetProgress(int value, bool allowRegression)
        {
            _progressTracker.SetProgress(value + _offset, allowRegression);
        }

        public override string ToString()
        {
            if (!_progressTracker) return base.ToString();

            return $"Progress: {_progressTracker.Progress}{Environment.NewLine}" +
                $"Relative Progress: {Progress}";
        }
    }
}
