// Copyright (c) Meta Platforms, Inc. and affiliates.

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Links the visibility of HandVisuals to an active state
    /// e.g. when gloves are worn can be used to hide the oculus hands
    /// </summary>
    public class ConditionalHandVisual : ActiveStateObserver
    {
        HandVisual _visual;

        protected override void Start()
        {
            base.Start();
            _visual = GetComponent<HandVisual>();
            HandleActiveStateChanged();
        }

        protected override void HandleActiveStateChanged()
        {
            _visual.ForceOffVisibility = !Active;
        }
    }
}
