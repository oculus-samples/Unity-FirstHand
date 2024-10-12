// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Base class for specific effects that may require keyword enabling
    /// </summary>
    [ExecuteAlways]
    public abstract class RendererEffect : MonoBehaviour, IComparable<RendererEffect>
    {
        private static MaterialPropertyBlock _propertyBlock;
        private static Dictionary<Renderer, RendererEffectList> _rendererEffects = new Dictionary<Renderer, RendererEffectList>();

        string _profilerName;
        int _id => GetInstanceID();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _rendererEffects = new Dictionary<Renderer, RendererEffectList>();
        }

        [SerializeField]
        protected Renderer[] _renderers;

        protected RendererEffect()
        {
            _profilerName = $"{nameof(RendererEffect)}.{GetType().Name}";
        }

        [ContextMenu("GetRenderersInChildren")]
        protected virtual void Reset()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            RemoveAll(ref _renderers, r => !(r is SkinnedMeshRenderer || r is MeshRenderer));
        }

        protected virtual void OnEnable()
        {
            if (!Application.isPlaying) return;

            for (int i = 0; i < _renderers.Length; i++)
            {
                Renderer renderer = _renderers[i];
                if (!renderer || !renderer.sharedMaterial)
                {
                    if (Application.isEditor) Debug.LogWarning($"Skipped {renderer}", renderer);
                    continue;
                }

                if (!_rendererEffects.ContainsKey(renderer))
                {
                    _rendererEffects.Add(renderer, new RendererEffectList(renderer));
                }
                _rendererEffects[renderer].Add(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (!Application.isPlaying) return;

            for (int i = 0; i < _renderers.Length; i++)
            {
                Renderer renderer = _renderers[i];
                if (renderer == null) continue;

                _rendererEffects[renderer].Remove(this);
            }
        }

        protected virtual void Update()
        {
            if (_propertyBlock == null) _propertyBlock = new MaterialPropertyBlock();

            Profiler.BeginSample(_profilerName);
            for (int i = 0; i < _renderers.Length; i++)
            {
                var r = _renderers[i];
                if (!r || !r.enabled) continue;

                if (Application.isPlaying)
                {
                    _rendererEffects[r].CheckMaterialIsUpToDate();
                }

                r.GetPropertyBlock(_propertyBlock);
                UpdateProperties(_propertyBlock);
                r.SetPropertyBlock(_propertyBlock);
            }
            Profiler.EndSample();
        }

        protected virtual void UpdateMaterial(Material material) { }

        protected virtual void UpdateProperties(MaterialPropertyBlock block) { }

        private static int RemoveAll<T>(ref T[] array, Predicate<T> predicate)
        {
            int removed = 0;
            for (int i = array.Length - 1; i >= 0; i--)
            {
                if (!predicate(array[i])) continue;

                int end = array.Length - 1 - removed++;
                T temp = array[end];
                array[end] = array[i];
                array[i] = temp;
            }

            if (removed > 0) Array.Resize(ref array, array.Length - removed);

            return removed;
        }

        public int CompareTo(RendererEffect other) => _id.CompareTo(other._id);

        class RendererEffectList
        {
            private Renderer _renderer;
            private Material _originalMaterial;
            private List<RendererEffect> _effects = new List<RendererEffect>();

            public RendererEffectList(Renderer renderer)
            {
                _renderer = renderer;
                _originalMaterial = renderer.sharedMaterial;

                if (!_originalMaterial) Debug.LogException(new NullReferenceException($"Missing material on {_renderer}"), _renderer);
            }

            public void Add(RendererEffect effect)
            {
                var index = _effects.BinarySearch(effect);
                if (index < 0) index = ~index;
                _effects.Insert(index, effect);
                UpdateMaterial();
            }

            public void Remove(RendererEffect effect)
            {
                _effects.Remove(effect);
                UpdateMaterial();
            }

            public void CheckMaterialIsUpToDate()
            {
                var currentMaterial = _renderer.sharedMaterial;
                if (currentMaterial == _originalMaterial) return;

                var effectMaterial = EffectedMaterialRepo.GetMaterial(_originalMaterial, _effects);
                if (currentMaterial == effectMaterial) return;

                _originalMaterial = currentMaterial;
                UpdateMaterial();
            }

            private void UpdateMaterial()
            {
                if (Application.isPlaying)
                {
                    _renderer.sharedMaterial = _effects.Count > 0 ?
                        EffectedMaterialRepo.GetMaterial(_originalMaterial, _effects) : _originalMaterial;
                }
            }
        }

        static class EffectedMaterialRepo
        {
            static Dictionary<long, Material> _materials = new Dictionary<long, Material>();

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            static void Init()
            {
                _materials = new Dictionary<long, Material>();
            }

            public static Material GetMaterial(Material originalMaterial, List<RendererEffect> effects)
            {
                if (!Application.isPlaying) return originalMaterial;
                Assert.IsNotNull(originalMaterial);

                var id = EffectList.GetID(originalMaterial, effects);

                if (!_materials.ContainsKey(id))
                {
                    Material copy = new Material(originalMaterial);
                    copy.name += " (Copy)";
                    effects.ForEach(x => x.UpdateMaterial(copy));
                    _materials.Add(id, copy);
                }

                return _materials[id];
            }

            struct EffectList
            {
                public static long GetID(Material baseMaterial, List<RendererEffect> effects)
                {
                    long id = baseMaterial.GetInstanceID();
                    for (int i = 0; i < effects.Count; i++)
                    {
                        id += effects[i]._id;
                    }
                    return id;
                }
            }
        }
    }
}
