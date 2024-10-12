// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class SprayCan : MonoBehaviour, IHandGrabUseDelegate
    {
        private static RaycastHit[] _hits = new RaycastHit[12];

        [SerializeField]
        Transform _nozzle;
        [SerializeField]
        StringPropertyRef _color;
        [SerializeField]
        private float _sprayDistance;
        [SerializeField]
        private float _sprayRadius;

        private bool _isUsing;

        public void BeginUse()
        {
            _isUsing = true;
        }

        public float ComputeUseStrength(float strength)
        {
            return strength;
        }

        void Update()
        {
            if (Time.timeScale == 0) return;
            if (!_isUsing) return;

            var capStart = _nozzle.position;
            var capDir = _nozzle.forward;
            var capEnd = capStart + capDir * _sprayDistance;

            int hits = Physics.CapsuleCastNonAlloc(capStart, capEnd, _sprayRadius, capDir, _hits, 2, ~0, QueryTriggerInteraction.Collide);

            for (int i = 0; i < hits; i++)
            {
                var hit = _hits[i];
                var hitObject = hit.rigidbody ? hit.rigidbody.gameObject : hit.collider.gameObject;

                if (hitObject.TryGetComponent<ISprayCan>(out var spray))
                {
                    spray.Spray(_color.Value);
                    return;
                }
            }
        }

        public void EndUse()
        {
            _isUsing = false;
        }

    }

    interface ISprayCan
    {
        void Spray(string value);
    }
}
