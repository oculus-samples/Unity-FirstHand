// Copyright (c) Meta Platforms, Inc. and affiliates.

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Active state that stays true
    /// </summary>
    public class SingleFireActiveState : ActiveStateObserver, IActiveState
    {
        bool _fired = false;

        bool IActiveState.Active => _fired;

        protected override void HandleActiveStateChanged()
        {
            if (!_fired && Active) { _fired = true; }
        }
    }
}
