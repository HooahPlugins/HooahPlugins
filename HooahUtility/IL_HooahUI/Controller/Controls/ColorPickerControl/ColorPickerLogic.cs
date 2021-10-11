using System;
using System.Collections.Generic;
using HooahUtility.AdvancedStudioUI.Model.Class;
using UnityEngine;

namespace AdvancedStudioUI
{
    public partial class ColorPickerControl
    {
        private static Dictionary<PickerType, ColorReference.Dimension> ColorMap =
            new Dictionary<PickerType, ColorReference.Dimension>
            {
                {PickerType.R, ColorReference.Dimension.R},
                {PickerType.G, ColorReference.Dimension.G},
                {PickerType.B, ColorReference.Dimension.B},
                {PickerType.H, ColorReference.Dimension.H},
                {PickerType.S, ColorReference.Dimension.S},
                {PickerType.V, ColorReference.Dimension.V},
                {PickerType.A, ColorReference.Dimension.A},
            };

        private void PickColor2D(ColorReference color, PickerType xpt, float xv, PickerType ypt, float yv)
        {
            if (
                ColorMap.TryGetValue(xpt, out var xt) &&
                ColorMap.TryGetValue(ypt, out var yt)
            ) color.SetDoubleHSV(xt, xv, yt, yv);
        }

        private void PickColor1D(ColorReference color, PickerType type, Vector2 v)
        {
            if (!Pickers.TryGetValue(type, out var pickerData)) return;
            var value = pickerData.Direction == PickerDirection.Horizontal ? v.x : v.y;
            PickColor1D(color, type, value);
        }

        private void PickColor1D(ColorReference color, PickerType type, float value)
        {
            switch (type)
            {
                case PickerType.R:
                case PickerType.G:
                case PickerType.B:
                case PickerType.S:
                case PickerType.V:
                case PickerType.A:
                case PickerType.H:
                    if (ColorMap.TryGetValue(type, out var d))
                        color.SetSingle(d, value);
                    break;
                // color.SetSingle(ColorReference.Dimension.H, value * HUE_LOOP);
                case PickerType.Main:
                case PickerType.Preview:
                case PickerType.PreviewAlpha:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void PickColor(ColorReference color, PickerType type, Vector2 v)
        {
            switch (type)
            {
                case PickerType.Main:
                    PickColorMain(color, v);
                    break;
                case PickerType.Preview:
                case PickerType.PreviewAlpha:
                    return;
                default:
                    PickColor1D(color, type, v);
                    break;
            }

            UpdateMinMax();
        }

        private Dictionary<PickerType, Color[]> GradientColor =
            new Dictionary<PickerType, Color[]>
            {
                {PickerType.Main, new[] {Color.black, Color.white}},
                {PickerType.R, new[] {Color.black, Color.white}},
                {PickerType.G, new[] {Color.black, Color.white}},
                {PickerType.B, new[] {Color.black, Color.white}},
                {PickerType.H, new[] {Color.black, Color.white}},
                {PickerType.S, new[] {Color.black, Color.white}},
                {PickerType.V, new[] {Color.black, Color.white}},
                {PickerType.A, new[] {Color.black, Color.white}},
            };

        private void UpdateMinMax()
        {
            GradientColor[PickerType.R][0] = new Color(0, ColorReference.G, ColorReference.B);
            GradientColor[PickerType.R][1] = new Color(1, ColorReference.G, ColorReference.B);
            GradientColor[PickerType.G][0] = new Color(ColorReference.R, 0, ColorReference.B);
            GradientColor[PickerType.G][1] = new Color(ColorReference.R, 1, ColorReference.B);
            GradientColor[PickerType.B][0] = new Color(ColorReference.R, ColorReference.G, 0);
            GradientColor[PickerType.B][1] = new Color(ColorReference.R, ColorReference.G, 1);
            GradientColor[PickerType.A][0] = new Color(ColorReference.R, ColorReference.G, ColorReference.B, 0);
            GradientColor[PickerType.A][1] = new Color(ColorReference.R, ColorReference.G, ColorReference.B, 1);

            GradientColor[PickerType.H][0] = Color.HSVToRGB(0, ColorReference.S, ColorReference.V);
            GradientColor[PickerType.H][1] = Color.HSVToRGB(1, ColorReference.S, ColorReference.V);
            GradientColor[PickerType.S][0] = Color.HSVToRGB(ColorReference.H, 0, ColorReference.V);
            GradientColor[PickerType.S][1] = Color.HSVToRGB(ColorReference.H, 1, ColorReference.V);
            GradientColor[PickerType.V][0] = Color.HSVToRGB(ColorReference.H, ColorReference.S, 0);
            GradientColor[PickerType.V][1] = Color.HSVToRGB(ColorReference.H, ColorReference.S, 1);


            switch (mode)
            {
                case MainPickingMode.HS:
                case MainPickingMode.SH:
                    GradientColor[PickerType.Main][0] = Color.HSVToRGB(0, 0, ColorReference.V);
                    GradientColor[PickerType.Main][1] = Color.HSVToRGB(1, 1, ColorReference.V);
                    break;
                case MainPickingMode.HV:
                case MainPickingMode.VH:
                    GradientColor[PickerType.Main][0] = Color.HSVToRGB(0, ColorReference.S, 0);
                    GradientColor[PickerType.Main][1] = Color.HSVToRGB(1, ColorReference.S, 1);
                    break;
                case MainPickingMode.SV:
                case MainPickingMode.VS:
                    GradientColor[PickerType.Main][0] = Color.HSVToRGB(ColorReference.H, 0, 0);
                    GradientColor[PickerType.Main][1] = Color.HSVToRGB(ColorReference.H, 1, 1);
                    break;
            }
        }


        private void PickColorMain(ColorReference color, Vector2 v)
        {
            switch (mode)
            {
                case MainPickingMode.HS:
                    PickColor2D(color, PickerType.H, v.x, PickerType.S, v.y);
                    break;
                case MainPickingMode.HV:
                    PickColor2D(color, PickerType.H, v.x, PickerType.V, v.y);
                    break;
                case MainPickingMode.SH:
                    PickColor2D(color, PickerType.S, v.x, PickerType.H, v.y);
                    break;
                case MainPickingMode.SV:
                    PickColor2D(color, PickerType.S, v.x, PickerType.V, v.y);
                    break;
                case MainPickingMode.VH:
                    PickColor2D(color, PickerType.V, v.x, PickerType.H, v.y);
                    break;
                case MainPickingMode.VS:
                    PickColor2D(color, PickerType.V, v.x, PickerType.S, v.y);
                    break;
            }
        }

        private void UpdateTexture(PickerData pickerData, bool standardized)
        {
            var image = pickerData.SliderImage;
            var type = pickerData.PickerType;
            if (!image || !image.gameObject.activeInHierarchy)
                return;

            var m = image.materialForRendering;

            var alpha = pickerData.IsAlpha;
            if (!GradientColor.TryGetValue(pickerData.PickerType, out var gradient)) return;

            m.SetInt(Mode, GetGradientMode(type));
            m.SetColor(Color1, gradient[0]);
            m.SetColor(Color2, gradient[1]);
            if (type == PickerType.Main) m.SetInt(DoubleMode, (int) mode);
            m.SetVector(HSV,
                standardized
                    ? new Vector4(0f, 1f, 1f, 1f)
                    : new Vector4(ColorReference.H / HUE_LOOP, ColorReference.S, ColorReference.V,
                        alpha ? ColorReference.A : 1f)
            );
        }

        private float GetValue1D(PickerType type)
        {
            switch (type)
            {
                case PickerType.R: return ColorReference.R;
                case PickerType.G: return ColorReference.G;
                case PickerType.B: return ColorReference.B;
                case PickerType.H: return ColorReference.H;
                case PickerType.S: return ColorReference.S;
                case PickerType.V: return ColorReference.V;
                case PickerType.A: return ColorReference.A;
                default:
                    throw new Exception("Picker type " + type + " is not associated with a single color value.");
            }
        }

        private Vector2 GetValue(PickerType type)
        {
            switch (type)
            {
                case PickerType.Main: return GetValue(mode);
                case PickerType.Preview:
                case PickerType.PreviewAlpha:
                    return Vector2.zero;
                default:
                    var value = GetValue1D(type);
                    return new Vector2(value, value);
            }
        }

        private Vector2 GetValue(MainPickingMode mode)
        {
            switch (mode)
            {
                case MainPickingMode.HS: return new Vector2(GetValue1D(PickerType.H), GetValue1D(PickerType.S));
                case MainPickingMode.HV: return new Vector2(GetValue1D(PickerType.H), GetValue1D(PickerType.V));
                case MainPickingMode.SH: return new Vector2(GetValue1D(PickerType.S), GetValue1D(PickerType.H));
                case MainPickingMode.SV: return new Vector2(GetValue1D(PickerType.S), GetValue1D(PickerType.V));
                case MainPickingMode.VH: return new Vector2(GetValue1D(PickerType.V), GetValue1D(PickerType.H));
                case MainPickingMode.VS: return new Vector2(GetValue1D(PickerType.V), GetValue1D(PickerType.S));
                default: throw new Exception("Unknown main picking mode: " + mode);
            }
        }
    }
}