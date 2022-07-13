/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Similar to MaterialPropertyBlockEditor but a single property without specifying implementation techinique
    /// Using an Object per property (rather than a list) allows animation via the Animation/Animator components    ///
    /// </summary>
    /* TODO shouldnt use MaterialPropertyBlock on URP since it breaks SRPBatcher,
     * Definition already supports materials, just need to add material management */
    public class MaterialProperty : MonoBehaviour
    {
        private static MaterialPropertyBlock _propertyBlock;

        [SerializeField]
        private Definition _definition;

        [SerializeField]
        private List<Renderer> _renderers;

        protected virtual void Awake()
        {
            _propertyBlock ??= new MaterialPropertyBlock();
        }

        protected virtual void LateUpdate()
        {
            for (int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].GetPropertyBlock(_propertyBlock);
                _definition.ApplyTo(_propertyBlock);
                _renderers[i].SetPropertyBlock(_propertyBlock);
            }
        }

        protected virtual void OnDisable()
        {
            _propertyBlock.Clear();
            for (int i = 0; i < _renderers.Count; i++)
            {
                _renderers[i].SetPropertyBlock(_propertyBlock);
            }
        }

        /// <summary>
        /// Holds a definition for a material property and exposes to
        /// ApplyTo methods for appling to materials or propertyblocks
        /// </summary>
        [Serializable]
        public struct Definition : ISerializationCallbackReceiver
        {
            [SerializeField]
            Type _type;

            [SerializeField]
            private string _property;

            [SerializeField, ColorUsage(true, true)]
            private Color _colorValue;

            [SerializeField]
            private float _floatValue;

            [SerializeField]
            private Vector4 _vectorValue;

            [SerializeField]
            private Texture _textureValue;

            [SerializeField]
            private float _multiplier;

            private int _propertyID;

            public void ApplyTo(MaterialPropertyBlock target)
            {
                switch (_type)
                {
                    case Type.Color:
                        target.SetColor(_propertyID, _colorValue * (1 + _multiplier));
                        break;
                    case Type.Float:
                        target.SetFloat(_propertyID, _floatValue * (1 + _multiplier));
                        break;
                    case Type.Vector:
                        target.SetVector(_propertyID, _vectorValue * (1 + _multiplier));
                        break;
                    case Type.Texture:
                        target.SetTexture(_propertyID, _textureValue);
                        break;
                    default:
                        throw new Exception($"Can't handle material property type {_type}");
                }
            }

            public void ApplyTo(Material target)
            {
                switch (_type)
                {
                    case Type.Color:
                        target.SetColor(_propertyID, _colorValue * (1 + _multiplier));
                        break;
                    case Type.Float:
                        target.SetFloat(_propertyID, _floatValue * (1 + _multiplier));
                        break;
                    case Type.Vector:
                        target.SetVector(_propertyID, _vectorValue * (1 + _multiplier));
                        break;
                    case Type.Texture:
                        target.SetTexture(_propertyID, _textureValue);
                        break;
                    default:
                        throw new Exception($"Can't handle material property type {_type}");
                }
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize() => _propertyID = Shader.PropertyToID(_property);
            void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        }

        public enum Type
        {
            Color,
            Float,
            Vector,
            Texture
        }
    }
}
