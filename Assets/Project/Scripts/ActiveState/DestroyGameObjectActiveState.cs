// Copyright (c) Meta Platforms, Inc. and affiliates.

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// If active returns true destroy the gameobject
    /// </summary>
    public class DestroyGameObjectActiveState : ActiveStateObserver
    {
        protected override void HandleActiveStateChanged()
        {
            if (Active)
                Destroy(gameObject);
        }
    }
}
