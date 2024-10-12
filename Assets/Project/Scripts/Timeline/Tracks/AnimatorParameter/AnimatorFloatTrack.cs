// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackColor(0.07843138f, 0.7075472f, 0.02499633f)]
    [TrackClipType(typeof(AnimatorFloatClip))]
    [TrackBindingType(typeof(Animator))]
    public class AnimatorFloatTrack : AnimatorParameterTrack<AnimatorFloatMixerBehaviour> { }

    public class AnimatorFloatMixerBehaviour : AnimatorParameterMixerBehaviour
    {
        protected override void AnimatorProcessFrame(Playable playable)
        {
            int inputCount = playable.GetInputCount();

            float blendedValue = 0;
            float totalWeight = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<AnimatorFloatBehaviour> inputPlayable = (ScriptPlayable<AnimatorFloatBehaviour>)playable.GetInput(i);
                AnimatorFloatBehaviour input = inputPlayable.GetBehaviour();

                blendedValue += input.value * inputWeight;
                totalWeight += inputWeight;
            }

            _trackBinding.SetFloat(_parameterID, blendedValue);
        }
    }
}
