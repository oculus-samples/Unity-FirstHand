// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Updates the UI based on completed drone parts
    /// </summary>
    public class DroidSchematicUIManager : ActiveStateObserver, IActiveState
    {
        [SerializeField]
        private UICanvas _module, _completed;

        bool IActiveState.Active => Active;

        protected override void HandleActiveStateChanged()
        {
            _module.IsShown = !Active;
            _completed.IsShown = Active;
        }
    }
}
