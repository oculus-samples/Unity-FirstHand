// Copyright (c) Meta Platforms, Inc. and affiliates.

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Assigns the dropzones timeout interactable to the zone it's in
    /// This is useful for key items that need to return to their last good position
    /// TODO material effect to hide and show when the item returns
    /// </summary>
    public class DefaultDropZone : MonoBehaviour
    {
        [SerializeField]
        SnapInteractor _dropZoneInteractor;
        [SerializeField, Optional]
        SnapInteractable _returnDropZoneOverride;
        [SerializeField]
        private int _minTimesSelected = 0;

        InteractionTracker _interactionTracker;
        SnapInteractable _returnDropZone;

        public override string ToString()
        {
            return base.ToString() + System.Environment.NewLine +
                _interactionTracker?.ToString();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_dropZoneInteractor && !_returnDropZoneOverride)
            {
                SerializedObject so = new SerializedObject(_dropZoneInteractor);
                var timeout = so.FindProperty("_timeOutInteractable");
                if (timeout.objectReferenceValue)
                {
                    _returnDropZoneOverride = timeout.objectReferenceValue as SnapInteractable;
                    timeout.objectReferenceValue = null;
                    so.ApplyModifiedProperties();
                }
            }
        }
#endif

        private void Reset()
        {
            _dropZoneInteractor = GetComponent<SnapInteractor>();
        }

        private void Start()
        {
            _interactionTracker = new InteractionTracker(_dropZoneInteractor.PointableElement);
            _interactionTracker.WhenChanged += ReturnToDefault;
            _dropZoneInteractor.WhenStateChanged += UpdateDefaultDropZone;
            UpdateDefaultDropZone(default);
            ReturnToDefault();
        }

        private void ReturnToDefault()
        {
            if (!ShouldReturn()) return;

            /* Need to delay it because if the item is in the snapzone when it's grabbed
             * by the hand, Cancel will be called which will invoke ReturnToDefault
             * while the selecting count is still zero, and the snapzone will reselect
             */
            TweenRunner.DelayedCall(0.01f, () =>
            {
                if (!ShouldReturn()) return;

                var dropInteractable = _returnDropZoneOverride ? _returnDropZoneOverride : _returnDropZone;

                // if the item is in a dropzone grab it immediatly
                if (_dropZoneInteractor.HasCandidate && _dropZoneInteractor.Candidate == dropInteractable)
                {
                    _dropZoneInteractor.ForceSelect(dropInteractable);
                }
                // otherwise if the items not in the zone add another delay
                // for other snap zones to have the opportunity to grab
                else
                {
                    TweenRunner.DelayedCall(0.1f, () =>
                    {
                        if (ShouldReturn()) _dropZoneInteractor.ForceSelect(dropInteractable);
                    }).IgnoreTimeScale(true).SetID(this);
                }
            }).IgnoreTimeScale(true).SetID(this);
        }

        private void OnDestroy()
        {
            _dropZoneInteractor.WhenStateChanged -= UpdateDefaultDropZone;
        }

        private void UpdateDefaultDropZone(InteractorStateChangeArgs _)
        {
            if (!_dropZoneInteractor.HasSelectedInteractable) return;
            _returnDropZone = _dropZoneInteractor.SelectedInteractable;
        }

        private bool ShouldReturn()
        {
            if (_interactionTracker.TimesSelected < _minTimesSelected) return false;
            if (_interactionTracker.SelectingInteractors.Count > 0) return false;
            if (_dropZoneInteractor.HasSelectedInteractable) return false;
            if (!_dropZoneInteractor.isActiveAndEnabled) return false;
            return true;
        }
    }
}
