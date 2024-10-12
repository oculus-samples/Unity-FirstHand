// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Dissolve effect to be used on plane surfaces
    /// </summary>
    public class DissolvePlaneEffect : RendererEffect
    {
        private static readonly int _dissolvePlaneID = Shader.PropertyToID("Dissolve_Plane");
        private static readonly int _dissolveEdgeID = Shader.PropertyToID("Dissolve_Edge");
        private static readonly int _dissolveEdgeSettingsID = Shader.PropertyToID("Dissolve_EdgeSettings");
        private static readonly int _dissolveColorID = Shader.PropertyToID("Dissolve_Color");

        [SerializeField]
        private Transform _plane;
        [SerializeField, Optional]
        private Transform _fallbackPlane;

        [SerializeField] private float _visibility = 1;
        [SerializeField] private Color _dissolveColor;
        [SerializeField, Tooltip("Inner Fade, Band Width, Outer Fade, Tiling")]
        private Vector4 _dissolveEdgeSettings;
        [SerializeField]
        private bool _controlRendersEnabled;

        private Vector4 _planeValue;

        protected override void UpdateMaterial(Material material)
        {
            base.UpdateMaterial(material);
            material.EnableKeyword("DISSOLVE_PLANE_ON");
        }

        protected override void Update()
        {
            Transform planeSource = _plane;
            if ((!planeSource || !planeSource.gameObject.activeInHierarchy) && _fallbackPlane)
            {
                planeSource = _fallbackPlane;
            }

            var plane = new Plane(planeSource.forward, planeSource.position);
            _planeValue = PlaneToVector4(plane);

            base.Update();

            if (Application.isPlaying && _controlRendersEnabled)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderers[i].enabled = !GetSide(plane, _renderers[i].bounds);
                }
            }
        }

        protected override void UpdateProperties(MaterialPropertyBlock block)
        {
            base.UpdateProperties(block);
            block.SetVector(_dissolvePlaneID, _planeValue);
            block.SetFloat(_dissolveEdgeID, _visibility);
            block.SetVector(_dissolveEdgeSettingsID, _dissolveEdgeSettings);
            block.SetColor(_dissolveColorID, _dissolveColor);
        }

        static Vector4 PlaneToVector4(Plane plane)
        {
            Vector4 result = plane.normal;
            result.w = plane.distance;
            return result;
        }

        static bool GetSide(Plane plane, Bounds bounds)
        {
            var nnn = bounds.min;
            var xxx = bounds.max;
            var nnx = new Vector3(nnn.x, nnn.y, xxx.z);
            var nxn = new Vector3(nnn.x, xxx.y, nnn.z);
            var xnn = new Vector3(xxx.x, nnn.y, nnn.z);
            var nxx = new Vector3(nnn.x, xxx.y, xxx.z);
            var xnx = new Vector3(xxx.x, nnn.y, xxx.z);
            var xxn = new Vector3(xxx.x, xxx.y, nnn.z);
            return
                plane.GetSide(nnn) &&
                plane.GetSide(xxx) &&
                plane.GetSide(nnx) &&
                plane.GetSide(nxn) &&
                plane.GetSide(xnn) &&
                plane.GetSide(nxx) &&
                plane.GetSide(xnx) &&
                plane.GetSide(xxn);
        }
    }
}
