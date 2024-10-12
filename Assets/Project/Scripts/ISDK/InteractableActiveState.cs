// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// IActiveState that evaluates the state of an Interactable and its Interactors
    /// Becomes active when enough Interactors are interacting with the interactable
    /// </summary>
    public class InteractableActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField, Interface(typeof(IInteractableView))]
        private MonoBehaviour _interactable;
        public IInteractableView Interactable;

        [Header("Minimum Counts")]
        [SerializeField]
        private InteractableProperty _property;

        [SerializeField]
        private int _minimumInteractorCount = 1;
        public int MinimumInteractorCount => _minimumInteractorCount;

        [Header("Times Interacted")]
        [SerializeField]
        private FloatRange _timesSelected;

        [Header("Specific Interactor")]
        [SerializeField, Interface(typeof(IInteractorView)), Optional]
        private MonoBehaviour _selectedBy;
        public IInteractorView SelectedBy;

        private InteractionTracker _interactionTracker;


        public InteractableProperty Property
        {
            get => _property;
            set => _property = value;
        }

        public bool Active
        {
            get
            {
                if (_interactionTracker == null)
                {
                    return false;
                }

                if (!_timesSelected.Contains(_interactionTracker.TimesSelected)) return false;

                if ((_property & InteractableProperty.IsCandidate) != 0
                    && _interactionTracker.Interactors.Count >= _minimumInteractorCount)
                {
                    return true;
                }

                if ((_property & InteractableProperty.IsSelected) != 0
                    && _interactionTracker.SelectingInteractors.Count >= _minimumInteractorCount)
                {
                    return true;
                }

                if (SelectedBy != null)
                {
                    return _interactionTracker.IsSelectingInteractor(SelectedBy);
                }

                return _property == 0 || MinimumInteractorCount <= 0;
            }
        }

        public InteractionTracker InteractionTracker => _interactionTracker;

        protected virtual void Reset()
        {
            _interactable = GetComponent<IInteractableView>() as MonoBehaviour;
        }

        protected virtual void Awake()
        {
            Interactable = _interactable as IInteractableView;
            SelectedBy = _selectedBy as IInteractorView;
        }

        protected virtual void Start()
        {
            Assert.IsNotNull(Interactable);
            _interactionTracker = new InteractionTracker(Interactable);
        }

        protected virtual void OnDestroy()
        {
            _interactionTracker?.Dispose();
            _interactionTracker = null;
        }

        [Flags]
        public enum InteractableProperty
        {
            IsCandidate = 1 << 0,
            IsSelected = 1 << 2
        }
    }
}
