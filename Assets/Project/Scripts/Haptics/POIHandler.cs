// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles POI visual state
    /// </summary>
    public class POIHandler : MonoBehaviour
    {
        [SerializeField] Grabbable _grabbable;
        [SerializeField] GameObject _activeState;
        [SerializeField] GameObject _hoverState;
        [SerializeField] GameObject _grabbedState;
        [SerializeField] GameObject _pulse;

        private InteractionTracker _interactionTracker;

        private void Awake()
        {
            _interactionTracker = new InteractionTracker(_grabbable);
        }

        private void Update()
        {
            bool selected = _interactionTracker.IsSelected();
            bool hovered = _interactionTracker.IsHovered();

            var activeVisual =
                selected ? _grabbedState :
                hovered ? _hoverState :
                _activeState;

            _activeState.SetActive(activeVisual == _activeState);
            _hoverState.SetActive(activeVisual == _hoverState);
            _grabbedState.SetActive(activeVisual == _grabbedState);
            _pulse.SetActive(_interactionTracker.TimesSelected < 1);
        }
    }
}
