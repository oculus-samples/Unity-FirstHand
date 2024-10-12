// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class LightIntensityMixerBehaviour : PlayableBehaviour
    {
        float _defaultIntensity = 0;
        float _assignedIntensity;
        Light _trackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _trackBinding = playerData as Light;

            if (_trackBinding == null)
                return;

            int inputCount = playable.GetInputCount();

            float blendedIntensity = 0f;
            float totalWeight = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<LightIntensityBehaviour> inputPlayable = (ScriptPlayable<LightIntensityBehaviour>)playable.GetInput(i);
                LightIntensityBehaviour input = inputPlayable.GetBehaviour();

                blendedIntensity += input.intensity * inputWeight;
                totalWeight += inputWeight;
            }

            _assignedIntensity = blendedIntensity + _defaultIntensity * (1f - totalWeight);
            _trackBinding.intensity = _assignedIntensity;
        }
    }
}
