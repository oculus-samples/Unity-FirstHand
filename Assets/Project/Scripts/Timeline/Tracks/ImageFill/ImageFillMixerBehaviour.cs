/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class ImageFillMixerBehaviour : PlayableBehaviour
    {
        private float _defaultFillAmount;
        private float _assignedFillAmount;
        private Image _trackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _trackBinding = playerData as Image;

            if (_trackBinding == null)
            {
                return;
            }

            if (!Mathf.Approximately(_trackBinding.fillAmount, _assignedFillAmount))
            {
                _defaultFillAmount = _trackBinding.fillAmount;
            }

            int inputCount = playable.GetInputCount();
            float blendedFillAmount = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<ImageFillBehaviour> inputPlayable = (ScriptPlayable<ImageFillBehaviour>)playable.GetInput(i);
                ImageFillBehaviour input = inputPlayable.GetBehaviour();
                blendedFillAmount += input.FillAmount * inputWeight;
            }

            _trackBinding.fillAmount = _assignedFillAmount = blendedFillAmount;
        }

        public override void OnGraphStop(Playable playable)
        {
            if (_trackBinding)
            {
                _trackBinding.fillAmount = _defaultFillAmount;
            }
        }
    }
}
