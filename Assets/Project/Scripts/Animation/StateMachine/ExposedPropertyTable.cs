// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Basic implementation of IExposedPropertyTable used for ExposedReferences
    /// </summary>
    public class ExposedPropertyTable : MonoBehaviour, IExposedPropertyTable
    {
        // TODO use dictionary at runtime
        [SerializeField]
        List<PropertyName> _propertyNames = new List<PropertyName>();
        [SerializeField]
        List<Object> _objects = new List<Object>();

        public void ClearReferenceValue(PropertyName id)
        {
            for (int i = _propertyNames.Count - 1; i >= 0; i--)
            {
                if (_propertyNames[i] == id)
                {
                    _propertyNames.RemoveAt(i);
                    _objects.RemoveAt(i);
                }
            }
        }

        public Object GetReferenceValue(PropertyName id, out bool idValid)
        {
            var index = _propertyNames.IndexOf(id);
            idValid = index != -1;
            return idValid ? _objects[index] : null;
        }

        public void SetReferenceValue(PropertyName id, Object value)
        {
            if (PropertyName.IsNullOrEmpty(id)) { return; }

            var index = _propertyNames.IndexOf(id);

            if (index != -1)
            {
                _propertyNames[index] = id;
                _objects[index] = value;
            }
            else if (value == null)
            {
                _propertyNames.Add(id);
                _objects.Add(null);
            }

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}
