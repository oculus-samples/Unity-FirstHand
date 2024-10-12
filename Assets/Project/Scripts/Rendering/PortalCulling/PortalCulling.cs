// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Culls the room the portal is displaying based
    /// on the position / angle of the player
    /// </summary>
    public class PortalCulling : MonoBehaviour
    {
        static Camera _mainCamera;

        [SerializeField]
        private Transform _portal;
        [SerializeField]
        public readonly List<Renderer> _renderers = new List<Renderer>();
        [SerializeField]
        private float _radius;
        [SerializeField]
        private LayerMask _layerPassFilter = ~0;

        readonly Plane[] _portalPlanes = new Plane[6];
        private bool _cullAll;

        private void Update()
        {
            UpdatePlanes();
            _renderers.ForEach(CullRenderer);
        }

        private void OnDisable()
        {
            _renderers.ForEach(x => { if (x) x.forceRenderingOff = false; });
        }

        private void CullRenderer(Renderer obj)
        {
            if (!obj.enabled || !obj.gameObject.activeInHierarchy || (_layerPassFilter & 1 << obj.gameObject.layer) == 0) return;

            bool shouldCull = _cullAll || ShouldCull(obj.bounds);
            if (obj.forceRenderingOff != shouldCull)
            {
                obj.forceRenderingOff = shouldCull;
            }
        }

        private void UpdatePlanes()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            GeometryUtility.CalculateFrustumPlanes(_mainCamera, _portalPlanes);
            var portalBounds = new Bounds(_portal.position, Vector3.one * _radius * 2);
            _cullAll = ShouldCull(portalBounds);
            if (_cullAll) return;

            Transform camera = _mainCamera.transform;
            var cameraPosition = camera.position;
            var portalLeft = _portal.position - _portal.right * _radius;
            var portalRight = _portal.position + _portal.right * _radius;
            var portalUp = _portal.position + _portal.up * _radius;
            var portalDown = _portal.position - _portal.up * _radius;

            _portalPlanes[0] = new Plane(Vector3.Cross(portalLeft - cameraPosition, _portal.up), portalLeft);
            _portalPlanes[1] = new Plane(Vector3.Cross(portalRight - cameraPosition, -_portal.up), portalRight);
            _portalPlanes[2] = new Plane(Vector3.Cross(portalDown - cameraPosition, -_portal.right), portalDown);
            _portalPlanes[3] = new Plane(Vector3.Cross(portalUp - cameraPosition, _portal.right), portalUp);
            _portalPlanes[4] = new Plane(-_portal.forward, _portal.position);
            _portalPlanes[5] = new Plane(-camera.forward, cameraPosition + camera.forward * _mainCamera.farClipPlane);

            _cullAll = ShouldCull(portalBounds);
        }

        private bool ShouldCull(Bounds renderer)
        {
            return !GeometryUtility.TestPlanesAABB(_portalPlanes, renderer);
        }
    }
}
