// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Inserts an ActiveState into a ConfigurableActiveState
    /// useful to prevent a ConfigurableActiveState from spreading its fingers all over a scene
    /// </summary>
    public class ConfigurableActiveStateInsert : MonoBehaviour
    {
        [SerializeField]
        ReferenceActiveState _activeState;
        [SerializeField, FormerlySerializedAs("_configurableActiveState")]
        ConfigurableActiveState _insertInto;

        private void Reset() => _activeState.InjectActiveState(GetComponent<IActiveState>());
        private void OnEnable() => _insertInto.AddRuntimeCondition(_activeState);
        private void OnDisable() => _insertInto?.RemoveRuntimeCondition(_activeState);
    }
}
