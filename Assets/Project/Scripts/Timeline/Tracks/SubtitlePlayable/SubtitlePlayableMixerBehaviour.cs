// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.ComprehensiveSample;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class SubtitlePlayableMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying) return;

            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<SubtitlePlayableBehaviour> inputPlayable = (ScriptPlayable<SubtitlePlayableBehaviour>)playable.GetInput(i);
                SubtitlePlayableBehaviour input = inputPlayable.GetBehaviour();

                if (inputWeight > 0f && !input.Mute)
                {
                    //TODO not call if sub is already showing
                    var duration = input.DurationOverride > 0 ? input.DurationOverride : (float)inputPlayable.GetDuration();
                    SubtitleManager.Instance.ShowSubtitle(input.String, input.Preset, duration, input.Transform);
                }
            }
        }
    }
}
