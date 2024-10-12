// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Haptics;
using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Reference class for Distance Haptic Source
    /// </summary>
    public class DistanceHapticSourceRef : MonoBehaviour, IDistanceHapticSource
    {
        [SerializeField]
        ReferenceDistanceHapticSource _source;

        public Vector3 Position => _source.Position;
        public HapticClip Clip => _source.Clip;
        public bool Loop => _source.Loop;
        public float Amplitude => _source.Amplitude;
        public float Frequency => _source.Frequency;

        public event Action WhenHaptics
        {
            add => _source.WhenHaptics += value;
            remove => _source.WhenHaptics -= value;
        }
    }
}
