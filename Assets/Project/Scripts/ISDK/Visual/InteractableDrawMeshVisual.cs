// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Copies the renderers of the Interactable, assigns the material to the copies, enabled the renderers when hovered.
    /// Used as a highlighting technique that doesnt involve manipulating the items renderers (allowing those renderers to
    /// have custom shaders that might not conform to what expected)
    /// Assumes a single material on the mesh
    /// </summary>
    public class InteractableDrawMeshVisual : MonoBehaviour
    {
        static MaterialPropertyBlock _block;

        [SerializeField]
        private Material _material;

        [SerializeField, Tooltip("Choose which renderers to include in hightlight, logical OR, empty list means any renderer")]
        List<RendererFilter> _passFilters = new List<RendererFilter>()
        {
            new RendererFilter("Opaque", "1000-2500"),
            new RendererFilter("Glass", ">2500", new RendererFilter.MaterialFloatProperty("_Blend", "1"))
        };

        private InteractableGroupView _interactableView;
        private List<Renderer> _renderers = new List<Renderer>();
        private InteractionTracker _interactionTracker;

        bool _isHighlighted;
        float _highlightAmount;

        protected virtual void Start()
        {
            if (_block == null) _block = new MaterialPropertyBlock();

            Assert.IsNotNull(_material);

            _interactableView = GetInteractableView();
            if (_interactableView == null)
            {
                Debug.LogWarning($"{nameof(InteractableDrawMeshVisual)} expects to be a child of an InteractableGroupView");
                return;
            }

            _interactableView.GetComponentsInChildren(true, _renderers);

            RendererFilter.RemoveAll(_renderers, _passFilters);
            _renderers.RemoveAll(x => !RendererCopy.CanCopy(x));

            for (int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i] = RendererCopy.CreateCopyChildWithMaterial(_renderers[i], _material);
                _renderers[i].enabled = false;
            }

            _interactionTracker = new InteractionTracker(_interactableView);

            UpdateVisual();
        }

        private InteractableGroupView GetInteractableView()
        {
            for (var t = transform.parent; t; t = t.parent)
            {
                if (t.TryGetComponent<InteractableGroupView>(out var result))
                {
                    return result;
                }
            }
            return null;
        }

        static bool TrueForAny<T>(List<T> list, Predicate<T> predicate)
        {
            return !list.TrueForAll(x => !predicate(x));
        }

        private void Update()
        {
            UpdateVisual();
        }

        private void OnDisable()
        {
            UpdateVisual();
        }

        void UpdateVisual()
        {
            bool shouldHighlight = isActiveAndEnabled && _interactionTracker.Interactors.Count > _interactionTracker.SelectingInteractors.Count;
            SetHighlightEnabled(shouldHighlight);
        }

        private void SetHighlightEnabled(bool enable)
        {
            if (_isHighlighted == enable) return;

            _isHighlighted = enable;

            TweenRunner.Tween(_highlightAmount, enable ? 1 : 0, 0.15f, SetHightlightAmount).SetID(this);

            void SetHightlightAmount(float obj)
            {
                _highlightAmount = obj;
                bool visible = _highlightAmount > 0;

                for (int i = 0; i < _renderers.Count; i++)
                {
                    _renderers[i].enabled = visible;
                    if (visible)
                    {
                        _renderers[i].GetPropertyBlock(_block);
                        _block.SetFloat("_Alpha", _highlightAmount);
                        _renderers[i].SetPropertyBlock(_block);
                    }
                }
            }
        }
    }
}
