// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true when the Level is in the right state
    /// </summary>
    public class MasterLoaderActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        string _levelName = "*";

        [SerializeField]
        private MasterLoader.Level.State _state;
        private MasterLoader.Level _level;

        void Start()
        {
#if UNITY_EDITOR
            if (!MasterLoader.ExistsInEditor) return;
#endif
            if (_levelName != "*")
            {
                _level = MasterLoader.GetLevel(_levelName);
                if (_level == null) throw new System.Exception($"{_levelName} not found");
            }
        }

        public bool Active
        {
            get
            {
#if UNITY_EDITOR
                if (!MasterLoader.ExistsInEditor) return _state == MasterLoader.Level.State.Active;
#endif
                return (_level != null && (_level.state & _state) != 0) || MasterLoader.FindLevel(x => (x.state & _state) != 0) != null;
            }
        }
    }
}
