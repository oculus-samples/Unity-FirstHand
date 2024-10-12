// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Used to track consecutive knocks on the door in the gesture minigame
    /// </summary>
    public class DoorKnock : MonoBehaviour
    {
        [SerializeField]
        private ReferenceActiveState _canKnock;
        [SerializeField]
        private ReferenceActiveState _lKnockGesture, _rKnockGesture;
        [SerializeField]
        private int _maxAmountOfKnocks;
        [SerializeField]
        private Interaction.PokeInteractor _leftKnocker, _rightKnocker;

        [Header("Reference")]
        [SerializeField]
        private ProgressTrackerRef _progressTrackerRef;
        [SerializeField]
        private int _progressOnKnock = 310;
        [SerializeField]
        private GameObject _ghostHand;

        private int _consecutiveKocks = 0;
        private PokeInteractable _interactable;

        private InteractionTracker _interactionTracker;

        private void Start()
        {
            _interactable = GetComponent<PokeInteractable>();
            _interactionTracker = new InteractionTracker(_interactable);
            _interactionTracker.WhenSelectAdded += KnockOnDoor;
        }

        private void KnockOnDoor(IInteractorView obj)
        {
            if (!(obj is Interaction.PokeInteractor poker)) return;

            ReferenceActiveState? gestureRecogniser = GetGestureForInteractor(poker);

            if (gestureRecogniser.HasValue && gestureRecogniser.Value.Active)
            {
                _consecutiveKocks++;
                if (_consecutiveKocks >= _maxAmountOfKnocks)
                {
                    _progressTrackerRef.SetProgress(_progressOnKnock);
                }
            }
        }

        private ReferenceActiveState? GetGestureForInteractor(Interaction.PokeInteractor poker)
        {
            if (poker == _rightKnocker) return _rKnockGesture;
            if (poker == _leftKnocker) return _lKnockGesture;
            return null;
        }
    }
}
