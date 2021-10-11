using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ColorSpriteMesh : MonoBehaviour
{
    public int x, y;
    public MeshType meshType;

    public enum MeshType { CenterPoint, Forward, Backward }

    public Sprite sprite;
    private int bufferedHash;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        var hash = GetSettingHash();
        if (hash == 0 || hash == bufferedHash) return;
        MakeMesh(sprite, x, y, meshType);
        var im = _image;
        if (im)
        {
#if HS2
            im.useSpriteMesh = false;
            im.useSpriteMesh = true;
#endif
        }

        bufferedHash = hash;
    }

    private int GetSettingHash()
    {
        if (sprite == null || x <= 0 || y <= 0)
            return 0;
        return sprite.GetHashCode() * (x ^ 136) * (y ^ 1342) * ((int) (meshType + 1) ^ 99999);
    }

    private void MakeMesh(Sprite sprite, int x, int y, MeshType meshType)
    {
        var centerPoints = this.meshType == MeshType.CenterPoint;

        var px = x + 1;
        var py = y + 1;
        var t = px * py;
        var verts = new Vector2[centerPoints ? t + x * y : t];
        var faces = new ushort[x * y * (centerPoints ? 12 : 6)];

        //cardinal vertices
        for (var i = 0; i < px; i++)
        {
            var xi = (float) i / x;
            for (var j = 0; j < py; j++)
            {
                var yi = (float) j / y;
                verts[px * j + i] = new Vector2(xi, yi);
            }
        }


        switch (meshType)
        {
            case MeshType.CenterPoint:
                for (var i = 0; i < x; i++)
                {
                    var xi = (i + .5f) / x;
                    for (var j = 0; j < y; j++)
                    {
                        var yi = (j + .5f) / y;
                        verts[j * x + i + t] = new Vector2(xi, yi);
                    }
                }

                for (var i = 0; i < x; i++)
                {
                    for (var j = 0; j < y; j++)
                    {
                        var f = 12 * (j * x + i);
                        var s = j * px + i;
                        var ns = (ushort) (j * x + i + t);
                        faces[f + 11] = faces[f] = (ushort) s;
                        faces[f + 3] = faces[f + 2] = (ushort) (s + 1);
                        faces[f + 6] = faces[f + 5] = (ushort) (s + px + 1);
                        faces[f + 9] = faces[f + 8] = (ushort) (s + px);
                        faces[f + 1] = faces[f + 4] = faces[f + 7] = faces[f + 10] = ns;
                    }
                }

                break;
            case MeshType.Forward:
                for (var i = 0; i < x; i++)
                {
                    for (var j = 0; j < y; j++)
                    {
                        var f = 6 * (j * x + i);
                        var s = j * px + i;
                        faces[f + 5] = faces[f + 1] = (ushort) s;
                        faces[f] = (ushort) (s + 1);
                        faces[f + 4] = faces[f + 2] = (ushort) (s + px + 1);
                        faces[f + 3] = (ushort) (s + px);
                    }
                }

                break;
            case MeshType.Backward:
                for (var i = 0; i < x; i++)
                {
                    for (var j = 0; j < y; j++)
                    {
                        var f = 6 * (j * x + i);
                        var s = (j * px + i);
                        faces[f] = (ushort) s;
                        faces[f + 4] = faces[f + 2] = (ushort) (s + 1);
                        faces[f + 3] = (ushort) (s + px + 1);
                        faces[f + 5] = faces[f + 1] = (ushort) (s + px);
                    }
                }

                break;
        }

        sprite.OverrideGeometry(verts, faces);
    }
}