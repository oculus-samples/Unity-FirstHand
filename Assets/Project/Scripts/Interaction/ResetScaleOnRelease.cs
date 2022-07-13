/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Tweens a Grabbable back to it's initial scale when it's released
    /// </summary>
    [RequireComponent(typeof(Grabbable))]
    public class ResetScaleOnRelease : MonoBehaviour
    {
        private Grabbable _grabbable;
        private Vector3 _initialScale;

        private void Awake()
        {
            _grabbable = GetComponent<Grabbable>();
            _initialScale = transform.localScale;
            _grabbable.WhenPointerEventRaised += HandlePointerRaised;
        }

        private void OnDestroy()
        {
            _grabbable.WhenPointerEventRaised -= HandlePointerRaised;
        }

        private void HandlePointerRaised(PointerArgs obj)
        {
            if (obj.PointerEvent != PointerEvent.Unselect || _grabbable.SelectingPointsCount > 0) { return; }

            var startScale = transform.localScale;
            TweenRunner.Tween01(0.5f, x => transform.localScale = Vector3.Lerp(startScale, _initialScale, x));
        }
    }
}
