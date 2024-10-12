// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using ISerialize = UnityEngine.ISerializationCallbackReceiver;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Creates copies of materials from child renderers
    /// </summary>
    public class RendererCopy
    {
        public static Renderer CreateCopyChildWithMaterial(Renderer renderer, Material material)
        {
            switch (renderer)
            {
                case SkinnedMeshRenderer skinned: return CreateCopyChildWithMaterial(skinned, material);
                case MeshRenderer mesh: return CreateCopyChildWithMaterial(mesh, material);
                default: throw new System.Exception("Couldnt create copy");
            }
        }

        public static bool CanCopy(Renderer renderer)
        {
            switch (renderer)
            {
                case SkinnedMeshRenderer:
                case MeshRenderer: return true;
                default: return false;
            }
        }

        public static MeshRenderer CreateCopyChildWithMaterial(MeshRenderer renderer, Material material)
        {
            Transform child = CreateChild(renderer.gameObject);

            var result = child.gameObject.AddComponent<MeshRenderer>();
            if (renderer.TryGetComponent<MeshFilter>(out var filter))
            {
                child.gameObject.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
            }

            CopyCommonProperties(renderer, result);

            result.sharedMaterial = material;
            return result;
        }

        private static Transform CreateChild(GameObject renderer)
        {
            var child = new GameObject(renderer.name).transform;
            child.gameObject.layer = renderer.layer;
            child.SetParent(renderer.transform);
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
            return child;
        }

        public static SkinnedMeshRenderer CreateCopyChildWithMaterial(SkinnedMeshRenderer renderer, Material material)
        {
            Transform child = CreateChild(renderer.gameObject);
            var result = child.gameObject.AddComponent<SkinnedMeshRenderer>();
            CopyCommonProperties(renderer, result);
            result.bones = renderer.bones;
            result.rootBone = renderer.rootBone;
            result.sharedMesh = renderer.sharedMesh;
            result.updateWhenOffscreen = renderer.updateWhenOffscreen;
            result.quality = renderer.quality;

            result.sharedMaterial = material;

            return result;
        }

        public static void CopyCommonProperties(Renderer from, Renderer to)
        {
            to.allowOcclusionWhenDynamic = from.allowOcclusionWhenDynamic;
            to.lightmapIndex = from.lightmapIndex;
            to.lightmapScaleOffset = from.lightmapScaleOffset;
            to.lightProbeProxyVolumeOverride = from.lightProbeProxyVolumeOverride;
            to.lightProbeUsage = from.lightProbeUsage;
            to.motionVectorGenerationMode = from.motionVectorGenerationMode;
            to.probeAnchor = from.probeAnchor;
            to.receiveShadows = from.receiveShadows;
            to.reflectionProbeUsage = from.reflectionProbeUsage;
            to.rendererPriority = from.rendererPriority;
            to.shadowCastingMode = from.shadowCastingMode;
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

        public static int RemoveAll<T>(List<T> arg, List<RendererFilter> filters) where T : Renderer
        {
            if (filters == null || filters.Count == 0) return 0;
            return arg.RemoveAll(r => !TrueForAny(filters, filter => filter.Includes(r)));
        }

        static bool TrueForAny(List<RendererFilter> list, System.Predicate<RendererFilter> predicate) => !list.TrueForAll(x => !predicate(x));

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
