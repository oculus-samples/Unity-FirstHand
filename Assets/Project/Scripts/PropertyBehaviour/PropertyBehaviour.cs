// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Base class for PropertyBehaviour<T>
    /// </summary>
    public abstract class PropertyBehaviour : MonoBehaviour, IProperty
    {
        public event Action WhenChanged = delegate { };

        protected void InvokeWhenChanged()
        {
            WhenChanged();
        }
    }

    /// <summary>
    /// IProperty implementation that holds a value of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PropertyBehaviour<T> : PropertyBehaviour, IProperty<T>
    {
        public virtual T Value { get => _value; set => SetValue(value); }
        protected T _value = default(T);

        public void SetValue(T value)
        {
            if (_value == null && value == null) { return; }
            if (_value != null && _value.Equals(value)) { return; }

            _value = value;
            InvokeWhenChanged();
        }

        public override string ToString()
        {
            return _value != null ? _value.ToString() : "";
        }
    }
}
