// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class UICanvasMixerBehaviour : PlayableBehaviour
    {
        UICanvas _trackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _trackBinding = playerData as UICanvas;
            if (_trackBinding == null) return;

            bool show = false;
            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                if (!Mathf.Approximately(inputWeight, 0f))
                {
                    show = true;
                    break;
                }
            }

            _trackBinding.IsShown = show;

            if (Application.isPlaying != true)
            {
                CanvasGroup group = _trackBinding.GetComponent<CanvasGroup>();
                group.alpha = _trackBinding.IsShown ? 1f : 0f;
            }
        }
    }
}
