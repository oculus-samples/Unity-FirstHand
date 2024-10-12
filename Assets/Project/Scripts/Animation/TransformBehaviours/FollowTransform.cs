// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Profiling;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Moves this transform to the position of another transform every frame,
    /// with optional modifications of the other transforms pose.
    /// Used to make the Prompts follow thier target objects and face the user
    /// </summary>
    [ExecuteInEditMode]
    public class FollowTransform : MonoBehaviour
    {
        public Transform Source;
        public bool _assignMainCamera = false;
        public FollowPosition positionSettings;
        public FollowRotation rotationSettings;
        public When when = When.Update;
        public FollowTransform _updateAfter;
        private bool _started;

        [SerializeField]
        private bool _updateInEditMode;

        public float PositionWeight { get; set; } = 1;
        public float RotationWeight { get; set; } = 1;
        public bool UpdateInEditMode => _updateInEditMode;

        public event Action WhenTransformUpdated;

        private void Awake()
        {
            positionSettings.Init();
            rotationSettings.Init();
        }

        private void Start()
        {
            if (!Application.isPlaying) return;
            _started = true;
            UpdatePose(smoothing: false);
        }

        private void OnEnable()
        {
            if (!Application.isPlaying) return;
            if (_started)
            {
                UpdatePose(smoothing: false);
            }
            if (_updateAfter) _updateAfter.WhenTransformUpdated += UpdatePose;
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) return;
            if (_updateAfter) _updateAfter.WhenTransformUpdated -= UpdatePose;
        }

        private void Reset()
        {
            positionSettings.enabled = true;
            positionSettings.PositionMask = (Vector3Mask)~0;
            rotationSettings.UpDirection = OffsetSpace.Target;
        }

        private void Update()
        {
            if (!Application.isPlaying && !_updateInEditMode) return;
            if ((when & When.Update) != 0)
            {
                UpdatePose();
            }
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying) return;
            if ((when & When.LateUpdate) != 0)
            {
                UpdatePose();
            }
        }

        private void FixedUpdate()
        {
            if (!Application.isPlaying) return;
            if ((when & When.FixedUpdate) != 0)
            {
                UpdatePose();
            }
        }

        public void UpdatePose() => UpdatePose(true);

        public void UpdatePose(bool smoothing)
        {
            Profiler.BeginSample("FollowTransform.UpdatePose");
            if (_assignMainCamera && Application.isPlaying && !Source)
            {
                Profiler.BeginSample("FollowTransform.GetMainCamera");
                Camera main = Camera.main;
                if (main) { Source = main.transform; }
                Profiler.EndSample();
            }

            if (Source)
            {
                bool modified = positionSettings.UpdatePosition(transform, Source, smoothing, PositionWeight);
                modified |= rotationSettings.UpdateRotation(transform, Source, smoothing, RotationWeight);

                if (modified)
                {
                    Profiler.BeginSample("FollowTransform.WhenTransformUpdated");
                    WhenTransformUpdated?.Invoke();
                    Profiler.EndSample();
                }
            }
            Profiler.EndSample();
        }

        [ContextMenu("Apply Now")]
        void ApplyNow() => UpdatePose(false);

        [Flags]
        public enum When
        {
            Update = 1 << 0,
            LateUpdate = 1 << 1,
            FixedUpdate = 1 << 2
        }

        [Serializable]
        public struct FollowPosition
        {
            public bool enabled;
            public Vector3 PositionOffset;
            public OffsetSpace PositionOffsetSpace;
            public Vector3Mask PositionMask;
            [Range(0, 1f)]
            public float PositionSmoothing;
            [Tooltip("When greater than zero, sets a limit on how far from the source position this can be")]
            public float MaxSmoothingDisplacement;
            public Noise3 Noise;

            public void Init() => Noise.offset = UnityEngine.Random.value * 10;

            public bool UpdatePosition(Transform forTransform, Transform fromSource, bool smoothing, float weight = 1)
            {
                if (!enabled || weight == 0) { return false; }

                Profiler.BeginSample("FollowTransform.FollowPosition");
                Vector3 sourcePosition = fromSource.position + GetPositionOffset(forTransform, fromSource) + Noise.Evaluate(Time.time);
                Vector3 currentPosition = forTransform.position;

                Vector3 nextPosition = sourcePosition;
                if ((PositionMask & Vector3Mask.X) == 0) { nextPosition.x = currentPosition.x; }
                if ((PositionMask & Vector3Mask.Y) == 0) { nextPosition.y = currentPosition.y; }
                if ((PositionMask & Vector3Mask.Z) == 0) { nextPosition.z = currentPosition.z; }

                if (PositionSmoothing != 0 && smoothing)
                {
                    nextPosition = Vector3.Lerp(currentPosition, nextPosition, 1 - Mathf.Pow(PositionSmoothing, Time.deltaTime));

                    if (MaxSmoothingDisplacement > 0)
                    {
                        nextPosition = Vector3.MoveTowards(sourcePosition, nextPosition, MaxSmoothingDisplacement);
                    }
                }

                nextPosition = Vector3.Lerp(currentPosition, nextPosition, weight);

                forTransform.position = nextPosition;
                Profiler.EndSample();

                return true;
            }

            private Vector3 GetPositionOffset(Transform transform, Transform Source)
            {
                switch (PositionOffsetSpace)
                {
                    case OffsetSpace.World: return PositionOffset;
                    case OffsetSpace.Target: return Source.TransformVector(PositionOffset);
                    case OffsetSpace.This: return transform.TransformVector(PositionOffset);
                    default: throw new System.Exception($"Cant handle offset space {PositionOffsetSpace}");
                }
            }
        }

        [Serializable]
        public struct FollowRotation
        {
            public bool enabled;
            public Rotation RotationType;
            public OffsetSpace UpDirection;
            public bool PrioritizeUp;
            public Vector3 RotationOffset;
            [Range(0, 0.99f)]
            public float RotationSmoothing;
            public Noise3 Noise;
            public void Init() => Noise.offset = UnityEngine.Random.value * 10;

            public bool UpdateRotation(Transform forTransform, Transform fromSource, bool smoothing, float weight = 1)
            {
                if (!enabled || weight == 0) { return false; }

                Profiler.BeginSample("FollowTransform.FollowRotation");

                Vector3 up = GetUpDirection(forTransform, fromSource);
                Vector3 forward = RotationType == Rotation.LookAt ? fromSource.position - forTransform.position : fromSource.forward;
                if (PrioritizeUp) { forward = (forward - Vector3.Project(forward, up)).normalized; }

                if (forward == Vector3.zero || up == Vector3.zero) return false;

                Quaternion currentRotation = forTransform.rotation;
                // create rotation
                Quaternion newRotation = Quaternion.LookRotation(forward, up) * Quaternion.Euler(RotationOffset);
                // add noise
                newRotation = Quaternion.Euler(Noise.Evaluate(Time.time)) * newRotation;
                // add smoothing
                if (RotationSmoothing != 0 && smoothing)
                {
                    newRotation = Quaternion.Lerp(currentRotation, newRotation, 1 - Mathf.Pow(RotationSmoothing, Time.deltaTime));
                }
                // weight
                newRotation = Quaternion.Lerp(currentRotation, newRotation, weight);

                forTransform.rotation = newRotation;
                Profiler.EndSample();

                return true;
            }

            private Vector3 GetUpDirection(Transform forTransform, Transform fromSource)
            {
                switch (UpDirection)
                {
                    case OffsetSpace.World: return Vector3.up;
                    case OffsetSpace.Target: return fromSource.up;
                    case OffsetSpace.This: return forTransform.parent ? forTransform.parent.up : Vector3.up;
                    default: throw new System.Exception($"Cant handle offset space {UpDirection}");
                }
            }
        }

        public enum Rotation { Rotation, LookAt }
        public enum OffsetSpace { World, Target, This }

        [Serializable, Flags]
        public enum Vector3Mask
        {
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2
        }

        [Serializable]
        public struct Noise3
        {
            public Vector3 amplitude;
            public float frequency;
            [HideInInspector, NonSerialized]
            public float offset;

            public Vector3 Evaluate(float time)
            {
                Vector3 result = Vector3.zero;
                if (!Application.isPlaying) return result;
                if (amplitude.x != 0) { result.x += amplitude.x * (Mathf.PerlinNoise((time * frequency), offset + 1) - 0.5f); }
                if (amplitude.y != 0) { result.y += amplitude.y * (Mathf.PerlinNoise((time * frequency), offset + 2) - 0.5f); }
                if (amplitude.z != 0) { result.z += amplitude.z * (Mathf.PerlinNoise((time * frequency), offset + 3) - 0.5f); }
                return result;
            }
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(FollowPosition))]
        [CustomPropertyDrawer(typeof(FollowRotation))]
        public class PositionSettingsDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var enabledProp = property.FindPropertyRelative("enabled");
                property.isExpanded = enabledProp.boolValue;

                if (property.isExpanded)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
                else
                {
                    EditorGUI.PropertyField(position, enabledProp, label);
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (property.FindPropertyRelative("enabled").boolValue == false) return EditorGUIUtility.singleLineHeight;

                float height = EditorGUI.GetPropertyHeight(property);
                return property.isExpanded ? height + 20f : height;
            }
        }
#endif
    }
}
