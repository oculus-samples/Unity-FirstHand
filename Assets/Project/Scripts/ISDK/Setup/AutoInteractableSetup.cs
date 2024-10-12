// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Automatically sets up Interactables to reduce drag dropping
    /// </summary>
    public class AutoInteractableSetup : MonoBehaviour
    {
        static List<HandGrabInteractable> _interactables = new List<HandGrabInteractable>();

        void Awake()
        {
            var physicsGrabbable = GetComponentInParent();
            if (physicsGrabbable == null)
            {
                Debug.LogWarning($"{nameof(AutoInteractableSetup)} expects to be a child of a {nameof(PhysicsGrabbable)}");
                return;
            }

            physicsGrabbable.GetComponentsInChildren(true, _interactables);
            _interactables.ForEach(x => x.InjectOptionalPhysicsGrabbable(physicsGrabbable));
        }


        private PhysicsGrabbable GetComponentInParent()
        {
            for (var t = transform.parent; t; t = t.parent)
            {
                if (t.TryGetComponent<PhysicsGrabbable>(out var result))
                {
                    return result;
                }
            }
            return null;
        }
    }
}
