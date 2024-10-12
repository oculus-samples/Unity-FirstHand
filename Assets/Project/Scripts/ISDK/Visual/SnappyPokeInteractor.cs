// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// When used in conjunciton with SnappyPokeLimiterVisual snaps the hand so the index finger is always above its candidtae
    /// </summary>
    public partial class SnappyPokeInteractor : PokeInteractor
    {
    }

    interface ISnapYourFingers
    {
        internal Vector3 GetHoverPoint(Vector3 origin);
        internal Vector3 GetTouchPoint(Vector3 origin);
    }
}
