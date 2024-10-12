// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the scaling of the collectable components in the Street scene as they enter the inventory
    /// </summary>
    public class ComponentInteracted : MonoBehaviour
    {
        [SerializeField, Tooltip("This is for MR Build A Drone Minigame")]
        private SnapInteractable _parentInteractable;
        [SerializeField]
        private bool _repairDroneMode;

        private Vector3 _inventoryScale = new(0.36f, 0.36f, 0.36f);

        private SnapInteractor _snapInteractor;
        private InteractionTracker _interactionTracker;

        private void Start()
        {
            _snapInteractor = GetComponentInChildren<SnapInteractor>();

            _interactionTracker = new InteractionTracker(GetComponent<IPointableElement>());
            _interactionTracker.WhenSelectRemoved += HandleReleased;
            _interactionTracker.WhenChanged += HandleSnapInteractable;
        }

        private void HandleSnapInteractable()
        {
            bool inSnapZone = _interactionTracker.TryGetSelectingInteractor<SnapInteractor>(out var snap);

            if (!inSnapZone)
                return;

            transform.SetParent(snap.SelectedInteractable.transform);

            if (!_repairDroneMode)
                TweenRunner.Tween(transform.localScale, _inventoryScale, 0.3f, x => transform.localScale = x).SetID(this);
        }

        private void HandleReleased(IInteractorView interactor)
        {
            bool willSnapAutomatically = _snapInteractor.HasCandidate;
            if (willSnapAutomatically) return;

            bool wasReleasedByHand = InteractionTracker.IsGrabInteractor(interactor);
            bool isNoLongerHeld = !_interactionTracker.IsGrabbed();

            if (wasReleasedByHand && isNoLongerHeld)
            {
                var snap = _repairDroneMode ? _parentInteractable : FindSnapInteractable();

                if (snap != null)
                {
                    _snapInteractor.ForceSelect(snap);
                }
                else
                {
                    Debug.Log("no snap interactable!");
                }
            }
        }

        private SnapInteractable FindSnapInteractable()
        {
            return FindSnapInteractable(_snapInteractor);
        }

        private static SnapInteractable FindSnapInteractable(SnapInteractor _snapInteractor1)
        {
            var interactables = SnapInteractable.Registry.List();

            foreach (SnapInteractable interactable in interactables)
            {
                if (interactable.CanBeSelectedBy(_snapInteractor1)) return interactable;
            }

            return null;
        }
    }
}
