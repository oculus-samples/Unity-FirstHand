// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Gets the source of the portal culling
    /// </summary>
    public class PortalCullingRendererSource : MonoBehaviour
    {
        [SerializeField]
        PortalCulling _portal;

        [SerializeField, Optional]
        GuidReference _guidReference;

        void Start()
        {
            if (_guidReference.gameObject != null)
            {
                AddChildren(_guidReference.gameObject);
            }
            else
            {
                _guidReference.OnGuidAdded += AddChildren;
            }

            AddChildren(gameObject);
        }

        private void OnDestroy()
        {
            _guidReference.OnGuidAdded -= AddChildren;
        }

        private void AddChildren(GameObject obj)
        {
            Renderer[] collection = obj.GetComponentsInChildren<Renderer>(true);
            _portal._renderers.AddRange(collection);
        }
    }
}
