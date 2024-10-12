// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Sets string property value when active state return true
    /// </summary>
    public class ConditionalSetProperty : ActiveStateObserver
    {
        [SerializeField]
        private StringPropertyRef _stringProperty;

        [SerializeField]
        private string _value;

        protected override void HandleActiveStateChanged()
        {
            if (Active)
            {
                _stringProperty.Value = _value;
            }
        }
    }
}
