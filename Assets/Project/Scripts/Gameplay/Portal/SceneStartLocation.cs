// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Locomotion;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Dictates the starting location of the player
    /// </summary>
    public class SceneStartLocation : MonoBehaviour
    {
        [SerializeField]
        private bool _setPlayerLocationOnStart = false;
        [SerializeField]
        private ReferenceActiveState _teleportHereOnRecenter = ReferenceActiveState.OptionalFalse();

        private bool _dirty;

        private void Start()
        {
            if (OVRManager.display != null) OVRManager.display.RecenteredPose += HandleRecenter;

            if (_setPlayerLocationOnStart)
            {
                SetPlayerLocation();
            }
        }

        void OnDestroy()
        {
            if (OVRManager.display != null) OVRManager.display.RecenteredPose -= HandleRecenter;
        }

        private void Update()
        {
            if (_dirty)
            {
                _dirty = false;
                SetPlayerLocation();
            }
        }

        void HandleRecenter()
        {
            if (_teleportHereOnRecenter)
            {
                _dirty = true;
            }
        }

        public void SetPlayerLocation() => LocomotePlayer(true, true);

        public void SetPlayerPosition() => LocomotePlayer(true, false);

        void LocomotePlayer(bool position = true, bool rotation = true)
        {
            PlayerLocomotor playerLocomotor = FindObjectOfType<PlayerLocomotor>();// TODO fix
            PlayerSounds.SkipNext = true;
            playerLocomotor.HandleLocomotionEvent(new LocomotionEvent(0, transform.GetPose(),
                position ? LocomotionEvent.TranslationType.Absolute : LocomotionEvent.TranslationType.None,
                rotation ? LocomotionEvent.RotationType.Absolute : LocomotionEvent.RotationType.None));
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one.SetY(0));
            Gizmos.DrawWireSphere(Vector3.zero, 0.4f); // a comfortable reach distance
            Gizmos.DrawWireSphere(Vector3.zero, 0.75f); // extreme reach
            Gizmos.DrawWireSphere(Vector3.zero, 1.70f / 2f); // stationary boundary (I think)

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(0.3f, 0, 1));
            Gizmos.DrawWireSphere(new Vector3(0.35f, 0, 0), 0.12f); // where my feet are
            Gizmos.DrawWireSphere(new Vector3(-0.35f, 0, 0), 0.12f);
        }
    }
}
