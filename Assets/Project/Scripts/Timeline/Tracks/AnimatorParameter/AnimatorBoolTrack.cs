// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackColor(0.07843138f, 0.7075472f, 0.02499633f)]
    [TrackClipType(typeof(AnimatorBoolClip))]
    [TrackBindingType(typeof(Animator))]
    public class AnimatorBoolTrack : AnimatorParameterTrack<AnimatorBoolMixerBehaviour> { }

    public class AnimatorBoolMixerBehaviour : AnimatorParameterMixerBehaviour
    {
        protected override void AnimatorProcessFrame(Playable playable)
        {
            int inputCount = playable.GetInputCount();

            bool blendedValue = false;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);

                if (inputWeight > 0)
                {
                    blendedValue = true;
                }
            }

            _trackBinding.SetBool(_parameterID, blendedValue);
        }
    }
}
