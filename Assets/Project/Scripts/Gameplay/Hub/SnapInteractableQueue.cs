// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Queues the selected item in a position, swapping things out if its in said position
    /// </summary>
    public class SnapInteractableQueue : MonoBehaviour
    {
        [SerializeField]
        private SnapInteractable _snapInteractable;
        [SerializeField]
        private int _maxCount = 1;

        private InteractionTracker _interactionTracker;

        private void Start()
        {
            _interactionTracker = new InteractionTracker(_snapInteractable);
            _interactionTracker.WhenSelectAdded += HandleSelect;
        }

        private void HandleSelect(IInteractorView obj)
        {
            while (_interactionTracker.SelectingInteractors.Count > _maxCount)
                _interactionTracker.ForceUnselect(0);
        }
    }
}
