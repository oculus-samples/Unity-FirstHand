// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Component to hold a surface tag, used to select what sound to play when landed on during a locomotion event
    /// </summary>
    public class SurfaceType : MonoBehaviour
    {
        [SerializeField]
        private SurfaceTag _preset;
        public SurfaceTag Preset => _preset;
    }
}
