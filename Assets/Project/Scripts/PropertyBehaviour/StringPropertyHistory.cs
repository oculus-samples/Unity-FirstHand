// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Adds a history stack to StringProperty
    /// </summary>
    // TODO? use generic class with concrete implementation for strings
    public class StringPropertyHistory : MonoBehaviour, INavigationHistory
    {
        [SerializeField]
        private StringPropertyRef _stringProperty;

        private List<string> _history = new List<string>();

        [SerializeField, Tooltip("When true, if a value is added that's already " +
            "in the stack its treated the same as going back to that value")]
        private bool _collapse;

        public bool CanGoBack => _history.Count > 1 && _head != 1;

        public bool CanGoForward => _head != _history.Count;

        int _head = 0;

        public event Action WhenChanged;

        private void Reset()
        {
            _stringProperty.Property = GetComponent<IProperty<string>>();
        }

        private void Start()
        {
            _stringProperty.AssertNotNull();
            Clear();
            _stringProperty.WhenChanged += AddHistory;
        }

        private void OnDestroy()
        {
            _stringProperty.WhenChanged -= AddHistory;
        }

        private void AddHistory()
        {
            string value = _stringProperty.Value;

            if (CanGoForward)
            {
                _history.RemoveRange(_head, _history.Count - _head);
            }

            if (_collapse && _history.IndexOf(value) is var i && ++i != 0)
            {
                _history.RemoveRange(i, _history.Count - i);
            }
            else
            {
                _history.Add(value);
            }

            _head = _history.Count;

            WhenChanged?.Invoke();
        }

        public void Back()
        {
            if (!CanGoBack) throw new Exception("Cant call back right now");

            _head--;
            SetValueToHead();
        }

        public void Forward()
        {
            if (!CanGoForward) throw new Exception("Cant call Forward right now");

            _head++;
            SetValueToHead();
        }

        private void SetValueToHead()
        {
            _stringProperty.WhenChanged -= AddHistory;
            _stringProperty.Value = _history[_head - 1];
            _stringProperty.WhenChanged += AddHistory;
            WhenChanged?.Invoke();
        }

        public void Clear()
        {
            _history.Clear();
            AddHistory();
        }

        public int FindInBack(string value, int maxSearch = -1)
        {
            int end = 0;
            if (maxSearch >= 0) end = Math.Max(_head - (maxSearch + 1), end);

            for (int i = _head - 1; i >= end; i--)
            {
                if (_history[i] == value) return _head - i;
            }
            return -1;
        }

        public int FindInForward(string value, int maxSearch)
        {
            int end = _history.Count - 1;
            if (maxSearch >= 0) end = Math.Min(_head + maxSearch, _history.Count - 1);

            for (int i = _head - 1; i <= end; i++)
            {
                if (_history[i] == value) return i - _head;
            }
            return -1;
        }
    }
    public interface INavigationHistory
    {
        event Action WhenChanged;
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        void Back();
        void Forward();
        void Clear();
    }
}
