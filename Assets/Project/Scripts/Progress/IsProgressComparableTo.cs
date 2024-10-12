// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// An ActiveState that becomes Active when the comparison's condition is met
    /// </summary>
    public class IsProgressComparableTo : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private ProgressTracker _progress;
        [SerializeField]
        private CompareFunction _comparison = CompareFunction.Equal;
        [SerializeField]
        private int _compareToValue;

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
            _active = Compare(_comparison, _progress.Progress, _compareToValue);
        }

        private static bool Compare(CompareFunction comparison, int value, int compareTo)
        {
            switch (comparison)
            {
                case CompareFunction.Disabled: return false;
                case CompareFunction.Never: return false;
                case CompareFunction.Always: return true;
                case CompareFunction.Less: return value < compareTo;
                case CompareFunction.Equal: return value == compareTo;
                case CompareFunction.LessEqual: return value <= compareTo;
                case CompareFunction.Greater: return value > compareTo;
                case CompareFunction.NotEqual: return value != compareTo;
                case CompareFunction.GreaterEqual: return value >= compareTo;
                default: throw new Exception();
            }
        }
    }
}
