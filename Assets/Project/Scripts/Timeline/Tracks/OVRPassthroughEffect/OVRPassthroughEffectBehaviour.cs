// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class OVRPassthroughEffectBehaviour : PlayableBehaviour
    {
        public float brightness;
        public float contrast;
        public float saturation;
    }
}
