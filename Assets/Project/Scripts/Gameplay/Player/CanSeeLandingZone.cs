// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.DistanceReticles;
using Oculus.Interaction.Locomotion;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Marks the TeleportInteractable as not teleportable if the player cant see it
    /// e.g. If the user aims thier arc over a wall
    /// </summary>
    public class CanSeeLandingZone : MonoBehaviour
    {
        [SerializeField]
        private Transform _startPoint;
        [SerializeField]
        private Vector3 _offset;
        [SerializeField]
        private LayerMask _layerMask = 0;
        [SerializeField]
        private TeleportInteractor _interactor;

        private TeleportInteractable _modifiedInteractable;
        private ReticleDataTeleport _modifiedReticle;

        private void OnEnable()
        {
            //Delayed so the arc visual (which also runs in these callbacks) runs first
            TweenRunner.NextFrame(() =>
            {
                _interactor.WhenPreprocessed += UpdateCandidateAllowTeleport;
                _interactor.WhenPostprocessed += RestoreModifiedInteractable;
                _interactor.WhenInteractableSet.Action += HandleInteractableSet;
            }).SetID(this);

        }

        private void OnDisable()
        {
            _interactor.WhenPreprocessed -= UpdateCandidateAllowTeleport;
            _interactor.WhenPostprocessed -= RestoreModifiedInteractable;
            _interactor.WhenInteractableSet.Action -= HandleInteractableSet;
            TweenRunner.Kill(this);
        }

        /// <summary>
        /// Called when, during the Interactor.Process, the interactor's hovered interactable changes
        /// We need to restore the old interactable's allowTeleport and update the new ones allowTeleport
        /// </summary>
        private void HandleInteractableSet(TeleportInteractable obj)
        {
            RestoreModifiedInteractable();
            UpdateCandidateAllowTeleport();
        }

        private void UpdateCandidateAllowTeleport()
        {
            var candidate = _interactor.Candidate;

            if (candidate == null || !candidate.AllowTeleport || candidate.TryGetComponent<CanSeeLandingZoneSettings>(out var _))
                return;

            var endPoint = _interactor.TeleportTarget.position;
            var startPoint = _startPoint.position;

            // assume 0.5m above players eyeline is probably out of view
            if (endPoint.y > startPoint.y + 0.5f)
            {
                ModifyInteractable(candidate);
                return;
            }

            bool visionBlocked = Physics.Linecast(startPoint, endPoint, out RaycastHit hit, _layerMask, QueryTriggerInteraction.Ignore);
            bool colliderIsPartOfInteractable = visionBlocked && hit.collider.transform.IsChildOf(candidate.transform);

            if (!visionBlocked || colliderIsPartOfInteractable)
            {
                return;
            }

            // allow some leeway where the navmesh may be slightly under the geometry
            var dist = hit.point - endPoint;
            float hitPointOnNormal = Vector3.Dot(dist, hit.normal);
            bool closeEnough = hitPointOnNormal >= -0.02f && dist.sqrMagnitude < 0.2f * 0.2f;

            if (!closeEnough)
            {
                ModifyInteractable(candidate);
            }
        }

        private void ModifyInteractable(TeleportInteractable interactable)
        {
            interactable.AllowTeleport = false;
            _modifiedInteractable = interactable;

            if (_modifiedInteractable.TryGetComponent(out ReticleDataTeleport reticle) &&
                reticle.ReticleMode == ReticleDataTeleport.TeleportReticleMode.ValidTarget)
            {
                reticle.ReticleMode = ReticleDataTeleport.TeleportReticleMode.InvalidTarget;
                _modifiedReticle = reticle;
            }
        }

        private void RestoreModifiedInteractable()
        {
            if (_modifiedInteractable != null)
            {
                _modifiedInteractable.AllowTeleport = true;
                _modifiedInteractable = null;
            }

            if (_modifiedReticle != null)
            {
                _modifiedReticle.ReticleMode = ReticleDataTeleport.TeleportReticleMode.ValidTarget;
                _modifiedReticle = null;
            }
        }
    }
}
