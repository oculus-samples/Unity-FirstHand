// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class BuildADroneSnapZone : MonoBehaviour
    {
        [SerializeField]
        private SnapInteractable _snapZone;
        [SerializeField]
        private float _moveDuration = 1;
        [SerializeField]
        StringPropertyRef[] _connectedProperties;
        [SerializeField]
        private bool _deactivateDropZone = false;

        private InteractionTracker _interactionTracker;

        private void Reset()
        {
            _snapZone = GetComponent<SnapInteractable>();
        }

        private void Awake()
        {
            _interactionTracker = new InteractionTracker(_snapZone);
            _interactionTracker.WhenSelectAdded += UpdateGloveColor;
        }

        private void UpdateGloveColor(IInteractorView obj)
        {
            var interactor = obj as SnapInteractor;
            var mainItem = (interactor.PointableElement as PointableElement).gameObject;
            var colors = mainItem.GetComponentsInChildren<StringPropertyBehaviour>(true);

            StartCoroutine(Swap());
            IEnumerator Swap()
            {
                yield return new WaitForSeconds(_moveDuration);
                for (int i = 0; i < colors.Length; i++)
                {
                    _connectedProperties[i].Value = colors[i].Value;
                }
                mainItem.gameObject.SetActive(false);

                if (_deactivateDropZone)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
