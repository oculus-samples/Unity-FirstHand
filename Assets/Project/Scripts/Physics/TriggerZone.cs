// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Tracks colliders that enter and exit this trigger zone
    /// </summary>
    public class TriggerZone : MonoBehaviour
    {
        private HashSet<Collider> _current = new HashSet<Collider>();
        private List<Collider> _previous = new List<Collider>();

        public event Action<Collider> WhenAdded;
        public event Action<Collider> WhenRemoved;

        private void OnTriggerEnter(Collider other) => Add(other);
        private void OnTriggerStay(Collider other) => Add(other);
        private void FixedUpdate() => Flush();

        private void Add(Collider other) => _current.Add(other);

        private void Flush()
        {
            RemoveCollidersThatExited();
            AddCollidersThatEntered();
        }

        private void RemoveCollidersThatExited()
        {
            for (int i = _previous.Count - 1; i >= 0; i--)
            {
                var collider = _previous[i];
                if (!_current.Contains(collider))
                {
                    _previous.RemoveAt(i);
                    WhenRemoved?.Invoke(collider);
                }
            }
        }

        private void AddCollidersThatEntered()
        {
            _previous.ForEach(x => _current.Remove(x));
            if (_current.Count == 0) return;

            foreach (var collider in _current)
            {
                if (collider == null) continue;

                _previous.Add(collider);
                WhenAdded?.Invoke(collider);
            }

            _current.Clear();
        }

        public void ForEach(Action<Collider> action)
        {
            foreach (var collider in _current)
            {
                action(collider);
            }
        }
    }

    public class TriggerZoneList<T> : IDisposable
    {
        private readonly TriggerZone _zone;
        private readonly Dictionary<T, int> _inZone = new Dictionary<T, int>();
        private readonly List<T> _inZoneList = new List<T>();

        public int Count => _inZone.Count;

        public event Action WhenChanged;
        public event Action<T> WhenAdded;
        public event Action<T> WhenRemoved;

        public T this[int index] => _inZoneList[index];

        public TriggerZoneList(TriggerZone zone)
        {
            _zone = zone;
            _zone.WhenAdded += Add;
            _zone.WhenRemoved += Remove;
        }

        public void Dispose()
        {
            _zone.WhenAdded -= Add;
            _zone.WhenRemoved -= Remove;
        }

        protected void Add(Collider body)
        {
            var comp = body.GetComponentInParent<T>();
            if (comp == null) return;

            _inZone.TryGetValue(comp, out int v);
            _inZone[comp] = v + 1;

            if (v == 0)
            {
                _inZoneList.Add(comp);
                WhenAdded?.Invoke(comp);
            }

            WhenChanged?.Invoke();
        }

        protected void Remove(Collider body)
        {
            if (HandleDestroyed(body))
            {
                WhenChanged?.Invoke();
                return;
            }

            var comp = body.GetComponentInParent<T>();

            if (comp == null) return;
            if (!_inZone.TryGetValue(comp, out int v)) return;

            _inZone[comp] = v - 1;
            if (v == 1)
            {
                _inZone.Remove(comp);
                _inZoneList.Remove(comp);
                WhenRemoved?.Invoke(comp);
                WhenChanged?.Invoke();
            }
        }

        private bool HandleDestroyed(Collider body)
        {
            if (body == null)
            {
                var keys = new List<T>(_inZone.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i] == null)
                    {
                        _inZone.Remove(keys[i]);
                    }
                }
                return true;
            }
            return false;
        }
    }
}
