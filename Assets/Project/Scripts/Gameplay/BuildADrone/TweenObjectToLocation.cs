// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Used to hide remaining build a dron parts
    /// </summary>
    public class TweenObjectToLocation : ActiveStateObserver
    {
        [SerializeField]
        private GameObject _itemToMove;

        [SerializeField, FormerlySerializedAs("_thisPosition")]
        private GameObject _location;

        [SerializeField]
        private bool _affectScale = false;

        [SerializeField]
        private Tween.Ease _ease = Tween.Ease.Linear;

        [SerializeField]
        private float _duration = 0.3f;

        [SerializeField]
        private bool _disableGameObjectOnComplete = false;

        protected override void HandleActiveStateChanged()
        {
            if (Active)
            {
                Transform itemToMove = _itemToMove.transform;
                Transform target = _location.transform;

                var tween = TweenRunner.TweenTransform(itemToMove, target, _duration).SetEase(_ease);
                if (_affectScale) TweenRunner.Tween(itemToMove.localScale, target.localScale, _duration, x => itemToMove.localScale = x);

                if (_disableGameObjectOnComplete) tween.OnComplete(() => gameObject.SetActive(false));
            }
        }
    }
}
