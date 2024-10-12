// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// This component can be attached to a trigger volume, and will enable the drone's "Gun" and/or "Target" actions
    /// when the drone is within that volume.
    /// </summary>
    public class DroneActionVolume : MonoBehaviour
    {
        public bool CanDeployGunInVolume;
        public bool CanDeployTargetInVolume;
    }
}
