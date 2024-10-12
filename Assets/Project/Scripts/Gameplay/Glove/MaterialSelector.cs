// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using Was = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Applies a material to a set of renderers matching the value of a string property
    /// e.g. If a use can choose different skins for parts of a glove, the skin selection
    /// would be the property, this behaviour would apply the material
    /// </summary>
    public class MaterialSelector : MonoBehaviour
    {
        [SerializeField]
        private StringPropertyRef _selection;
        [SerializeField]
        private List<Renderer> _renderers = new List<Renderer>();
        [SerializeField, Was("_skinMaterials")]
        private List<NamedMaterial> _materials = new List<NamedMaterial>()
        {
            new NamedMaterial(){ name = "retro" },
            new NamedMaterial(){ name = "mech" },
            new NamedMaterial(){ name = "toy" }
        };
        [SerializeField]
        private Material _fallbackMaterial;

        private void Reset()
        {
            GetRenderersInChildren();
        }

        [ContextMenu("Get Renderers In Children")]
        private void GetRenderersInChildren()
        {
            GetComponentsInChildren(true, _renderers);
        }

        protected void Start()
        {
            _selection.AssertNotNull();
            _selection.WhenChanged += UpdateMaterial;
            UpdateMaterial();
        }

        private void OnDestroy()
        {
            _selection.WhenChanged -= UpdateMaterial;
        }

        private void UpdateMaterial()
        {
            var material = _materials.Find(x => x.name == _selection.Value).material;
            if (material == null) { material = _fallbackMaterial; }
            _renderers.ForEach(x => x.sharedMaterial = material);
        }

        internal void SetColor(Color white)
        {
            Debug.LogError($"{nameof(MaterialSelector)}.SetColor not supported!!");
        }
    }

    [System.Serializable]
    struct NamedMaterial
    {
        public string name;
        public Material material;
    }
}
