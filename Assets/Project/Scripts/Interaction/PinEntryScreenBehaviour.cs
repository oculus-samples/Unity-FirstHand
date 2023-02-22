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

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the behaviour of the pin screen, InputXYZ methods are called by UnityEvents
    /// </summary>
    public class PinEntryScreenBehaviour : MonoBehaviour
    {
        private static readonly int PIN_LENGTH = 4;

        [SerializeField]
        private TextMeshProUGUI _pinEntryText;
        [SerializeField]
        private bool _requireEnterToSubmit;
        [SerializeField]
        private int[] _correctPin = new int[PIN_LENGTH];
        [SerializeField]
        private PlayableDirector _correctTimeline;
        [SerializeField]
        private PlayableDirector _incorrectTimeline;

        private int[] _enteredPin = new int[PIN_LENGTH];
        private int _currentIndex = 0;
        private int _attempts = 0;
        private bool _enteredPinCorrectly;

        private bool IsInteractable => !_enteredPinCorrectly && isActiveAndEnabled;

        public void InputKey(int key)
        {
            if (!IsInteractable || _currentIndex >= PIN_LENGTH)
            {
                return;
            }

            _enteredPin[_currentIndex++] = key;
            UpdateText();

            if (_currentIndex == PIN_LENGTH && !_requireEnterToSubmit)
            {
                InputEnter();
            }
        }

        public void InputBackspace()
        {
            if (!IsInteractable || _currentIndex <= 0)
            {
                return;
            }

            _currentIndex--;
            UpdateText();
        }

        public void InputEnter()
        {
            if (!IsInteractable || _currentIndex < PIN_LENGTH)
            {
                return;
            }

            bool match = ((IStructuralEquatable)_enteredPin).Equals(_correctPin, StructuralComparisons.StructuralEqualityComparer);
            InputClear();

            _enteredPinCorrectly = match || ++_attempts == 3;
            (match ? _correctTimeline : _incorrectTimeline).Play();
        }

        private void InputClear()
        {
            _enteredPinCorrectly = false;
            _currentIndex = 0;
            UpdateText();
        }

        private void UpdateText()
        {
            string display = "<mspace=1em>";
            for (int i = 0; i < _currentIndex; i++)
            {
                display += _enteredPin[i];
            }
            _pinEntryText.text = display;
        }
    }
}
