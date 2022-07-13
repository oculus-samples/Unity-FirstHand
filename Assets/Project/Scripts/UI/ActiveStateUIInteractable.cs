/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Maps an ActiveState onto a UGUI Canvas Group or Selectables .interactable property
    /// </summary>
    [DisallowMultipleComponent]
    public class ActiveStateUIInteractable : ActiveStateObserver
    {
        [SerializeField]
        private bool _controlBlocksRaycast;

        private bool _hasCanvasGroup;
        private bool _hasSelectable;

        private CanvasGroup _canvasGroup;
        private Selectable _selectable;

        protected override void Start()
        {
            base.Start();
            _hasCanvasGroup = TryGetComponent(out _canvasGroup);
            _hasSelectable = TryGetComponent(out _selectable);
            Assert.IsTrue(_hasCanvasGroup || _hasSelectable, $"{nameof(ActiveStateUIInteractable)} should have a CanvasGroup or Selectable component");
        }

        protected override void HandleActiveStateChanged()
        {
            if (_hasSelectable) { _selectable.interactable = Active; }
            if (_hasCanvasGroup) { _canvasGroup.interactable = Active; }
            if (_hasCanvasGroup && _controlBlocksRaycast) { _canvasGroup.blocksRaycasts = Active; }
        }
    }
}
