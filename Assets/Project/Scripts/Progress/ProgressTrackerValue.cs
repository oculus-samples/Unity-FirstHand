// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Progress Tracker with a real value (as opposed to a ProgressTrackerRef)
    /// </summary>
    public class ProgressTrackerValue : ProgressTracker
    {
        public override int Progress { get => _progress; protected set => _progress = value; }

        [SerializeField, Delayed]
        private int _progress;

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                var temp = _progress;
                _progress = 0;
                SetProgress(temp);
            }
        }
    }
}
