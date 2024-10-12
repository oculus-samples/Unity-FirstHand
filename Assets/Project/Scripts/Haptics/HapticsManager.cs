// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Add a haptics manager to an object in your scene as it is required
    /// for haptics to work
    /// </summary>
    public class HapticsManager : MonoBehaviour
    {
        private static HapticsManager _instance;
        public static HapticsManager Instance => _instance ? _instance : _instance = FindAnyObjectByType<HapticsManager>();

        [SerializeField]
        public float globalAmplitude = 1.0f;
    }
}
