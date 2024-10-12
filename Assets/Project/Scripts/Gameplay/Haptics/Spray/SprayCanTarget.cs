// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class SprayCanTarget : MonoBehaviour, ISprayCan
    {
        [SerializeField]
        StringPropertyRef _stringProperty;

        public void Spray(string value)
        {
            _stringProperty.Value = value;
        }
    }
}
