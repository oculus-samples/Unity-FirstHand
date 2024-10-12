// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackColor(0.07843138f, 0.7075472f, 0.02499633f)]
    [TrackClipType(typeof(AnimatorIntClip))]
    [TrackBindingType(typeof(Animator))]
    public class AnimatorIntTrack : AnimatorParameterTrack<AnimatorIntMixerBehaviour> { }

    public class AnimatorIntMixerBehaviour : AnimatorParameterMixerBehaviour
    {
        protected override void AnimatorProcessFrame(Playable playable)
        {
            int inputCount = playable.GetInputCount();

            int blendedValue = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<AnimatorIntBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                if (inputWeight > 0)
                {
                    blendedValue = input.value;
                }
            }

            _trackBinding.SetInteger(_parameterID, blendedValue);
        }
    }
}
