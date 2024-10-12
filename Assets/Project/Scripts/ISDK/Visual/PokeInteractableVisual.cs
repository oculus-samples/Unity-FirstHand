// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class PokeInteractableVisual : MonoBehaviour
    {
        [SerializeField]
        private PokeInteractable _pokeInteractable;

        [SerializeField]
        private Transform _buttonBaseTransform;

        [SerializeField, Tooltip("When true the visual can move beyond the pokable surface")]
        private bool _allowNegative;

        [SerializeField, Tooltip("The speed which the button returns to its normal position when a poke is cancelled, <= 0 means instant")]
        private float _recoverySpeed = 0;

        [SerializeField]
        private ReferenceActiveState _active = ReferenceActiveState.Optional();

        private float _maxOffsetAlongNormal;
        private Vector2 _planarOffset;

        private HashSet<Interaction.PokeInteractor> _pokeInteractors;

        protected bool _started = false;

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(_pokeInteractable);
            Assert.IsNotNull(_buttonBaseTransform);
            _pokeInteractors = new HashSet<Interaction.PokeInteractor>();
            _maxOffsetAlongNormal = Vector3.Dot(transform.position - _buttonBaseTransform.position, -1f * _buttonBaseTransform.forward);
            Vector3 pointOnPlane = transform.position - _maxOffsetAlongNormal * _buttonBaseTransform.forward;
            _planarOffset = new Vector2(
                                Vector3.Dot(pointOnPlane - _buttonBaseTransform.position, _buttonBaseTransform.right),
                                Vector3.Dot(pointOnPlane - _buttonBaseTransform.position, _buttonBaseTransform.up));
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                _pokeInteractors.Clear();
                _pokeInteractors.UnionWith(_pokeInteractable.Interactors);
                _pokeInteractable.WhenInteractorAdded.Action += HandleInteractorAdded;
                _pokeInteractable.WhenInteractorRemoved.Action += HandleInteractorRemoved;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                _pokeInteractors.Clear();
                _pokeInteractable.WhenInteractorAdded.Action -= HandleInteractorAdded;
                _pokeInteractable.WhenInteractorRemoved.Action -= HandleInteractorRemoved;
            }
        }

        private void HandleInteractorAdded(Interaction.PokeInteractor pokeInteractor)
        {
            _pokeInteractors.Add(pokeInteractor);
        }
        private void HandleInteractorRemoved(Interaction.PokeInteractor pokeInteractor)
        {
            _pokeInteractors.Remove(pokeInteractor);
        }

        private void Update()
        {
            float closestDistance = _active ? _maxOffsetAlongNormal : 0;

            if (_pokeInteractable.enabled)
            {
                // To create a pressy button visual, we check each near poke interactor's
                // depth against the base of the button and use the most pressed-in
                // value as our depth. We cap this at the button base as the stopping
                // point. If no interactors exist, we sit the button at the original offset

                foreach (Interaction.PokeInteractor pokeInteractor in _pokeInteractors)
                {
                    // Scalar project the poke interactor's position onto the button base's normal vector
                    float pokeDistance =
                        Vector3.Dot(pokeInteractor.Origin - _buttonBaseTransform.position,
                            -1f * _buttonBaseTransform.forward);
                    pokeDistance -= pokeInteractor.Radius;
                    if (pokeDistance < 0f && !_allowNegative)
                    {
                        pokeDistance = 0f;
                    }
                    closestDistance = Math.Min(pokeDistance, closestDistance);
                }
            }

            // Position our transformation at our button base plus
            // the most pressed in distance along the normal plus
            // the original planar offset of the button from the button base
            Vector3 targetPosition = _buttonBaseTransform.position +
                                             _buttonBaseTransform.forward * -1f * closestDistance +
                                             _buttonBaseTransform.right * _planarOffset.x +
                                             _buttonBaseTransform.up * _planarOffset.y;

            if (_pokeInteractors.Count > 0 || _recoverySpeed <= 0)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * _recoverySpeed);
            }
        }

        #region Inject

        public void InjectAllPokeInteractableVisual(PokeInteractable pokeInteractable,
            Transform buttonBaseTransform)
        {
            InjectPokeInteractable(pokeInteractable);
            InjectButtonBaseTransform(buttonBaseTransform);
        }

        public void InjectPokeInteractable(PokeInteractable pokeInteractable)
        {
            _pokeInteractable = pokeInteractable;
        }

        public void InjectButtonBaseTransform(Transform buttonBaseTransform)
        {
            _buttonBaseTransform = buttonBaseTransform;
        }

        #endregion
    }
}
