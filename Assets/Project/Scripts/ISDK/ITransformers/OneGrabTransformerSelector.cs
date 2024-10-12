// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class OneGrabTransformerSelector : MonoBehaviour, ITransformer
    {
        [SerializeField]
        private List<TransformerOption> _transformers;
        [SerializeField]
        private ITransformerReference _fallbackTransformer;

        private ITransformer _currentTransformer;

        public void Initialize(IGrabbable grabbable)
        {
            _transformers.ForEach(x => x.Transformer.Initialize(grabbable));
            _fallbackTransformer.Initialize(grabbable);
        }

        public void BeginTransform()
        {
            _currentTransformer = GetBestTransformer();
            _currentTransformer.BeginTransform();
        }

        public void UpdateTransform()
        {
            var newBest = GetBestTransformer();
            if (!newBest.Equals(_currentTransformer))
            {
                print("CHANGING TRANSFORM");
                _currentTransformer.EndTransform();
                _currentTransformer = newBest;
                _currentTransformer.BeginTransform();
            }
            _currentTransformer.UpdateTransform();
        }

        public void EndTransform()
        {
            _currentTransformer.EndTransform();
            _currentTransformer = null;
        }

        /// <summary>
        /// Returns the first transformer that is 'Active', if none are active returns the _fallbackTransformer
        /// </summary>
        private ITransformer GetBestTransformer()
        {
            var activeIndex = _transformers.FindIndex(x => x.Active);
            return activeIndex >= 0 ? _transformers[activeIndex].Transformer : _fallbackTransformer;
        }

        [Serializable]
        struct TransformerOption
        {
            [SerializeField]
            public ITransformerReference Transformer;
            [SerializeField]
            public ReferenceActiveState Active;
        }
    }
}
