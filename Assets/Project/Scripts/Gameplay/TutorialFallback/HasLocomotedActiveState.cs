// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Locomotion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true if the player has locomoted
    /// </summary>
    public class HasLocomotedActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private List<TeleportInteractor> _playerLocomotor;
        public UnityEvent WhenPlayerHasLocomoted;

        private bool _hasLocomoted;

        public bool Active => _hasLocomoted;

        private void Start()
        {
            _playerLocomotor.ForEach(x => x.WhenInteractableSelected.Action += HandleSelect);
        }

        private void HandleSelect(TeleportInteractable obj)
        {
            if (!obj.AllowTeleport) return;

            _hasLocomoted = true;
            WhenPlayerHasLocomoted?.Invoke();
        }
    }
}
