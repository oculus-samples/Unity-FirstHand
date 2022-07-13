/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Use of the material below is subject to the terms of the MIT License
 * https://github.com/oculus-samples/Unity-FirstHand/tree/main/Assets/Project/LICENSE.txt
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Sets up and runs a linear interpolated value with a callback.
    /// Used to animated various things over time
    /// </summary>
    public static class TweenRunner
    {
        private static List<Tween> _tweens = new List<Tween>();

        public static Tween Tween(double start, double end, double duration, Action<double> onUpdate)
        {
            return Tween((float)start, (float)end, (float)duration, x => onUpdate(x));
        }

        public static Tween Tween(float start, float end, float duration, Action<float> onUpdate)
        {
            TweenUpdater.Exist();

            Tween item = new Tween(start, end, duration, onUpdate);
            _tweens.Add(item);
            return item;
        }

        public static Tween Tween(Vector3 start, Vector3 end, float duration, Action<Vector3> onUpdate)
        {
            return Tween01(duration, x => onUpdate(Vector3.Lerp(start, end, x)));
        }

        public static Tween Tween(Quaternion start, Quaternion end, float duration, Action<Quaternion> onUpdate)
        {
            return Tween01(duration, x => onUpdate(Quaternion.Lerp(start, end, x)));
        }

        internal static Tween TweenTransform(Transform transform, Transform to, float duration)
        {
            var start = transform.GetPose();
            return Tween01(duration, x =>
            {
                var end = to.GetPose();
                var pos = Vector3.Lerp(start.position, end.position, x);
                var rot = Quaternion.Lerp(start.rotation, end.rotation, x);
                transform.SetPositionAndRotation(pos, rot);
            }).SetID(transform);
        }

        internal static Tween NextFrame(Action action)
        {
            return DelayedCall(0, action);
        }

        public static Tween Tween01(float duration, Action<float> onUpdate) => Tween(0, 1, duration, onUpdate);

        public static int Kill(object id)
        {
            int count = 0;
            for (int i = 0; i < _tweens.Count; i++)
            {
                if (!_tweens[i].IsKilled && _tweens[i].ID == id)
                {
                    _tweens[i].Kill();
                    count++;
                }
            }
            return count;
        }

        public static Tween DelayedCall(float delay, Action action)
        {
            return Tween01(delay, x => { }).OnComplete(action);
        }

        class TweenUpdater : MonoBehaviour
        {
            static GameObject _instance;

            public static void Exist()
            {
                if (_instance) { return; }

                _instance = new GameObject(nameof(TweenUpdater), typeof(TweenUpdater));
                DontDestroyOnLoad(_instance);
            }

            private void Update()
            {
                UpdateTweens(_tweens, ComprehensiveSample.Tween.UpdateTime.Update);
            }

            private void LateUpdate()
            {
                UpdateTweens(_tweens, ComprehensiveSample.Tween.UpdateTime.LateUpdate);
            }

            private static void UpdateTweens(List<Tween> tweens, Tween.UpdateTime time)
            {
                for (int i = 0; i < tweens.Count; i++)
                {
                    if (tweens[i].updateTime == time && !tweens[i].Advance(Time.deltaTime))
                    {
                        tweens.RemoveAt(i--);
                    }
                }
            }
        }
    }

    public class Tween
    {
        private float _start, _end, _duration, _current;
        Action<float> _onUpdate;

        bool _isKilled = false;
        public bool IsKilled => _isKilled;

        public object ID;
        public Ease ease;
        public UpdateTime updateTime;

        event Action WhenComplete = delegate { };
        event Action WhenKilled = delegate { };

        public Tween(float start, float end, float duration, Action<float> onUpdate)
        {
            _start = start;
            _end = end;
            _duration = Mathf.Max(duration, float.Epsilon);
            _onUpdate = onUpdate;
        }

        public bool Advance(float deltaTime)
        {
            if (_isKilled) { return false; }

            _current += deltaTime;

            if (_current >= 0 && _duration > 0)
            {
                var value = Mathf.Lerp(_start, _end, EaseValue(_current / _duration, ease));
                _onUpdate(value);
            }

            bool moveNext = _current < _duration;
            if (!moveNext)
            {
                _isKilled = true;
                WhenComplete();
            }
            return moveNext;
        }

        public Tween SetID(object id, IDMode idMode = IDMode.KillPrevious)
        {
            if (idMode == IDMode.KillPrevious) { TweenRunner.Kill(id); }
            ID = id;
            return this;
        }

        public Tween SetEase(Ease ease)
        {
            this.ease = ease;
            return this;
        }

        public Tween OnComplete(Action action)
        {
            WhenComplete += action;
            return this;
        }

        public Tween OnKill(Action action)
        {
            WhenKilled += action;
            return this;
        }

        public void Kill()
        {
            if (!_isKilled)
            {
                _isKilled = true;
                WhenKilled();
            }
        }

        public Tween Delay(float duration)
        {
            _current -= duration;
            return this;
        }

        public Tween SetUpdate(UpdateTime time)
        {
            updateTime = time;
            return this;
        }

        public async Task<bool> ToTask()
        {
            if (_isKilled) { return true; }

            TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
            WhenComplete += () => task.TrySetResult(true);
            WhenKilled += () => task.TrySetResult(false);

            return await task.Task;
        }

        static float EaseValue(float t, Ease ease)
        {
            switch (ease)
            {
                case Ease.Linear: return t;
                case Ease.QuadIn: return Quad(t);
                case Ease.QuadOut: return 1 - Quad(1);
                case Ease.QuadInOut: return t < 0.5f ? Quad(t * 2) / 2 : 1 - Quad((1 - t) * 2) / 2;
                default: throw new Exception($"Cant ease {ease}");
            }

            static float Quad(float t) => t * t;
        }

        public enum Ease
        {
            Linear,
            QuadIn,
            QuadOut,
            QuadInOut
        }

        public enum IDMode
        {
            KillPrevious,
            KeepPrevious
        }

        public enum UpdateTime
        {
            Update,
            LateUpdate
        }
    }
}
