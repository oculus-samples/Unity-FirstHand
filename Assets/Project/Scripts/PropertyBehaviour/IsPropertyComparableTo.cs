// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// ActiveState that returns true if the IProperty can be compared to the value
    /// </summary>
    public class IsPropertyComparableTo : MonoBehaviour, IActiveState
    {
        [SerializeField, Interface(typeof(IProperty))]
        private MonoBehaviour _property = null;
        public IProperty Property;

        [SerializeField]
        private CompareFunction _compareFunction = CompareFunction.Equal;
        [SerializeField]
        private string _toValue = "";

        private bool _active;

        public bool Active => _active;

        void Reset()
        {
            _property = GetComponent(typeof(IProperty)) as MonoBehaviour;
        }

        void Awake()
        {
            Property = _property as IProperty;
        }

        void Start()
        {
            Assert.IsNotNull(Property);
            Property.WhenChanged += UpdateActive;
            UpdateActive();
        }

        void OnDestroy()
        {
            Property.WhenChanged -= UpdateActive;
        }

        private void UpdateActive()
        {
            _active = Compare();
        }

        private bool Compare()
        {
            switch (_compareFunction)
            {
                case CompareFunction.Disabled:
                case CompareFunction.Never: return false;
                case CompareFunction.Always: return true;
            }

            if (Property is IComparable comparable)
            {
                int compare = comparable.CompareTo(_toValue);
                switch (_compareFunction)
                {
                    case CompareFunction.Less: return compare < 0;
                    case CompareFunction.Equal: return compare == 0;
                    case CompareFunction.LessEqual: return compare <= 0;
                    case CompareFunction.Greater: return compare > 0;
                    case CompareFunction.NotEqual: return compare != 0;
                    case CompareFunction.GreaterEqual: return compare >= 0;
                    default: throw new Exception();
                }
            }
            else
            {
                bool areEqual = Property.ToString() == _toValue;
                switch (_compareFunction)
                {
                    case CompareFunction.Equal: return areEqual;
                    case CompareFunction.NotEqual: return !areEqual;
                    default: throw new Exception();
                }
            }
        }
    }
}
