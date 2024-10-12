// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class OVRPassthroughEffectMixerBehaviour : PlayableBehaviour
    {
        OVRPassthroughLayer _trackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying) return;

            _trackBinding = playerData as OVRPassthroughLayer;

            if (_trackBinding == null) _trackBinding = OVRPassthroughEffectTrack.Instance;
            if (_trackBinding == null) return;

            int inputCount = playable.GetInputCount();

            float brightness = 0f;
            float contrast = 0f;
            float saturation = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<OVRPassthroughEffectBehaviour> inputPlayable = (ScriptPlayable<OVRPassthroughEffectBehaviour>)playable.GetInput(i);
                OVRPassthroughEffectBehaviour input = inputPlayable.GetBehaviour();

                brightness += input.brightness * inputWeight;
                contrast += input.contrast * inputWeight;
                saturation += input.saturation * inputWeight;
            }

            _trackBinding.SetBrightnessContrastSaturation(brightness, contrast, saturation);
        }
    }
}
