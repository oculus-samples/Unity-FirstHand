/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using ISerialize = UnityEngine.ISerializationCallbackReceiver;

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
        [SerializeField]
        private Material _material;

        [SerializeField, Tooltip("Choose which renderers to include in hightlight, logical OR, empty list means any renderer")]
        List<RendererFilter> _passFilters = new List<RendererFilter>()
        {
            new RendererFilter("Opaque", "1000-2500"),
            new RendererFilter("Glass", ">2500", new RendererFilter.MaterialFloatProperty("_Blend", "1"))
        };

        private InteractableGroupView _interactableView;
        private List<MeshRenderer> _renderers = new List<MeshRenderer>();

        protected virtual void Start()
        {
            Assert.IsNotNull(_material);

            _interactableView = GetComponentInParent<InteractableGroupView>();
            if (_interactableView == null)
            {
                Debug.LogWarning($"{nameof(InteractableDrawMeshVisual)} expects to be a child of an InteractableGroupView");
                return;
            }

            _interactableView.GetComponentsInChildren(true, _renderers);
            _renderers.RemoveAll(r => !TrueForAny(_passFilters, filter => filter.Includes(r)));

            for (int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i] = CreateChild(_renderers[i]);
            }

            UpdateVisual();
        }

        static bool TrueForAny<T>(List<T> list, System.Predicate<T> predicate)
        {
            return !list.TrueForAll(x => !predicate(x));
        }

        private MeshRenderer CreateChild(MeshRenderer renderer)
        {
            var child = new GameObject(renderer.name).transform;
            child.SetParent(renderer.transform);
            child.SetPose(renderer.transform.GetPose());
            child.localScale = Vector3.one;

            var result = child.gameObject.AddComponent<MeshRenderer>();
            if (renderer.TryGetComponent<MeshFilter>(out var filter))
            {
                child.gameObject.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
            }

            result.sharedMaterial = _material;
            return result;
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
            bool shouldHighlight = isActiveAndEnabled && _interactableView.InteractorsCount > _interactableView.SelectingInteractorsCount;
            for (int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].enabled = shouldHighlight;
            }
        }
    }

    [System.Serializable]
    public class RendererFilter
    {
        [SerializeField]
        string _name;

        [SerializeField]
        bool _requireRendererEnabled = false;

        [SerializeField]
        FloatRanges _queue = "2000-2500";

        [SerializeField]
        MaterialFloatProperty _floatProperty = new MaterialFloatProperty();

        public RendererFilter(string name, string queue = "", MaterialFloatProperty floatProperty = default, bool requireRendererEnabled = false)
        {
            _name = name;
            _requireRendererEnabled = requireRendererEnabled;
            _queue = queue;
            _floatProperty = floatProperty;
        }

        public bool Includes(Renderer renderer)
        {
            if (_requireRendererEnabled && !(renderer.enabled && renderer.gameObject.activeInHierarchy))
            {
                return false;
            }

            var material = renderer.sharedMaterial;
            if (material)
            {
                if (!_queue.Contains(material.renderQueue))
                {
                    return false;
                }

                if (_floatProperty.IsValid && (!material.TryGetFloat(_floatProperty.hash, out var value) || !_floatProperty.range.Contains(value)))
                {
                    return false;
                }
            }

            return true;
        }

        [System.Serializable]
        public struct MaterialFloatProperty : ISerialize
        {
            public string name;
            public FloatRanges range;

            public MaterialFloatProperty(string name, string values) : this()
            {
                this.name = name;
                range = values;
                hash = Shader.PropertyToID(name);
            }

            public int hash { get; private set; }
            public bool IsValid => !string.IsNullOrEmpty(name);

            void ISerialize.OnAfterDeserialize() => hash = Shader.PropertyToID(name);
            void ISerialize.OnBeforeSerialize() { }
        }
    }
}
