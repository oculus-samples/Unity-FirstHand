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
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Maps an ActiveState onto a UGUI Canvas Group or Selectables .interactable property
    /// </summary>
    [DisallowMultipleComponent]
    public class ActiveStateUIInteractable : ActiveStateObserver
    {
        [SerializeField]
        private bool _controlBlocksRaycast;

        private bool _hasCanvasGroup;
        private bool _hasSelectable;

        private CanvasGroup _canvasGroup;
        private Selectable _selectable;

        protected override void Start()
        {
            base.Start();
            _hasCanvasGroup = TryGetComponent(out _canvasGroup);
            _hasSelectable = TryGetComponent(out _selectable);
            Assert.IsTrue(_hasCanvasGroup || _hasSelectable, $"{nameof(ActiveStateUIInteractable)} should have a CanvasGroup or Selectable component");
        }

        protected override void HandleActiveStateChanged()
        {
            if (_hasSelectable) { _selectable.interactable = Active; }
            if (_hasCanvasGroup) { _canvasGroup.interactable = Active; }
            if (_hasCanvasGroup && _controlBlocksRaycast) { _canvasGroup.blocksRaycasts = Active; }
        }
    }
}
