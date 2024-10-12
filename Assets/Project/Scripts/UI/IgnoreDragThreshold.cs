// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Does not use the drag threshold
    /// </summary>
    public class IgnoreDragThreshold : MonoBehaviour, IInitializePotentialDragHandler
    {
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
    }
}
