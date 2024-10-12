// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Proxy for an IProperty, useful for cross prefab references
    /// </summary>
    public class PropertyBehaviourRef : MonoBehaviour, IProperty, ISerializationCallbackReceiver
    {
        [SerializeField, Interface(typeof(IProperty))]
        MonoBehaviour _property;
        public IProperty Property;

        public event Action WhenChanged
        {
            add => _whenChanged += value;
            remove => _whenChanged -= value;
        }

        private event Action _whenChanged;

        protected virtual void Awake()
        {
            Property.WhenChanged += HandlePropertyChanged;
        }

        protected virtual void OnDestroy()
        {
            if (Property != null)
            {
                Property.WhenChanged -= HandlePropertyChanged;
            }
        }

        protected virtual void HandlePropertyChanged() => _whenChanged?.Invoke();

        protected virtual void Start()
        {
            Assert.IsNotNull(Property);
        }

        public override string ToString()
        {
            return Property.ToString();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Property = _property as IProperty;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    }
}
