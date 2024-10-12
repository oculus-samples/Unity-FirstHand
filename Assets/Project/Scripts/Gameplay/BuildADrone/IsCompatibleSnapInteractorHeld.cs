// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class IsCompatibleSnapInteractorHeld : MonoBehaviour, IActiveState, ISnapInteractorGrabbedBroadcastReciever
    {
        [SerializeField]
        private SnapInteractable _snapInteractable;

        private List<SnapInteractor> _heldInteractors = new List<SnapInteractor>();

        public bool Active => _active;

        private bool _active;

        public void HandleSnapInteractorGrabbed(SnapInteractor snapInteractor, bool add)
        {
            bool shouldAdd = add && _snapInteractable.CanBeSelectedBy(snapInteractor) && snapInteractor.CanSelect(_snapInteractable);

            if (shouldAdd) _heldInteractors.Add(snapInteractor);
            else _heldInteractors.Remove(snapInteractor);

            _active = _heldInteractors.Count > 0;
        }
    }
}
