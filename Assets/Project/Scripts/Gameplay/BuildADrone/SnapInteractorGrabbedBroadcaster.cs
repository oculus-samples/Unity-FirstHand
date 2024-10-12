// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Broadcasts SnapInteractor events
    /// </summary>
    public class SnapInteractorGrabbedBroadcaster : MonoBehaviour
    {
        [SerializeField]
        SnapInteractor _snapInteractor;
        InteractionTracker _interactionTracker;

        bool _grabbed = false;

        private void Awake()
        {
            _interactionTracker = new InteractionTracker(_snapInteractor.PointableElement);
            _interactionTracker.WhenChanged += BroadcastGrab;
        }

        private void BroadcastGrab()
        {
            bool grabbed = _interactionTracker.IsGrabbed();
            if (grabbed != _grabbed)
            {
                _grabbed = grabbed;

                var interactables = SnapInteractable.Registry.List();
                foreach (SnapInteractable interactable in interactables)
                {
                    if (interactable.TryGetComponent<ISnapInteractorGrabbedBroadcastReciever>(out var receiver))
                    {
                        receiver.HandleSnapInteractorGrabbed(_snapInteractor, grabbed);
                    }
                }
            }
        }
    }

    interface ISnapInteractorGrabbedBroadcastReciever
    {
        void HandleSnapInteractorGrabbed(SnapInteractor snapInteractor, bool grabbed);
    }
}
