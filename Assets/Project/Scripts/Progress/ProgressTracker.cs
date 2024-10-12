// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// A referenceable integer that represents the users progress, combine with IsProgressComparableTo
    /// Note progress cannot be rewound except by ResetProgress
    /// TODO implement as IProperty<int>
    /// </summary>
    public abstract class ProgressTracker : MonoBehaviour
    {
        public abstract int Progress { get; protected set; }

        public virtual event Action WhenChanged;

        public void ResetProgress()
        {
            SetProgress(0, true);
        }

        // called by UnityEvent
        public void Next()
        {
            SetProgress(Progress + 1);
        }

        public void SetProgress(int value)
        {
            SetProgress(value, false);
        }

        public virtual void SetProgress(int value, bool allowRegression)
        {
            Debug.Log($"Set Prog: {value} {Progress} {allowRegression}");

            if (value < Progress && !allowRegression) return;
            if (value == Progress) { return; }

            Progress = value;

#if UNITY_EDITOR
            if (WhenChanged != null)
            {
                var callbacks = WhenChanged.GetInvocationList();
                for (int i = 0; i < callbacks.Length; i++)
                {
                    try { callbacks[i].DynamicInvoke(); }
                    catch (Exception e) { Debug.LogException(e); }
                }
            }
#else
            WhenChanged?.Invoke();
#endif
        }
    }
}
