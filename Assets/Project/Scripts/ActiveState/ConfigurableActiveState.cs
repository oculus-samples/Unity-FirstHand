// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// A composite IActiveState with it's own value
    /// Can be used to combine serveral active states into one
    /// </summary>
    public class ConfigurableActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private bool _active = true;
        [SerializeField]
        private float _minActiveTime = 0;

        [Header("Compound Settings")]
        [SerializeField]
        private Mode _compoundMode = Mode.JustThis;
        [SerializeField]
        private List<ReferenceActiveState> _compoundStates = new List<ReferenceActiveState>();

        private List<IActiveState> _runtimeConditions = new List<IActiveState>();

        bool _lastUpdateActive;
        float _activeStartTime;

        public bool Active
        {
            get
            {
                if (_minActiveTime <= 0)
                {
                    return GetActive();
                }
                else
                {
                    return _lastUpdateActive && (Time.time - _activeStartTime) >= _minActiveTime;
                }
            }
        }

        private bool GetActive()
        {
            if (_compoundStates.Count == 0 && _runtimeConditions.Count == 0) return _active;

            try
            {
                switch (_compoundMode)
                {
                    case Mode.JustThis: return _active;
                    case Mode.AllCompounds: return GetCompoundStateAll();
                    case Mode.AnyCompounds: return GetCompoundStateAny();
                    case Mode.ThisAndAllCompounds: return _active && GetCompoundStateAll();
                    case Mode.ThisAndAnyCompounds: return _active && GetCompoundStateAny();
                    default: throw new Exception();
                }
            }
            catch (UnityException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                throw e;
            }
        }

        public bool ActiveSelf { get => _active; set => _active = value; }
        public Mode CompoundMode { get => _compoundMode; set => _compoundMode = value; }
        public List<ReferenceActiveState> CompoundStates { get => _compoundStates; }

        public void AddRuntimeCondition(IActiveState activeState) => _runtimeConditions.Add(activeState);
        public bool RemoveRuntimeCondition(IActiveState activeState) => _runtimeConditions.Remove(activeState);

        private bool GetCompoundStateAll()
        {
            return _compoundStates.TrueForAll(x => x.Active) && _runtimeConditions.TrueForAll(x => x.Active);
        }

        private bool GetCompoundStateAny()
        {
            return (_compoundStates.Count > 0 && !_compoundStates.TrueForAll(x => !x.Active)) ||
                (_runtimeConditions.Count > 0 && !_runtimeConditions.TrueForAll(x => !x.Active));
        }

        private void OnValidate()
        {
            for (int i = 0; i < _compoundStates.Count; i++)
            {
                if (Equals(_compoundStates[i].ActiveState))
                {
                    _compoundStates[i].InjectActiveState(null);
                    Debug.LogError("ConfigurableActiveState was assigned to itself!", this);
                }
            }
        }

        public virtual void Update()
        {
            if (_minActiveTime > 0)
            {
                var active = GetActive();
                if (_lastUpdateActive != active)
                {
                    _lastUpdateActive = active;
                    _activeStartTime = Time.time;
                }
            }
        }

        public override string ToString()
        {
            return $"Active: {Active}\n" +
                $"Self: {_active}\n" +
                $"{JoinCompounds()}";
        }

        string JoinCompounds()
        {
            string result = "Compound:\n";
            for (int i = 0; i < _compoundStates.Count; i++)
            {
                result += $"   {StringifyReference(_compoundStates[i])}\n";
            }

            result += "Runtime:\n";
            for (int i = 0; i < _runtimeConditions.Count; i++)
            {
                var state = _runtimeConditions[i];
                if (state is ReferenceActiveState reference)
                {
                    result += $"   {StringifyReference(reference)}\n";
                }
                else
                {
                    result += $"   {state.GetType().Name} {state.Active}\n";
                }
            }

            string StringifyReference(ReferenceActiveState r)
            {
                string nameType = r.ActiveState.GetType().Name;
                if (r.ActiveState is Component c)
                {
                    nameType = $"{c.name} {nameType}";
                }
                return $"{nameType} {(r.Active != r.ActiveState.Active ? "(inverted)" : "")} {r.Active}";
            }

            return result;
        }

        public enum Mode
        {
            JustThis,
            AllCompounds,
            AnyCompounds,
            ThisAndAllCompounds,
            ThisAndAnyCompounds
        }

#if UNITY_EDITOR
        /// <summary>
        /// Makes the compoind section look a bit disabled when it wont be used
        /// </summary>
        [UnityEditor.CustomEditor(typeof(ConfigurableActiveState)), UnityEditor.CanEditMultipleObjects]
        public class CongigurableActiveStateEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                var activeSelf = serializedObject.FindProperty("_active").boolValue;
                var mode = serializedObject.FindProperty("_compoundMode").enumValueIndex;


                UnityEditor.EditorGUI.BeginChangeCheck();
                DrawPropertiesExcluding(serializedObject, "_compoundStates");

                var c = GUI.color;
                bool ignoreCompounds = mode == 0 || (mode >= 3 && !activeSelf);
                if (ignoreCompounds)
                {
                    GUI.color = new Color(c.r, c.g, c.b, c.a * 0.5f);
                }

                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("_compoundStates"));

                var rect = GUILayoutUtility.GetLastRect();
                GUI.color = new Color(c.r, c.g, c.b, c.a * 0.1f);
                var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = (int)rect.height };
                var modeString = mode > 0 && mode % 2 != 1 ? "ANY" : "";
                UnityEditor.EditorGUI.LabelField(rect, modeString, style);

                GUI.color = c;

                if (UnityEditor.EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
