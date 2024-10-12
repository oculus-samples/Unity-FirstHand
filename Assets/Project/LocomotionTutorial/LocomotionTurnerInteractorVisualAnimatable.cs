// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
    public class LocomotionTurnerInteractorVisualAnimatable : MonoBehaviour
    {
        [SerializeField, Range(-1, 1)]
        private float _axis = -1;
        [SerializeField]
        private bool _select = false;

        [Space]
        [SerializeField]
        private float _radius = 0.07f;
        [SerializeField]
        private float _margin = 2f;
        [SerializeField]
        private float _trailLength = 15f;
        [SerializeField]
        private float _maxAngle = 15f;
        [SerializeField]
        private float _railGap = 0.005f;
        [SerializeField]
        private float _squeezeLength = 7f;

        [SerializeField]
        private Renderer _leftArrow;
        [SerializeField]
        private Renderer _rightArrow;
        [SerializeField]
        private Interaction.TubeRenderer _leftTrail;
        [SerializeField]
        private Interaction.TubeRenderer _rightTrail;

        [SerializeField]
        private Color _disabledColor = new Color(1, 1, 1, 0.2f);
        [SerializeField]
        private Color _enabledColor = new Color(1, 1, 1, 0.6f);
        [SerializeField]
        private Color _highligtedColor = new Color(1, 1, 1, 1f);

        [SerializeField]
        private MaterialPropertyBlockEditor _leftMaterialBlock;
        [SerializeField]
        private MaterialPropertyBlockEditor _rightMaterialBlock;

        private const float _degreesPerSegment = 1f;

        private static readonly Quaternion _rotationCorrectionLeft = Quaternion.Euler(0f, -90f, 0f);
        private static readonly Quaternion _rotationCorrectionRight = Quaternion.Euler(0f, 90f, 0f);
        private static readonly int _colorShaderPropertyID = Shader.PropertyToID("_Color");

        public void Hide()
        {
            SetShown(false);
        }

        void SetShown(bool shown)
        {
            _leftArrow.enabled = _rightArrow.enabled = _leftTrail.enabled = _rightTrail.enabled = shown;
        }

        public void HoverNone()
        {
            SetShown(true);
            _axis = 0;
            _select = false;
        }

        public void HoverLeft()
        {
            SetShown(true);
            _axis = -1f;
            _select = false;
        }

        public void HoverRight()
        {
            SetShown(true);
            _axis = 1f;
            _select = false;
        }

        public void SelectLeft()
        {
            SetShown(true);
            _axis = -1f;
            _select = true;
        }

        public void SelectRight()
        {
            SetShown(true);
            _axis = 1f;
            _select = true;
        }

        protected virtual void Start()
        {
            InitializeTrails();
        }

        private void Update()
        {
            UpdateArrows();
            UpdateColors();
        }

        private void InitializeTrails()
        {
            float min = _margin;
            float max = _maxAngle + _squeezeLength;
            TubePoint[] leftTrailPoints = InitializeSegment(new Vector2(min, max));
            TubePoint[] rightTrailPoints = InitializeSegment(new Vector2(-max, -min));
            _leftTrail.RenderTube(leftTrailPoints, Space.Self);
            _rightTrail.RenderTube(rightTrailPoints, Space.Self);
        }

        private void UpdateArrows()
        {
            float value = _axis;
            float angle = Mathf.Lerp(0f, _maxAngle, Mathf.Abs(value));

            float squeeze = _select ? _squeezeLength : 0f;
            if (value < 0)
            {
                UpdateArrowPosition(_trailLength, 0f, _rightArrow.transform, _rightTrail, false);
                UpdateArrowPosition(_trailLength, squeeze, _leftArrow.transform, _leftTrail, true);
            }
            else
            {
                UpdateArrowPosition(_trailLength, squeeze, _rightArrow.transform, _rightTrail, false);
                UpdateArrowPosition(_trailLength, 0f, _leftArrow.transform, _leftTrail, true);
            }
        }

        private void UpdateArrowPosition(float angle, float extra, Transform arrow, Interaction.TubeRenderer tube, bool isLeft)
        {
            UpdateTrail(angle, extra, isLeft, tube, !_select);
            if (isLeft)
            {
                angle = -angle;
                extra = -extra;
            }

            Quaternion rotation = Quaternion.AngleAxis(angle + extra, Vector3.up);
            arrow.localPosition = rotation * Vector3.forward * _radius;
            arrow.localRotation = rotation * (isLeft ? _rotationCorrectionLeft : _rotationCorrectionRight);
        }

        private void UpdateTrail(float angle, float extra, bool isLeft, Interaction.TubeRenderer trail, bool followArrow)
        {
            if (followArrow)
            {
                angle = isLeft ? Mathf.Min(-angle + _trailLength, 0f) : Mathf.Max(angle - _trailLength, 0f);
                trail.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
                angle = _trailLength;
            }
            else
            {
                trail.transform.localRotation = Quaternion.identity;
            }
            angle = angle + extra;

            float max = _maxAngle + _squeezeLength;
            float segmentLenght = trail.TotalLength;
            float start = -100;
            float end = (max - angle - _margin) / max;

            if (!isLeft)
            {
                (start, end) = (end, start);
            }

            trail.StartFadeThresold = segmentLenght * start;
            trail.EndFadeThresold = segmentLenght * end;
            trail.InvertThreshold = false;
            trail.RedrawFadeThresholds();
        }

        private void UpdateColors()
        {
            bool isSelecting = _select;

            var leftColor = _axis < 0 ? isSelecting ? _highligtedColor : _enabledColor : _disabledColor;
            var rightColor = _axis > 0 ? isSelecting ? _highligtedColor : _enabledColor : _disabledColor;

            _leftMaterialBlock.MaterialPropertyBlock.SetColor(_colorShaderPropertyID, leftColor);
            _rightMaterialBlock.MaterialPropertyBlock.SetColor(_colorShaderPropertyID, rightColor);

            _leftMaterialBlock.UpdateMaterialPropertyBlock();
            _rightMaterialBlock.UpdateMaterialPropertyBlock();
        }

        private TubePoint[] InitializeSegment(Vector2 minMax)
        {
            float lowLimit = minMax.x;
            float upLimit = minMax.y;
            int segments = Mathf.RoundToInt(Mathf.Repeat(upLimit - lowLimit, 360f) / _degreesPerSegment);
            TubePoint[] tubePoints = new TubePoint[segments];
            float segmentLenght = 1f / segments;
            for (int i = 0; i < segments; i++)
            {
                Quaternion rotation = Quaternion.AngleAxis(-i * _degreesPerSegment - lowLimit, Vector3.up);
                tubePoints[i] = new TubePoint()
                {
                    position = rotation * Vector3.forward * _radius,
                    rotation = rotation * _rotationCorrectionLeft,
                    relativeLength = i * segmentLenght
                };
            }
            return tubePoints;
        }
    }
}
