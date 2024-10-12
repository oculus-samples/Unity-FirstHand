// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Controls raising the items in the mixed reality level
    /// </summary>
    public class RaiseItemsWhenBayActive : MonoBehaviour
    {
        [SerializeField] ReferenceActiveState _progressReference;
        [SerializeField] float _raiseDistance;
        [SerializeField] float _raiseTime;
        [SerializeField] GameObject _thisObject;

        private bool _hasRaised = false;

        void Update()
        {
            if (!_hasRaised && _thisObject.transform.position.y < _raiseDistance && _progressReference.Active)
            {
                RaiseItem();
            }
        }

        public void RaiseItem()
        {
            _hasRaised = true;

            var targetPosition = _thisObject.transform.position.SetY(_raiseDistance);
            TweenRunner.TweenPosition(_thisObject.transform, targetPosition, _raiseTime).SetEase(Tween.Ease.QuadOut);
        }
    }
}
