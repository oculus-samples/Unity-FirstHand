// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    [System.Serializable]
    public struct ITransformerReference : ISerializationCallbackReceiver, ITransformer
    {
        [SerializeField, Interface(typeof(ITransformer))]
        private MonoBehaviour _transformer;
        public ITransformer Transformer;
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        void ISerializationCallbackReceiver.OnAfterDeserialize() => Transformer = _transformer as ITransformer;
        public void Initialize(IGrabbable grabbable) => Transformer.Initialize(grabbable);
        public void BeginTransform() => Transformer.BeginTransform();
        public void UpdateTransform() => Transformer.UpdateTransform();
        public void EndTransform() => Transformer.EndTransform();

        public override bool Equals(object obj)
        {
            return (obj is ITransformerReference reference &&
                   EqualityComparer<MonoBehaviour>.Default.Equals(_transformer, reference._transformer))
                   || obj is MonoBehaviour b && b == _transformer;
        }

        public override int GetHashCode()
        {
            return -1235736359 + EqualityComparer<MonoBehaviour>.Default.GetHashCode(_transformer);
        }
    }

    [System.Serializable]
    public struct IHandDataReference : ISerializationCallbackReceiver, IDataSource<HandDataAsset>
    {
        [SerializeField, Interface(typeof(IDataSource<HandDataAsset>))]
        private MonoBehaviour _hand;
        public IDataSource<HandDataAsset> Hand;

        public int CurrentDataVersion => Hand.CurrentDataVersion;

        public event Action InputDataAvailable
        {
            add => Hand.InputDataAvailable += value;
            remove => Hand.InputDataAvailable -= value;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        void ISerializationCallbackReceiver.OnAfterDeserialize() => Hand = _hand as IDataSource<HandDataAsset>;

        public override bool Equals(object obj)
        {
            return (obj is IHandDataReference reference &&
                   EqualityComparer<MonoBehaviour>.Default.Equals(_hand, reference._hand))
                   || obj is MonoBehaviour b && b == _hand;
        }

        public override int GetHashCode()
        {
            return -1235736359 + EqualityComparer<MonoBehaviour>.Default.GetHashCode(_hand);
        }

        public HandDataAsset GetData()
        {
            return Hand.GetData();
        }

        public void MarkInputDataRequiresUpdate()
        {
            Hand.MarkInputDataRequiresUpdate();
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ITransformerReference))]
    class ITransformerReferenceDrawer : InlineDrawer { }

    [UnityEditor.CustomPropertyDrawer(typeof(IHandDataReference))]
    class IHandReferenceDrawer : InlineDrawer { }

    class InlineDrawer : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight(UnityEditor.SerializedProperty _, GUIContent __) => UnityEditor.EditorGUIUtility.singleLineHeight;
        public override void OnGUI(Rect rect, UnityEditor.SerializedProperty prop, GUIContent label)
        {
            var enumerator = prop.GetEnumerator();
            enumerator.MoveNext();
            prop = enumerator.Current as UnityEditor.SerializedProperty;
            UnityEditor.EditorGUI.PropertyField(rect, prop, label);
        }
    }
#endif

}
