// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Duplicates renderers with their materials attached
    /// </summary>
    public class RendererCopyWithMaterial : MonoBehaviour
    {
        [SerializeField]
        private Material _material;
        [SerializeField, Optional]
        private Material _additionalMaterial;
        [SerializeField]
        private ReferenceActiveState _enabled = ReferenceActiveState.Optional();

        private List<Renderer> _renderers = new();
        private List<Renderer> _copies = new();

        void Start()
        {
            GetComponentsInChildren(true, _renderers);
            _renderers.RemoveAll(x => !RendererCopy.CanCopy(x));
            _renderers.ForEach(x => _copies.Add(RendererCopy.CreateCopyChildWithMaterial(x, _material)));

            if (_additionalMaterial)
            {
                _copies.ForEach(x => x.sharedMaterials = new Material[] { _material, _additionalMaterial });
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _renderers.Count; i++)
            {
                _copies[i].enabled = _renderers[i].enabled && _enabled;
            }
        }
    }
}
