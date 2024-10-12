// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// A set of extensions for UnityEngine.GameObject
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Sets gameobjects in a list active
        /// </summary>
        public static void SetActive(this List<GameObject> list, bool active)
        {
            list.ForEach(x => x.SetActive(active));
        }

        public static Vector3 SetY(this Vector3 vector, float y)
        {
            vector.y = y;
            return vector;
        }

        public static Vector2 XZ(this Vector3 v) => new Vector2(v.x, v.z);

        public static int CompareMagnitude(this Vector3 v, float to) => Math.Sign(v.sqrMagnitude - (to * to));
        public static int CompareMagnitude(this Vector3 v, Vector3 to) => Math.Sign(v.sqrMagnitude - to.sqrMagnitude);

        public static bool IsMagnitudeLessThan(this Vector3 v, Vector3 to) => v.CompareMagnitude(to) < 0;
        public static bool IsMagnitudeLessThan(this Vector3 v, float to) => v.CompareMagnitude(to) < 0;
    }
}
