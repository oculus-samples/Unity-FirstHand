// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Aiming for feature parity with CSS box styling
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class Rectangle : MaskableGraphic
    {
        readonly static Vector3 _rightUp = new Vector2(1, 1);
        readonly static Vector3 _rightDown = new Vector2(1, -1);
        readonly static Vector3 _leftDown = new Vector2(-1, -1);
        readonly static Vector3 _leftUp = new Vector2(-1, 1);
        readonly static Vector3[] _insetDirections = new Vector3[] { _rightUp, _rightDown, _leftDown, _leftUp };

        [SerializeField] private Sprite _sprite;

        [SerializeField]
        int _divisions = 12;
        [SerializeField]
        private float _radius = 12;
        [SerializeField]
        private float _stroke = 0;
        [SerializeField]
        private float _antiAlias = 0;
        [SerializeField, Range(1, 12)]
        private int _shadowQuality = 1;

        [Header("Linear Gradient")]
        [SerializeField] float _angle = 180;
        [SerializeField] Color32 _startColor = Color.white;
        [SerializeField] Color32 _endColor = Color.white;
        [SerializeField] List<Shadow> _dropShadows = new List<Shadow>();
        [SerializeField] List<Shadow> _innerShadows = new List<Shadow>();

        [SerializeField] CornerFloats _radiusOffsets = Vector4.zero;

        [SerializeField, Tooltip("Use a more optimized Raycast " +
            "function that assumes no canvas sorting overrides")]
        private bool _optimizedRaycast = true;

        [SerializeField]
        private RectangleMask _strokeMask = (RectangleMask)~0;

        public Sprite sprite
        {
            get { return _sprite; }
            set
            {
                if (_sprite != value)
                {
                    _sprite = value;
                    SetAllDirty();
                }
            }
        }

        public override Texture mainTexture
        {
            get
            {
                return _sprite != null ? _sprite.texture : base.mainTexture;
            }
        }

        public float DropShadowVisibility
        {
            get => _dropShadowVisibility;
            set
            {
                if (_dropShadowVisibility != value)
                {
                    _dropShadowVisibility = value;
                    SetVerticesDirty();
                }
            }
        }

        private float _radiusOverride = -1;
        public float RadiusOverride
        {
            get => _radiusOverride;
            set
            {
                if (_radiusOverride != value)
                {
                    _radiusOverride = value;
                    SetVerticesDirty();
                }
            }
        }

        private float UsedRadius
        {
            get
            {
                var rect = rectTransform.rect;
                float usedRadius = _radiusOverride >= 0 ? _radiusOverride : _radius;
                var radius = Mathf.Min(Mathf.Min(rect.size.x / 2, rect.size.y / 2), usedRadius);
                return radius;
            }
        }

        private List<UIVertex> _verts = new List<UIVertex>();
        private List<int> _indicies = new List<int>();
        private float _dropShadowVisibility = 1;
        private GradientHelper _gradientHelper;

#if UNITY_EDITOR
        protected override void Reset()
        {
            var parent = transform.parent?.GetComponent<Rectangle>();
            if (parent)
            {
                _radius = parent._radius;
                _divisions = parent._divisions;
            }
            raycastTarget = false;
        }
#endif

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            _gradientHelper.Update(rectTransform.rect, _angle, _startColor, _endColor);

            vh.Clear();

            for (int i = 0; i < _dropShadows.Count; i++)
            {
                DrawDropShadow(_dropShadows[i]);
            }

            if (_stroke == 0)
            {
                DrawRectangle();
            }

            for (int i = 0; i < _innerShadows.Count; i++)
            {
                DrawInnerShadow(_innerShadows[i]);
            }

            if (_stroke != 0)
            {
                DrawStroke();
            }

            vh.AddUIVertexStream(_verts, _indicies);
            _verts.Clear();
            _indicies.Clear();
        }

        private void DrawStroke()
        {
            var stroke4 = new CornerFloats(_stroke);// * Vector4.one;
            var offsetRadii = UsedRadius + _radiusOffsets;
            CornerFloats innerOffsetRadius = Vector4.Max(offsetRadii, stroke4);

            if (_antiAlias > 0)
            {
                var aa = _antiAlias * Mathf.Sign(_stroke);
                var edgeColor = new Color(color.r, color.g, color.b, 0);
                var outerAA = AddRectangle(offsetRadii, offsetRadii + aa * 0.5f, edgeColor);
                var outer = AddRectangle(offsetRadii, offsetRadii - aa * 0.5f, color);
                var inner = AddRectangle(innerOffsetRadius, offsetRadii - stroke4 + aa * 0.5f, color);
                var innerAA = AddRectangle(innerOffsetRadius, offsetRadii - _stroke - aa * 0.5f, edgeColor);
                Bridge(outerAA, outer, _strokeMask);
                Bridge(outer, inner, _strokeMask);
                Bridge(inner, innerAA, _strokeMask);
            }
            else
            {
                var outer = AddRectangle(offsetRadii, offsetRadii, color);
                var inner = AddRectangle(innerOffsetRadius, offsetRadii - stroke4, color);
                Bridge(outer, inner, _strokeMask);
            }
        }

        private void Bridge(RectangleSegment a, RectangleSegment b, RectangleMask mask = (RectangleMask)~0)
        {
            var topLeft = Bridge(a.topLeft, b.topLeft, (mask & RectangleMask.TopLeft) != 0);
            var topRight = Bridge(a.topRight, b.topRight, (mask & RectangleMask.TopRight) != 0);
            var bottomRight = Bridge(a.bottomRight, b.bottomRight, (mask & RectangleMask.BottomRight) != 0);
            var bottomLeft = Bridge(a.bottomLeft, b.bottomLeft, (mask & RectangleMask.BottomLeft) != 0);

            if ((mask & RectangleMask.Top) != 0)
            {
                AddQuad(topLeft.maxA, topRight.minA, topRight.minB, topLeft.maxB);
            }
            if ((mask & RectangleMask.Right) != 0)
            {
                AddQuad(topRight.maxA, bottomRight.minA, bottomRight.minB, topRight.maxB);
            }
            if ((mask & RectangleMask.Bottom) != 0)
            {
                AddQuad(bottomRight.maxA, bottomLeft.minA, bottomLeft.minB, bottomRight.maxB);
            }
            if ((mask & RectangleMask.Left) != 0)
            {
                AddQuad(bottomLeft.maxA, topLeft.minA, topLeft.minB, bottomLeft.maxB);
            }
        }

        private BridgedCornerSegmentEdges Bridge(CornerSegment a, CornerSegment b, bool triangulate = true)
        {
            if (triangulate)
            {
                if (a.count == b.count)
                {
                    for (int i = 0; i < a.count - 1; i++)
                    {
                        int ai = a.start + i;
                        int bi = b.start + i;
                        AddQuad(ai, ai + 1, bi + 1, bi);
                    }
                }
                else if (a.count > b.count)
                {
                    // to bridge 2 circle segments when the inner (B) has less verts than the outer (A)
                    // we'll maintain 2 indexes for A and B, B will be incremented in fractions

                    // calc how mush to incremement B when when we increment A by one
                    float bIncrement = b.count / (float)a.count;
                    float bFloatIndex = 0;

                    for (int i = 0; i < a.count - 1; i++)
                    {
                        int aIndex = a.start + i;
                        int bIndex = b.start + (int)bFloatIndex;

                        AddTri(bIndex, aIndex, aIndex + 1);

                        bFloatIndex += bIncrement;

                        // if bIndex changes to the next vert we need to make a triangle between the
                        // two verts on B
                        var nextbIndex = b.start + (int)bFloatIndex;
                        if (nextbIndex != bIndex)
                        {
                            AddTri(bIndex, aIndex + 1, nextbIndex);
                        }
                    }
                }
                else
                {
                    throw new System.Exception();
                }
            }

            return new BridgedCornerSegmentEdges(a, b);
        }

        private void DrawRectangle()
        {
            var offsetRadii = (float)UsedRadius + _radiusOffsets;

            if (color.a <= 0) { return; }

            if (_antiAlias > 0)
            {
                var outerAA = AddRectangle(offsetRadii, offsetRadii + _antiAlias * 0.5f, new Color(color.r, color.g, color.b, 0));
                var fill = AddRectangle(offsetRadii, offsetRadii - _antiAlias * 0.5f, color);
                Bridge(outerAA, fill);
                Triangulate(fill);
            }
            else
            {
                var fill = AddRectangle(offsetRadii, offsetRadii, color);
                Triangulate(fill);
            }
        }

        private RectangleSegment AddRectangle(float centerInset, float radius, Color32 color) => AddRectangle(centerInset, radius, color, Vector3.zero);
        private RectangleSegment AddRectangle(float inset, float radius, Color32 color, Vector3 offset, bool affectByGradient = true, bool clampOffset = false)
        {
            return AddRectangle(inset * Vector4.one, radius * Vector4.one, color, offset, affectByGradient, clampOffset);
        }

        private RectangleSegment AddRectangle(Vector4 insets, Vector4 radii, Color32 color, Vector3 offset = default, bool affectByGradient = true, bool clampOffset = false)
        {
            var rect = rectTransform.rect;
            RectangleSegment result = default;

            for (int i = 0; i < 4; i++)
            {
                var inset = insets[i];
                // inset controls how far from the edge the circle can be
                // to prevent corners overlapping its clamped to half the size of the rect
                var clampedInset = Mathf.Min(Mathf.Min(rect.size.x / 2, rect.size.y / 2), inset);
                var corner = GetCorner(rect, i);
                var cornerOffset = corner + offset - _insetDirections[i] * clampedInset;
                var radius = radii[i] - (inset - clampedInset);

                // offset shifts the rects corners, to change shadows position
                // clamp offset is used by inner shadows to force offset corners to stay in the main rect
                if (clampOffset)
                {
                    cornerOffset = ClampOffset(i, corner, cornerOffset, radius);
                }

                result[i] = AddCornerSegment(cornerOffset, i / 4f, (i + 1) / 4f, radius, color, affectByGradient);
            }

            return result;
        }

        private CornerSegment AddCornerSegment(Vector3 center, float from, float to, float radius, Color32 color, bool affectByGradient)
        {
            // if the radius is less than half a pixel set it to zero to create less geometry
            radius = radius < 0.5f ? 0 : radius;

            // reduce the number of divisions for small radii
            float quarterCircumference = radius * 0.5f * Mathf.PI;
            float maxCircumference = 10 * Mathf.PI;
            int divisions = Mathf.CeilToInt(_divisions * Mathf.Clamp01(quarterCircumference / maxCircumference));
            if (divisions < 2) { divisions = 2; }

            if (radius > 0)
            {
                CornerSegment result = new CornerSegment(_verts.Count, divisions);

                for (float i = 0; i < divisions; i++)
                {
                    var n = i / (divisions - 1);
                    var circ = CalcCircle(Mathf.Lerp(from, to, n));
                    AddVert(center + (Vector3)(circ * radius));
                }

                return result;
            }
            else
            {
                AddVert(center);
                return new CornerSegment(_verts.Count - 1, 1);
            }

            void AddVert(Vector3 position)
            {
                UIVertex vert = UIVertex.simpleVert;
                vert.position = position;
                vert.uv0 = ToUV(vert.position);
                vert.color = color;
                if (affectByGradient)
                {
                    vert.color *= _gradientHelper.ColorAtPosition(vert.position);
                }
                _verts.Add(vert);
            }
        }

        private void AddTri(int a, int b, int c)
        {
            if (a == b || b == c || c == a) return; //dont add zero area tris
            _indicies.Add(a);
            _indicies.Add(b);
            _indicies.Add(c);
        }

        private Vector2 ToUV(Vector2 position)
        {
            var rect = rectTransform.rect;
            return new Vector2((position.x - rect.xMin) / rect.size.x, (position.y - rect.yMin) / rect.size.y);
        }

        private void AddQuad(int a, int b, int c, int d)
        {
            AddTri(a, b, d);
            AddTri(d, b, c);
        }

        private void Triangulate(RectangleSegment rectangle)
        {
            if (rectangle.IsQuad())
            {
                AddQuad(rectangle.topLeft.start, rectangle.topRight.start, rectangle.bottomRight.start, rectangle.bottomLeft.start);
                return;
            }

            /*
            var toPoint = rectangle.topLeft.start;
            Triangulate(rectangle.topLeft, toPoint, 1);
            AddTri(toPoint, rectangle.topLeft.end, rectangle.topRight.start);
            Triangulate(rectangle.topRight, toPoint);
            AddTri(toPoint, rectangle.topRight.end, rectangle.bottomRight.start);
            Triangulate(rectangle.bottomRight, toPoint);
            AddTri(toPoint, rectangle.bottomRight.end, rectangle.bottomLeft.start);
            Triangulate(rectangle.bottomLeft, toPoint);
            AddTri(toPoint, rectangle.bottomLeft.end, rectangle.topLeft.start);
            */

            Triangulate(rectangle.topLeft, rectangle.topLeft.start, 1);
            Triangulate(rectangle.topRight, rectangle.topRight.start, 1);
            Triangulate(rectangle.bottomRight, rectangle.bottomRight.start, 1);
            Triangulate(rectangle.bottomLeft, rectangle.bottomLeft.start, 1);
            AddQuad(rectangle.topLeft.start, rectangle.topLeft.end, rectangle.topRight.start, rectangle.topRight.end);
            AddQuad(rectangle.bottomRight.start, rectangle.bottomRight.end, rectangle.bottomLeft.start, rectangle.bottomLeft.end);
            AddQuad(rectangle.topLeft.start, rectangle.topRight.end, rectangle.bottomRight.start, rectangle.bottomLeft.end);
        }

        private void Triangulate(CornerSegment segment, int toPoint, int startOffset = 0)
        {
            int start = segment.start + startOffset;
            int end = segment.start + segment.count - 1;
            for (int i = start; i < end; i++)
            {
                AddTri(toPoint, i, i + 1);
            }
        }

        private void DrawDropShadow(Shadow shadow)
        {
            var middleColor = shadow.color;
            middleColor.a = (byte)(middleColor.a * _dropShadowVisibility);
            if (middleColor.a == 0) return;

            var radius = UsedRadius;
            var offset = Vector3.Scale(shadow.position, new Vector3(1, -1, 1));
            var edgeColor = shadow.color;
            edgeColor.a = 0;

            var centerInset = Mathf.Max(radius, -shadow.spread + shadow.blur);
            var outerRadius = centerInset + shadow.spread + shadow.blur;
            var innerRadius = ClampRadiusToHalfRectSize(centerInset + shadow.spread - shadow.blur);

            var clampedCenterInset = ClampRadiusToHalfRectSize(centerInset);

            outerRadius -= centerInset - clampedCenterInset;
            middleColor = Color.Lerp(edgeColor, middleColor, clampedCenterInset / centerInset);
            centerInset = clampedCenterInset;

            if (shadow.blur > 0)
            {
                var outer = AddRectangle(_radiusOffsets + centerInset, _radiusOffsets + outerRadius, edgeColor, offset, false);
                for (float i = 1; i <= _shadowQuality - 1; i++)
                {
                    float n = i / _shadowQuality;
                    var mid = AddRectangle(_radiusOffsets + centerInset, _radiusOffsets + Mathf.Lerp(outerRadius, innerRadius, n), Color.Lerp(edgeColor, middleColor, n * n), offset, false);
                    Bridge(outer, mid);
                    outer = mid;
                }
                var inner = AddRectangle(_radiusOffsets + centerInset, _radiusOffsets + innerRadius, middleColor, offset, false);
                Bridge(outer, inner);
                Triangulate(inner);
            }
            else
            {
                var rectangle = AddRectangle(_radiusOffsets + centerInset, _radiusOffsets + outerRadius, shadow.color, offset, false);
                Triangulate(rectangle);
            }
        }

        private void DrawInnerShadow(Shadow shadow)
        {
            if (shadow.color.a == 0) { return; }

            var radius = UsedRadius;

            var edgeColor = shadow.color;
            edgeColor.a = 0;

            var offset = Vector3.Scale(shadow.position, new Vector3(1, -1, 1));
            var centerInset = Mathf.Max(radius, -shadow.spread + shadow.blur);
            var outerRadius = centerInset + shadow.spread + shadow.blur;
            var innerRadius = centerInset + shadow.spread - shadow.blur;

            // TODO no inner if blur is 0
            // TODO better clamp inside rect
            var outerLerp = Mathf.Clamp01(UsedRadius / outerRadius);
            var clampedOuterRadius = Mathf.Min(outerRadius, UsedRadius);
            var outerColor = Color.Lerp(edgeColor, shadow.color, outerLerp);
            var outer = AddRectangle(_radiusOffsets + centerInset, _radiusOffsets + clampedOuterRadius, outerColor, offset, false, true);
            var inner = AddRectangle(_radiusOffsets + centerInset, _radiusOffsets + innerRadius, edgeColor, offset, false, true);
            Bridge(outer, inner);

            if (offset != Vector3.zero || shadow.spread != 0 || clampedOuterRadius < centerInset)
            {
                var original = AddRectangle(_radiusOffsets + radius, _radiusOffsets + UsedRadius, shadow.color, Vector3.zero, false);
                Bridge(original, outer);
            }
        }

        private static Vector2 CalcCircle(float t)
        {
            return new Vector2(Mathf.Sin(2 * Mathf.PI * t), Mathf.Cos(2 * Mathf.PI * t));
        }

        private float ClampRadiusToHalfRectSize(float radius)
        {
            var halfSize = rectTransform.rect.size / 2f;
            return Mathf.Clamp(radius, 0, Mathf.Min(halfSize.x, halfSize.y));
        }

        /// <summary>
        /// returns the corners of the rect starting top right and moving clockwise
        /// </summary>
        Vector3 GetCorner(Rect rect, int index)
        {
            switch (index)
            {
                case 0: return rect.max;
                case 1: return new Vector2(rect.xMax, rect.yMin);
                case 2: return rect.min;
                case 3: return new Vector2(rect.xMin, rect.yMax);
                default: throw new Exception();
            }
        }

        private static Vector3 ClampOffset(int i, Vector3 corner, Vector3 cornerOffset, float radius)
        {
            if ((i == 0 || i == 3) && cornerOffset.y + radius > corner.y)
            {
                cornerOffset.y = corner.y - radius;
            }
            if ((i == 1 | i == 2) && cornerOffset.y - radius < corner.y)
            {
                cornerOffset.y = corner.y + radius;
            }
            if ((i == 0 || i == 1) && cornerOffset.x + radius > corner.x)
            {
                cornerOffset.x = corner.x - radius;
            }
            if ((i == 2 || i == 3) && cornerOffset.x - radius < corner.x)
            {
                cornerOffset.x = corner.x + radius;
            }

            return cornerOffset;
        }

        static List<ICanvasRaycastFilter> _components = new List<ICanvasRaycastFilter>();

        public override bool Raycast(Vector2 sp, Camera eventCamera)
        {
            if (!_optimizedRaycast) return base.Raycast(sp, eventCamera);

            if (!isActiveAndEnabled)
                return false;

            bool ignoreCanvasGroups = false;

            GetComponentsInParent(false, _components);

            var count = _components.Count;
            for (int i = 0; i < count; i++)
            {
                var filter = _components[i];
                var raycastValid = true;

                if (!(filter is CanvasGroup group))
                {
                    raycastValid = filter.IsRaycastLocationValid(sp, eventCamera);
                }
                else if (!ignoreCanvasGroups)
                {
                    raycastValid = group.blocksRaycasts;
                    ignoreCanvasGroups = group.ignoreParentGroups;
                }

                if (!raycastValid)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetDropShadow(int index, Shadow shadow)
        {
            _dropShadows[index] = shadow;
            SetVerticesDirty();
        }

        public Shadow GetDropShadow(int index) => _dropShadows[index];

        public int DropShadowCount
        {
            get => _dropShadows.Count;
            set
            {
                int diff = _dropShadows.Count - value;
                while (diff-- > 0) _dropShadows.RemoveAt(value + diff);
                while (++diff < 0) _dropShadows.Add(default);

                // we only need to mark dirty if shadows are removed
                // new shadows start full transparent so there's no visual change
                if (diff > 0) SetVerticesDirty();
            }
        }

        [Serializable]
        public struct Shadow
        {
            public Vector3 position;
            public float blur;
            public float spread;
            public Color32 color;
        }

        struct GradientHelper
        {
            private float _min;
            private float _range;
            private Color _startColor;
            private Color _endColor;
            private Vector2 _gradientDirection;
            private bool _isMonochromatic;

            public void Update(Rect rect, float angle, Color startColor, Color endColor)
            {
                _startColor = startColor;
                _endColor = endColor;

                _isMonochromatic = startColor == endColor;
                if (_isMonochromatic) { return; }

                _gradientDirection = CalcCircle(angle / 360);
                var tl = Vector3.Dot(new Vector2(rect.xMin, rect.yMax), _gradientDirection);
                var tr = Vector3.Dot(new Vector2(rect.xMax, rect.yMax), _gradientDirection);
                var br = Vector3.Dot(new Vector2(rect.xMax, rect.yMin), _gradientDirection);
                var bl = Vector3.Dot(new Vector2(rect.xMin, rect.yMin), _gradientDirection);
                _min = Mathf.Min(Mathf.Min(tl, tr), Mathf.Min(bl, br));
                var max = Mathf.Max(Mathf.Max(tl, tr), Mathf.Max(bl, br));
                _range = max - _min;
            }

            public Color ColorAtPosition(Vector3 position)
            {
                if (_isMonochromatic) { return _startColor; }

                float t = (Vector3.Dot(_gradientDirection, position) - _min) / _range;
                return Color.Lerp(_startColor, _endColor, t);
            }
        }

        /// <summary>
        /// The 2 edges of an inner and outer corner that have been bridged
        /// </summary>
        struct BridgedCornerSegmentEdges
        {
            public int minA;
            public int minB;
            public int maxA;
            public int maxB;

            public BridgedCornerSegmentEdges(CornerSegment a, CornerSegment b)
            {
                minA = a.start;
                minB = b.start;
                maxA = a.end;
                maxB = b.end;
            }
        }

        /// <summary>
        /// A set of verts that have been added to the mesh that form a circle segment e.g. a rounded corner
        /// </summary>
        struct CornerSegment
        {
            public int start;
            public int count;
            public int end;

            public CornerSegment(int start, int count) : this()
            {
                this.start = start;
                this.count = count;
                this.end = start + count - 1;
            }
        }

        /// <summary>
        /// A set of 4 corners
        /// </summary>
        struct RectangleSegment
        {
            public CornerSegment topLeft;
            public CornerSegment topRight;
            public CornerSegment bottomRight;
            public CornerSegment bottomLeft;

            public CornerSegment this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return topRight;
                        case 1: return bottomRight;
                        case 2: return bottomLeft;
                        case 3: return topLeft;
                        default: throw new Exception();
                    }
                }
                set
                {
                    switch (i)
                    {
                        case 0: topRight = value; break;
                        case 1: bottomRight = value; break;
                        case 2: bottomLeft = value; break;
                        case 3: topLeft = value; break;
                        default: throw new Exception();
                    }
                }
            }

            public RectangleSegment(CornerSegment topLeft, CornerSegment topRight, CornerSegment bottomRight, CornerSegment bottomLeft) : this()
            {
                this.topLeft = topLeft;
                this.topRight = topRight;
                this.bottomRight = bottomRight;
                this.bottomLeft = bottomLeft;
            }

            public int VertCount => topLeft.count + topRight.count + bottomRight.count + bottomLeft.count;

            public bool IsQuad() => VertCount == 4;
        }

        [Serializable]
        struct CornerFloats
        {
            public float topRight;
            public float bottomRight;
            public float bottomLeft;
            public float topLeft;

            public static readonly CornerFloats one = new CornerFloats(1);

            public CornerFloats(float all) : this(all, all, all, all) { }

            public CornerFloats(float topRight, float bottomRight, float bottomLeft, float topLeft)
            {
                this.topRight = topRight;
                this.bottomRight = bottomRight;
                this.bottomLeft = bottomLeft;
                this.topLeft = topLeft;
            }

            public static implicit operator CornerFloats(Vector4 v) => new CornerFloats(v.x, v.y, v.z, v.w);
            public static implicit operator Vector4(CornerFloats v) => new Vector4(v.topRight, v.bottomRight, v.bottomLeft, v.topLeft);
            public static CornerFloats operator *(CornerFloats c, float v) => new CornerFloats(c.topRight * v, c.bottomRight * v, c.bottomLeft * v, c.topLeft * v);
            public static CornerFloats operator +(CornerFloats c, float v) => new CornerFloats(c.topRight + v, c.bottomRight + v, c.bottomLeft + v, c.topLeft + v);
            public static CornerFloats operator +(float v, CornerFloats c) => c + v;
            public static CornerFloats operator -(CornerFloats c, float v) => new CornerFloats(c.topRight - v, c.bottomRight - v, c.bottomLeft - v, c.topLeft - v);
            public static CornerFloats operator -(CornerFloats a, CornerFloats b) => new CornerFloats(a.topRight - b.topRight, a.bottomRight - b.bottomRight, a.bottomLeft - b.bottomLeft, a.topLeft - b.topLeft);
        }

        [Flags]
        public enum RectangleMask
        {
            Top = 1 << 0,
            TopRight = 1 << 1,
            Right = 1 << 2,
            BottomRight = 1 << 3,
            Bottom = 1 << 4,
            BottomLeft = 1 << 5,
            Left = 1 << 6,
            TopLeft = 1 << 7
        }
    }
}
