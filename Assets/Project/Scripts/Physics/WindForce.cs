// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Adds wind force to area in level
    /// </summary>
    public class WindForce : MonoBehaviour
    {
        [SerializeField] public TriggerZone _windZone;
        [SerializeField] public float _windForce;
        [SerializeField] public float _variance;
        [SerializeField] public Transform _directionMarker;

        public TriggerZoneList<Rigidbody> _rigidbodiesInZone;
        private List<Rigidbody> _rigidbodyList = new List<Rigidbody>();

        private void Awake()
        {
            _rigidbodiesInZone = new TriggerZoneList<Rigidbody>(_windZone);
            _rigidbodiesInZone.WhenAdded += _rigidbodyList.Add;
            _rigidbodiesInZone.WhenRemoved += HandleRemove;
        }

        private void FixedUpdate()
        {
            float windForceMin = _windForce - _variance;
            float windForceMax = _windForce + _variance;

            foreach (Rigidbody rb in _rigidbodyList)
            {
                float windForceRandom = Random.Range(windForceMin, windForceMax);
                Vector3 forceInDirection = _directionMarker.forward * windForceRandom;
                rb.AddForce(forceInDirection, ForceMode.Force);
            }
        }

        private void HandleRemove(Rigidbody rb)
        {
            _rigidbodyList.Remove(rb);
        }
    }
}
