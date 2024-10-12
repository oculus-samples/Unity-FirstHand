// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Oculus.Interaction.ComprehensiveSample.Tween;

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

        public static bool IsTweening(object id)
        {
            return TryGetTween(id, out var _);
        }

        public static bool TryGetTween(object id, out Tween tween)
        {
            for (int i = 0; i < _tweens.Count; i++)
            {
                if (!_tweens[i].IsKilled && _tweens[i].ID == id)
                {
                    tween = _tweens[i];
                    return true;
                }
            }

            tween = null;
            return false;
        }

        public static Tween Tween(Vector3 start, Vector3 end, float duration, Action<Vector3> onUpdate)
        {
            return Tween01(duration, x => onUpdate(Vector3.LerpUnclamped(start, end, x)));
        }

        public static Tween Tween(Quaternion start, Quaternion end, float duration, Action<Quaternion> onUpdate)
        {
            return Tween01(duration, x =>
            {
                Quaternion rotation = Quaternion.LerpUnclamped(start.normalized, end.normalized, x);
                onUpdate(rotation);
            });
        }

        public static Tween TweenRotation(Transform transform, Quaternion end, float duration)
        {
            return Tween(transform.rotation, end, duration, x => transform.rotation = x).SetID(transform);
        }

        public static Tween TweenRotationLocal(Transform transform, Quaternion end, float duration)
        {
            return Tween(transform.localRotation, end, duration, x => transform.localRotation = x).SetID(transform);
        }

        /// <summary>
        /// Tweens the transforms euler angles to the specified value, setting the transform as the ID
        /// </summary>
        public static Tween TweenEulerAngles(Transform transform, Vector3 end, float duration)
        {
            return Tween(transform.eulerAngles, end, duration, x => transform.eulerAngles = x).SetID(transform);
        }

        /// <summary>
        /// Tweens the transforms local euler angles to the specified value, setting the transform as the ID
        /// </summary>
        public static Tween TweenEulerAnglesLocal(Transform transform, Vector3 end, float duration)
        {
            return Tween(transform.localEulerAngles, end, duration, x => transform.localEulerAngles = x).SetID(transform);
        }

        public static Tween Tween(Color start, Color end, float duration, Action<Color> onUpdate)
        {
            return Tween01(duration, x => onUpdate(Color.LerpUnclamped(start, end, x)));
        }

        /// <summary>
        /// Tweens the transform to the position of the other transform, setting the transform as the ID
        /// </summary>
        public static Tween TweenTransform(Transform transform, Transform to, float duration)
        {
            var start = transform.GetPose();
            return Tween01(duration, x =>
            {
                var end = to.GetPose();
                var pos = Vector3.LerpUnclamped(start.position, end.position, x);
                var rot = Quaternion.LerpUnclamped(start.rotation, end.rotation, x);
                transform.SetPositionAndRotation(pos, rot);
            }).SetID(transform);
        }

        public static Tween TweenTransform(Transform transform, Pose end, float duration)
        {
            var start = transform.GetPose();
            return Tween01(duration, x =>
            {
                var pos = Vector3.LerpUnclamped(start.position, end.position, x);
                var rot = Quaternion.LerpUnclamped(start.rotation, end.rotation, x);
                transform.SetPositionAndRotation(pos, rot);
            }).SetID(transform);
        }

        public static Tween TweenTransformLocal(Transform transform, Pose end, float duration)
        {
            var start = transform.GetPose(Space.Self);
            return Tween01(duration, x =>
            {
                var pos = Vector3.LerpUnclamped(start.position, end.position, x);
                var rot = Quaternion.LerpUnclamped(start.rotation, end.rotation, x);
                transform.SetPose(new Pose(pos, rot), Space.Self);
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

        public static Tween TweenPosition(Transform transform, Vector3 end, float duration)
        {
            return Tween(transform.position, end, duration, x => transform.position = x).SetID(transform);
        }

        public static Tween TweenPositionLocal(Transform transform, Vector3 end, float duration)
        {
            return Tween(transform.localPosition, end, duration, x => transform.localPosition = x).SetID(transform);
        }

        class TweenUpdater : MonoBehaviour
        {
            static GameObject _instance;

            public static void Exist()
            {
                if (_instance || !Application.isPlaying) { return; }

                _instance = new GameObject(nameof(TweenUpdater), typeof(TweenUpdater));
                DontDestroyOnLoad(_instance);
            }

            private void Update()
            {
                UpdateTweens(_tweens, UpdateTime.Update);
            }

            private void LateUpdate()
            {
                UpdateTweens(_tweens, UpdateTime.LateUpdate);
            }

            private static void UpdateTweens(List<Tween> tweens, UpdateTime time)
            {
                for (int i = 0; i < tweens.Count; i++)
                {
                    if (tweens[i].updateTime == time)
                    {
                        var deltaTime = tweens[i].ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                        bool tweenFinished = !tweens[i].Advance(deltaTime);
                        if (tweenFinished)
                        {
                            tweens.RemoveAt(i--);
                        }
                    }
                }
            }
        }
    }

    public class Tween
    {
        private float _start, _end, _duration, _current;
        private int _loopCount = 1;
        Action<float> _onUpdate;

        bool _isKilled = false;
        public bool IsKilled => _isKilled;

        public object ID;
        public Ease ease;
        public UpdateTime updateTime;
        public bool ignoreTimeScale;
        public Player requireFocus = Player.None;

        event Action WhenComplete = delegate { };
        event Action WhenKilled = delegate { };

        public Tween(float start, float end, float duration, Action<float> onUpdate)
        {
            _start = start;
            _end = end;
            _duration = Mathf.Max(duration, float.Epsilon);
            _onUpdate = onUpdate;
        }

        /// <summary>
        /// Advances the tween and returns true if the tween can continue advancing
        /// </summary>
        public bool Advance(float deltaTime)
        {
            // already killed externally
            if (_isKilled) { return false; }

            // object got destroyed
            if (ID is UnityEngine.Object obj && obj == null)
            {
                Kill();
                return false;
            }

            // internally paused (like if the tween requires the app to be focused)
            // return true because we want the tween to continue, its just paused
            if (!CanAdvance()) return true;

            _current += deltaTime;
            bool stillTweening = _loopCount <= 0 || _current < _duration * _loopCount;

            if (_current >= 0) //_current will start negative on delayed tweens
            {
                var value = _end;
                if (_duration > 0 && stillTweening)
                {
                    float normalized = (_current / _duration) % 1;
                    value = Mathf.LerpUnclamped(_start, _end, EaseValue(normalized, ease));
                }
                _onUpdate(value);
            }

            if (!stillTweening)
            {
                _isKilled = true;
                WhenComplete();
            }
            return stillTweening;
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

        public Tween SetLoops(int loops)
        {
            _loopCount = loops;
            return this;
        }

        public Tween IgnoreTimeScale(bool value = true)
        {
            ignoreTimeScale = value;
            return this;
        }

        public Tween RequireFocus(Player value = Player.Any)
        {
            requireFocus = value;
            return this;
        }

        public bool CanAdvance()
        {
            Player stage = Application.isEditor ? Player.Editor : Player.Build;
            if ((stage & requireFocus) != 0) return Application.isFocused;

            return true;
        }

        public bool Skip() => Skip(true);

        public bool Skip(bool skip)
        {
            return skip && Advance(float.MaxValue);
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
            if (t <= 0) { return 0; }
            if (t >= 1) { return 1; }

            switch (ease)
            {
                case Ease.Linear: return t;
                case Ease.QuadIn: return Quad(t);
                case Ease.QuadOut: return 1 - Quad(1 - t);
                case Ease.QuadInOut: return t < 0.5f ? Quad(t * 2) / 2 : 1 - Quad((1 - t) * 2) / 2;
                case Ease.CubicIn: return Cube(t);
                case Ease.CubicOut: return 1 - Cube(1 - t);
                case Ease.CubicInOut: return t < 0.5f ? Cube(t * 2) / 2 : 1 - Cube((1 - t) * 2) / 2;
                case Ease.QuartIn: return Quart(t);
                case Ease.QuartOut: return 1 - Quart(1 - t);
                case Ease.QuartInOut: return t < 0.5f ? Quart(t * 2) / 2 : 1 - Quart((1 - t) * 2) / 2;
                case Ease.ElasticIn: return 1 - Elastic(1 - t);
                case Ease.ElasticOut: return Elastic(t);
                case Ease.ElasticInOut: return t < 0.5f ? (1 - Elastic(1 - t * 2)) / 2 : (Elastic(2 * t - 1) + 1) / 2;
                default: throw new Exception($"Cant ease {ease}");
            }

            static float Quad(float t) => t * t;
            static float Cube(float t) => t * t * t;
            static float Quart(float t) => t * t * t * t;
            static float Elastic(float t) => Mathf.Pow(2, -10 * t) * Mathf.Sin((t - 0.3f / 4) * (2 * Mathf.PI) / 0.3f) + 1;
        }

        public Tween OnUpdate(Action<float> p)
        {
            _onUpdate += p;
            return this;
        }

        public enum Ease
        {
            Linear,
            QuadIn,
            QuadOut,
            QuadInOut,
            CubicIn,
            CubicOut,
            CubicInOut,
            QuartIn,
            QuartOut,
            QuartInOut,
            ElasticIn,
            ElasticOut,
            ElasticInOut,
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

        public enum Player
        {
            None = 0,
            Editor = 1,
            Build = 2,
            Any = Editor | Build
        }
    }

    public static class EaseExtensions
    {
        public static bool IsClamped01(this Ease ease)
        {
            switch (ease)
            {
                case Ease.ElasticIn:
                case Ease.ElasticOut:
                case Ease.ElasticInOut:
                    return false;
            }
            return true;
        }
    }
}
