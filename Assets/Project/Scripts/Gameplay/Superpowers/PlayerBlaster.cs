/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// This is a simple wrapper around the "Blaster" components that the player controls, that allows it to be enabled
    /// and disabled via an ActiveState.
    /// </summary>
    public class PlayerBlaster : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IFireable))]
        private MonoBehaviour _blaster;

        [SerializeField, Interface(typeof(IActiveState))]
        private MonoBehaviour _activeState;

        public IActiveState ActiveState { get; private set; }
        public IFireable Fireable { get; private set; }

        private void Awake()
        {
            ActiveState = _activeState as IActiveState;
            Fireable = _blaster as IFireable;
        }

        private void Start()
        {
            Assert.IsNotNull(Fireable);
            Assert.IsNotNull(ActiveState);
        }

        public void OnBlastGesture()
        {
            if (ActiveState.Active)
            {
                Fireable.Fire();
            }
        }
    }
}
