// Copyright (c) Meta Platforms, Inc. and affiliates.

using TMPro;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class TextMeshProPlayableMixerBehaviour : PlayableBehaviour
    {
        TMP_Text _trackBinding;

        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _trackBinding = playerData as TMP_Text;

            if (!_trackBinding)
                return;

            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<TextMeshProPlayableBehaviour> inputPlayable = (ScriptPlayable<TextMeshProPlayableBehaviour>)playable.GetInput(i);
                TextMeshProPlayableBehaviour input = inputPlayable.GetBehaviour();

                // Use the above variables to process each frame of this playable.
                if (inputWeight > 0f)
                {
                    _trackBinding.text = LocalizedText.GetSubtitle(input.String);
                    return;
                }
            }

            _trackBinding.text = string.Empty;
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (_trackBinding)
            {
                _trackBinding.text = string.Empty;
            }
        }
    }
}
