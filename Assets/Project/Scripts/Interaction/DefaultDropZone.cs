/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using Oculus.Interaction;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Assigns the dropzones timeout interactable to the zone it's in
    /// This is useful for key items that need to return to their last good position
    /// </summary>
    public class DefaultDropZone : MonoBehaviour
    {
        [SerializeField]
        private SnapInteractor _dropZoneInteractor;

        private void Start()
        {
            _dropZoneInteractor.WhenStateChanged += UpdateDropZone;
            UpdateDropZone(default);
        }

        private void OnDestroy()
        {
            _dropZoneInteractor.WhenStateChanged -= UpdateDropZone;
        }

        private void UpdateDropZone(InteractorStateChangeArgs _)
        {
            if (_dropZoneInteractor.HasSelectedInteractable)
            {
                _dropZoneInteractor.InjectOptionalTimeOutInteractable(_dropZoneInteractor.SelectedInteractable);
            }
        }
    }
}
