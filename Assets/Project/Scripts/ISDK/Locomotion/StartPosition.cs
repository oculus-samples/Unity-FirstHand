// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Locomotion;
using System;
using System.Collections;
using UnityEngine;
using Rotation = Oculus.Interaction.Locomotion.LocomotionEvent.RotationType;
using Translation = Oculus.Interaction.Locomotion.LocomotionEvent.TranslationType;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Positions the player in Start
    /// Useful if the player did a 180 turn while starting the game and we want them to start facing something
    /// </summary>
    public class StartPosition : MonoBehaviour, ILocomotionEventBroadcaster
    {
        [SerializeField, Optional]
        Transform _pose;

        public event Action<LocomotionEvent> WhenLocomotionPerformed;

        private IEnumerator Start()
        {
            yield return null; //delayed to allow locomotor setup
            if (_pose != null)
            {
                var locomotion = new LocomotionEvent(0, _pose.GetPose(), Translation.Absolute, Rotation.Absolute);
                WhenLocomotionPerformed?.Invoke(locomotion);
            }
        }
    }
}
