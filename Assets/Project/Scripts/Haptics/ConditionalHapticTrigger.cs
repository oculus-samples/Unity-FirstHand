// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Plays a haptic through HapticTrigger on the condition
    /// that an active state returns true
    /// </summary>
    [RequireComponent(typeof(HapticTrigger))]
    public class ConditionalHapticTrigger : ActiveStateObserver
    {
        protected override void HandleActiveStateChanged()
        {
            HapticTrigger hapticTrigger = GetComponent<HapticTrigger>();
            if (Active)
            {
                hapticTrigger.PlayHaptic();
            }
            else if (hapticTrigger.Loop)
            {
                hapticTrigger.StopHaptic();
            }
        }
    }

}
