// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class BuildADroneAutoSequence : MonoBehaviour
    {
        [SerializeField]
        private List<PortalLocations> _portalLocations = new();
        [SerializeField]
        private StringPropertyBehaviour _stringProp;

        private void Update()
        {
            var portalLocations = _portalLocations.Find(x => x.Update());
            if (portalLocations != null && portalLocations.PortalLocationString == _stringProp.Value)
            {
                var nonCompletedObjective = _portalLocations.Find(x => !x.Active);
                if (nonCompletedObjective != null)
                {
                    _stringProp.SetValue(nonCompletedObjective.PortalLocationString);
                }
            }
        }
    }

    [System.Serializable]
    public class PortalLocations : IActiveState
    {
        public ReferenceActiveState CompletedActiveState;
        public string PortalLocationString;

        private bool _lastActive = false;

        public bool Active => _lastActive;

        public bool Update()
        {
            if (_lastActive != CompletedActiveState)
            {
                _lastActive = !_lastActive;
                return true;
            }

            return false;
        }
    }
}
