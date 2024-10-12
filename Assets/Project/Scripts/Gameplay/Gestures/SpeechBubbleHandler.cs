// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Was = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the UI in the speech bubble for the door gesture minigame
    /// </summary>
    public class SpeechBubbleHandler : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField]
        private GestureVisual _leftHandVisual;
        [SerializeField]
        private GestureVisual _rightHandVisual;

        [SerializeField, Was("_correctColour")]
        private Color _correctColor;
        [SerializeField, Was("_wrongColour")]
        private Color _wrongColor;

        [SerializeField]
        private Sprite _correctIcon, _wrongIcon, _defaultIcon;

        [Header("References")]
        [SerializeField]
        private GesturesWithinTriggerArea _triggerArea;
        [SerializeField]
        private ReferenceActiveState _inProgress;
        [SerializeField]
        private ReferenceActiveState _completed;
        [SerializeField]
        private GameObject _completedVisual;

        private void Awake()
        {
            _leftHandVisual.Init();
            _rightHandVisual.Init();
        }

        private void Update()
        {
            if (_inProgress)
            {
                UpdateHandState(_leftHandVisual, _triggerArea.LeftHandActive, _triggerArea.LeftHandInZone);
                UpdateHandState(_rightHandVisual, _triggerArea.RightHandActive, _triggerArea.RightHandInZone);
                _completedVisual.SetActive(_completed);
            }
        }

        private void UpdateHandState(GestureVisual gestureVisual, bool correctGesture, bool inTriggerArea)
        {
            gestureVisual.SetHandEnabled(!_completed);

            Color color = Get(correctGesture, inTriggerArea, _correctColor, _wrongColor, Color.white);
            gestureVisual.SetColor(color);

            Sprite sprite = Get(correctGesture, inTriggerArea, _correctIcon, _wrongIcon, _defaultIcon);
            gestureVisual.SetSprite(sprite);
        }

        /// <summary>
        /// Returns the 'correctValue' if the task is completed or correct is true
        /// Returns the 'wrongValue' if the task is in the zone but wrong
        /// otherwise returns the default value
        /// </summary>
        private T Get<T>(bool correct, bool inZone, T correctValue, T wrongValue, T defaultValue)
        {
            if (_completed) return correctValue;
            if (!inZone) return defaultValue;
            return correct ? correctValue : wrongValue;
        }
    }
}

[System.Serializable]
public struct GestureVisual
{
    [Header("Sprites")]
    [SerializeField]
    private GameObject _hand;

    [Header("Components")]
    [SerializeField]
    private Image _iconImage;
    [SerializeField]
    private Image _bar;

    private List<Graphic> _handElements;

    public void Init()
    {
        _handElements = new List<Graphic>();
        _hand.GetComponentsInChildren(_handElements);
    }

    public void SetColor(Color color)
    {
        _handElements.ForEach(x => x.color = color);
        _iconImage.color = color;
        _bar.color = color;
    }

    public void SetSprite(Sprite sprite)
    {
        _iconImage.sprite = sprite;
    }

    public void SetHandEnabled(bool enabled)
    {
        _hand.SetActive(enabled);
    }
}
