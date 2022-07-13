/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

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
