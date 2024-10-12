// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// IActiveState returns true if the GloveBuildBehaviour is selecting the specified part
    /// Used to hook other behaviours into the state of GloveBuildBehaviour
    /// </summary>
    public class IsGloveBehaviourActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        GloveBuildBehaviour _gloveBehaviour;
        [SerializeField]
        GlovePart _part;
        [SerializeField]
        ActiveStateExpectation _selectingSchematic = ActiveStateExpectation.True;
        [SerializeField]
        ActiveStateExpectation _hasPrinted = ActiveStateExpectation.Any;

        public bool Active
        {
            get
            {
                if (_hasPrinted != ActiveStateExpectation.Any)
                {
                    return _hasPrinted.Matches(_gloveBehaviour.IsPartPrinted(_part));
                }

                if (_selectingSchematic != ActiveStateExpectation.Any)
                {
                    bool isSelecting = _gloveBehaviour.IsSelectingSchematic && _gloveBehaviour.SelectedSchematic == _part;
                    return _selectingSchematic.Matches(isSelecting);
                }

                return false;
            }
        }
    }
}
