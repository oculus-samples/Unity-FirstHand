// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using static Oculus.Interaction.ComprehensiveSample.ConditionalSetParent;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Parents a SnapInteractor's Grabbable to the snap interactable
    /// Functions via ConditionalSetParent, which selects the SnapZone transform when ths SnapZone holds this
    /// </summary>
    [RequireComponent(typeof(SnapInteractor))]
    public class SnapZoneParent : MonoBehaviour, IConditionalTransform
    {
        [SerializeField]
        private ConditionalSetParent _conditionalSetParent;
        [SerializeField, Tooltip("the 'priority' this should have in the ConditionalSetParent")]
        private int _insertIndex = 0;

        private SnapInteractor _snap;

        public bool Active => _snap.HasSelectedInteractable;
        public Transform Transform => _snap.SelectedInteractable.transform;

        void Start()
        {
            _snap = GetComponent<SnapInteractor>();
            _conditionalSetParent.Insert(_insertIndex, this);
        }
    }
}
