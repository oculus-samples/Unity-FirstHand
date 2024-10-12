// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Toggles ability to enable SeatedMode
    /// </summary>
    public class SeatedModeToggle : MonoBehaviour
    {
        private Toggle _toggle;

        void Start()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.isOn = SeatedMode.IsOn;
            _toggle.onValueChanged.AddListener(UpdateSeatedMode);
        }

        private void UpdateSeatedMode(bool _)
        {
            SeatedMode.SetSeatedMode(_toggle.isOn);
        }
    }
}
