// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// When applied to a Button, causes it to perform its onClick function when it receives OnPointerDown
    /// This is useful in MR where poke limiting may not be applied, where simply touching the button causes its function rather than press and release
    /// </summary>
    public class ExecuteClickOnPointerDown : MonoBehaviour, IPointerDownHandler
    {
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            var pointerClickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);

            if (gameObject == pointerClickHandler && eventData.eligibleForClick)
            {
                ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerClickHandler);
            }

            eventData.eligibleForClick = false;
        }
    }
}
