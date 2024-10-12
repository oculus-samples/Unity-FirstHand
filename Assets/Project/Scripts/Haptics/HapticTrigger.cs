// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Haptics;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Setup to call haptic clips in other places
    /// </summary>
    public class HapticTrigger : MonoBehaviour
    {
        [SerializeField]
        public float _amplitudeBoost = 1.0f;
        [SerializeField]
        private HapticClip _clip;
        [SerializeField]
        private Controller _hand;
        [SerializeField]
        private bool _loop;
        public bool Loop => _loop;

        public void PlayHaptic()
        {
            HandHaptics.Get(_hand).PlayHaptic(_clip, _loop, _amplitudeBoost);
        }

        public void StopHaptic()
        {
            HandHaptics.Get(_hand).Stop();
        }
    }
}
