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
    /// Checks for an ActiveState to have changed in Update
    /// Similar to ActiveStateSelector but using ReferenceActiveState and designed for inheritance
    /// </summary>
    public abstract class ActiveStateObserver : MonoBehaviour
    {
        [SerializeField]
        private ReferenceActiveState _activeState;

        protected bool Active { get; private set; }

        protected virtual void Reset()
        {
            _activeState.InjectActiveState(GetComponent<IActiveState>());
        }

        protected virtual void Start()
        {
            _activeState.AssertNotNull($"{name} ({GetType()}) requires an IActiveState assigned");
        }

        protected virtual void Update()
        {
            if (Active != _activeState.Active)
            {
                Active = !Active;
                HandleActiveStateChanged();
            }
        }

        protected abstract void HandleActiveStateChanged();
    }
}
