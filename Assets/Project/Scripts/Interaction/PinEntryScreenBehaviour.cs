/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
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
