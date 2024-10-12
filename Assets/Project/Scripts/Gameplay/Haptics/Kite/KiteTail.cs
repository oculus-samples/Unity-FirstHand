// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class KiteTail : MonoBehaviour
    {
        [SerializeField]
        private Transform _initialTail;
        [SerializeField]
        private GameObject _tailPrefab;
        [SerializeField]
        private float _step;
        [SerializeField]
        private float _xOffset, _yOffset, _zOffset;
        [SerializeField]
        private bool _hasRandomRotation;

        private List<Transform> _tails = new List<Transform>();
        private int _maxLength = 12;

        public int Length => _tails.Count - 1;

        private void Start()
        {
            _tails.Add(_initialTail);
        }

        public void GrowTail()
        {
            if (_tails.Count == _maxLength) return;

            GameObject tail = Instantiate(_tailPrefab);
            Transform previousTail = _tails[_tails.Count - 1];
            tail.GetComponent<FollowTransform>().Source = previousTail;
            tail.GetComponentInChildren<LineBetweenTransforms>().Transforms[0] = previousTail;
            tail.transform.position = previousTail.position;
            _tails.Add(tail.transform);
        }

        public void TransferTo(Transform t, float destroyAfter = -1)
        {
            _tails[1].GetComponent<FollowTransform>().Source = t;
            _tails[1].GetComponentInChildren<LineBetweenTransforms>().Transforms[0] = _tails[1];

            if (destroyAfter >= 0)
            {
                for (int i = 1; i < _tails.Count; i++)
                {
                    var tail = _tails[i].gameObject;
                    var ft = tail.GetComponent<FollowTransform>();
                    var rotSettings = ft.rotationSettings;
                    rotSettings.RotationSmoothing = 0;
                    ft.rotationSettings = rotSettings;
                    var posSettings = ft.positionSettings;
                    posSettings.PositionOffset = Vector3.zero;
                    posSettings.MaxSmoothingDisplacement = 1;
                    ft.positionSettings = posSettings;
                    Destroy(tail, destroyAfter);
                }
            }

            _tails.Clear();
            _tails.Add(_initialTail);
        }
    }
}
