// Copyright (c) Meta Platforms, Inc. and affiliates.

/*
 *Copyright(c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
*you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 *Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Controls Rectangle shadow based on UIState
    /// </summary>
    public class UIStateRectangleShadow : UIStateVisual
    {
        [SerializeField]
        private float _duration = 0.2f;

        [SerializeField]
        UIStateValues<float> _shadowVisibility;

        private Rectangle _rectangle;

        protected override void OnEnable()
        {
            _rectangle = GetComponent<Rectangle>();
            base.OnEnable();
        }

        protected override void UpdateVisual(IUIState uiState, bool animate)
        {
            var shadows = _shadowVisibility.GetValue(uiState.State, 1);
            TweenRunner.Tween(_rectangle.DropShadowVisibility, shadows, _duration, x => _rectangle.DropShadowVisibility = x)
                .SetID(this)
                .Skip(!animate);
        }
    }
}
