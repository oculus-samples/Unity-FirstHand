// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Unity event wrapper when property value changes
    /// </summary>
    [RequireComponent(typeof(IProperty))]
    public class PropertyChangedUnityEventWrapper : MonoBehaviour
    {
        [SerializeField]
        UnityEvent _whenChanged;

        private IProperty _property;

        private void OnEnable()
        {
            _property = GetComponent<IProperty>();
            _property.WhenChanged += InvokeEvent;
        }

        private void OnDisable()
        {
            if (_property != null)
            {
                _property.WhenChanged -= InvokeEvent;
                _property = null;
            }
        }

        private void InvokeEvent() => _whenChanged.Invoke();
    }
}
