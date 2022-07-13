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
    /// Maps an ActiveState onto a UICanvas's Show value
    /// </summary>
    [RequireComponent(typeof(UICanvas)), DisallowMultipleComponent]
    public class ActiveStateUICanvas : ActiveStateObserver
    {
        private UICanvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<UICanvas>();
            HandleActiveStateChanged();
        }

        protected override void HandleActiveStateChanged()
        {
            _canvas.Show(Active);
        }
    }
}
