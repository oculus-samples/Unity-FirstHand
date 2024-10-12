// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true when pressing the trigger down
    /// </summary>
    public class UseInteractableActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField, Interface(typeof(IAxis1D))]
        private MonoBehaviour _axis;
        [SerializeField]
        private float _pressedValue = 0.2f;

        public bool Active => HandTriggerDown();

        private bool HandTriggerDown()
        {
            return (_axis as IAxis1D).Value() > _pressedValue ? true : false;
        }
    }
}
