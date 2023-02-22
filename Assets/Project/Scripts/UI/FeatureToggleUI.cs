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

using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class FeatureToggleUI : MonoBehaviour, IActiveState
    {
        private bool _enabled;


        [SerializeField] private TMP_Text _promptText;
        [SerializeField] private string _promptStringWhenEnabled;
        [SerializeField] private string _promptStringWhenDisabled;


        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private string _buttonStringWhenEnabled;
        [SerializeField] private string _buttonStringWhenDisabled;

        private void Start()
        {
            Assert.IsNotNull(_promptText);
            Assert.IsNotNull(_buttonText);
            UpdateText();
        }

        public void ToggleFeature()
        {
            _enabled = !_enabled;
            UpdateText();
        }

        private void UpdateText()
        {
            _promptText.text = _enabled ? _promptStringWhenEnabled : _promptStringWhenDisabled;
            _buttonText.text = _enabled ? _buttonStringWhenEnabled : _buttonStringWhenDisabled;
        }

        public bool Active => _enabled;
    }
}
