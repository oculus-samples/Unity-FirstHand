// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Reference to a StringPropertyBehaviour
    /// </summary>
    public class StringPropertyBehaviourRef : PropertyBehaviourRef, IProperty<string>
    {
        [SerializeField]
        Advanced _advanced;

        private string _cachedValue;
        private Coroutine _delayRoutine;

        public string Value
        {
            get => _cachedValue;// Property.ToString();
            set
            {
                SetPropertyWithString(Property, value);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _cachedValue = Property.ToString();
        }

        public static void SetPropertyWithString(IProperty property, string value)
        {
            switch (property)
            {
                case IProperty<float> floatProp:
                    floatProp.Value = float.Parse(value);
                    break;
                case IProperty<int> intProp:
                    intProp.Value = int.Parse(value);
                    break;
                case IProperty<string> stringProp:
                    stringProp.Value = value;
                    break;
                default:
                    throw new Exception($"Tried to set an unknown type of property ({property.GetType()})");
            }
        }

        protected override void HandlePropertyChanged()
        {
            if (_advanced.delay > 0)
            {
                ChangeValueWithDelay();
            }
            else
            {
                SetCachedvalue(Property.ToString());
            }
        }

        void ChangeValueWithDelay()
        {
            // the property may change again while a delay is in progress from an earlier change
            // we will only return the newest value
            // if there is a coroutine in progress then we dont need to start a new one
            if (_delayRoutine != null) return;

            _delayRoutine = StartCoroutine(DelayChange());
            IEnumerator DelayChange()
            {
                if (_advanced.useTempValueDuringDelay)
                {
                    SetCachedvalue(_advanced.tempValue);
                }

                yield return WaitForSecondsNonAlloc.WaitForSeconds(_advanced.delay);
                _delayRoutine = null;

                SetCachedvalue(Property.ToString());
            }
        }

        private void SetCachedvalue(string newValue)
        {
            if (_cachedValue != newValue)
            {
                _cachedValue = newValue;
                base.HandlePropertyChanged();
            }
        }

        [Serializable]
        struct Advanced
        {
            /// <summary>
            /// When the referenced property changes a delay can be applied to this reference, allowing things like timed transitions
            /// </summary>
            public float delay;
            /// <summary>
            /// If a change is delayed a temporary value can be assigned during the delay to help with transitions<br/>
            /// e.g. if a property changes from 'main_menu' to 'settings' with a delay of 1 second and a tempValue of 'transition',<br/>
            /// to observers of the property ref the change will appear as 'main_menu' > 'transition' > 'settings'
            /// </summary>
            public bool useTempValueDuringDelay;
            /// <summary>
            /// Temp value to be returned by this ref during a delay if <c>useTempValueDuringDelay</c> is true
            /// </summary>
            public string tempValue;
        }
    }
}
