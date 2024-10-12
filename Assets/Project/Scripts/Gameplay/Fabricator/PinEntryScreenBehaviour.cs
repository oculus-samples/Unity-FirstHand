// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the behaviour of the pin screen, InputXYZ methods are called by UnityEvents
    /// </summary>
    public class PinEntryScreenBehaviour : MonoBehaviour, IActiveState
    {
        private static readonly int PIN_LENGTH = 4;
        private static readonly string[] PIN_TEXT_OPTIONS =
            new string[] { "", "<mspace=1em>•", "<mspace=1em>••", "<mspace=1em>•••", "<mspace=1em>••••" };

        [SerializeField]
        ReferenceActiveState _powered;
        [SerializeField]
        private TextMeshProUGUI _pinEntryText;
        [SerializeField]
        private bool _requireEnterToSubmit = false;
        [SerializeField]
        private int[] _correctPin = new int[PIN_LENGTH];
        [SerializeField]
        private PlayableDirector _correctTimeline;
        [SerializeField]
        private PlayableDirector _incorrectTimeline;
        [SerializeField]
        private TMP_Text _codeAcceptedText;

        private int[] _enteredPin = new int[PIN_LENGTH];
        private int _currentIndex = 0;
        private int _attempts = 0;
        private bool _enteredPinCorrectly;

        public bool Active => _enteredPinCorrectly;
        private bool IsInteractable => _powered && !_enteredPinCorrectly && isActiveAndEnabled;

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

            _enteredPinCorrectly = match;

            if (++_attempts == 3)
            {
                _enteredPinCorrectly = true;
                _codeAcceptedText.text = LocalizedText.GetUIText("Close Enough");
            }

            (_enteredPinCorrectly ? _correctTimeline : _incorrectTimeline).Play();
        }

        public void InputClear()
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
