using System;
using UnityEngine;

namespace HooahUtility.AdvancedStudioUI.Model.Class
{
    public class ColorReference
    {
        public Action<Color> onColorChange;
        
        private Color _color;

        public Color TargetColor
        {
            get => _color;
            set
            {
                _color = value;
                UpdateHSV();
            }
        }

        private Vector3 _hsv;
        public Vector3 TargetHSV
        {
            get => _hsv;
            set
            {
                _hsv = value;
                UpdateColor();
            }
        }
        
        public float R => _color.r;
        public float G => _color.g;
        public float B => _color.b;
        public float A => _color.a;
        public float H => _hsv.x;
        public float S => _hsv.y;
        public float V => _hsv.z;

        public ColorReference()
        {
            TargetColor = Color.black;
        }

        public ColorReference(Color color)
        {
            TargetColor = color;
        }

        public enum Dimension
        {
            R, G, B, H, S,
            V, A
        }

        public void UpdateHSV()
        {
            Color.RGBToHSV(_color, out var h, out var s, out var v);
            _hsv.x = h;
            _hsv.y = s;
            _hsv.z = v;
            onColorChange?.Invoke(_color);
        }

        public void UpdateColor()
        {
            var alpha = _color.a;
            _color = Color.HSVToRGB(TargetHSV.x, TargetHSV.y, TargetHSV.z);
            _color.a = alpha;
            onColorChange?.Invoke(_color);
        }

        public void SetSingleInternal(Dimension dimension, float value)
        {
            switch (dimension)
            {
                case Dimension.R:
                    _color.r = value;
                    UpdateHSV();
                    break;
                case Dimension.G:
                    _color.g = value;
                    UpdateHSV();
                    break;
                case Dimension.B:
                    _color.b = value;
                    UpdateHSV();
                    break;
                case Dimension.A:
                    _color.a = value;
                    break;
                case Dimension.H:
                    _hsv.x = value;
                    UpdateColor();
                    break;
                case Dimension.S:
                    _hsv.y = value;
                    UpdateColor();
                    break;
                case Dimension.V:
                    _hsv.z = value;
                    UpdateColor();
                    break;
            }
        }

        public void SetSingle(Dimension dimension, float value)
        {
            switch (dimension)
            {
                case Dimension.R:
                case Dimension.G:
                case Dimension.B:
                    SetSingleInternal(dimension, value);
                    UpdateHSV();
                    break;
                case Dimension.A:
                    _color.a = value;
                    break;
                case Dimension.H:
                case Dimension.S:
                case Dimension.V:
                    SetSingleInternal(dimension, value);
                    UpdateColor();
                    break;
            }
        }

        public void SetDoubleHSV(Dimension x, float xv, Dimension y, float yv)
        {
            SetSingleInternal(x, xv);
            SetSingleInternal(y, yv);
            UpdateColor();
        }
    }
}