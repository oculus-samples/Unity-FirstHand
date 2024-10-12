// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class AutoSeatedMode : MonoBehaviour
    {
        [SerializeField]
        private float _minimumHeight = 1.1f;

        IEnumerator Start()
        {
            yield return null;
            yield return null; // can take 2 frames for the camera to be positioned
            yield return null; // wait a 3rd frame for SeatedMode to have started

            var rig = FindAnyObjectByType<OVRCameraRig>();
            var head = rig.centerEyeAnchor;
            var root = rig.trackingSpace;
            var height = root.InverseTransformPoint(head.position).y;

            if (height < _minimumHeight)
            {
                Debug.Log("Auto seated mode");
                SeatedMode.SetSeatedMode(true);
            }
        }
    }
}
