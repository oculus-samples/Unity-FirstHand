/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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
