/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
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
