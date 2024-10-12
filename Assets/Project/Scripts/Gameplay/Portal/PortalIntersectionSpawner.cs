// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    [RequireComponent(typeof(UIList))]
    public class PortalIntersectionSpawner : MonoBehaviour
    {
        [SerializeField]
        TriggerZone _zone;

        UIList _list;
        TriggerZoneList<Collider> _zoneList;
        List<Collider> _intersectingColliders = new List<Collider>();

        private void Awake()
        {
            _zoneList = new TriggerZoneList<Collider>(_zone);
            _zoneList.WhenAdded += HandleAdd;
            _zoneList.WhenRemoved += HandleRemove;

            _list = GetComponent<UIList>();
            _list.SetList(_intersectingColliders);
        }

        private void Update()
        {
            _list.ForEach((collider, instance) =>
            {
                var position = (collider as Collider).bounds.center;
                var positionOnLocalSurface = transform.InverseTransformPoint(position).SetY(0);
                var positionWorld = transform.TransformPoint(positionOnLocalSurface);
                instance.transform.position = position;
            });
        }

        private void HandleAdd(Collider obj)
        {
            _intersectingColliders.Add(obj);
            _list.UpdateUI();
        }

        private void HandleRemove(Collider obj)
        {
            _intersectingColliders.RemoveAll(x => x == obj || x == null);
            _list.UpdateUI();
        }
    }
}
