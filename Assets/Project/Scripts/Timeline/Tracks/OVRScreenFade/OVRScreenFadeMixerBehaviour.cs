// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class OVRScreenFadeMixerBehaviour : PlayableBehaviour
    {
        OVRScreenFade _screenFade;

        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (_screenFade == null)
            {
                _screenFade = UnityEngine.Object.FindObjectOfType<OVRScreenFade>();
            }

            if (_screenFade == null) return;

            int inputCount = playable.GetInputCount();

            float alpha = 0;
            Color color = _screenFade.fadeColor;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<OVRScreenFadeBehaviour> inputPlayable = (ScriptPlayable<OVRScreenFadeBehaviour>)playable.GetInput(i);
                OVRScreenFadeBehaviour input = inputPlayable.GetBehaviour();

                alpha += inputWeight * input.alpha;

                if (inputWeight > 0)
                {
                    color = input.color;
                }
            }

            _screenFade.SetExplicitFade(alpha);
            if (_screenFade.fadeColor != color)
            {
                _screenFade.fadeColor = color;
            }
        }
    }
}
