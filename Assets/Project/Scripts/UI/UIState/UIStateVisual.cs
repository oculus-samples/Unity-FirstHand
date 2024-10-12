// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Base for classes that want to respond to state changes on UI
    /// </summary>
    [ExecuteAlways]
    public abstract class UIStateVisual : MonoBehaviour
    {
        private UIStateParentReference _uiState;
        static IUIState none = new NoneState();

        protected virtual void OnValidate()
        {
            if (_uiState.IsValid && isActiveAndEnabled)
            {
                UpdateVisual(_uiState, false);
            }
        }

        private void InternalUpdateVisual()
        {
            UpdateVisual(_uiState.IsValid && isActiveAndEnabled ? _uiState : none, Application.isPlaying);
        }

        protected virtual void OnEnable()
        {
            _uiState = new UIStateParentReference(transform, InternalUpdateVisual);
            UpdateVisual(_uiState, false);
        }

        protected virtual void OnDisable()
        {
            _uiState.Dispose();
            UpdateVisual(none, false);
        }

        protected abstract void UpdateVisual(IUIState uiState, bool animate);

        private class NoneState : IUIState
        {
            public bool Focused => false;
            public bool Active => false;
            public UIStates State => UIStates.None;
        }
    }

    public struct UIStateParentReference : IDisposable, IUIState
    {
        private ParentChangedListener _parentChangeListener;
        private UIState _uiState;
        private Action _callback;

        public bool IsValid { get; private set; }
        public bool Interactable => IsValid ? _uiState.Interactable : true;
        public bool Focused => IsValid ? _uiState.Focused : false;
        public bool Active => IsValid ? _uiState.Active : false;
        public UIStates State => IsValid ? _uiState.State : UIStates.Normal;

        public UIStateParentReference(Transform child, Action callback) : this()
        {
            _callback = callback;

            _parentChangeListener = ParentChangedListener.Get(child.gameObject);
            _parentChangeListener.WhenParentWillChange += Unregister;
            _parentChangeListener.WhenParentChanged += UpdateUIStateParent;

            UpdateUIStateParent();
        }

        private void UpdateUIStateParent()
        {
            _uiState = _parentChangeListener.GetComponentInParent<UIState>();

            if (_uiState == null)
            {
                var selectable = _parentChangeListener.GetComponentInParent<Selectable>();
                if (selectable != null)
                {
                    _uiState = selectable.gameObject.AddComponent<UIState>();
                    Debug.LogWarning($"UIState added to {selectable.gameObject.name}", selectable);
                }
            }

            if (_uiState != null)
            {
                _uiState.WhenChanged += InvokeWhenChanged;
                IsValid = true;
            }
        }

        private void Unregister()
        {
            if (_uiState != null)
            {
                _uiState.WhenChanged -= InvokeWhenChanged;
                _uiState = null;
            }
            IsValid = false;
        }

        private void InvokeWhenChanged()
        {
            _callback?.Invoke();
        }

        public void Dispose()
        {
            _parentChangeListener.Dispose();
            Unregister();
        }

        private class ParentChangedListener : MonoBehaviour, IDisposable
        {
            public event Action WhenParentWillChange;
            public event Action WhenParentChanged;

            private void OnBeforeTransformParentChanged() => WhenParentWillChange?.Invoke();
            private void OnTransformParentChanged() => WhenParentChanged?.Invoke();

            public static ParentChangedListener Get(GameObject source)
            {
                var result = source.AddComponent<ParentChangedListener>();
                result.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
                return result;
            }

            public void Dispose()
            {
                if (!Application.isPlaying)
                {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.CallbackFunction destroySelf = null;
                destroySelf = () =>
                {
                    UnityEditor.EditorApplication.update -= destroySelf;
                    if (this != null) DestroyImmediate(this);
                };
                UnityEditor.EditorApplication.update += destroySelf;
#endif
                }
                else
                {
                    Destroy(this);
                }
            }
        }
    }
}
