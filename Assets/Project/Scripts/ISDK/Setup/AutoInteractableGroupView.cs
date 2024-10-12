// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Automatically finds Interactables in children
    /// </summary>
    public class AutoInteractableGroupView : InteractableGroupView
    {
        protected override void Awake()
        {
            base.Awake();
            var list = new List<IInteractableView>();
            GetComponentsInChildren(true, list);
            list.RemoveAll(x => (Object)x == this || x.GetType().IsAssignableFrom(typeof(HandGrabUseInteractable)));
            list.RemoveAll(x => (Object)x == this || x.GetType().IsAssignableFrom(typeof(PokeInteractable)));
            InjectInteractables(list);
        }
    }
}
