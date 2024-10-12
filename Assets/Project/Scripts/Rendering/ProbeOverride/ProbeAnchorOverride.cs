// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class ProbeAnchorOverride : ActiveStateObserver
    {
        private static MaterialPropertyBlock _propertyBlock;
        private static readonly List<Renderer> _renderers = new List<Renderer>();

        [SerializeField]
        private Transform _originA;
        [SerializeField]
        private Transform _originB;
        [SerializeField]
        private Transform _overrideOriginA;
        [SerializeField]
        private Transform _overrideOriginB;

        private bool? _valid = null;
        private Renderer _renderer;
        private Transform _probeAnchorA;
        private Renderer _probeAnchorRendererA;
        private Transform _probeAnchorB;
        private Renderer _probeAnchorRendererB;

        [SerializeField] float _weight;

        bool _usesProbes;
        bool _usesReflections;

        void Awake()
        {
            if (_propertyBlock == null) _propertyBlock = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();

            _valid = !_renderer.probeAnchor;
            _usesProbes = _renderer.lightmapIndex < 0 && _renderer.lightProbeUsage == LightProbeUsage.BlendProbes;
            _usesReflections = _renderer.reflectionProbeUsage != ReflectionProbeUsage.Off;

            if (_valid.Value)
            {
                SetOriginAndOverrides(_originA, _originB, _overrideOriginA, _overrideOriginB);
            }
        }

        protected override void Start()
        {
            if (_activeState.HasReference)
            {
                base.Start();
                _weight = Active ? 1 : 0;
            }
        }

        void SetOriginAndOverrides(Transform originA, Transform originB, Transform overrideOriginA, Transform overrideOriginB, IActiveState activeState = null)
        {
            if (originA == _originA && originB == _originB && overrideOriginA == _overrideOriginA && _overrideOriginB == overrideOriginB) return;

            if (_probeAnchorA != null) { Destroy(_probeAnchorA.gameObject); }
            if (_probeAnchorB != null) { Destroy(_probeAnchorB.gameObject); }

            _originA = originA;
            _originB = originB;
            _overrideOriginA = overrideOriginA;
            _overrideOriginB = overrideOriginB;

            if (activeState != null)
            {
                _activeState.InjectActiveState(activeState);
            }

            if (!_originA || !_overrideOriginA || !_overrideOriginB) return;

            _probeAnchorA = new GameObject($"{overrideOriginA.name}:{name}").transform;
            _probeAnchorRendererA = _probeAnchorA.gameObject.AddComponent<MeshRenderer>();
            _probeAnchorA.gameObject.hideFlags = HideFlags.HideAndDontSave;
            SceneManager.MoveGameObjectToScene(_probeAnchorA.gameObject, gameObject.scene);

            _probeAnchorB = new GameObject($"{overrideOriginB.name}:{name}").transform;
            _probeAnchorRendererB = _probeAnchorB.gameObject.AddComponent<MeshRenderer>();
            _probeAnchorB.gameObject.hideFlags = HideFlags.HideAndDontSave;
            SceneManager.MoveGameObjectToScene(_probeAnchorB.gameObject, gameObject.scene);
        }

        protected override void Update()
        {
            if (_activeState.HasReference) //manual check to make the _activeState 'optional'
            {
                base.Update();
            }
            UpdateProbeCoefficients();
        }

        protected override void LateUpdate()
        {
            if (_activeState.HasReference) //manual check to make the _activeState 'optional'
            {
                base.LateUpdate();
            }
        }

        protected override void HandleActiveStateChanged()
        {
            TweenRunner.Tween(_weight, Active ? 1 : 0, 0.3f, x => _weight = x).SetID(this);
        }

        void UpdateProbeCoefficients()
        {
            if (_renderer.enabled) return;
            if (!_usesProbes && !_usesReflections) return;
            if (!_valid.HasValue || !_valid.Value) return;
            if (!_originA || !_overrideOriginA || !_overrideOriginB) return;

            SphericalHarmonicsL2 overrideProbeA = new SphericalHarmonicsL2();
            if (_weight < 1)
            {
                var toOriginAPose = PoseUtils.Delta(_originA.GetPose(), transform.GetPose());
                var overridePoseA = toOriginAPose.GetTransformedBy(_overrideOriginA.GetPose());
                _probeAnchorA.SetPose(overridePoseA);
                if (_usesProbes) LightProbes.GetInterpolatedProbe(overridePoseA.position, _probeAnchorRendererA, out overrideProbeA);
            }

            SphericalHarmonicsL2 overrideProbeB = new SphericalHarmonicsL2();
            if (_weight > 0)
            {
                var toOriginBPose = PoseUtils.Delta(_originB.GetPose(), transform.GetPose());
                var overridePoseB = toOriginBPose.GetTransformedBy(_overrideOriginB.GetPose());
                _probeAnchorB.SetPose(overridePoseB);
                if (_usesProbes) LightProbes.GetInterpolatedProbe(overridePoseB.position, _probeAnchorRendererB, out overrideProbeB);
            }

            if (_usesReflections) _renderer.probeAnchor = _weight <= 0.5 ? _probeAnchorA : _probeAnchorB;

            if (_usesProbes)
            {
                var probe = LightProbeUtility.Lerp(overrideProbeA, overrideProbeB, _weight);
                _renderer.GetPropertyBlock(_propertyBlock);
                LightProbeUtility.SetSHCoefficients(_propertyBlock, probe);
                _renderer.SetPropertyBlock(_propertyBlock);
            }
        }

        /// <summary>
        /// Sets up or overwrites a Renderers ProbeOverrides
        /// </summary>
        public static void Initialize(Renderer renderer, Transform originA, Transform originB, Transform overrideOriginA, Transform overrideOriginB, IActiveState active)
        {
            ProbeAnchorOverride ov;
            if (!renderer.gameObject.TryGetComponent(out ov))
            {
                ov = renderer.gameObject.AddComponent<ProbeAnchorOverride>();
            }
            ov.SetOriginAndOverrides(originA, originB, overrideOriginA, overrideOriginB, active);
        }

        /// <summary>
        /// Sets up ProbeAnchorOverrides on a full hierarchy
        /// </summary>
        public static void InitializeHierachy(Transform root, Transform originA, Transform originB, Transform overrideOriginA, Transform overrideOriginB, IActiveState active)
        {
            root.GetComponentsInChildren(true, _renderers);
            _renderers.ForEach(x => Initialize(x, originA, originB, overrideOriginA, overrideOriginB, active));
        }

        //https://github.com/keijiro/LightProbeUtility/blob/master/Assets/LightProbeUtility.cs
        public static class LightProbeUtility
        {
            // Set SH coefficients to MaterialPropertyBlock
            public static void SetSHCoefficients(Vector3 position, MaterialPropertyBlock properties)
            {
                SphericalHarmonicsL2 sh;
                LightProbes.GetInterpolatedProbe(position, null, out sh);
                SetSHCoefficients(properties, sh);
            }

            public static void SetSHCoefficients(MaterialPropertyBlock properties, SphericalHarmonicsL2 sh)
            {
                // Constant + Linear
                for (var i = 0; i < 3; i++)
                    properties.SetVector(_idSHA[i], new Vector4(sh[i, 3], sh[i, 1], sh[i, 2], sh[i, 0] - sh[i, 6]));

                // Quadratic polynomials
                for (var i = 0; i < 3; i++)
                    properties.SetVector(_idSHB[i], new Vector4(sh[i, 4], sh[i, 6], sh[i, 5] * 3, sh[i, 7]));

                // Final quadratic polynomial
                properties.SetVector(_idSHC, new Vector4(sh[0, 8], sh[2, 8], sh[1, 8], 1));
            }

            // Set SH coefficients to Material
            public static void SetSHCoefficients(Vector3 position, Material material)
            {
                SphericalHarmonicsL2 sh;
                LightProbes.GetInterpolatedProbe(position, null, out sh);
                SetSHCoefficients(material, sh);
            }

            public static void SetSHCoefficients(Material material, SphericalHarmonicsL2 sh)
            {

                // Constant + Linear
                for (var i = 0; i < 3; i++)
                    material.SetVector(_idSHA[i], new Vector4(sh[i, 3], sh[i, 1], sh[i, 2], sh[i, 0] - sh[i, 6]));

                // Quadratic polynomials
                for (var i = 0; i < 3; i++)
                    material.SetVector(_idSHB[i], new Vector4(sh[i, 4], sh[i, 6], sh[i, 5] * 3, sh[i, 7]));

                // Final quadratic polynomial
                material.SetVector(_idSHC, new Vector4(sh[0, 8], sh[2, 8], sh[1, 8], 1));
            }

            public static SphericalHarmonicsL2 Lerp(SphericalHarmonicsL2 a, SphericalHarmonicsL2 b, float t)
            {
                if (t <= 0) return a;
                if (t >= 1) return b;

                SphericalHarmonicsL2 result = new SphericalHarmonicsL2();
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        result[i, j] = Mathf.Lerp(a[i, j], b[i, j], t);
                    }
                }
                return result;
            }

            static int[] _idSHA = {
            Shader.PropertyToID("unity_SHAr"),
            Shader.PropertyToID("unity_SHAg"),
            Shader.PropertyToID("unity_SHAb")
        };

            static int[] _idSHB = {
            Shader.PropertyToID("unity_SHBr"),
            Shader.PropertyToID("unity_SHBg"),
            Shader.PropertyToID("unity_SHBb")
        };

            static int _idSHC = Shader.PropertyToID("unity_SHC");
        }
    }
}
