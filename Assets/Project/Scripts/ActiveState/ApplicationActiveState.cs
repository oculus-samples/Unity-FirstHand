// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true if the App is in focus
    /// </summary>
    public class ApplicationActiveState : MonoBehaviour, IActiveState
    {
        private bool _isFocused = true;

        public bool Active => _isFocused;

        private void OnApplicationFocus(bool hasFocus)
        {
            _isFocused = hasFocus;
        }
    }
}
