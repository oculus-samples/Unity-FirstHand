// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Records animation clips to be played on AnimatedGhostHands
    /// </summary>
    public class ObjectAnimationRecorder : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private string _outputAnimationClipsDirectory;

        [SerializeField] private GameObject _gameObject;
        [SerializeField, Optional] private Transform _relativeTransform;
        [SerializeField] private KeyCode _holdToRecordKey = KeyCode.Space;

        private GameObjectRecorder _recorder;
        private AnimationClip _animationClip;
        private GameObject _objectToRecord;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(_holdToRecordKey))
            {
                StartRecording();
            }

            if (UnityEngine.Input.GetKeyUp(_holdToRecordKey))
            {
                StopRecording();
            }
        }

        private void StartRecording()
        {
            if (_recorder)
            {
                return;
            }

            _animationClip = new AnimationClip();

            if (_relativeTransform)
            {
                // To record to a relative transform, a whole copy of the target object's transform hierarchy is created
                // and updated every frame. This is not very efficient but it is far simpler than fixing up the animation
                // curves with the relative transform once recording is finished.
                _objectToRecord = CloneTransformHierarchy(_gameObject);
                _objectToRecord.transform.SetParent(_relativeTransform);
            }
            else
            {
                _objectToRecord = _gameObject;
            }

            // creating a new recorder for each clip because recorder.ResetRecording() is not working as expected
            _recorder = new GameObjectRecorder(_objectToRecord);
            _recorder.BindComponentsOfType<Transform>(_objectToRecord, true);
        }

        private void StopRecording()
        {
            if (!_recorder)
            {
                return;
            }

            _recorder.SaveToClip(_animationClip);

            Destroy(_recorder);
            _recorder = null;

            // save the animation clip to file
            string clipPath = $"{_outputAnimationClipsDirectory}/{_gameObject.name}.anim";

            int assetIndex = 0;
            while (AssetDatabase.GetMainAssetTypeAtPath(clipPath) != null)
            {
                ++assetIndex;
                clipPath = $"{_outputAnimationClipsDirectory}/{_gameObject.name}_{assetIndex}.anim";
            }

            // an "Animation" component with a legacy clip is still the simplest way to play
            // a single animation on an object
            _animationClip.legacy = true;
            AssetDatabase.CreateAsset(_animationClip, clipPath);
            AssetDatabase.SaveAssets();

            if (_objectToRecord != _gameObject)
            {
                Destroy(_objectToRecord);
            }

            Debug.Log($"[Animation Recorder] Wrote animation to file: {clipPath}");
        }

        private void LateUpdate()
        {
            if (_recorder)
            {
                if (_objectToRecord != _gameObject)
                {
                    CopyTransformHierarchy(_gameObject.transform, _objectToRecord.transform);
                }

                _recorder.TakeSnapshot(Time.deltaTime);
            }
        }

        // creates a copy of a gameobject and its children, with all components removed except for transforms
        private GameObject CloneTransformHierarchy(GameObject gameObject)
        {
            GameObject copy = Instantiate(gameObject);
            Component[] components = copy.GetComponentsInChildren<Component>();
            foreach (Component component in components)
            {
                if (component.GetType() != typeof(Transform))
                {
                    Destroy(component);
                }
            }

            copy.name = gameObject.name + "_TransformCopy";
            return copy;
        }

        // assumes an identical transform hierarchy
        private void CopyTransformHierarchy(Transform from, Transform to)
        {
            to.position = from.position;
            to.rotation = from.rotation;
            to.localScale = from.localScale;

            Assert.AreEqual(from.childCount, to.childCount);
            for (int i = 0; i < from.childCount; ++i)
            {
                CopyTransformHierarchy(from.GetChild(i), to.GetChild(i));
            }
        }
#endif
    }
}
