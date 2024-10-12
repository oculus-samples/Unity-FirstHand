// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Shows a preview of the UI states applied to an object
    /// </summary>
    [ExecuteAlways]
    public class UIState : MonoBehaviour, IUIState,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private UIStates _editorPreview = UIStates.None;

        public event Action WhenChanged;
        public bool Interactable => _groupsAllowInteraction;
        public bool Focused { get; private set; } = false;
        public bool Active => _isSelectable && _selectable is Toggle t && t.isOn;

        public UIStates State
        {
            get
            {
#if UNITY_EDITOR
            if (!Application.isPlaying && _editorPreview != UIStates.None) return _editorPreview;
#endif
                if (!_groupsAllowInteraction || (_isSelectable && !_isSelectableInteractable)) return UIStates.Disabled;
                if (_isPointerDown) return UIStates.Pressed;
                if (_isPointerInside) return UIStates.Hovered;
                return UIStates.Normal;
            }
        }

        private readonly List<CanvasGroup> _canvasGroupCache = new List<CanvasGroup>();
        private bool _groupsAllowInteraction = true;
        private bool _isPointerInside;
        private bool _isPointerDown;
        private Selectable _selectable;
        private bool _isSelectable;
        private bool _isSelectableInteractable;

        private void OnValidate()
        {
            InvokeWhenChanged();
        }

        void OnEnable()
        {
            _editorPreview = UIStates.None;

            _isSelectable = TryGetComponent(out _selectable);
            _isSelectableInteractable = _isSelectable && _selectable.interactable;
            if (_isSelectable && _selectable is Toggle toggle)
            {
                toggle.onValueChanged.AddListener(InvokeWhenChanged);
            }
        }

        void OnDisable()
        {
            if (_isSelectable && _selectable is Toggle toggle)
            {
                toggle.onValueChanged.RemoveListener(InvokeWhenChanged);
            }
        }

        void LateUpdate()
        {
            if (_isSelectable && _isSelectableInteractable != _selectable.interactable)
            {
                _isSelectableInteractable = _selectable.interactable;
                InvokeWhenChanged();
            }
        }

        private void InvokeWhenChanged(bool _) => InvokeWhenChanged();

        private void InvokeWhenChanged()
        {
            WhenChanged?.Invoke();
        }

        private void OnCanvasGroupChanged()
        {
            var groupAllowInteraction = true;
            Transform t = transform;
            while (t != null)
            {
                t.GetComponents(_canvasGroupCache);
                bool shouldBreak = false;
                for (var i = 0; i < _canvasGroupCache.Count; i++)
                {
                    if (!_canvasGroupCache[i].interactable)
                    {
                        groupAllowInteraction = false;
                        shouldBreak = true;
                    }

                    if (_canvasGroupCache[i].ignoreParentGroups)
                        shouldBreak = true;
                }
                if (shouldBreak)
                    break;

                t = t.parent;
            }

            if (groupAllowInteraction != _groupsAllowInteraction)
            {
                _groupsAllowInteraction = groupAllowInteraction;
                InvokeWhenChanged();
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _isPointerDown = true;
            InvokeWhenChanged();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _isPointerDown = false;
            InvokeWhenChanged();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerInside = true;
            InvokeWhenChanged();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            _isPointerInside = false;
            InvokeWhenChanged();
        }
    }

    public interface IUIState
    {
        public bool Focused { get; }
        public bool Active { get; }
        public UIStates State { get; }
    }

    public enum UIStates
    {
        Normal,
        Hovered,
        Pressed,
        Disabled,
        None
    }

    [Serializable]
    public struct UIStateValues<T>
    {
        public T normal;
        public T hovered;
        public T pressed;
        public T disabled;

        public T GetValue(UIStates state, T defaultValue)
        {
            switch (state)
            {
                case UIStates.Normal: return normal;
                case UIStates.Hovered: return hovered;
                case UIStates.Pressed: return pressed;
                case UIStates.Disabled: return disabled;
                case UIStates.None: return defaultValue;
                default: throw new Exception($"Cant handle {state}");
            }
        }
    }
}
