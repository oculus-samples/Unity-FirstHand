// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true when the progress tracker's value matches the range e.g. "1-5, 8, 11-13, >=15"
    /// </summary>
    public class IsProgressInRange : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private ProgressTracker _progress;
        [SerializeField]
        private FloatRanges _range;

        private bool _active;
        public bool Active => _active;

        private void Awake()
        {
            UpdateActive();
            _progress.WhenChanged += UpdateActive;
        }

        private void OnDestroy()
        {
            _progress.WhenChanged -= UpdateActive;
        }

        private void UpdateActive()
        {
            _active = _range.Contains(_progress.Progress);
        }

        /// <summary>
        /// Tries to set the pogress to a value that will result in thie being Active
        /// </summary>
        // Called by UnityEvents
        [ContextMenu("Try Set Progress")]
        public void TrySetProgress()
        {
            var min = (int)_range.GetMin();
            if (_progress.Progress < min)
            {
                _progress.SetProgress(min);
            }
        }
    }
}
