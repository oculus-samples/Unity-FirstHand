// Copyright (c) Meta Platforms, Inc. and affiliates.

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
        public bool _isMask;

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

            if (_isMask)
            {
                _trackBinding.enabled = _trackBinding.fillAmount < 1;
            }
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
