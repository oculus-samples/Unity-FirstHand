// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class MaterialSortingLayer : MonoBehaviour
    {
        [SerializeField]
        private string _mySortingLayer;

        private void Start()
        {
            GetComponent<Renderer>().sortingLayerName = _mySortingLayer;
        }
    }
}
