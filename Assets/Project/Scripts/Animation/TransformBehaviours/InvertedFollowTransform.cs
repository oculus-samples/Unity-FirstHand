// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using static Oculus.Interaction.ComprehensiveSample.FollowTransform;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class InvertedFollowTransform : MonoBehaviour
    {
        static readonly Dictionary<GameObject, InvertedFollowTransform> _mostRecentActivator = new Dictionary<GameObject, InvertedFollowTransform>();

        [SerializeField] GuidReference _folllower;
        [SerializeField] FollowPosition _positionSettings;
        [SerializeField] FollowRotation _rotationSettings;
        [SerializeField] When _when = When.Update;
        [SerializeField] bool _controlActiveStateAndLayer;

        private FollowTransform _followerComponent;

        [ContextMenu("Align With Target")]
        private void AlignWithTarget()
        {
            if (_folllower.gameObject)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(transform, "Align With Target");
#endif
                transform.SetPose(_folllower.gameObject.transform.GetPose());
            }
        }


        private void Start()
        {
            _folllower.OnGuidAdded += OnGuidAdded;
            CreateFollower();
            void OnGuidAdded(GameObject _) => CreateFollower();
        }

        private void OnEnable()
        {
            SetFollowerActive(true);
        }

        private void SetFollowerActive(bool active)
        {
            if (!_followerComponent) return;

            _followerComponent.enabled = active;

            if (_controlActiveStateAndLayer)
            {
                var go = _followerComponent.gameObject;
                if (active || (_mostRecentActivator.ContainsKey(go) && _mostRecentActivator[go] == this))
                {
                    _mostRecentActivator[go] = active ? this : null;
                    go.SetActive(active);
                }
            }
        }

        private void OnDisable()
        {
            SetFollowerActive(false);
        }

        private void OnDestroy()
        {
            if (!_followerComponent) return;

            if (_controlActiveStateAndLayer) _folllower.gameObject.SetActive(true);
            Destroy(_followerComponent);
        }

        private void CreateFollower()
        {
            if (_followerComponent) return;

            var followerGO = _folllower.gameObject;
            if (!followerGO) return;

            _followerComponent = followerGO.AddComponent<FollowTransform>();
            _followerComponent.Source = transform;
            _followerComponent.positionSettings = _positionSettings;
            _followerComponent.rotationSettings = _rotationSettings;
            _followerComponent.when = _when;

            if (_controlActiveStateAndLayer)
            {
                SetLayer(_followerComponent.transform, gameObject.layer);
                SetFollowerActive(gameObject.activeInHierarchy);
            }
        }

        public static void SetLayer(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform child in t)
                SetLayer(child, layer);
        }
    }
}
