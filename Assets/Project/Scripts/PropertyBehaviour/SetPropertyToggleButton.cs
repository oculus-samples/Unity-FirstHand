// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Updates StringProperty value based on a string value
    /// </summary>
    public class SetPropertyToggleButton : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private StringPropertyRef _property;

        public string OnValue;
        public string OffValue;

        [SerializeField]
        UserInteraction _userInteraction = UserInteraction.OnlyOn;

        Selectable _selectable;
        ToggleGroup _toggleGroup;

        public bool Active => _property.Value == OnValue;

        private void Awake()
        {
            _selectable = TryGetComponent<Button>(out var button) ? button : (Selectable)GetComponent<Toggle>();
        }

        private void Start()
        {
            _property.AssertNotNull();
            Assert.IsNotNull(_selectable);

            switch (_selectable)
            {
                case Button button:
                    button.onClick.AddListener(SetProperty);
                    break;
                case Toggle toggle:
                    _toggleGroup = gameObject.AddComponent<InternalToggleGroup>();
                    _toggleGroup.allowSwitchOff = true;
                    toggle.group = _toggleGroup;
                    _property.WhenChanged += UpdateToggle;
                    UpdateToggle();
                    break;
                default:
                    throw new Exception();
            }
        }

        private void UpdateToggle()
        {
            var toggle = _selectable as Toggle;
            bool shouldBeOn = _property.Value == OnValue;

            _toggleGroup.allowSwitchOff = true;

            toggle.onValueChanged.RemoveListener(SetProperty);
            toggle.isOn = shouldBeOn;
            toggle.onValueChanged.AddListener(SetProperty);

            _toggleGroup.allowSwitchOff = _userInteraction == UserInteraction.OnOff;
        }

        private void SetProperty() => SetProperty(true);
        private void SetProperty(bool _)
        {
            bool isOn = _selectable is Toggle toggle ? toggle.isOn : true;
            StringPropertyBehaviourRef.SetPropertyWithString(_property, isOn ? OnValue : OffValue);
        }

        /// <summary>
        /// Prevents ToggleGroup from calling EnsureValidState as to allow
        /// us to have full control over the toggle component
        /// </summary>
        private class InternalToggleGroup : ToggleGroup
        {
            protected override void Start() { }

            protected override void OnEnable() { }
        }

        enum UserInteraction
        {
            OnOff,
            OnlyOn
        }
    }
}
