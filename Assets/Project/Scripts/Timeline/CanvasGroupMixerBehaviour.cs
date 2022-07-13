/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class CanvasGroupMixerBehaviour : PlayableBehaviour
    {
        private float _defaultAlpha = -1;
        private float _assignedAlpha = -1;
        private CanvasGroup _trackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _trackBinding = playerData as CanvasGroup;

            if (_trackBinding == null)
                return;

            if (!Mathf.Approximately(_trackBinding.alpha, _assignedAlpha))
                _defaultAlpha = _trackBinding.alpha;

            int inputCount = playable.GetInputCount();
            float blendedAlpha = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<CanvasGroupBehaviour> inputPlayable = (ScriptPlayable<CanvasGroupBehaviour>)playable.GetInput(i);
                CanvasGroupBehaviour input = inputPlayable.GetBehaviour();
                blendedAlpha += input.Alpha * inputWeight;
            }

            _assignedAlpha = blendedAlpha;
            _trackBinding.alpha = _assignedAlpha;
        }

        public override void OnGraphStop(Playable playable)
        {
            if (_trackBinding)
            {
                _trackBinding.alpha = _defaultAlpha;
            }
        }
    }
}
